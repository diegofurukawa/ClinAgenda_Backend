using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
   // DTO para atualização (entrada)
    public class DoctorUpdateDTO
    {
        [Required(ErrorMessage = "O nome do médico é obrigatório")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 255 caracteres")]
        public required string Name { get; set; }
        
        [Required(ErrorMessage = "O status é obrigatório")]
        public int StatusId { get; set; }
        
        [Required(ErrorMessage = "Pelo menos uma especialidade é obrigatória")]
        [MinLength(1, ErrorMessage = "O médico deve ter pelo menos uma especialidade")]
        public required List<int> SpecialtyIds { get; set; }
    }
}