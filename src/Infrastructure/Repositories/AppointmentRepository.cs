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
        
        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int appointmentId)
        {
            string query = @"
                SELECT 
                    AppointmentId,
                    PATIENTID AS PatientId,
                    DOCTORID AS DoctorId,
                    SPECIALTYID AS SpecialtyId,
                    STATUSID AS StatusId,
                    APPOINTMENTDATE AS DAppointmentDate,
                    OBSERVATION AS Observation,
                    DCreated AS DCreated,
                    dlastupdated AS DLastUpdated,
                    lActive AS LActive
                FROM appointment
                WHERE ID = @Id";

            var parameters = new { AppointmentId = appointmentId };
            
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
                queryBase.Append(" AND A.dAppointmentDate >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }
            
            if (endDate.HasValue)
            {
                queryBase.Append(" AND A.dAppointmentDate <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }
            
            if (lActive.HasValue)
            {
                queryBase.Append(" AND A.lActive = @LActive");
                parameters.Add("LActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT A.AppointmentId) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
                    SELECT 
                        A.AppointmentId, 
                        P.PatientId,
                        P.PatientName,
                        D.DoctorId,
                        D.DoctorName,
                        SP.SpecialtyId,
                        SP.SpecialtyName,
                        S.StatusId,
                        S.StatusName,
                        A.dAppointmentDate,
                        A.OBSERVATION AS Observation,
                        A.DCreated AS DCreated,
                        A.dlastupdated AS DLastUpdated,
                        A.lActive AS LActive
                    {queryBase}
                    ORDER BY A.dAppointmentDate
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
                    DCreated,
                    lActive
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
        
        public async Task<int> UpdateAppointmentAsync(int appointmentId, AppointmentInsertDTO appointmentInsertDTO)
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
                    dlastupdated = NOW(),
                    lActive = @LActive
                WHERE AppointmentId = @AppointmentId";

            var parameters = new DynamicParameters(appointmentInsertDTO);
            parameters.Add("AppointmentId", appointmentId);
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> ToggleAppointmentActiveAsync(int appointmentId, bool active)
        {
            string query = @"
                UPDATE appointment 
                SET 
                    lActive = @Active,
                    dlastupdated = NOW()
                WHERE AppointmentId = @AppointmentId";

            var parameters = new { AppointmentId = appointmentId, Active = active };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> DeleteAppointmentAsync(int appointmentId)
        {
            string query = @"
                DELETE FROM appointment
                WHERE AppointmentId = @AppointmentId";

            var parameters = new { AppointmentId = appointmentId };
            
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
                INNER JOIN specialty SP ON SP.SPECIALTYID = A.SPECIALTYID
                WHERE A.DOCTORID = @DoctorId
                AND A.lActive = 1
                AND (
                    (@StartTime BETWEEN A.dAppointmentDate AND DATE_ADD(A.dAppointmentDate, INTERVAL SP.SCHEDULEDURATION MINUTE))
                    OR
                    (@EndTime BETWEEN A.dAppointmentDate AND DATE_ADD(A.dAppointmentDate, INTERVAL SP.SCHEDULEDURATION MINUTE))
                    OR
                    (A.dAppointmentDate BETWEEN @StartTime AND @EndTime)
                )";
                
            var parameters = new DynamicParameters();
            parameters.Add("DoctorId", doctorId);
            parameters.Add("StartTime", startTime);
            parameters.Add("EndTime", endTime);
            
            // Se estivermos atualizando um agendamento existente, excluímos ele da verificação
            if (appointmentId.HasValue)
            {
                query += " AND A.AppointmentId != @AppointmentId";
                parameters.Add("AppointmentId", appointmentId.Value);
            }
            
            var count = await _connection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }
    }
}