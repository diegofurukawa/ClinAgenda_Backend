using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly MySqlConnection _connection;

        public DoctorRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<(int total, IEnumerable<DoctorListDTO> doctors)> GetDoctorsAsync(
            string? doctorName, 
            int? specialtyId, 
            int? statusId,
            bool? lActive, 
            int offset, 
            int itemsPerPage
            )
        {
            // int offset = (page - 1) * itemsPerPage;

            var queryBase = new StringBuilder(@"
                FROM doctor D
                LEFT JOIN status S ON S.statusId = D.statusId
                LEFT JOIN doctor_specialty DS ON DS.doctorId = D.doctorId
                WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(doctorName))
            {
                queryBase.Append(" AND D.doctorName LIKE @DoctorName");
                parameters.Add("doctorName", $"%{doctorName}%");
            }

            if (specialtyId.HasValue)
            {
                queryBase.Append(" AND DS.specialtyId = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (statusId.HasValue)
            {
                queryBase.Append(" AND D.statusId = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }
            
            if (lActive.HasValue)
            {
                queryBase.Append(" AND D.lActive = @lActive");
                parameters.Add("lActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT D.doctorId) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);
            
            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", offset);

            var query = $@"
                SELECT DISTINCT
                    D.doctorId,
                    D.doctorName,
                    S.statusId,
                    S.statusName,
                    D.lActive
                {queryBase}
                ORDER BY D.doctorId
                LIMIT @Limit OFFSET @Offset
                ";

            var doctors = await _connection.QueryAsync<DoctorListDTO>(query, parameters);

            return (total, doctors);
        }

        public async Task<IEnumerable<SpecialtyDoctorDTO>> GetDoctorSpecialtiesAsync(int[] doctorIds)
        {
            var query = @"
                SELECT 
                    DS.doctorId,
                    SP.specialtyId,
                    SP.specialtyName,
                    SP.nScheduleDuration,
                    DS.lActive
                FROM doctor_specialty DS
                INNER JOIN specialty SP ON DS.specialtyId = SP.specialtyId
                WHERE DS.doctorId IN @DoctorIds";

            var parameters = new { DoctorIds = doctorIds };

            return await _connection.QueryAsync<SpecialtyDoctorDTO>(query, parameters);
        }
        
        public async Task<int> InsertDoctorAsync(DoctorInsertDTO doctor)
        {
            string query = @"
            INSERT INTO doctor (
                doctorName, 
                statusId, 
                dCreated, 
                lActive
            ) 
            VALUES (
                @DoctorName, 
                @StatusId, 
                NOW(),
                1
            );
            SELECT LAST_INSERT_ID();";
                    
            return await _connection.ExecuteScalarAsync<int>(query, doctor);
        }
        
        public async Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int doctorId)
        {
            var query = @"
                SELECT 
                    D.doctorId,
                    D.doctorName,
                    D.statusId,
                    S.statusName,
                    D.lActive
                FROM doctor D
                LEFT JOIN status S ON S.statusId = D.statusId
                WHERE D.doctorId = @DoctorId";

            var parameters = new { DoctorId = doctorId };

            return await _connection.QueryAsync<DoctorListDTO>(query, parameters);
        }
        
        public async Task<bool> UpdateDoctorByIdAsync(DoctorDTO doctor)
        {
            string query = @"
            UPDATE doctor SET 
                doctorName = @DoctorName,
                statusId = @StatusId,
                dLastUpdated = NOW(),
                lActive = @lActive
            WHERE doctorId = @DoctorId";
            
            int rowsAffected = await _connection.ExecuteAsync(query, doctor);
            return rowsAffected > 0;
        }
        
        public async Task<bool> ToggleDoctorActiveAsync(int doctorId, bool active)
        {
            string query = @"
            UPDATE doctor 
            SET 
                lActive = @Active,
                dLastUpdated = NOW()
            WHERE doctorId = @DoctorId";

            var parameters = new { DoctorId = doctorId, Active = active };
            
            int rowsAffected = await _connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }
        
        public async Task<int> DeleteDoctorByIdAsync(int doctorId)
        {
            string query = "DELETE FROM doctor WHERE doctorId = @DoctorId";

            var parameters = new { DoctorId = doctorId };

            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}