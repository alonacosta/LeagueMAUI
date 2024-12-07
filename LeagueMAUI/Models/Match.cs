using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueMAUI.Models
{
    public class Match
    {
        public int Id { get; set; }

        public string? HomeTeam { get; set; }

        public string? AwayTeam { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public bool IsClosed { get; set; }

        public DateTime StartDate { get; set; }

        public bool IsFinished { get; set; }
        public string? ImageHomeTeamUrl { get; set; }
        public string? ImageAwayTeamUrl { get; set; }
    }
}
