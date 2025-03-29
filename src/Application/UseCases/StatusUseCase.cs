using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.StatusUseCase
{
    public class StatusUseCase
    {
        // Declaração de um campo somente leitura (_statusRepository) que armazenará a instância do repositório de status.
        private readonly IStatusRepository _statusRepository;

        // Construtor da classe StatusUseCase, que recebe uma implementação de IStatusRepository como dependência.
        public StatusUseCase(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository; // Atribui o repositório injetado ao campo privado.
        }

        // Método assíncrono que obtém uma lista paginada de status.
        public async Task<object> GetStatusAsync(int itemsPerPage, int page)
        {
            // Chama o repositório para obter os dados paginados e o total de registros.
            var (total, rawData) = await _statusRepository.GetAllStatusAsync(itemsPerPage, page);

            // Retorna um objeto anônimo contendo o total de itens e a lista de status formatada.
            return new
            {
                total,
                items = rawData.ToList()
            };
        }

        // Método assíncrono que obtém um status pelo seu ID.
        public async Task<StatusDTO?> GetStatusByIdAsync(int id)
        {
            // Chama o repositório para buscar um status específico pelo ID.
            return await _statusRepository.GetStatusByIdAsync(id);
        }

        // Método assíncrono que cria um novo status e retorna o ID gerado.
        public async Task<int> CreateStatusAsync(StatusInsertDTO statusDTO)
        {
            // Cria uma nova instância de StatusInsertDTO com os dados fornecidos.
            var status = new StatusInsertDTO
            {
                StatusName = statusDTO.StatusName
            };

            // Chama o repositório para inserir o novo status e obtém o ID gerado.
            var newStatusId = await _statusRepository.InsertStatusAsync(status);

            return newStatusId; // Retorna o ID do novo status criado.
        }
    }
}
