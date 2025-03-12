using UKParliament.CodeTest.Web.ViewModels;
using UKParliament.CodeTest.Services.Models;
namespace UKParliament.CodeTest.Web.Mappings{

    public static class PersonMappingExtensions
    {
        /// <summary>
        /// Mapping from PersonViewModel used by the client app to PersonDetailsDto used by the service
        /// </summary>
        /// <param name="personVm"></param>
        /// <returns></returns>
        public static PersonDetailsDto ToDto(this PersonViewModel personVm)
        {
            //PersonViewModel.DateOfBirth is passed in format "yyyy-MM-dd" but PersonDetailsDto.DateOfBirth is DateTime
            return new PersonDetailsDto
            {
                Id = personVm.Id,
                FirstName = personVm.FirstName,
                LastName = personVm.LastName,
                DateOfBirth = TryParseDob(personVm.DateOfBirth),                
                DepartmentId = personVm.DepartmentId                
            };
        }

        private static DateTime TryParseDob(string dob)
        {
            if (DateTime.TryParse(dob, out DateTime result))
                return result;
            else
                return default;
        }


        /// <summary>
        /// Mapping from PersonDetailsDto used by the service, to PersonViewModel used by the client app 
        /// </summary>
        /// <param name="personDto"></param>
        /// <returns></returns>
        public static PersonViewModel  ToViewModel(this PersonDetailsDto personDto)
        {
            //PersonViewModel.DateOfBirth is passed in format "yyyy-MM-dd" but PersonDetailsDto.DateOfBirth is DateTime
            return new PersonViewModel
            {
                Id = personDto.Id,
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                DateOfBirth = personDto.DateOfBirth.ToString("yyyy-MM-dd"),
                DepartmentId = personDto.DepartmentId
            };
        }

        public static IEnumerable<PersonViewModel> ToViewModel(this IEnumerable<PersonDetailsDto> personDtos)
        {
            return personDtos.Select(p => p.ToViewModel());
        }
    }

}
