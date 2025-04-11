using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime? DLastLogin { get; set; }
        public int NFailedLoginAttempts { get; set; } = 0;
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool lActive { get; set; } = true;
    }
}