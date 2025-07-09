using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Contracts.DTO;

public class GameDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateTime Time { get; set; }
}
