using System;
using System.Collections.Generic;

namespace Service.Contracts.Services.DTO;

public class TournamentDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<GameDTO>? Games { get; set; }
    // Add other properties as needed
}
