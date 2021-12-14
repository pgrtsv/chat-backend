using System.Linq;
using ChatBackend.Persons.Domain.Operations;
using FluentValidation;

#pragma warning disable 1591

namespace ChatBackend.Persons.Domain.Validators
{
    public sealed class CreatePersonValidator : AbstractValidator<CreatePerson>
    {
        public CreatePersonValidator()
        {
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать ФИО военнослужащего")
                ;
            RuleFor(x => x.MilitaryRankId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Необходимо указать Id воинского звания")
                .Must(x => MilitaryRank.GetAll().Any(rank => rank.Id == x!.Value))
                .WithMessage("Воинского звания с указанным Id не существует")
                ;
        }
    }
}