namespace ClinAgenda.src.Core.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public required string Name { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Relacionamentos
        public virtual ICollection<DoctorSpecialty> Specialties { get; set; } = new List<DoctorSpecialty>();
    }
}