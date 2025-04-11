// AuthRepository.cs
using ClinAgenda.src.Core.Entities;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MySqlConnection _connection;

        public AuthRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<AuthToken> CreateTokenAsync(int userId, string token, DateTime expires)
        {
            string query = @"
                INSERT INTO auth_token (
                    userId, 
                    token, 
                    dExpires, 
                    dCreated, 
                    lActive
                ) 
                VALUES (
                    @UserId, 
                    @Token, 
                    @Expires, 
                    NOW(), 
                    1
                );
                SELECT LAST_INSERT_ID();";

            var tokenId = await _connection.ExecuteScalarAsync<int>(query, 
                new { UserId = userId, Token = token, Expires = expires });

            return new AuthToken
            {
                TokenId = tokenId,
                UserId = userId,
                Token = token,
                DExpires = expires,
                DCreated = DateTime.Now,
                lActive = true
            };
        }

        public async Task<AuthToken?> GetValidTokenAsync(string token)
        {
            string query = @"
                SELECT 
                    tokenId, 
                    userId, 
                    token, 
                    dExpires, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM auth_token
                WHERE token = @Token AND lActive = 1 AND dExpires > NOW()";

            return await _connection.QueryFirstOrDefaultAsync<AuthToken>(query, new { Token = token });
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            string query = @"
                UPDATE auth_token 
                SET lActive = 0, dLastUpdated = NOW()
                WHERE token = @Token";

            var rowsAffected = await _connection.ExecuteAsync(query, new { Token = token });
            return rowsAffected > 0;
        }

        public async Task<bool> RevokeAllUserTokensAsync(int userId)
        {
            string query = @"
                UPDATE auth_token 
                SET lActive = 0, dLastUpdated = NOW()
                WHERE userId = @UserId";

            var rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId });
            return rowsAffected > 0;
        }
    }
}
