using System.Text; 
using ClinAgenda.src.Application.DTOs.Status; 
using ClinAgenda.src.Core.Interfaces; 
using Dapper; 
using MySql.Data.MySqlClient; 
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly MySqlConnection _connection;

        public StatusRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<StatusDTO> GetStatusByIdAsync(int statusId)
        {
            string query = @"
            SELECT 
                StatusId, 
                StatusName,
                StatusType,
                DCreated,
                DLastUpdated,
                lActive
            FROM status
            WHERE StatusId = @StatusId";

            var parameters = new { StatusId = statusId };

            var status = await _connection.QueryFirstOrDefaultAsync<StatusDTO>(query, parameters);

            return status;
        }

        public async Task<int> DeleteStatusAsync(int statusId)
        {
            string query = @"
            DELETE FROM status
            WHERE StatusId = @StatusId";

            var parameters = new { StatusId = statusId };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }

        public async Task<int> InsertStatusAsync(StatusInsertDTO statusInsertDTO)
        {
            try
            {
                _connection.Open(); // Garantir que a conexão está aberta
                
                string query = @"
                INSERT INTO status (
                    statusName, 
                    statusType, 
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir status: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
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
                queryBase.Append(" AND S.lActive = @lActive");
                parameters.Add("lActive", lActive.Value);
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
                S.lActive
            {queryBase}
            ORDER BY S.StatusId
            LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", ((page ?? 1) - 1) * (itemsPerPage ?? 10));

            var status = await _connection.QueryAsync<StatusDTO>(dataQuery, parameters);

            return (total, status);
        }

        public async Task<int> UpdateStatusAsync(int statusId, StatusInsertDTO statusInsertDTO)
        {
            string query = @"
            UPDATE status 
            SET 
                StatusName = @StatusName, 
                StatusType = @StatusType, 
                dlastupdated = NOW(), 
                lActive = @lActive
            WHERE StatusId = @StatusId";

            var parameters = new DynamicParameters(statusInsertDTO);
            parameters.Add("StatusId", statusId);

            return await _connection.ExecuteAsync(query, parameters);
        }

        // Implementação correta do método ToggleStatusActiveAsync
        public async Task<int> ToggleStatusActiveAsync(int statusId, bool active)
        {
            string query = @"
            UPDATE status 
            SET 
                lActive = @Active,
                dlastupdated = NOW()
            WHERE StatusId = @StatusId";

            var parameters = new { StatusId = statusId, Active = active };

            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}