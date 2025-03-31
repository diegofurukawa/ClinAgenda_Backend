using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorSpecialtyRepository
    {
        /// <summary>
        /// Insere relacionamentos entre médico e especialidades
        /// </summary>
        Task InsertAsync(DoctorSpecialtyDTO doctorSpecialtyDTO);
        
        /// <summary>
        /// Exclui todos os relacionamentos de um médico
        /// </summary>
        Task DeleteByDoctorIdAsync(int doctorId);
        
        /// <summary>
        /// Obtém todas as especialidades de um médico
        /// </summary>
        Task<IEnumerable<DoctorSpecialtyDetailDTO>> GetByDoctorIdAsync(int doctorId);
        
        /// <summary>
        /// Obtém todos os médicos que possuem uma determinada especialidade
        /// </summary>
        Task<IEnumerable<DoctorSpecialtyDetailDTO>> GetBySpecialtyIdAsync(int specialtyId);
        
        /// <summary>
        /// Ativa ou desativa uma especialidade de um médico
        /// </summary>
        Task<bool> ToggleActiveAsync(int doctorId, int specialtyId, bool active);
        
        /// <summary>
        /// Verifica se existe o relacionamento entre médico e especialidade
        /// </summary>
        Task<bool> ExistsAsync(int doctorId, int specialtyId);
    }
}