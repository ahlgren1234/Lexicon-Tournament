using Service.Contracts.DTO;

namespace Service.Contracts.Services;

public interface IGameService
{
    Task<IEnumerable<GameDTO>> GetAllAsync();
    Task<GameDTO?> GetAsync(int id);
    Task<GameDTO> AddAsync(GameDTO gameDTO);
    Task<bool> UpdateAsync(int id, GameDTO gameDTO);
    Task<bool> DeleteAsync(int id);
}
