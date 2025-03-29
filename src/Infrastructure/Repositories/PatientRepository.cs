using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        // Conexão com o banco pode ser acessada pelo UseCase para validações
public readonly MySqlConnection _connection;

        public PatientRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    p.id,
                    p.name,
                    p.phonenumber,
                    p.documentnumber,
                    p.birthdate,
                    p.statusid
                FROM patient p
                WHERE p.id = @Id";

            var parameters = new { Id = id };
            
            var patient = await _connection.QueryFirstOrDefaultAsync<PatientDTO>(query, parameters);
            
            return patient;
        }


        public async Task<(int total, IEnumerable<PatientListDTO> patient)> GetAllPatientAsync(string? name, string? documentNumber, int? statusId, int itemsPerPage, int page)
        {
            var queryBase = new StringBuilder(@"     
                    FROM patient P
                    INNER JOIN status S ON S.id = P.statusid
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
                queryBase.Append(" AND S.ID = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }

            var countQuery = $"SELECT COUNT(DISTINCT P.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
                    SELECT 
                        P.ID, 
                        P.NAME,
                        P.PHONENUMBER,
                        P.DOCUMENTNUMBER,
                        P.BIRTHDATE ,
                        P.STATUSID AS STATUSID, 
                        S.NAME AS STATUSNAME
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
                string normalizedDate = patientInsertDTO.BirthDate;
                if (DateTime.TryParse(patientInsertDTO.BirthDate, out DateTime birthDate))
                {
                    normalizedDate = birthDate.ToString("yyyy-MM-dd");
                }
                
                // Preparar os parâmetros para garantir o formato correto
                var parameters = new
                {
                    patientInsertDTO.Name,
                    patientInsertDTO.PhoneNumber,
                    patientInsertDTO.DocumentNumber,
                    BirthDate = normalizedDate,
                    patientInsertDTO.StatusId
                };
                
                string query = @"
                    INSERT INTO patient (name, phonenumber, documentnumber, birthdate, statusid) 
                    VALUES (@Name, @PhoneNumber, @DocumentNumber, @BirthDate, @StatusId);
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
            string query = @"
                UPDATE patient 
                SET name = @Name, 
                    phonenumber = @PhoneNumber, 
                    documentnumber = @DocumentNumber, 
                    birthdate = @BirthDate, 
                    statusid = @StatusId
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                patientInsertDTO.Name,
                patientInsertDTO.PhoneNumber,
                patientInsertDTO.DocumentNumber,
                patientInsertDTO.BirthDate,
                patientInsertDTO.StatusId
            };
            
            return await _connection.ExecuteAsync(query, parameters);
        }


        public async Task<int> DeletePatientAsync(int id)
        {
            string query = @"
                DELETE FROM patient
                WHERE id = @Id";

            var parameters = new { Id = id };
            
            return await _connection.ExecuteAsync(query, parameters);
        }


    }
}