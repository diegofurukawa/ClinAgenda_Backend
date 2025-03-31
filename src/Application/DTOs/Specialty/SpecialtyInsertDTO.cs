using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Specialty
{

    public class SpecialtyInsertDTO
    {
        [Required(ErrorMessage = "O nome da especialidade é obrigatório", AllowEmptyStrings = false)]
        public required string SpecialtyName { get; set; }

        [Required(ErrorMessage = "A duração do agendamento é obrigatória")]
        [Range(15, 60, ErrorMessage = "A duração deve estar entre 1 e 480 minutos")]
        public required int NScheduleDuration { get; set; }
        
        public bool LActive { get; set; } = true;
    }
}