using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO para atualização parcial (toggle active)
    public class DoctorToggleActiveDTO
    {
        [Required(ErrorMessage = "O estado ativo é obrigatório")]
        public required bool IsActive { get; set; }
    }
}