using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueMAUI.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? ClubName { get; set; }
        public string? PositionName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
