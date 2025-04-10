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
        
        public async Task InsertAsync(DoctorSpecialtyDTO doctor)
        {
            string query = @"
                INSERT INTO doctor_specialty (
                    doctorId, 
                    specialtyId, 
                    dCreated, 
                    lActive
                ) 
                VALUES (
                    @DoctorId, 
                    @SpecialtyId, 
                    NOW(), 
                    1
                )";

            var parameters = doctor.SpecialtyId.Select(specialtyId => new
            {
                DoctorId = doctor.DoctorId,
                SpecialtyId = specialtyId
            });

            await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task DeleteByDoctorIdAsync(int doctorId)
        {
            string query = "DELETE FROM doctor_specialty WHERE doctorId = @DoctorId";
            await _connection.ExecuteAsync(query, new { DoctorId = doctorId });
        }
        
        public async Task<bool> ToggleActiveAsync(int doctorId, int specialtyId, bool active)
        {
            string query = @"
                UPDATE doctor_specialty
                SET 
                    lActive = @Active,
                    dLastUpdated = NOW()
                WHERE doctorId = @DoctorId AND specialtyId = @SpecialtyId";
            
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
                WHERE doctorId = @DoctorId AND specialtyId = @SpecialtyId";
            
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