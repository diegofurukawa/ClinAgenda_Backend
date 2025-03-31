using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Appointment;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly MySqlConnection _connection;

        public AppointmentRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    ID AS AppointmentId,
                    PATIENTID AS PatientId,
                    DOCTORID AS DoctorId,
                    SPECIALTYID AS SpecialtyId,
                    STATUSID AS StatusId,
                    APPOINTMENTDATE AS DAppointmentDate,
                    OBSERVATION AS Observation,
                    D_CREATED AS DCreated,
                    D_LAST_UPDATED AS DLastUpdated,
                    L_ACTIVE AS LActive
                FROM appointment
                WHERE ID = @Id";

            var parameters = new { Id = id };
            
            var appointment = await _connection.QueryFirstOrDefaultAsync<AppointmentDTO>(query, parameters);
            
            return appointment;
        }
        
        public async Task<(int total, IEnumerable<AppointmentListDTO> appointments)> GetAllAppointmentsAsync(
            int? patientId = null, 
            int? doctorId = null, 
            int? specialtyId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? lActive = null,
            int itemsPerPage = 10, 
            int page = 1)
        {
            var queryBase = new StringBuilder(@"     
                    FROM appointment A
                    INNER JOIN patient P ON P.PATIENTID = A.PATIENTID
                    INNER JOIN doctor D ON D.DOCTORID = A.DOCTORID
                    INNER JOIN specialty SP ON SP.SPECIALTYID = A.SPECIALTYID
                    INNER JOIN status S ON S.STATUSID = A.STATUSID
                    WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (patientId.HasValue)
            {
                queryBase.Append(" AND A.PATIENTID = @PatientId");
                parameters.Add("PatientId", patientId.Value);
            }

            if (doctorId.HasValue)
            {
                queryBase.Append(" AND A.DOCTORID = @DoctorId");
                parameters.Add("DoctorId", doctorId.Value);
            }

            if (specialtyId.HasValue)
            {
                queryBase.Append(" AND A.SPECIALTYID = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (statusId.HasValue)
            {
                queryBase.Append(" AND A.STATUSID = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }
            
            if (startDate.HasValue)
            {
                queryBase.Append(" AND A.APPOINTMENTDATE >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }
            
            if (endDate.HasValue)
            {
                queryBase.Append(" AND A.APPOINTMENTDATE <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }
            
            if (lActive.HasValue)
            {
                queryBase.Append(" AND A.L_ACTIVE = @LActive");
                parameters.Add("LActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT A.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
                    SELECT 
                        A.AppointmentId, 
                        P.PatientId,
                        P.PatientName,
                        D.DoctorId,
                        D.DoctorName,
                        SP.ID AS SpecialtyId,
                        SP.SpecialtyName,
                        S.StatusId,
                        S.StatusName,
                        A.APPOINTMENTDATE AS DAppointmentDate,
                        A.OBSERVATION AS Observation,
                        A.D_CREATED AS DCreated,
                        A.D_LAST_UPDATED AS DLastUpdated,
                        A.L_ACTIVE AS LActive
                    {queryBase}
                    ORDER BY A.APPOINTMENTDATE
                    LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            var appointments = await _connection.QueryAsync<AppointmentListDTO>(dataQuery, parameters);

            return (total, appointments);
        }
        
        public async Task<int> InsertAppointmentAsync(AppointmentInsertDTO appointmentInsertDTO)
        {
            string query = @"
                INSERT INTO appointment (
                    PATIENTID, 
                    DOCTORID, 
                    SPECIALTYID, 
                    STATUSID, 
                    APPOINTMENTDATE, 
                    OBSERVATION,
                    D_CREATED,
                    L_ACTIVE
                ) 
                VALUES (
                    @PatientId, 
                    @DoctorId, 
                    @SpecialtyId, 
                    @StatusId, 
                    @DAppointmentDate, 
                    @Observation,
                    NOW(),
                    @LActive
                );
                SELECT LAST_INSERT_ID();";
                
            return await _connection.ExecuteScalarAsync<int>(query, appointmentInsertDTO);
        }
        
        public async Task<int> UpdateAppointmentAsync(int id, AppointmentInsertDTO appointmentInsertDTO)
        {
            string query = @"
                UPDATE appointment 
                SET 
                    PATIENTID = @PatientId, 
                    DOCTORID = @DoctorId, 
                    SPECIALTYID = @SpecialtyId, 
                    STATUSID = @StatusId, 
                    APPOINTMENTDATE = @DAppointmentDate, 
                    OBSERVATION = @Observation,
                    D_LAST_UPDATED = NOW(),
                    L_ACTIVE = @LActive
                WHERE ID = @Id";

            var parameters = new DynamicParameters(appointmentInsertDTO);
            parameters.Add("Id", id);
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> ToggleAppointmentActiveAsync(int id, bool active)
        {
            string query = @"
                UPDATE appointment 
                SET 
                    L_ACTIVE = @Active,
                    D_LAST_UPDATED = NOW()
                WHERE ID = @Id";

            var parameters = new { Id = id, Active = active };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> DeleteAppointmentAsync(int id)
        {
            string query = @"
                DELETE FROM appointment
                WHERE ID = @Id";

            var parameters = new { Id = id };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<bool> CheckConflictingAppointmentsAsync(
            int doctorId, 
            int? appointmentId, 
            DateTime startTime, 
            DateTime endTime)
        {
            var query = @"
                SELECT COUNT(1) 
                FROM appointment A
                INNER JOIN specialty SP ON SP.ID = A.SPECIALTYID
                WHERE A.DOCTORID = @DoctorId
                AND A.L_ACTIVE = 1
                AND (
                    (@StartTime BETWEEN A.APPOINTMENTDATE AND DATE_ADD(A.APPOINTMENTDATE, INTERVAL SP.SCHEDULEDURATION MINUTE))
                    OR
                    (@EndTime BETWEEN A.APPOINTMENTDATE AND DATE_ADD(A.APPOINTMENTDATE, INTERVAL SP.SCHEDULEDURATION MINUTE))
                    OR
                    (A.APPOINTMENTDATE BETWEEN @StartTime AND @EndTime)
                )";
                
            var parameters = new DynamicParameters();
            parameters.Add("DoctorId", doctorId);
            parameters.Add("StartTime", startTime);
            parameters.Add("EndTime", endTime);
            
            // Se estivermos atualizando um agendamento existente, excluímos ele da verificação
            if (appointmentId.HasValue)
            {
                query += " AND A.ID != @AppointmentId";
                parameters.Add("AppointmentId", appointmentId.Value);
            }
            
            var count = await _connection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }
    }
}