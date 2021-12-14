using ChatBackend.Chat.Domain.Operations;
using FluentValidation;

#pragma warning disable 1591

namespace ChatBackend.Chat.Domain.Validators
{
    public sealed class MarkMessageAsReadValidator : AbstractValidator<MarkMessageAsRead>
    {
        public MarkMessageAsReadValidator(IChatValidatorsRepository chatValidatorsService)
        {
            RuleFor(x => x.MessageGuid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать Guid сообщения")
                .MustAsync(chatValidatorsService.DoesMessageWithGuidExistAsync)
                .WithMessage("Сообщение с указанным Guid не существует")
                .MustAsync((operation, messageGuid, cancellationToken) =>
                    chatValidatorsService.DoesMessageHaveReciever(
                        messageGuid, operation.RecieverGuid, cancellationToken))
                .WithMessage(markMessageAsRead => $"Сообщение с Guid {markMessageAsRead.MessageGuid} не было отправлено пользователю с Guid")
                ;

            RuleFor(x => x.RecieverGuid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать Guid получателя сообщения")
                .MustAsync((recieverGuid, cancellationToken) => 
                    chatValidatorsService.DoUsersWithGuidsExistAsync(new [] {recieverGuid}, cancellationToken))
                .WithMessage("Получатель с указанным Guid не существует")
                ;
        }
    }
}