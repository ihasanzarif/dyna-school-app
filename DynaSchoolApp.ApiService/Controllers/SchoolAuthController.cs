using DynaSchoolApp.ApiService.Handlers;
using DynaSchoolApp.BL.Services;
using DynaSchoolApp.Database.Data;
using DynaSchoolApp.Models.Entities;
using DynaSchoolApp.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynaSchoolApp.ApiService.Controllers
{
    [Route("school/[controller]")]
    [ApiController]
    [Authorize(Policy = "JwtOnly")]
    public class SchoolAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuation;
        private readonly ITenantLoginResolver _tenantLoginResolver;
        private readonly TenantDbContextFactory _tenantDbContextFactory;

        public SchoolAuthController(IAuthService authService, 
            IConfiguration configuration, 
            ITenantLoginResolver tenantLoginResolver,
            TenantDbContextFactory tenantDbContextFactory)
        {
            _authService = authService;
            _configuation = configuration;
            _tenantLoginResolver = tenantLoginResolver;
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid login request");
            }

            TenantLoginResolutionResult resolution;
            try
            {
                // 1. Resolve Tenant ID and Connection String from username (e.g., zarif@pirerbag)
                resolution = _tenantLoginResolver.ResolveTenant(model.Username);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, "Server configuration error during tenant resolution.");
            }

            SchoolUserModel user = null;

            // 2. Use the dedicated factory to create the context for authentication
            // The DbContext is scoped to this single login request and disposed automatically.
            using (var dbContext = _tenantDbContextFactory.Create(resolution.ConnectionString))
            {
                // 3. Authenticate User against the specific Tenant Database
                user = await dbContext.SchoolUsers
                    .SingleOrDefaultAsync(u => u.Username == resolution.Username);
            }

            // 4. Verify password hash and handle null user
            if (user is null || !PasswordHashHandlers.CheckPassword(model.Password, user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(GenerateJwtToken(user.Username));
        }

        private LoginResponseModel GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Staff")
                };

            string secret = _configuation.GetValue<string>("Jwt:Secret");
            string issuer = _configuation.GetValue<string>("Jwt:Issuer");
            string audience = _configuation.GetValue<string>("Jwt:Audience");
            var tokenExpiryInMinutes = _configuation.GetValue<int>("Jwt:TokenExpiryInMinutes");
            var setTokenExpiration = DateTime.UtcNow.AddHours(tokenExpiryInMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: setTokenExpiration,
                signingCredentials: creds
                );
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
            return new LoginResponseModel
            {
                Username = username,
                Token = tokenHandler,
                TokenExpired = setTokenExpiration.Ticks,
                RefreshToken = Guid.NewGuid().ToString() // Generate a new refresh token
            };
        }
    }
}
