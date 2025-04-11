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

        public async Task<SpecialtyDTO> GetSpecialtyByIdAsync(int SpecialtyId)
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

            var parameters = new { SpecialtyId = SpecialtyId };
            
            var specialty = await _connection.QueryFirstOrDefaultAsync<SpecialtyDTO>(query, parameters);

            return specialty;
        }

        public async Task<(int total, IEnumerable<SpecialtyDTO> specialties)> GetAllSpecialtyAsync(
            string? specialtyName = null, 
            bool? lActive = null, 
            int? itemsPerPage = 10, 
            int? page = 1)
        {
            var queryBase = new StringBuilder(@"
                FROM specialty S WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(specialtyName))
            {
                queryBase.Append(" AND S.specialtyName LIKE @SpecialtyName");
                parameters.Add("specialtyName", $"%{specialtyName}%");
            }

            if (lActive.HasValue)
            {
                queryBase.Append(" AND S.lActive = @lActive");
                parameters.Add("lActive", lActive.Value);
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
                nScheduleDuration,
                dCREATED, 
                lActive
            ) 
            VALUES (
                @SpecialtyName, 
                @nScheduleDuration,
                NOW(),
                @lActive
            );
            SELECT LAST_INSERT_ID();";

            return await _connection.ExecuteScalarAsync<int>(query, specialtyInsertDTO);
        }

        public async Task<int> UpdateSpecialtyAsync(int SpecialtyId, SpecialtyInsertDTO specialtyInsertDTO)
        {
            string query = @"
            UPDATE specialty 
            SET 
                SpecialtyName = @SpecialtyName, 
                nScheduleDuration = @nScheduleDuration, 
                dLastUpdated = NOW(), 
                lActive = @lActive
            WHERE SpecialtyId = @SpecialtyId";

            var parameters = new DynamicParameters(specialtyInsertDTO);
            parameters.Add("SpecialtyId", SpecialtyId);

            return await _connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> ToggleSpecialtyActiveAsync(int specialtyId, bool active)
        {
            string query = @"
            UPDATE specialty 
            SET 
                lActive = @Active,
                dLastUpdated = NOW()
            WHERE SpecialtyId = @SpecialtyId";

            var parameters = new { SpecialtyId = specialtyId, Active = active };

            return await _connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> DeleteSpecialtyAsync(int specialtyId)
        {
            string query = "DELETE FROM specialty WHERE SpecialtyId = @SpecialtyId";

            var parameters = new { SpecialtyId = specialtyId };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }

        public async Task<IEnumerable<SpecialtyDTO>> GetSpecialtiesByIds(List<int> specialtiesId)
        {
            var query = @"
                select 
                    s.SpecialtyId, 
                    s.SpecialtyName,
                    s.nscheduleduration 
                from specialty s
                where s.SpecialtyId in @specialtiesid";

            var parameters = new { SpecialtiesID = specialtiesId };

            return await _connection.QueryAsync<SpecialtyDTO>(query, parameters);
        }
    }
}