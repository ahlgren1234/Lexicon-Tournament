using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities;

public class TournamentDetails
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(60, ErrorMessage = "Name of the title has to be less than 60 characters")]
    public string? Title { get; set; }
    public DateTime StartDate { get; set; }

    // Navigation property to represent the relationship with Game entities
    public ICollection<Game>? Games { get; set; }
}
