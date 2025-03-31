using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Core.Interfaces;
using ClinAgenda.src.Core.Entities;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly MySqlConnection _connection;

        public SpecialtyRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<SpecialtyDTO> GetSpecialtyByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    SpecialtyId, 
                    SpecialtyName,
                    nScheduleDuration,
                    dCreated,
                    dLastUpdated,
                    lActive
                FROM specialty
                WHERE SpecialtyId = @SpecialtyId";

            var parameters = new { Id = id };
            
            var specialty = await _connection.QueryFirstOrDefaultAsync<SpecialtyDTO>(query, parameters);

            return specialty;
        }

        public async Task<(int total, IEnumerable<SpecialtyDTO> specialties)> GetAllSpecialtyAsync(
            string? name = null, 
            bool? lActive = null, 
            int? itemsPerPage = 10, 
            int? page = 1)
        {
            var queryBase = new StringBuilder(@"
                FROM specialty S WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(name))
            {
                queryBase.Append(" AND S.STATUSNAME LIKE @StatusName");
                parameters.Add("Name", $"%{name}%");
            }

            if (lActive.HasValue)
            {
                queryBase.Append(" AND S.LActive = @LActive");
                parameters.Add("LActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT S.SpecialtyId) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
            SELECT 
                S.SpecialtyId, 
                S.SpecialtyName,
                S.nScheduleDuration,
                S.DCreated,
                S.dLastUpdated,
                S.lActive
            {queryBase}
            ORDER BY S.SpecialtyId
            LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", ((page ?? 1) - 1) * (itemsPerPage ?? 10));

            var specialties = await _connection.QueryAsync<SpecialtyDTO>(dataQuery, parameters);

            return (total, specialties);
        }

        public async Task<int> InsertSpecialtyAsync(SpecialtyInsertDTO specialtyInsertDTO)
        {
            string query = @"
            INSERT INTO specialty (
                SpecialtyName, 
                NScheduleDuration,
                dCREATED, 
                lActive
            ) 
            VALUES (
                @SpecialtyName, 
                @NScheduleDuration,
                NOW(),
                @LActive
            );
            SELECT LAST_INSERT_ID();";

            return await _connection.ExecuteScalarAsync<int>(query, specialtyInsertDTO);
        }

        public async Task<int> UpdateSpecialtyAsync(int id, SpecialtyInsertDTO specialtyInsertDTO)
        {
            string query = @"
            UPDATE specialty 
            SET 
                SpecialtyName = @SpecialtyName, 
                NScheduleDuratio = @NScheduleDuration, 
                dLastUpdated = NOW(), 
                LActive = @LActive
            WHERE ID = @SpecialtyId";

            var parameters = new DynamicParameters(specialtyInsertDTO);
            parameters.Add("Id", id);

            return await _connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> ToggleSpecialtyActiveAsync(int id, bool active)
        {
            string query = @"
            UPDATE specialty 
            SET 
                lActive = @Active,
                dLastUpdated = NOW()
            WHERE SpecialtyId = @SpecialtyId";

            var parameters = new { Id = id, Active = active };

            return await _connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> DeleteSpecialtyAsync(int id)
        {
            string query = "DELETE FROM specialty WHERE SpecialtyId = @SpecialtyId";

            var parameters = new { Id = id };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }
    }
}