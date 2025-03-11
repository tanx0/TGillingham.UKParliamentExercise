using AutoMapper;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services.Models;

namespace UKParliament.CodeTest.Services.Mappers
{
    public class PersonMappingProfile : Profile
    {
        /// <summary>
        /// Mapping profile for PersonEntity and PersonDto
        /// New DTOs can be mapped without modifying existing mappings
        /// </summary>
        public PersonMappingProfile()
        {
            // Original Mapping: PersonEntity <-> PersonDto
            CreateMap<PersonEntity, PersonDto>().ReverseMap();

            // Extended Mapping: PersonEntity <-> PersonDetailsDto
            CreateMap<PersonEntity, PersonDetailsDto>().ReverseMap();

        }
    }
}


