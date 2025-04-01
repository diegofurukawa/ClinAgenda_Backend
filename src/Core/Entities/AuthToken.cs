using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Core.Entities
{
    public class AuthToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public required string Token { get; set; }
        public required DateTime DExpires { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool LActive { get; set; } = true;
        
        // Navegação
        public virtual User User { get; set; }
    }
}