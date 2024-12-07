using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueMAUI.Models
{
    public class ImageProfile
    {
        public string? ImageUrl { get; set; }
        public string? PathImage => AppConfig.BaseUrl + ImageUrl;
    }
}
