using Service.Contracts.DTO;

namespace Service.Contracts.Services;

public interface ITournamentService
{
    Task<IEnumerable<TournamentDTO>> GetAllAsync(bool includeGames = false);
    Task<TournamentDTO?> GetAsync(int id, bool includeGames = false);
    Task<TournamentDTO> AddAsync(TournamentDTO tournamentDTO);
    Task<bool> UpdateAsync(int id, TournamentDTO tournamentDTO);
    Task<bool> DeleteAsync(int id);
}
