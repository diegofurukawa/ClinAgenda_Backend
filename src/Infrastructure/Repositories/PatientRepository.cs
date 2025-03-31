using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        public readonly MySqlConnection _connection;

        public PatientRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    p.id AS PatientId,
                    p.name AS PatientName,
                    p.phonenumber AS PhoneNumber,
                    p.documentnumber AS DocumentNumber,
                    p.birthdate AS DBirthDate,
                    p.statusid AS StatusId,
                    p.d_created AS DCreated,
                    p.d_last_updated AS DLastUpdated,
                    p.l_active AS LActive
                FROM patient p
                WHERE p.id = @Id";

            var parameters = new { Id = id };
            
            var patient = await _connection.QueryFirstOrDefaultAsync<PatientDTO>(query, parameters);
            
            return patient;
        }

        public async Task<(int total, IEnumerable<PatientListDTO> patients)> GetAllPatientAsync(
            string? name, 
            string? documentNumber, 
            int? statusId,
            bool? lActive,
            int itemsPerPage, 
            int page)
        {
            var queryBase = new StringBuilder(@"     
                    FROM patient P
                    INNER JOIN status S ON S.statusid = P.statusid
                    WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(name))
            {
                queryBase.Append(" AND P.NAME LIKE @Name");
                parameters.Add("Name", $"%{name}%");
            }

            if (!string.IsNullOrEmpty(documentNumber))
            {
                queryBase.Append(" AND P.DOCUMENTNUMBER LIKE @DocumentNumber");
                parameters.Add("DocumentNumber", $"%{documentNumber}%");
            }

            if (statusId.HasValue)
            {
                queryBase.Append(" AND S.statusid = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }
            
            if (lActive.HasValue)
            {
                queryBase.Append(" AND P.L_ACTIVE = @LActive");
                parameters.Add("LActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT P.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
                    SELECT 
                        P.PatientId, 
                        P.PatientName,
                        P.PHONENUMBER AS PhoneNumber,
                        P.DOCUMENTNUMBER AS DocumentNumber,
                        P.DBirthDate,
                        P.StatusId, 
                        S.StatusName,
                        P.D_CREATED AS DCreated,
                        P.D_LAST_UPDATED AS DLastUpdated,
                        P.L_ACTIVE AS LActive
                    {queryBase}
                    ORDER BY P.ID
                    LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            var patients = await _connection.QueryAsync<PatientListDTO>(dataQuery, parameters);

            return (total, patients);
        }

        public async Task<int> InsertPatientAsync(PatientInsertDTO patientInsertDTO)
        {
            try
            {
                // Verificar e normalizar a data se necessário
                string normalizedDate = patientInsertDTO.DBirthDate;
                if (DateTime.TryParse(patientInsertDTO.DBirthDate, out DateTime birthDate))
                {
                    normalizedDate = birthDate.ToString("yyyy-MM-dd");
                }
                
                // Preparar os parâmetros para garantir o formato correto
                var parameters = new
                {
                    patientInsertDTO.PatientName,
                    patientInsertDTO.PhoneNumber,
                    patientInsertDTO.DocumentNumber,
                    DBirthDate = normalizedDate,
                    patientInsertDTO.StatusId,
                    patientInsertDTO.LActive
                };
                
                string query = @"
                    INSERT INTO patient (
                        name, 
                        phonenumber, 
                        documentnumber, 
                        birthdate, 
                        statusid, 
                        d_created, 
                        l_active
                    ) 
                    VALUES (
                        @PatientName, 
                        @PhoneNumber, 
                        @DocumentNumber, 
                        @DBirthDate, 
                        @StatusId, 
                        NOW(), 
                        @LActive
                    );
                    SELECT LAST_INSERT_ID();";
                    
                return await _connection.ExecuteScalarAsync<int>(query, parameters);
            }
            catch (Exception ex)
            {
                // Melhorar a mensagem de erro para capturar problemas relacionados à data
                if (ex.Message.Contains("Incorrect date value") || ex.Message.Contains("birthdate"))
                {
                    throw new Exception($"Formato de data inválido para o campo birthDate. Use o formato YYYY-MM-DD. Erro original: {ex.Message}");
                }
                throw;
            }
        }

        public async Task<int> UpdatePatientAsync(int id, PatientInsertDTO patientInsertDTO)
        {
            try
            {
                // Verificar e normalizar a data se necessário
                string normalizedDate = patientInsertDTO.DBirthDate;
                if (DateTime.TryParse(patientInsertDTO.DBirthDate, out DateTime birthDate))
                {
                    normalizedDate = birthDate.ToString("yyyy-MM-dd");
                }
                
                string query = @"
                    UPDATE patient 
                    SET 
                        name = @PatientName, 
                        phonenumber = @PhoneNumber, 
                        documentnumber = @DocumentNumber, 
                        birthdate = @DBirthDate, 
                        statusid = @StatusId,
                        d_last_updated = NOW(),
                        l_active = @LActive
                    WHERE id = @Id";

                var parameters = new
                {
                    Id = id,
                    patientInsertDTO.PatientName,
                    patientInsertDTO.PhoneNumber,
                    patientInsertDTO.DocumentNumber,
                    DBirthDate = normalizedDate,
                    patientInsertDTO.StatusId,
                    patientInsertDTO.LActive
                };
                
                return await _connection.ExecuteAsync(query, parameters);
            }
            catch (Exception ex)
            {
                // Melhorar a mensagem de erro para capturar problemas relacionados à data
                if (ex.Message.Contains("Incorrect date value") || ex.Message.Contains("birthdate"))
                {
                    throw new Exception($"Formato de data inválido para o campo birthDate. Use o formato YYYY-MM-DD. Erro original: {ex.Message}");
                }
                throw;
            }
        }

        public async Task<int> DeletePatientAsync(int id)
        {
            string query = @"
                DELETE FROM patient
                WHERE id = @Id";

            var parameters = new { Id = id };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> TogglePatientActiveAsync(int id, bool active)
        {
            string query = @"
                UPDATE patient 
                SET 
                    l_active = @Active,
                    d_last_updated = NOW()
                WHERE id = @Id";

            var parameters = new { Id = id, Active = active };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}