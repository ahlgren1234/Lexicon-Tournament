using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Tournament.Api.Extensions;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Repositories;

namespace Tournament.Data
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<TournamentContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TournamentContext") ?? throw new InvalidOperationException("Connection string 'TournamentContext' not found.")));

            // Add services to the container.

            //builder.Services.AddControllers();
            builder.Services.AddControllers(opt => opt.ReturnHttpNotAcceptable = true)
            .AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(TournamentMappings));
            
            builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TournamentContext>();                
                await SeedData.InitAsync(context);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await app.SeedDataAsync();
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
