using DynaSchoolApp.ApiService.Handlers;
using DynaSchoolApp.BL.Repositories;
using DynaSchoolApp.BL.Services;
using DynaSchoolApp.Database.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert Jwt with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IConnectionStringResolver, TenantConnectionStringResolver>();
builder.Services.AddScoped<ITenantLoginResolver, TenantLoginResolver>();
builder.Services.AddSingleton<TenantDbContextFactory>();
builder.Services.AddDbContext<AppDbContext>((sp,options) =>
{
    var connResolver = sp.GetRequiredService<IConnectionStringResolver>();
    options.UseSqlServer(connResolver.GetConnectionString(),
        b => b.MigrationsAssembly("DynaSchoolApp.ApiService"));
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddProblemDetails();

// ✔️ Use the custom handler for Authorized data and use AddJwtBearer for login only
string secret = builder.Configuration.GetValue<string>("Jwt:Secret");
string issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
string audience = builder.Configuration.GetValue<string>("Jwt:Audience");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "GatewayAuthentication";
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddScheme<AuthenticationSchemeOptions, ClaimsFromHeaderAuthenticationHandler>("GatewayAuthentication", null)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtOnly", policy => policy.RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
    options.AddPolicy("GatewayOnly", policy => policy.RequireAuthenticatedUser().AddAuthenticationSchemes("GatewayAuthentication"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
