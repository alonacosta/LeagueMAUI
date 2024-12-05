using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueMAUI.Models
{
    public class Club
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Stadium { get; set; }

        public int Capacity { get; set; }

        public string? HeadCoach { get; set; }
        public string? ImageFullPath { get; set; }
    }
}
