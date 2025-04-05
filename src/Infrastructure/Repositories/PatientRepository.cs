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

        public async Task<PatientDTO> GetPatientByIdAsync(int patientId)
        {
            string query = @"
                SELECT 
                    p.PatientId,
                    p.name AS PatientName,
                    p.phonenumber AS PhoneNumber,
                    p.documentnumber AS DocumentNumber,
                    p.birthdate AS DBirthDate,
                    p.statusid AS StatusId,
                    p.DCreated AS DCreated,
                    p.dlastupdated AS DLastUpdated,
                    p.lActive AS LActive
                FROM patient p
                WHERE p.PatientId = @PatientId";

            var parameters = new { PatientId = patientId };
            
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
                queryBase.Append(" AND P.lActive = @LActive");
                parameters.Add("LActive", lActive.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT P.PatientId) {queryBase}";
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
                        P.DCreated AS DCreated,
                        P.dlastupdated AS DLastUpdated,
                        P.lActive AS LActive
                    {queryBase}
                    ORDER BY P.PatientId
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
                        patientname, 
                        phonenumber, 
                        documentnumber, 
                        birthdate, 
                        statusid, 
                        dCreated, 
                        lActive
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

        public async Task<int> UpdatePatientAsync(int patientId, PatientInsertDTO patientInsertDTO)
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
                        patientname = @PatientName, 
                        phonenumber = @PhoneNumber, 
                        documentnumber = @DocumentNumber, 
                        birthdate = @DBirthDate, 
                        statusid = @StatusId,
                        dlastupdated = NOW(),
                        lActive = @LActive
                    WHERE id = @PatientId";

                var parameters = new
                {
                    PatientId = patientId,
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

        public async Task<int> DeletePatientAsync(int patientId)
        {
            string query = @"
                DELETE FROM patient
                WHERE PatientId = @PatientId";

            var parameters = new { PatientId = patientId };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> TogglePatientActiveAsync(int patientId, bool active)
        {
            string query = @"
                UPDATE patient 
                SET 
                    lActive = @Active,
                    dlastupdated = NOW()
                WHERE PatientId = @PatientId";

            var parameters = new { PatientId = patientId, Active = active };
            
            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}