using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Patient
{
    public class PatientInsertDTO
    {
        [Required(ErrorMessage = "O nome do paciente é obrigatório", AllowEmptyStrings = false)]
        public required string PatientName { get; set; }

        [Required(ErrorMessage = "O telefone do paciente é obrigatório", AllowEmptyStrings = false)]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "O documento do paciente é obrigatório", AllowEmptyStrings = false)]
        public required string DocumentNumber { get; set; }

        [Required(ErrorMessage = "O status do paciente é obrigatório")]
        public required int StatusId { get; set; }

        [Required(ErrorMessage = "A data de nascimento do paciente é obrigatória", AllowEmptyStrings = false)]
        public required string dBirthdate { get; set; }
        
        public bool LActive { get; set; } = true;
    }
}