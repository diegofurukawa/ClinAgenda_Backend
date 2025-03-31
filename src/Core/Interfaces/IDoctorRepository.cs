using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorRepository
    {
        /// <summary>
        /// Obtém uma lista paginada de médicos com filtros opcionais
        /// </summary>
        Task<IEnumerable<DoctorListDTO>> GetDoctorsAsync(
            string? name, 
            int? specialtyId, 
            int? statusId, 
            bool? isActive,
            int offset, 
            int pageSize);
            
        /// <summary>
        /// Obtém as especialidades de um conjunto de médicos
        /// </summary>
        Task<IEnumerable<DoctorSpecialtyDTO>> GetDoctorSpecialtyAsync(int[] doctorIds);
        
        /// <summary>
        /// Insere um novo médico
        /// </summary>
        Task<int> InsertDoctorAsync(DoctorInsertDTO doctor);
        
        /// <summary>
        /// Obtém um médico por ID
        /// </summary>
        Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int id);
        
        /// <summary>
        /// Atualiza um médico existente
        /// </summary>
        Task<bool> UpdateDoctorAsync(int id, DoctorInsertDTO doctor);
        
        /// <summary>
        /// Exclui um médico por ID
        /// </summary>
        Task<int> DeleteDoctorByIdAsync(int id);
        
        /// <summary>
        /// Ativa ou desativa um médico
        /// </summary>
        Task<int> ToggleDoctorActiveAsync(int id, bool active);
    }
}