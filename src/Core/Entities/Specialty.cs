using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgendaAPI
{
    public class Specialty
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ScheduleDuration { get; set; }
    }
}