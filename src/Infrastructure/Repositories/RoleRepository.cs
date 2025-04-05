
// RoleRepository.cs
using ClinAgenda.src.Core.Entities;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MySqlConnection _connection;

        public RoleRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Role?> GetRoleByIdAsync(int roleid)
        {
            string query = @"
                SELECT 
                    roleId, 
                    roleName, 
                    description, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM role
                WHERE roleId = @roleId";

            return await _connection.QueryFirstOrDefaultAsync<Role>(query, new { roleId = roleid });
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            string query = @"
                SELECT 
                    roleId, 
                    roleName, 
                    description, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM role
                WHERE roleName = @Name";

            return await _connection.QueryFirstOrDefaultAsync<Role>(query, new { Name = name });
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            string query = @"
                SELECT 
                    roleId, 
                    roleName, 
                    description, 
                    dCreated, 
                    dLastUpdated, 
                    lActive
                FROM role
                WHERE lActive = 1";

            return await _connection.QueryAsync<Role>(query);
        }

        public async Task<int> CreateRoleAsync(Role role)
        {
            string query = @"
                INSERT INTO role (
                    roleName, 
                    description, 
                    dCreated, 
                    lActive
                ) 
                VALUES (
                    @RoleName, 
                    @Description, 
                    NOW(), 
                    @LActive
                );
                SELECT LAST_INSERT_ID();";

            return await _connection.ExecuteScalarAsync<int>(query, role);
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            string query = @"
                UPDATE role SET 
                    roleName = @RoleName, 
                    description = @Description, 
                    dLastUpdated = NOW(), 
                    lActive = @LActive
                WHERE roleId = @RoleId";

            var rowsAffected = await _connection.ExecuteAsync(query, role);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteRoleAsync(int roleid)
        {
            string query = "DELETE FROM role WHERE roleId = @Id";
            var rowsAffected = await _connection.ExecuteAsync(query, new { roleId = roleid });
            return rowsAffected > 0;
        }
    }
}