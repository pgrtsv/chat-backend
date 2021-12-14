using FluentValidation;
#pragma warning disable 1591

namespace ChatBackend.Chat.Domain.Validators
{
    public sealed class PagingSettingsValidator : AbstractValidator<PagingSettings>
    {
        public PagingSettingsValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Номер страницы должен быть больше 0")
                ;
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(5)
                .WithMessage("Размер страницы должен быть больше или равен 5")
                .LessThanOrEqualTo(100)
                .WithMessage("Размер страницы должен быть меньше или равен 40")
                ;
        }
    }
}