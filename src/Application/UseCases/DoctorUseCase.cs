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
        public DoctorUseCase(IDoctorRepository doctorRepository, IDoctorSpecialtyRepository doctorspecialtyRepository, ISpecialtyRepository specialtyRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorSpecialtyRepository = doctorspecialtyRepository;
            _specialtyRepository = specialtyRepository;
        }

        public async Task<DoctorResponseDTO> GetDoctorsAsync(
            string? doctorName, 
            int? specialtyId, 
            int? statusId, 
            bool? lActive, 
            int itemsPerPage, 
            int page)
        {
            int offset = (page - 1) * itemsPerPage;

            var rawData = await _doctorRepository.GetDoctorsAsync(doctorName, specialtyId, statusId, lActive, offset, itemsPerPage);

            if (!rawData.doctors.Any())
                return new DoctorResponseDTO { Total = 0, Items = new List<DoctorListReturnDTO>() };

            var doctorIds = rawData.doctors.Select(d => d.DoctorId).ToArray();
            var specialties = (await _doctorRepository.GetDoctorSpecialtiesAsync(doctorIds)).ToList();

            var result = rawData.doctors.Select(d => new DoctorListReturnDTO
            {
                DoctorId = d.DoctorId,
                DoctorName = d.DoctorName,
                Status = new StatusDTO
                {
                    StatusId = d.StatusId,
                    StatusName = d.StatusName,                    
                },
                Specialty = specialties.Where(s => s.DoctorId == d.DoctorId)
                    .Select(s => new SpecialtyDTO
                    {
                        SpecialtyId = s.SpecialtyId,
                        SpecialtyName = s.SpecialtyName,
                        nScheduleDuration = s.nScheduleDuration
                    }
                    ).ToList()
            });

            return new DoctorResponseDTO
            {
                Total = rawData.total,
                Items = result.ToList()
            };
        }
        public async Task<int> CreateDoctorAsync(DoctorInsertDTO doctorDto)
        {
            var newDoctorId = await _doctorRepository.InsertDoctorAsync(doctorDto);

            var doctor_specialities = new DoctorSpecialtyDTO
            {
                DoctorId = newDoctorId,
                SpecialtyId = doctorDto.Specialty
            };

            await _doctorSpecialtyRepository.InsertAsync(doctor_specialities);

            return newDoctorId;
        }
        public async Task<DoctorListReturnDTO> GetDoctorByIdAsync(int doctorId)
        {
            var rawData = await _doctorRepository.GetDoctorByIdAsync(doctorId);

            List<DoctorListReturnDTO> infoDoctor = new List<DoctorListReturnDTO>();

            foreach (var group in rawData.GroupBy(item => item.DoctorId))
            {
                DoctorListReturnDTO doctor = new DoctorListReturnDTO
                {
                    DoctorId = group.Key,
                    DoctorName = group.First().DoctorName,
                    Specialty = group.Select(s => new SpecialtyDTO
                    {
                        SpecialtyId = s.SpecialtyId,
                        SpecialtyName = s.SpecialtyName
                    }).ToList(),
                    Status = new StatusDTO
                    {
                        StatusId = group.First().StatusId,
                        StatusName = group.First().StatusName,                        
                    }
                };

                infoDoctor.Add(doctor);
            }

            return infoDoctor.First();
        }

        public async Task<bool> UpdateDoctorAsync(int id, DoctorInsertDTO doctorDto)
        {
            var doctorToUpdate = new DoctorDTO
            {
                DoctorId = id,
                DoctorName = doctorDto.DoctorName,
                StatusId = doctorDto.StatusId
            };

            await _doctorRepository.UpdateDoctorByIdAsync(doctorToUpdate);

            await _doctorSpecialtyRepository.DeleteByDoctorIdAsync(id);

            var doctorSpecialties = new DoctorSpecialtyDTO
            {
                DoctorId = id,
                SpecialtyId = doctorDto.Specialty
            };

            await _doctorSpecialtyRepository.InsertAsync(doctorSpecialties);

            return true;
        }
        public async Task<bool> DeleteDoctorByIdAsync(int id)
        {
            var rowsAffected = await _doctorRepository.DeleteDoctorByIdAsync(id);
            return rowsAffected > 0;
        }


        public async Task<bool> ToggleDoctorActiveAsync(int doctorId, bool active)
        {
            return await _doctorRepository.ToggleDoctorActiveAsync(doctorId, active);
        }
        
        public async Task<bool> ToggleDoctorSpecialtyActiveAsync(int doctorId, int specialtyId, bool active)
        {
            var exists = await _doctorSpecialtyRepository.ExistsAsync(doctorId, specialtyId);
            if (!exists)
                return false;
                
            return await _doctorSpecialtyRepository.ToggleActiveAsync(doctorId, specialtyId, active);
        }
        
    }

}