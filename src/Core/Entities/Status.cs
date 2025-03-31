using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda
{
    public class Status
    {
        public int StatusId { get; set; }
        public required string StatusName { get; set; }
        public required string StatusType { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool LActive { get; set; } = true;
    }
}