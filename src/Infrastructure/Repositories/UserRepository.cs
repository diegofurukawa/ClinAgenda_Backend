// UserRepository.cs
using ClinAgenda.src.Core.Entities;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnection _connection;

        public UserRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    userId, 
                    username, 
                    email, 
                    passwordHash, 
                    dLastLogin, 
                    nFailedLoginAttempts, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM user
                WHERE userId = @Id";

            return await _connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            string query = @"
                SELECT 
                    userId, 
                    username, 
                    email, 
                    passwordHash, 
                    dLastLogin, 
                    nFailedLoginAttempts, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM user
                WHERE username = @Username";

            return await _connection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string query = @"
                SELECT 
                    userId, 
                    username, 
                    email, 
                    passwordHash, 
                    dLastLogin, 
                    nFailedLoginAttempts, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM user
                WHERE email = @Email";

            return await _connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
        }

        public async Task<int> CreateUserAsync(User user)
        {
            string query = @"
                INSERT INTO user (
                    username, 
                    email, 
                    passwordHash, 
                    dCreated, 
                    lActive
                ) 
                VALUES (
                    @Username, 
                    @Email, 
                    @PasswordHash, 
                    NOW(), 
                    @LActive
                );
                SELECT LAST_INSERT_ID();";

            return await _connection.ExecuteScalarAsync<int>(query, user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            string query = @"
                UPDATE user SET 
                    username = @Username, 
                    email = @Email, 
                    passwordHash = @PasswordHash, 
                    dLastLogin = @DLastLogin, 
                    nFailedLoginAttempts = @NFailedLoginAttempts, 
                    dLastUpdated = NOW(), 
                    lActive = @LActive
                WHERE userId = @UserId";

            var rowsAffected = await _connection.ExecuteAsync(query, user);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            string query = "DELETE FROM user WHERE userId = @Id";
            var rowsAffected = await _connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            string query = "SELECT COUNT(1) FROM user WHERE username = @Username";
            var count = await _connection.ExecuteScalarAsync<int>(query, new { Username = username });
            return count > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            string query = "SELECT COUNT(1) FROM user WHERE email = @Email";
            var count = await _connection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            string query = @"
                SELECT 
                    r.roleId, 
                    r.roleName, 
                    r.description, 
                    r.dCreated, 
                    r.dLastUpdated, 
                    r.lActive
                FROM role r
                INNER JOIN user_role ur ON r.roleId = ur.roleId
                WHERE ur.userId = @UserId AND ur.lActive = 1";

            return await _connection.QueryAsync<Role>(query, new { UserId = userId });
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            string query = @"
                INSERT INTO user_role (
                    userId, 
                    roleId, 
                    dCreated, 
                    lActive
                ) 
                VALUES (
                    @UserId, 
                    @RoleId, 
                    NOW(), 
                    1
                )";

            var rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            string query = "DELETE FROM user_role WHERE userId = @UserId AND roleId = @RoleId";
            var rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            return rowsAffected > 0;
        }
    }
}