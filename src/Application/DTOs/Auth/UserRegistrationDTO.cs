using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Auth
{
    public class UserRegistrationDTO
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "O email não é válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public required string ConfirmPassword { get; set; }
        
        // Opcional - se o usuário for um médico ou paciente
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
    }
}