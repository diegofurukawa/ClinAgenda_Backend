using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Status
{
    public class StatusInsertDTO
    {
        [Required(ErrorMessage = "O nome do status é obrigatório", AllowEmptyStrings = false)]
        public required string StatusName { get; set; }
        
        [Required(ErrorMessage = "O tipo do status é obrigatório", AllowEmptyStrings = false)]
        [RegularExpression("^(patient|specialty|doctor|appointment)$", 
            ErrorMessage = "O tipo de status deve ser patient, specialty, doctor ou appointment")]
        public required string StatusType { get; set; }
        
        public bool lActive { get; set; } = true;
    }
}