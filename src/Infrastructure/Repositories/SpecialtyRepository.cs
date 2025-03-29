// Importa funcionalidades para manipulação de strings, incluindo StringBuilder.
using System.Text; 
// Importa os DTOs relacionados a Specialty.
using ClinAgenda.src.Application.DTOs.Specialty;
// Importa a interface que o repositório implementa.
using ClinAgenda.src.Core.Interfaces; 
// Biblioteca para conexão com MySQL.
using MySql.Data.MySqlClient; 
// Biblioteca para acesso a banco de dados de forma simplificada.
using Dapper;
using ClinAgenda.src.Core.Entities;


namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly MySqlConnection _connection;

        public SpecialtyRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<SpecialtyDTO> GetSpecialtyByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    ID, 
                    NAME,
                    SCHEDULEDURATION 
                FROM specialty
                WHERE ID = @Id";

            var specialty = await _connection.QueryFirstOrDefaultAsync<SpecialtyDTO>(query, new { Id = id });

            return specialty;
        }
        public async Task<(int total, IEnumerable<SpecialtyDTO> specialtys)> GetAllSpecialtyAsync(int? itemsPerPage, int? page)
        {
            var queryBase = new StringBuilder(@"
                FROM specialty S WHERE 1 = 1");

            var parameters = new DynamicParameters();

            var countQuery = $"SELECT COUNT(DISTINCT S.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            var dataQuery = $@"
            SELECT ID, 
            NAME, 
            SCHEDULEDURATION
            {queryBase}
            LIMIT @Limit OFFSET @Offset";

            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            var specialtys = await _connection.QueryAsync<SpecialtyDTO>(dataQuery, parameters);

            return (total, specialtys);
        }
        public async Task<int> InsertSpecialtyAsync(SpecialtyInsertDTO specialtyInsertDTO)
        {
            string query = @"
            INSERT INTO specialty (NAME, SCHEDULEDURATION) 
            VALUES (@Name, @Scheduleduration);
            SELECT LAST_INSERT_ID();";
            return await _connection.ExecuteScalarAsync<int>(query, specialtyInsertDTO);
        }
        public async Task<int> DeleteSpecialtyAsync(int id)
        {
            string query = @"
            DELETE FROM specialty
            WHERE ID = @Id";

            var parameters = new { Id = id };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }

        public Task<int> InsertSpecialtyAsync(Specialty specialty)
        {
            throw new NotImplementedException();
        }
    }


}