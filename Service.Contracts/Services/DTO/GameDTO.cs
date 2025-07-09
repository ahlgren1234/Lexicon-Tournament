namespace Service.Contracts.Services.DTO;

public class GameDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int TournamentId { get; set; }
    public int? Score1 { get; set; }
    public int? Score2 { get; set; }
    // Add other properties as needed
}
