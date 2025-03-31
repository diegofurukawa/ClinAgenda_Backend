using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Core.Entities;

namespace ClinAgenda
{
    public class DoctorSpecialty
    {
        public int DoctorId { get; set; }
        public int SpecialtyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navegação
        public virtual Doctor Doctor { get; set; }
        public virtual Specialty Specialty { get; set; }
    }
}