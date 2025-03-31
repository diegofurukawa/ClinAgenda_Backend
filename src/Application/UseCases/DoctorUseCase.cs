using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.DoctorUseCase
{
    public class DoctorUseCase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorSpecialtyRepository _doctorSpecialtyRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IStatusRepository _statusRepository;

        public DoctorUseCase(
            IDoctorRepository doctorRepository, 
            IDoctorSpecialtyRepository doctorspecialtyRepository, 
            ISpecialtyRepository specialtyRepository,
            IStatusRepository statusRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorSpecialtyRepository = doctorspecialtyRepository;
            _specialtyRepository = specialtyRepository;
            _statusRepository = statusRepository;
        }

        public async Task<DoctorPagedResultDTO> GetDoctorsAsync(
            string? name, 
            int? specialtyId, 
            int? statusId, 
            bool? isActive,
            int pageSize, 
            int page)
        {
            int offset = (page - 1) * pageSize;

            var doctors = (await _doctorRepository.GetDoctorsAsync(
                name, 
                specialtyId, 
                statusId, 
                isActive, 
                offset,
                pageSize)).ToList();

            if (!doctors.Any())
                return new DoctorPagedResultDTO 
                { 
                    Total = 0, 
                    Items = new List<DoctorListItemDTO>() 
                };

            var doctorIds = doctors.Select(d => d.Id).ToArray();
            var specialties = (await _doctorRepository.GetDoctorSpecialtyAsync(doctorIds)).ToList();

            // Obter informações de status para todos os médicos
            var statusIds = doctors.Select(d => d.StatusId).Distinct().ToList();
            var statusList = new Dictionary<int, StatusDTO>();
            
            foreach (var id in statusIds)
            {
                var status = await _statusRepository.GetStatusByIdAsync(id);
                if (status != null)
                {
                    statusList[id] = status;
                }
            }

            // Transformar o resultado em DTOs de listagem
            var doctorListItems = doctors.Select(d => new DoctorListItemDTO
            {
                Id = d.Id,
                Name = d.Name,
                StatusName = statusList.ContainsKey(d.StatusId) 
                    ? statusList[d.StatusId].StatusName
                    : "Desconhecido",
                SpecialtyNames = specialties
                    .Where(s => s.DoctorId == d.Id)
                    .Select(s => s.SpecialtyName)
                    .ToList(),
                IsActive = d.LActive
            }).ToList();

            return new DoctorPagedResultDTO
            {
                Total = doctorListItems.Count,
                Items = doctorListItems
            };
        }

        public async Task<int> CreateDoctorAsync(DoctorCreateDTO doctorDto)
        {
            // Verificar se o status existe
            var status = await _statusRepository.GetStatusByIdAsync(doctorDto.StatusId);
            if (status == null)
            {
                throw new ArgumentException($"Status com ID {doctorDto.StatusId} não existe.");
            }
            
            // Verificar se todas as especialidades existem
            var specialties = await _specialtyRepository.GetAllSpecialtyAsync();
            var specialtyIds = specialties.specialties.Select(s => s.SpecialtyId).ToList();
            
            foreach (var specialtyId in doctorDto.SpecialtyIds)
            {
                if (!specialtyIds.Contains(specialtyId))
                {
                    throw new ArgumentException($"Especialidade com ID {specialtyId} não existe.");
                }
            }
            
            // Mapear para o DTO que o repositório espera (adaptação para evitar alterações no repositório)
            var repositoryDto = new DoctorInsertDTO
            {
                DoctorName = doctorDto.Name,
                StatusId = doctorDto.StatusId,
                Specialty = doctorDto.SpecialtyIds,
                LActive = doctorDto.IsActive
            };
            
            var newDoctorId = await _doctorRepository.InsertDoctorAsync(repositoryDto);

            // Sem necessidade de inserir especialidades separadamente, pois o repositório já faz isso

            return newDoctorId;
        }
        
        public async Task<DoctorDetailDTO?> GetDoctorByIdAsync(int id)
        {
            var rawData = await _doctorRepository.GetDoctorByIdAsync(id);
            
            if (!rawData.Any())
                return null;

            // Obter informações detalhadas de status
            var statusId = rawData.First().StatusId;
            var statusInfo = await _statusRepository.GetStatusByIdAsync(statusId);
            
            var statusDto = new StatusResponseDTO 
            { 
                StatusId = statusId, 
                StatusName = statusInfo?.StatusName ?? rawData.First().StatusName, 
                StatusType = statusInfo?.StatusType ?? "doctor",
                lActive = statusInfo?.LActive ?? rawData.First().LActive
            };

            var doctorData = rawData.First();

            // Obter as relações doctor_specialty com detalhes de ativação
            var doctorSpecialties = await _doctorSpecialtyRepository.GetByDoctorIdAsync(id);

            // Mapear para o novo DTO
            var specialties = rawData
                .Where(s => s.SpecialtyId > 0)
                .Select(s => new SpecialtyResponseDTO
                {
                    SpecialtyId = s.SpecialtyId,
                    SpecialtyName = s.SpecialtyName,
                    ScheduleDuration = s.NScheduleDuration,
                    IsActive = doctorSpecialties
                        .FirstOrDefault(ds => ds.SpecialtyId == s.SpecialtyId)?.LActive ?? true
                })
                .ToList();

            return new DoctorDetailDTO
            {
                Id = doctorData.Id,
                Name = doctorData.Name,
                Status = statusDto,
                Specialties = specialties,
                CreatedAt = doctorData.DCreated,
                LastUpdatedAt = doctorData.DLastUpdated,
                IsActive = doctorData.LActive
            };
        }

        public async Task<bool> UpdateDoctorAsync(int doctorId, DoctorUpdateDTO doctorDto)
        {
            // Verificar se o médico existe
            var doctorData = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (!doctorData.Any())
            {
                return false;
            }
            
            // Verificar se o status existe
            var status = await _statusRepository.GetStatusByIdAsync(doctorDto.StatusId);
            if (status == null)
            {
                throw new ArgumentException($"Status com ID {doctorDto.StatusId} não existe.");
            }
            
            // Verificar se todas as especialidades existem
            var specialties = await _specialtyRepository.GetAllSpecialtyAsync();
            var specialtyIds = specialties.specialties.Select(s => s.SpecialtyId).ToList();
            
            foreach (var specialtyId in doctorDto.SpecialtyIds)
            {
                if (!specialtyIds.Contains(specialtyId))
                {
                    throw new ArgumentException($"Especialidade com ID {specialtyId} não existe.");
                }
            }

            // Mapear para o DTO que o repositório espera (adaptação para manter compatibilidade)
            var repositoryUpdateDto = new DoctorInsertDTO
            {
                DoctorName = doctorDto.Name,
                StatusId = doctorDto.StatusId,
                Specialty = doctorDto.SpecialtyIds,
                LActive = doctorData.First().LActive // Mantém o estado ativo atual
            };

            // Atualizar o médico
            var updated = await _doctorRepository.UpdateDoctorAsync(doctorId, repositoryUpdateDto);

            return updated;
        }
        
        public async Task<bool> ToggleDoctorActiveAsync(int id, bool active)
        {
            // Verificar se o médico existe
            var doctorData = await _doctorRepository.GetDoctorByIdAsync(id);
            if (!doctorData.Any())
            {
                return false;
            }
            
            var rowsAffected = await _doctorRepository.ToggleDoctorActiveAsync(id, active);
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteDoctorAsync(int id)
        {
            // Verificar se o médico existe
            var doctorData = await _doctorRepository.GetDoctorByIdAsync(id);
            if (!doctorData.Any())
            {
                return false;
            }
            
            // Excluir o médico (o repositório já se encarrega de excluir as relações)
            var rowsAffected = await _doctorRepository.DeleteDoctorByIdAsync(id);
            return rowsAffected > 0;
        }
        
        public async Task<bool> ToggleDoctorSpecialtyActiveAsync(int doctorId, int specialtyId, bool active)
        {
            // Verificar se a relação existe
            bool exists = await _doctorSpecialtyRepository.ExistsAsync(doctorId, specialtyId);
            if (!exists)
            {
                return false;
            }
            
            // Ativar/desativar a relação
            return await _doctorSpecialtyRepository.ToggleActiveAsync(doctorId, specialtyId, active);
        }
    }
}