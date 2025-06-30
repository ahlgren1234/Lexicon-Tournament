using Bogus;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;

namespace Tournament.Data.Data
{
    public static class SeedData
    {
        public static async Task InitAsync(TournamentContext context)
        {
            if (await context.TournamentDetails.AnyAsync())
            {
                return; // Database already seeded
            }

            var gameTitle = new[]
            {
                "Sky Clash","Gravity Blitz", "Orbital Showdown", "Zero Zone", "Ascension Duel",
                "Cloudbreaker Cup", "Hover Heat", "Vortex Strike", "Float Rush", "Zenith Slam",
                "Altitude War", "Jetspin Finals", "Nimbus Clash", "Halo Rally", "The Drift Open"
            };

            var gameFaker = new Faker<Game>()
                .RuleFor(g => g.Title, f => f.PickRandom(gameTitle))
                .RuleFor(g => g.Time, f => f.Date.Future());

            var tournamentFaker = new Faker<TournamentDetails>()
                .RuleFor(t => t.Title, f => $"{f.Company.CompanyName()} Championship")
                .RuleFor(t => t.StartDate, f => f.Date.Soon(30))
                .RuleFor(t => t.Games, f => gameFaker.Generate(f.Random.Int(2, 5)));

            var tournaments = tournamentFaker.Generate(3);

            context.TournamentDetails.AddRange(tournaments);
            await context.SaveChangesAsync();

        }
    }
}
