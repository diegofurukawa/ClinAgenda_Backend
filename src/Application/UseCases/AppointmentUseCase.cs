using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Appointment;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.AppointmentUseCase
{
    public class AppointmentUseCase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IStatusRepository _statusRepository;

        public AppointmentUseCase(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ISpecialtyRepository specialtyRepository,
            IStatusRepository statusRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _specialtyRepository = specialtyRepository;
            _statusRepository = statusRepository;
        }

        public async Task<object> GetAppointmentsAsync(
            int? patientId = null, 
            int? doctorId = null, 
            int? specialtyId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? lActive = null,
            int itemsPerPage = 10, 
            int page = 1)
        {
            var (total, rawData) = await _appointmentRepository.GetAllAppointmentsAsync(
                patientId,
                doctorId,
                specialtyId,
                statusId,
                startDate,
                endDate,
                lActive,
                itemsPerPage,
                page);

            // Convertemos para a DTO de retorno com objetos completos
            var appointments = await MapToListReturnDTOAsync(rawData);

            return new
            {
                total,
                items = appointments
            };
        }

        public async Task<object> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            
            if (appointment == null)
                return null;

            // Obtemos os dados relacionados
            var patient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
            var doctorData = await _doctorRepository.GetDoctorByIdAsync(appointment.DoctorId);
            var doctor = doctorData.FirstOrDefault();
            var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(appointment.SpecialtyId);
            var status = await _statusRepository.GetStatusByIdAsync(appointment.StatusId);

            return new
            {
                item = new
                {
                    id = appointment.AppointmentId,
                    patient = new
                    {
                        id = patient?.PatientId,
                        name = patient?.PatientName
                    },
                    doctor = new
                    {
                        id = doctor?.Id,
                        name = doctor?.Name
                    },
                    specialty = new
                    {
                        id = specialty?.SpecialtyId,
                        name = specialty?.SpecialtyName,
                        scheduleDuration = specialty?.NScheduleDuration
                    },
                    status = new
                    {
                        id = status?.StatusId,
                        name = status?.StatusName,
                        type = status?.StatusType
                    },
                    date = appointment.DAppointmentDate,
                    observation = appointment.Observation,
                    created = appointment.DCreated,
                    lastUpdated = appointment.DLastUpdated,
                    active = appointment.LActive
                }
            };
        }

        public async Task<int> CreateAppointmentAsync(AppointmentInsertDTO appointmentDTO)
        {
            // Validações básicas
            if (appointmentDTO.PatientId <= 0)
            {
                throw new ArgumentException("O ID do paciente é inválido");
            }

            if (appointmentDTO.DoctorId <= 0)
            {
                throw new ArgumentException("O ID do médico é inválido");
            }

            if (appointmentDTO.SpecialtyId <= 0)
            {
                throw new ArgumentException("O ID da especialidade é inválido");
            }

            if (appointmentDTO.StatusId <= 0)
            {
                throw new ArgumentException("O ID do status é inválido");
            }

            // Validar se o paciente existe
            var patient = await _patientRepository.GetPatientByIdAsync(appointmentDTO.PatientId);
            if (patient == null)
            {
                throw new ArgumentException($"Paciente com ID {appointmentDTO.PatientId} não existe.");
            }

            // Validar se o médico existe
            var doctorData = await _doctorRepository.GetDoctorByIdAsync(appointmentDTO.DoctorId);
            if (!doctorData.Any())
            {
                throw new ArgumentException($"Médico com ID {appointmentDTO.DoctorId} não existe.");
            }

            // Validar se a especialidade existe
            var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(appointmentDTO.SpecialtyId);
            if (specialty == null)
            {
                throw new ArgumentException($"Especialidade com ID {appointmentDTO.SpecialtyId} não existe.");
            }

            // Validar se o status existe
            var status = await _statusRepository.GetStatusByIdAsync(appointmentDTO.StatusId);
            if (status == null)
            {
                throw new ArgumentException($"Status com ID {appointmentDTO.StatusId} não existe.");
            }

            // Verificar se o médico atende a especialidade solicitada
            var doctorSpecialties = await _doctorRepository.GetDoctorSpecialtyAsync(new[] { appointmentDTO.DoctorId });
            bool hasSpecialty = doctorSpecialties.Any(ds => ds.SpecialtyId == appointmentDTO.SpecialtyId);
            
            if (!hasSpecialty)
            {
                throw new ArgumentException($"O médico com ID {appointmentDTO.DoctorId} não atende a especialidade com ID {appointmentDTO.SpecialtyId}.");
            }

            // Verificar conflitos de horários
            DateTime endTime = appointmentDTO.DAppointmentDate.AddMinutes(specialty.NScheduleDuration);
            bool hasConflict = await _appointmentRepository.CheckConflictingAppointmentsAsync(
                appointmentDTO.DoctorId,
                null,
                appointmentDTO.DAppointmentDate,
                endTime);

            if (hasConflict)
            {
                throw new ArgumentException("Já existe um agendamento para este médico no horário solicitado.");
            }

            return await _appointmentRepository.InsertAppointmentAsync(appointmentDTO);
        }

        public async Task<bool> UpdateAppointmentAsync(int id, AppointmentInsertDTO appointmentDTO)
        {
            // Validações básicas
            if (appointmentDTO.PatientId <= 0)
            {
                throw new ArgumentException("O ID do paciente é inválido");
            }

            if (appointmentDTO.DoctorId <= 0)
            {
                throw new ArgumentException("O ID do médico é inválido");
            }

            if (appointmentDTO.SpecialtyId <= 0)
            {
                throw new ArgumentException("O ID da especialidade é inválido");
            }

            if (appointmentDTO.StatusId <= 0)
            {
                throw new ArgumentException("O ID do status é inválido");
            }

            // Verificar se o agendamento existe
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                return false;
            }

            // Validar se o paciente existe
            var patient = await _patientRepository.GetPatientByIdAsync(appointmentDTO.PatientId);
            if (patient == null)
            {
                throw new ArgumentException($"Paciente com ID {appointmentDTO.PatientId} não existe.");
            }

            // Validar se o médico existe
            var doctorData = await _doctorRepository.GetDoctorByIdAsync(appointmentDTO.DoctorId);
            if (!doctorData.Any())
            {
                throw new ArgumentException($"Médico com ID {appointmentDTO.DoctorId} não existe.");
            }

            // Validar se a especialidade existe
            var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(appointmentDTO.SpecialtyId);
            if (specialty == null)
            {
                throw new ArgumentException($"Especialidade com ID {appointmentDTO.SpecialtyId} não existe.");
            }

            // Validar se o status existe
            var status = await _statusRepository.GetStatusByIdAsync(appointmentDTO.StatusId);
            if (status == null)
            {
                throw new ArgumentException($"Status com ID {appointmentDTO.StatusId} não existe.");
            }

            // Verificar se o médico atende a especialidade solicitada
            var doctorSpecialties = await _doctorRepository.GetDoctorSpecialtyAsync(new[] { appointmentDTO.DoctorId });
            bool hasSpecialty = doctorSpecialties.Any(ds => ds.SpecialtyId == appointmentDTO.SpecialtyId);
            
            if (!hasSpecialty)
            {
                throw new ArgumentException($"O médico com ID {appointmentDTO.DoctorId} não atende a especialidade com ID {appointmentDTO.SpecialtyId}.");
            }

            // Verificar conflitos de horários (excluindo o próprio agendamento)
            DateTime endTime = appointmentDTO.DAppointmentDate.AddMinutes(specialty.NScheduleDuration);
            bool hasConflict = await _appointmentRepository.CheckConflictingAppointmentsAsync(
                appointmentDTO.DoctorId,
                id,
                appointmentDTO.DAppointmentDate,
                endTime);

            if (hasConflict)
            {
                throw new ArgumentException("Já existe um agendamento para este médico no horário solicitado.");
            }

            var rowsAffected = await _appointmentRepository.UpdateAppointmentAsync(id, appointmentDTO);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleAppointmentActiveAsync(int id, bool active)
        {
            // Verificar se o agendamento existe
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                return false;
            }
            
            var rowsAffected = await _appointmentRepository.ToggleAppointmentActiveAsync(id, active);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            // Verificar se o agendamento existe
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                return false;
            }
            
            var rowsAffected = await _appointmentRepository.DeleteAppointmentAsync(id);
            return rowsAffected > 0;
        }
        
        // Método auxiliar para mapear as DTOs de lista para DTOs de retorno com objetos completos
        private async Task<List<AppointmentListReturnDTO>> MapToListReturnDTOAsync(IEnumerable<AppointmentListDTO> appointmentList)
        {
            var result = new List<AppointmentListReturnDTO>();
            
            foreach (var appointment in appointmentList)
            {
                // Obtemos informações completas dos objetos relacionados
                var patient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
                var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(appointment.SpecialtyId);
                var status = await _statusRepository.GetStatusByIdAsync(appointment.StatusId);
                
                // Converter para DTO completa
                result.Add(new AppointmentListReturnDTO
                {
                    AppointmentId = appointment.AppointmentId,
                    Patient = new PatientDTO 
                    { 
                        PatientId = appointment.PatientId,
                        PatientName = appointment.PatientName,
                        // Outros campos do paciente seriam preenchidos aqui
                        PhoneNumber = patient?.PhoneNumber ?? "",
                        DocumentNumber = patient?.DocumentNumber ?? "",
                        StatusId = patient?.StatusId ?? 0,
                        DBirthDate = patient?.DBirthDate ?? DateTime.MinValue,
                        DCreated = patient?.DCreated ?? appointment.DCreated,
                        DLastUpdated = patient?.DLastUpdated,
                        LActive = patient?.LActive ?? true
                    },
                    Doctor = new DoctorDTO
                    {
                        DoctorId = appointment.DoctorId,
                        DoctorName = appointment.DoctorName,
                        StatusId = 0, // Seria preenchido com dados reais
                        DCreated = appointment.DCreated,
                        DLastUpdated = appointment.DLastUpdated,
                        LActive = true
                    },
                    Specialty = specialty ?? new SpecialtyDTO
                    {
                        SpecialtyId = appointment.SpecialtyId,
                        SpecialtyName = appointment.SpecialtyName,
                        NScheduleDuration = 30, // Valor padrão
                        DCreated = appointment.DCreated,
                        DLastUpdated = appointment.DLastUpdated,
                        LActive = true
                    },
                    Status = status ?? new StatusDTO
                    {
                        StatusId = appointment.StatusId,
                        StatusName = appointment.StatusName,
                        StatusType = "appointment",
                        DCreated = appointment.DCreated,
                        DLastUpdated = appointment.DLastUpdated,
                        LActive = true
                    },
                    DAppointmentDate = appointment.DAppointmentDate,
                    Observation = appointment.Observation,
                    DCreated = appointment.DCreated,
                    DLastUpdated = appointment.DLastUpdated,
                    LActive = appointment.LActive
                });
            }
            
            return result;
        }
    }
}