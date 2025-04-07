using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class DoctorSpecialtyRepository : IDoctorSpecialtyRepository
    {
        private readonly MySqlConnection _connection;

        public DoctorSpecialtyRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        public async Task InsertAsync(DoctorSpecialtyDTO doctorSpecialtyDTO)
        {
            string query = @"
            INSERT INTO DOCTOR_SPECIALTY (
                DoctorId, 
                SpecialtyId, 
                DCreated, 
                lActive
            ) 
            VALUES (
                @DoctorId, 
                @SpecialtyId, 
                NOW(), 
                @LActive
            );";
            
            if (doctorSpecialtyDTO.SpecialtyId is List<int> specialtyIds)
            {
                var parameters = specialtyIds.Select(specialtyId => new
                {
                    DoctorId = doctorSpecialtyDTO.DoctorId,
                    SpecialtyId = specialtyId,
                    LActive = doctorSpecialtyDTO.lActive
                });

                await _connection.ExecuteAsync(query, parameters);
            }
            else
            {
                // Para compatibilidade com chamadas antigas que possam usar SpecialtyId como um único valor
                var parameter = new
                {
                    doctorSpecialtyDTO.DoctorId,
                    SpecialtyId = (int)doctorSpecialtyDTO.SpecialtyId,
                    doctorSpecialtyDTO.lActive
                };
                
                await _connection.ExecuteAsync(query, parameter);
            }
        }
        
        public async Task DeleteByDoctorIdAsync(int doctorId)
        {
            string query = "DELETE FROM doctor_specialty WHERE DoctorId = @DoctorId";
            await _connection.ExecuteAsync(query, new { DoctorId = doctorId });
        }
        
        public async Task<IEnumerable<DoctorSpecialtyDetailDTO>> GetByDoctorIdAsync(int doctorId)
        {
            string query = @"
            SELECT 
                DoctorId,
                SpecialtyId,
                DCreated AS DCreated,
                dlastupdated AS DLastUpdated,
                lActive AS LActive
            FROM doctor_specialty
            WHERE DoctorId = @DoctorId";
            
            return await _connection.QueryAsync<DoctorSpecialtyDetailDTO>(query, new { DoctorId = doctorId });
        }

        public async Task<IEnumerable<DoctorSpecialtyDetailDTO>> GetBySpecialtyIdAsync(int specialtyId)
        {
            string query = @"
            SELECT 
                DoctorId,
                SpecialtyId,
                DCreated AS DCreated,
                dlastupdated AS DLastUpdated,
                lActive AS LActive
            FROM doctor_specialty
            WHERE SpecialtyId = @SpecialtyId";
            
            return await _connection.QueryAsync<DoctorSpecialtyDetailDTO>(query, new { SpecialtyId = specialtyId });
        }

        public async Task<bool> ToggleActiveAsync(int doctorId, int specialtyId, bool active)
        {
            string query = @"
            UPDATE doctor_specialty
            SET 
                lActive = @Active,
                dlastupdated = NOW()
            WHERE DoctorId = @DoctorId AND SpecialtyId = @SpecialtyId";
            
            var parameters = new
            {
                DoctorId = doctorId,
                SpecialtyId = specialtyId,
                Active = active
            };
            
            int rowsAffected = await _connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int doctorId, int specialtyId)
        {
            string query = @"
            SELECT COUNT(1)
            FROM doctor_specialty
            WHERE DoctorId = @DoctorId AND SpecialtyId = @SpecialtyId";
            
            var parameters = new
            {
                DoctorId = doctorId,
                SpecialtyId = specialtyId
            };
            
            int count = await _connection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }
    }
}