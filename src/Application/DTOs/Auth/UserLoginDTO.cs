using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Auth
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public required string Password { get; set; }
    }
}