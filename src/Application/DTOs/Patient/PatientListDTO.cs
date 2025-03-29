using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Patient
{
    public class PatientListDTO
    {
        public int Id { get; set; }
         public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public required string DocumentNumber { get; set; }
        public int StatusId { get; set; }
        public required string StatusName { get; set; }
        public required string BirthDate { get; set; }
    }
}