using ChatBackend.Chat.Domain.Operations;
using FluentValidation;
#pragma warning disable 1591

namespace ChatBackend.Chat.Domain.Validators
{
    public sealed class FillPlaceholdersValidator : AbstractValidator<FillPlaceholders>
    {
        public FillPlaceholdersValidator()
        {
            RuleFor(x => x.MessageTemplateContent)
                .NotEmpty()
                .WithMessage("Необходимо указать текст шаблона сообщения")
                ;
            RuleFor(x => x.MessengerContext)
                .NotEmpty()
                .WithMessage("Необходимо указать контекст посыльного")
                ;
        }
    }
}