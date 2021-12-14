using System.Linq;
using ChatBackend.Chat.Domain.Operations;
using FluentValidation;
#pragma warning disable 1591

namespace ChatBackend.Chat.Domain.Validators
{
    public sealed class CreateMessageValidator : AbstractValidator<CreateMessage>
    {
        public CreateMessageValidator(IChatValidatorsRepository chatValidatorsService)
        {
            RuleFor(x => x.Query)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Query)
#pragma warning disable 8620
                        .SetValidator(new CreateMessageQueryValidator())
#pragma warning restore 8620
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Query!.RecieversGuids)
                                .Cascade(CascadeMode.Stop)
                                .Must((createMessage, recieversGuids) =>
                                    recieversGuids!.All(recieverGuid => recieverGuid != createMessage.SenderGuid))
                                .WithMessage("Список получателей не может содержать отправителя сообщения")
                                .MustAsync(chatValidatorsService.DoUsersWithGuidsExistAsync!)
                                .WithMessage("Один из пользователей-получателей с указанными Guid не существует")
                                ;
                            RuleFor(x => x.Query!.PersonGuid)
                                .MustAsync(chatValidatorsService.DoesPersonWithGuidExistAsync)
                                .WithMessage("Военнослужащего-отправителя с указанным Guid не существует")
                                ;
                        })
                        ;
                })
                ;

            RuleFor(x => x.SenderGuid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать Guid отправителя сообщения")
                .MustAsync((guid, cancellationToken) => 
                    chatValidatorsService.DoUsersWithGuidsExistAsync(new []{guid}, cancellationToken))
                .WithMessage("Пользователь с указанным Guid отправителя не существует")
                ;

         
        }
    }
}