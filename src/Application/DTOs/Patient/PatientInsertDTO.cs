using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Patient
{
    public class PatientInsertDTO
    {
        [Required(ErrorMessage = "O Nome do Paciente é obrigatório", AllowEmptyStrings = false)]
        public required string @Name { get; set; }

        [Required(ErrorMessage = "O Telefone do Paciente é obrigatório", AllowEmptyStrings = false)]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "O Documento do Paciente é obrigatório", AllowEmptyStrings = false)]
        public required string DocumentNumber { get; set; }

        [Required(ErrorMessage = "O Status Paciente é obrigatório", AllowEmptyStrings = false)]
        public required int StatusId { get; set; }

        [Required(ErrorMessage = "O Dia de nascimento do Paciente é obrigatório", AllowEmptyStrings = false)]
        public required string BirthDate { get; set; }
    }
}