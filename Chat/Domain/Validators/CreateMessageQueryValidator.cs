using ChatBackend.Chat.Domain.Operations;
using ChatBackend.Persons.Domain;
using FluentValidation;
#pragma warning disable 1591

namespace ChatBackend.Chat.Domain.Validators
{
    public sealed class CreateMessageQueryValidator : AbstractValidator<CreateMessageQuery>
    {
        public CreateMessageQueryValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Необходимо указать содержание сообщения")
                .MaximumLength(2000)
                .WithMessage("Размер сообщения не может превышать 2000 символов") 
                ;

            RuleFor(x => x.PersonGuid)
                .NotEmpty()
                .WithMessage("Необходимо указать Guid военнослужащего, отправляющего сообщение")
                .NotEqual(Person.Dummy.Guid)
                .WithMessage("Заглушка-отправитель может использоваться только со старыми сообщениями")
                ;

            RuleFor(x => x.RecieversGuids)
                .NotEmpty()
                .WithMessage("Необходимо указать Guid хотя бы одного получателя")
                ;
        }
    }
}