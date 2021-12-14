using ChatBackend.Persons.Domain.Operations;
using FluentValidation;

#pragma warning disable 1591

namespace ChatBackend.Persons.Domain.Validators
{
    public sealed class CreateFullNameValidator : AbstractValidator<CreateFullName>
    {
        public CreateFullNameValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать имя")
                .MaximumLength(500)
                .WithMessage("Имя слишком длинное")
                ;
            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать фамилию")
                .MaximumLength(500)
                .WithMessage("Фамилия слишком длинная")
                ;
            RuleFor(x => x.ParentName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать отчество")
                .MaximumLength(500)
                .WithMessage("Отчество слишком длинное")
                ;
        }
    }
}