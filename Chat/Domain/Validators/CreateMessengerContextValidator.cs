using ChatBackend.Chat.Domain.Operations;
using FluentValidation;
#pragma warning disable 1591

namespace ChatBackend.Chat.Domain.Validators
{
    public sealed class CreateMessengerContextValidator : AbstractValidator<CreateMessengerContext>
    {
        public CreateMessengerContextValidator(IChatValidatorsRepository chatValidatorsRepository)
        {
            RuleFor(x => x.Business)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => $"Количество л/с в командировке ({x.Business}) не может быть отрицательным")
                ;
            RuleFor(x => x.Company)
                .GreaterThan(0)
                .WithMessage(x => $"Номер роты ({x.Company}) должен быть положительным")
                ;
            RuleFor(x => x.Hospitalized)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => $"Количество л/с в госпитале ({x.Hospitalized}) не может быть отрицательным")
                ;
            RuleFor(x => x.Leave)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => $"Количество л/с в увольнении ({x.Leave}) не может быть отрицательным")
                ;
            RuleFor(x => x.Present)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => $"Количество л/с налицо ({x.Present}) не может быть отрицательным")
                ;
            RuleFor(x => x.Responsible)
                .MustAsync(chatValidatorsRepository.DoesPersonWithGuidExistAsync)
                .WithMessage(x => $"Ответственного с указанным Guid ({x.Responsible}) не существует")
                .When(x => x.Responsible != default)
                ;
            RuleFor(x => x.DailyMessenger)
                .MustAsync(chatValidatorsRepository.DoesPersonWithGuidExistAsync)
                .WithMessage(x => $"Посыльного дневной смены с указанным Guid ({x.DailyMessenger}) не существует")
                .When(x => x.DailyMessenger != default)
                ;
            RuleFor(x => x.DpcHelper)
                .MustAsync(chatValidatorsRepository.DoesPersonWithGuidExistAsync)
                .WithMessage(x => $"Помощника дежурного по ЦОД с указанным Guid ({x.DpcHelper}) не существует")
                .When(x => x.DpcHelper != default)
                ;
            RuleFor(x => x.NightlyMessenger)
                .MustAsync(chatValidatorsRepository.DoesPersonWithGuidExistAsync)
                .WithMessage(x => $"Посыльного ночной смены с указанным Guid ({x.NightlyMessenger}) не существует")
                .When(x => x.NightlyMessenger != default)
                ;
            RuleFor(x => x.TechnopolisHelper)
                .MustAsync(chatValidatorsRepository.DoesPersonWithGuidExistAsync)
                .WithMessage(x =>
                    $"Помощника дежурного по технополису с указанным Guid ({x.TechnopolisHelper}) не существует")
                .When(x => x.TechnopolisHelper != default)
                ;
            RuleFor(x => x.TechopolisMessenger)
                .MustAsync(chatValidatorsRepository.DoesPersonWithGuidExistAsync)
                .WithMessage(x => $"Посыльного по технополису с указанным Guid ({x.TechopolisMessenger}) не существует")
                .When(x => x.TechopolisMessenger != default)
                ;
        }
    }
}