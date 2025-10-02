using DynaSchoolApp.Models.Entities;
using DynaSchoolApp.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.BL.Services
{
    public interface IAuthService
    {
        Task<SchoolUserModel> GetUserByLogin(string username, string password);
        Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel);
        Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken);
    }
    public class AuthService(IAuthRepository authRepository) : IAuthService
    {
        public Task<SchoolUserModel> GetUserByLogin(string username, string password)
        {
            return authRepository.GetUserByLogin(username, password);
        }
        public async Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel)
        {
            await authRepository.RemoveRefreshTokenByUserID(refreshTokenModel.UserID);
            await authRepository.AddRefreshTokenModel(refreshTokenModel);
        }
        public Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken)
        {
            return authRepository.GetRefreshTokenModel(refreshToken);
        }
    }
}
