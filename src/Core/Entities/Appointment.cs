namespace ClinAgenda
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public required int PatientId { get; set; }
        public required int DoctorId { get; set; } 
        public required int SpecialtyId { get; set; }
        public required int StatusId { get; set; }
        public required DateTime DAppointmentDate { get; set; }  
        public required string Observation { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool lActive { get; set; } = true;
    }
}