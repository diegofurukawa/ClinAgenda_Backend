using System;
using System.Collections.Generic;
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
        
        public async Task<IEnumerable<DoctorListDTO>> GetDoctorsAsync(
            string? name, 
            int? specialtyId, 
            int? statusId,
            bool? isActive,
            int offset, 
            int pageSize)
        {
            var innerJoins = new StringBuilder(@"
                FROM DOCTOR D
                INNER JOIN STATUS S ON D.STATUSID = S.STATUSID
                INNER JOIN DOCTOR_SPECIALTY DSPE ON DSPE.DOCTORID = D.DOCTORID
                WHERE 1 = 1 ");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(name))
            {
                innerJoins.Append(" AND D.NAME LIKE @Name");
                parameters.Add("Name", $"%{name}%");
            }

            if (specialtyId.HasValue)
            {
                innerJoins.Append(" AND DSPE.SPECIALTYID = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (statusId.HasValue)
            {
                innerJoins.Append(" AND S.StatusId = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }
            
            if (isActive.HasValue)
            {
                innerJoins.Append(" AND D.L_ACTIVE = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            parameters.Add("Limit", pageSize);
            parameters.Add("Offset", offset);

            var query = $@"
                SELECT DISTINCT
                    D.DOCTORID AS Id,
                    D.DOCTORNAME AS Name,
                    S.StatusId,
                    S.StatusName,
                    D.D_CREATED AS DCreated,
                    D.D_LAST_UPDATED AS DLastUpdated,
                    D.L_ACTIVE AS LActive
                {innerJoins}
                ORDER BY D.ID
                LIMIT @Limit OFFSET @Offset";

            return await _connection.QueryAsync<DoctorListDTO>(query.ToString(), parameters);
        }

        public async Task<IEnumerable<DoctorSpecialtyDTO>> GetDoctorSpecialtyAsync(int[] doctorIds)
        {
            var query = @"
                SELECT 
                    DS.DOCTORID AS DoctorId,
                    SP.SpecialtyId,
                    SP.SpecialtyName,
                    SP.SCHEDULEDURATION AS NScheduleDuration,
                    SP.D_CREATED AS DCreated,
                    SP.D_LAST_UPDATED AS DLastUpdated,
                    SP.L_ACTIVE AS LActive
                FROM DOCTOR_SPECIALTY DS
                INNER JOIN SPECIALTY SP ON DS.SPECIALTYID = SP.SPECIALTYID
                WHERE DS.DOCTORID IN @DoctorIds";

            var parameters = new { DoctorIds = doctorIds };

            return await _connection.QueryAsync<DoctorSpecialtyDTO>(query, parameters);
        }
        
        public async Task<int> InsertDoctorAsync(DoctorInsertDTO doctor)
        {
            string query = @"
            INSERT INTO Doctor (
                NAME, 
                STATUSID, 
                D_CREATED, 
                L_ACTIVE
            ) 
            VALUES (
                @DoctorName, 
                @StatusId, 
                NOW(), 
                @LActive
            );
            SELECT LAST_INSERT_ID();";
            
            int newDoctorId = await _connection.ExecuteScalarAsync<int>(query, doctor);

            // Inserir especialidades para o novo médico
            if (doctor.Specialty != null && doctor.Specialty.Count > 0)
            {
                var specialtyInserts = new List<dynamic>();
                foreach (var specialtyId in doctor.Specialty)
                {
                    specialtyInserts.Add(new
                    {
                        DoctorId = newDoctorId,
                        SpecialtyId = specialtyId,
                        LActive = true
                    });
                }

                string specialtyQuery = @"
                INSERT INTO DOCTOR_SPECIALTY (
                    DOCTORID,
                    SPECIALTYID,
                    D_CREATED,
                    L_ACTIVE
                ) VALUES (
                    @DoctorId,
                    @SpecialtyId,
                    NOW(),
                    @LActive
                )";

                await _connection.ExecuteAsync(specialtyQuery, specialtyInserts);
            }

            return newDoctorId;
        }
        
        public async Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int id)
        {
            var queryBase = new StringBuilder(@"
                    FROM DOCTOR D
                LEFT JOIN DOCTOR_SPECIALTY DSPE ON D.DOCTORID = DSPE.DOCTORID
                LEFT JOIN STATUS S ON S.statusid = D.STATUSID
                LEFT JOIN SPECIALTY SP ON SP.ID = DSPE.SPECIALTYID
                    WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (id > 0)
            {
                queryBase.Append(" AND D.DOCTORID = @id");
                parameters.Add("id", id);
            }

            var dataQuery = $@"
        SELECT DISTINCT
            D.DOCTORID, 
            D.DOCTORNAME, 
            D.StatusId, 
            S.StatusName,
            DSPE.SpecialtyId,
            SP.NAME AS SpecialtyName,
            SP.SCHEDULEDURATION AS NScheduleDuration,
            D.D_CREATED AS DCreated,
            D.D_LAST_UPDATED AS DLastUpdated,
            D.L_ACTIVE AS LActive
        {queryBase}
        ORDER BY D.DOCTORID";

            var doctors = await _connection.QueryAsync<DoctorListDTO>(dataQuery, parameters);

            return doctors;
        }
        
        public async Task<bool> UpdateDoctorAsync(int id, DoctorInsertDTO doctor)
        {
            // Atualizar dados básicos do médico
            string query = @"
            UPDATE Doctor SET 
                NAME = @DoctorName,
                STATUSID = @StatusId,
                D_LAST_UPDATED = NOW(),
                L_ACTIVE = @LActive
            WHERE ID = @Id";
            
            var parameters = new DynamicParameters(doctor);
            parameters.Add("Id", id);
            
            int rowsAffected = await _connection.ExecuteAsync(query, parameters);
            
            if (rowsAffected <= 0)
                return false;

            // Remover especialidades existentes
            await _connection.ExecuteAsync(
                "DELETE FROM DOCTOR_SPECIALTY WHERE DOCTORID = @DoctorId", 
                new { DoctorId = id });

            // Adicionar novas especialidades
            if (doctor.Specialty != null && doctor.Specialty.Count > 0)
            {
                var specialtyInserts = new List<dynamic>();
                foreach (var specialtyId in doctor.Specialty)
                {
                    specialtyInserts.Add(new
                    {
                        DoctorId = id,
                        SpecialtyId = specialtyId,
                        LActive = true
                    });
                }

                string specialtyQuery = @"
                INSERT INTO DOCTOR_SPECIALTY (
                    DOCTORID,
                    SPECIALTYID,
                    D_CREATED,
                    L_ACTIVE
                ) VALUES (
                    @DoctorId,
                    @SpecialtyId,
                    NOW(),
                    @LActive
                )";

                await _connection.ExecuteAsync(specialtyQuery, specialtyInserts);
            }
            
            return true;
        }
        
        public async Task<int> DeleteDoctorByIdAsync(int id)
        {
            // Primeiro excluir as relações de especialidade
            await _connection.ExecuteAsync(
                "DELETE FROM DOCTOR_SPECIALTY WHERE DOCTORID = @Id", 
                new { Id = id });
                
            // Então excluir o médico
            string query = "DELETE FROM DOCTOR WHERE ID = @Id";
            var parameters = new { Id = id };
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> ToggleDoctorActiveAsync(int id, bool active)
        {
            string query = @"
            UPDATE Doctor SET 
                L_ACTIVE = @Active,
                D_LAST_UPDATED = NOW()
            WHERE ID = @Id";

            var parameters = new { Id = id, Active = active };
            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}