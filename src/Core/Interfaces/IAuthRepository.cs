using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Core.Entities;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthToken> CreateTokenAsync(int userId, string token, DateTime expires);
        Task<AuthToken?> GetValidTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<bool> RevokeAllUserTokensAsync(int userId);
    }
}