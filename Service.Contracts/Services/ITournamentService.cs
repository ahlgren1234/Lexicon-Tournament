using Service.Contracts.Services.DTO;
using Service.Contracts.Services;

namespace Service.Contracts.Services;

public interface ITournamentService
{
    Task<ServiceResult<IEnumerable<TournamentDTO>>> GetAllAsync(bool includeGames = false);
    Task<ServiceResult<TournamentDTO?>> GetAsync(int id, bool includeGames = false);
    Task<ServiceResult<TournamentDTO>> AddAsync(TournamentDTO tournamentDTO);
    Task<ServiceResult<bool>> UpdateAsync(int id, TournamentDTO tournamentDTO);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
