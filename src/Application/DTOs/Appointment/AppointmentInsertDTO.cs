using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentInsertDTO
    {
        [Required(ErrorMessage = "O ID do paciente é obrigatório")]
        public required int PatientId { get; set; }
        
        [Required(ErrorMessage = "O ID do médico é obrigatório")]
        public required int DoctorId { get; set; }
        
        [Required(ErrorMessage = "O ID da especialidade é obrigatório")]
        public required int SpecialtyId { get; set; }
        
        [Required(ErrorMessage = "O ID do status é obrigatório")]
        public required int StatusId { get; set; }
        
        [Required(ErrorMessage = "A data e hora da consulta são obrigatórias")]
        public required DateTime DAppointmentDate { get; set; }
        
        [Required(ErrorMessage = "A observação é obrigatória", AllowEmptyStrings = true)]
        public required string Observation { get; set; } = "";
        
        public bool LActive { get; set; } = true;
    }
}