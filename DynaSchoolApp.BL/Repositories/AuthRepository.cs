using DynaSchoolApp.Database.Data;
using DynaSchoolApp.Models.Entities;
using DynaSchoolApp.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.BL.Services
{
    public interface IAuthRepository
    {
        Task<SchoolUserModel> GetUserByLogin(string username, string password);
        Task RemoveRefreshTokenByUserID(int userID);
        Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel);
        Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken);
    }
    public class AuthRepository(AppDbContext dbContext) : IAuthRepository
    {
        public Task<SchoolUserModel> GetUserByLogin(string username, string password)
        {
            return dbContext.SchoolUsers.Include(n => n.UserRoles).ThenInclude(n => n.Role).FirstOrDefaultAsync(n => n.Username == username && n.Password == password);
        }
        public async Task RemoveRefreshTokenByUserID(int userID)
        {
            var refreshToken = dbContext.RefreshTokens.FirstOrDefault(n => n.UserID == userID);
            if (refreshToken != null)
            {
                dbContext.RemoveRange(refreshToken);
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel)
        {
            await dbContext.RefreshTokens.AddAsync(refreshTokenModel);
            await dbContext.SaveChangesAsync();
        }

        public Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken)
        {
            return dbContext.RefreshTokens.Include(n => n.User).ThenInclude(n => n.UserRoles).ThenInclude(n => n.Role).FirstOrDefaultAsync(n => n.RefreshToken == refreshToken);
        }
    }
}
