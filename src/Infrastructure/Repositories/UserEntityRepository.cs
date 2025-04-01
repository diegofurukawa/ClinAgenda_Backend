// UserEntityRepository.cs
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class UserEntityRepository : IUserEntityRepository
    {
        private readonly MySqlConnection _connection;

        public UserEntityRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> LinkUserToEntityAsync(int userId, string entityType, int entityId)
        {
            string query = @"
                INSERT INTO user_entity (
                    userId, 
                    entityType, 
                    entityId, 
                    dCreated, 
                    lActive
                ) 
                VALUES (
                    @UserId, 
                    @EntityType, 
                    @EntityId, 
                    NOW(), 
                    1
                )";

            var parameters = new
            {
                UserId = userId,
                EntityType = entityType,
                EntityId = entityId
            };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<(string entityType, int entityId)?> GetUserEntityAsync(int userId)
        {
            string query = @"
                SELECT 
                    entityType, 
                    entityId
                FROM user_entity
                WHERE userId = @UserId AND lActive = 1";

            var result = await _connection.QueryFirstOrDefaultAsync<dynamic>(query, new { UserId = userId });
            
            if (result == null)
                return null;

            return (result.entityType, result.entityId);
        }

        public async Task<int?> GetEntityUserIdAsync(string entityType, int entityId)
        {
            string query = @"
                SELECT userId
                FROM user_entity
                WHERE entityType = @EntityType AND entityId = @EntityId AND lActive = 1";

            var parameters = new
            {
                EntityType = entityType,
                EntityId = entityId
            };

            return await _connection.QueryFirstOrDefaultAsync<int?>(query, parameters);
        }

        public async Task<bool> UnlinkUserFromEntityAsync(int userId)
        {
            string query = @"
                UPDATE user_entity
                SET lActive = 0, dLastUpdated = NOW()
                WHERE userId = @UserId";

            var rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId });
            return rowsAffected > 0;
        }
    }
}