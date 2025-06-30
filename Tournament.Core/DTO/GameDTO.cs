using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTO
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime Time { get; set; }        
    }
}
