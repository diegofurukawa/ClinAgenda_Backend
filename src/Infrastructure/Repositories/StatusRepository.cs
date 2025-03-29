using System.Text; // Importa funcionalidades para manipulação de strings, incluindo StringBuilder.
using ClinAgenda.src.Application.DTOs.Status; // Importa os DTOs relacionados a Status.
using ClinAgenda.src.Core.Interfaces; // Importa a interface que o repositório implementa.
using Dapper; // Biblioteca para acesso a banco de dados de forma simplificada.
using MySql.Data.MySqlClient; // Biblioteca para conexão com MySQL.

namespace ClinAgenda.src.Infrastructure.Repositories
{
    // Implementação do repositório de Status, seguindo a interface IStatusRepository.
    public class StatusRepository : IStatusRepository
    {
        private readonly MySqlConnection _connection; // Conexão com o banco de dados.

        // Construtor que recebe a conexão com o banco de dados via injeção de dependência.
        public StatusRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        // Método assíncrono para buscar um status pelo ID.
        public async Task<StatusDTO> GetStatusByIdAsync(int id)
        {
            // Query SQL para selecionar o status pelo ID.
            string query = @"
            SELECT ID, 
                   NAME 
            FROM status
            WHERE ID = @Id";

            var parameters = new { Id = id }; // Parâmetro da query.

            // Executa a consulta no banco e retorna o primeiro resultado ou null caso não encontre.
            var status = await _connection.QueryFirstOrDefaultAsync<StatusDTO>(query, parameters);

            return status; // Retorna o status encontrado.
        }

        // Método assíncrono para excluir um status pelo ID.
        public async Task<int> DeleteStatusAsync(int id)
        {
            // Query SQL para deletar um status pelo ID.
            string query = @"
            DELETE FROM status
            WHERE ID = @Id";

            var parameters = new { Id = id }; // Parâmetro da query.

            // Executa a query e retorna o número de linhas afetadas.
            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected; // Retorna quantas linhas foram afetadas (1 se deletou, 0 se não encontrou o ID).
        }

        // Método assíncrono para inserir um novo status no banco.
        public async Task<int> InsertStatusAsync(StatusInsertDTO statusInsertDTO)
        {
            // Query SQL para inserir um novo status e obter o ID gerado.
            string query = @"
            INSERT INTO status (NAME) 
            VALUES (@Name);
            SELECT LAST_INSERT_ID();"; // Obtém o ID do último registro inserido.

            // Executa a query e retorna o ID do novo status.
            return await _connection.ExecuteScalarAsync<int>(query, statusInsertDTO);
        }

        // Método assíncrono para obter todos os status com paginação.
        public async Task<(int total, IEnumerable<StatusDTO> specialtys)> GetAllStatusAsync(int? itemsPerPage, int? page)
        {
            // Construção dinâmica da query base.
            var queryBase = new StringBuilder(@"
                FROM status S WHERE 1 = 1"); // "1 = 1" é usado para facilitar adição de filtros dinâmicos.

            var parameters = new DynamicParameters(); // Objeto para armazenar os parâmetros da query.

            // Query para contar o número total de registros sem a paginação.
            var countQuery = $"SELECT COUNT(DISTINCT S.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            // Query para buscar os dados paginados.
            var dataQuery = $@"
            SELECT ID, 
            NAME
            {queryBase}
            LIMIT @Limit OFFSET @Offset";

            // Adiciona os parâmetros de paginação.
            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            // Executa a consulta e retorna os resultados.
            var status = await _connection.QueryAsync<StatusDTO>(dataQuery, parameters);

            return (total, status); // Retorna o total de registros e a lista de status paginada.
        }
    }
}
