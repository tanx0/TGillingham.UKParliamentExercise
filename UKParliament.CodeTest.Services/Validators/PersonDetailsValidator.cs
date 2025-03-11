
namespace UKParliament.CodeTest.Services.Validators
{
    using FluentValidation;
    using UKParliament.CodeTest.Services.Models;
    using UKParliament.CodeTest.Data;

    /// <summary>
    /// PersonValidator with validation rules for PersonDto fields
    /// </summary>
    public class PersonValidator : AbstractValidator<PersonDto>
    {
        public PersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required");           

        }
    }

    /// <summary>
    /// Extended PersonDetailsValidator with additional validation rules
    ///  OCP – no modification of existing PersonValidator
    /// </summary>
    public class PersonDetailsValidator : AbstractValidator<PersonDetailsDto>
    {
        public PersonDetailsValidator(PersonValidator baseValidator, IDepartmentRepository departmentRepository, IPersonRepository personRepository)
        {
            Include(baseValidator); // Reuse all validation rules from the base validator

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Date of Birth must not be in the future");

            RuleFor(x => x.DepartmentId)
                .MustAsync(async (departmentId, cancellation) =>
                    await departmentRepository.DepartmentExistsAsync(departmentId))
                .WithMessage("Department is invalid");

            
            RuleFor(x => x)
                .MustAsync(async (personDetails, cancellation) =>
                    await IsUniquePersonAsync(personDetails, personRepository))
                .WithMessage("A person with the same first name, last name, and date of birth already exists.");
        }

        private async Task<bool> IsUniquePersonAsync(PersonDetailsDto personDetails, IPersonRepository personRepository)
        {
            var duplicatePerson = await personRepository.SearchForPersonAsync(
                personDetails.FirstName, personDetails.LastName, personDetails.DateOfBirth);

            return duplicatePerson == null || duplicatePerson.Id == personDetails.Id;
        }   
    }


}
