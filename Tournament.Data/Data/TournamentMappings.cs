using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTO;
using Tournament.Core.Entities;

namespace Tournament.Data.Data
{
    public class TournamentMappings : Profile
    {
        public TournamentMappings() 
        {
            // Entity to DTO mappings
            CreateMap<Game, GameDTO>().ReverseMap();
            CreateMap<TournamentDetails, TournamentDTO>().ReverseMap();

            // DTO to Entity mappings
            //CreateMap<GameDTO, Game>();
            //CreateMap<TournamentDTO, TournamentDetails >();
                
        }
    }
}
