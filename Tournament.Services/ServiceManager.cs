using Service.Contracts.Services;

namespace Tournament.Services;

public class ServiceManager : IServiceManager
{
    public ITournamentService TournamentService { get; }
    public IGameService GameService { get; }

    public ServiceManager(ITournamentService tournamentService, IGameService gameService)
    {
        TournamentService = tournamentService;
        GameService = gameService;
    }
}
