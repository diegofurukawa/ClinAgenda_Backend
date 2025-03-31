using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Specialty
{

    public class SpecialtyDTO
    {
        public int SpecialtyId { get; set; }
        public required string SpecialtyName { get; set; }
        public required int NScheduleDuration { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool LActive { get; set; }
    }
}