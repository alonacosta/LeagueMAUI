using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueMAUI.Models
{
    public class Round
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public bool IsClosed { get; set; }
    }
}
