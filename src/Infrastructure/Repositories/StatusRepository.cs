using System.Text; 
using ClinAgenda.src.Application.DTOs.Status; 
using ClinAgenda.src.Core.Interfaces; 
using Dapper; 
using MySql.Data.MySqlClient; 
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly MySqlConnection _connection;

        public StatusRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<StatusDTO> GetStatusByIdAsync(int id)
        {
            string query = @"
            SELECT 
                StatusId, 
                StatusName,
                StatusType,
                DCreated,
                DLastUpdated,
                LActive
            FROM status
            WHERE StatusId = @StatusId";

            var parameters = new { Id = id };

            var status = await _connection.QueryFirstOrDefaultAsync<StatusDTO>(query, parameters);

            return status;
        }

        public async Task<int> DeleteStatusAsync(int id)
        {
            string query = @"
            DELETE FROM status
            WHERE StatusId = @StatusId";

            var parameters = new { Id = id };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }

        public async Task<int> InsertStatusAsync(StatusInsertDTO statusInsertDTO)
        {
            string query = @"
            INSERT INTO status (
                StatusName, 
                StatusType, 
                dCreated, 
                lActive
            ) 
            VALUES (
                @StatusName, 
                @StatusType, 
                NOW(),
                @lActive
            );
            SELECT LAST_INSERT_ID();";

            return await _connection.ExecuteScalarAsync<int>(query, statusInsertDTO);
        }

        public async Task<(int total, IEnumerable<StatusDTO> statuses)> GetAllStatusAsync(
            string? statusType = null, 
            bool? lActive = null, 
            int? itemsPerPage = 10, 
            int? page = 1)
        {
            var queryBase = new StringBuilder(@"
                FROM status S WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(statusType))
            {
                queryBase.Append(" AND S.StatusType = @StatusType");
                parameters.Add("StatusType", statusType);
            }

            if (lActive.HasValue)
            {
                queryBase.Append(" AND S.LActive = @LActive");
                parameters.Add("LActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT S.StatusId) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
            SELECT 
                S.StatusId, 
                S.StatusName,
                S.StatusType,
                S.DCreated,
                S.DLastUpdated,
                S.LActive
            {queryBase}
            ORDER BY S.StatusId
            LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", ((page ?? 1) - 1) * (itemsPerPage ?? 10));

            var status = await _connection.QueryAsync<StatusDTO>(dataQuery, parameters);

            return (total, status);
        }

        public async Task<int> UpdateStatusAsync(int id, StatusInsertDTO statusInsertDTO)
        {
            string query = @"
            UPDATE status 
            SET 
                NAME = @StatusName, 
                StatusType = @StatusType, 
                D_LAST_UPDATED = NOW(), 
                LActive = @LActive
            WHERE ID = @StatusId";

            var parameters = new DynamicParameters(statusInsertDTO);
            parameters.Add("Id", id);

            return await _connection.ExecuteAsync(query, parameters);
        }

        // Implementação correta do método ToggleStatusActiveAsync
        public async Task<int> ToggleStatusActiveAsync(int id, bool active)
        {
            string query = @"
            UPDATE status 
            SET 
                LActive = @Active,
                D_LAST_UPDATED = NOW()
            WHERE ID = @StatusId";

            var parameters = new { Id = id, Active = active };

            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}