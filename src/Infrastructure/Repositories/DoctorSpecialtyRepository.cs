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
            INSERT INTO DOCTOR_SPECIALTY (DoctorId, SpecialtyId) 
            VALUES (@DoctorId, @SpecialtyId);";

            var parameters = doctor.SpecialtyId.Select(specialtyId => new
            {
                DoctorId = doctor.DoctorId,
                SpecialtyId = specialtyId
            });

            await _connection.ExecuteAsync(query, parameters);
        }
        public async Task DeleteByDoctorIdAsync(int doctorId)
        {
            string query = "DELETE FROM doctor_specialty WHERE DoctorId = @DoctorId";
            await _connection.ExecuteAsync(query, new { DoctorId = doctorId });
        }
    }

}