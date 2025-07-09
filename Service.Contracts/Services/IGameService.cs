using Service.Contracts.Services.DTO;
using Service.Contracts.Services;

namespace Service.Contracts.Services;

public interface IGameService
{
    Task<ServiceResult<IEnumerable<GameDTO>>> GetAllAsync();
    Task<ServiceResult<GameDTO?>> GetAsync(int id);
    Task<ServiceResult<GameDTO>> AddAsync(GameDTO gameDTO);
    Task<ServiceResult<bool>> UpdateAsync(int id, GameDTO gameDTO);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
