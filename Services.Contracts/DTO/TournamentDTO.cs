using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.Contracts.DTO;

public class TournamentDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(60, ErrorMessage = "Name of the title has to be less than 60 characters")]
    public string? Title { get; set; }
    public DateTime StartDate { get; set; }

    public List<GameDTO>? Games { get; set; }
}
