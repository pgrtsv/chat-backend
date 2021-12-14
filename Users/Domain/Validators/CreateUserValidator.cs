using System.Linq;
using ChatBackend.Users.Domain.Operations;
using FluentValidation;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable StringLiteralTypo
#pragma warning disable 1591

namespace ChatBackend.Users.Domain.Validators
{
    public sealed class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public static int MinLoginLength => 3;
        public static int MaxLoginLength => 30;
        public static string LoginAllowedSymbols => "латинские буквы, цифры и символ _";

        public static int MinPasswordLength => 6;
        public static int MaxPasswordLength => 30;
        public static string PasswordAllowedSymbols => "латинские буквы, цифры и символ _";

        public static int MinNameLength => 3;
        public static int MaxNameLength => 100;
        
        public CreateUserValidator(
            IUserValidatorsService userValidatorsService)
        {
            RuleFor(x => x.Login)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать логин пользователя")
                .MinimumLength(MinLoginLength)
                .WithMessage($"Логин пользователя слишком короткий (менее {MinLoginLength} символов)")
                .MaximumLength(MaxLoginLength)
                .WithMessage($"Логин пользователя слишком длинный (более {MaxLoginLength} символов)")
                .Matches(@"^[aA-zZ0-9_]*$")
                .WithMessage($"Логин пользователя может содержать только {LoginAllowedSymbols}")
                .MustAsync(async (login, cancellationToken) => !await userValidatorsService
                    .CheckIfLoginExistsAsync(login, cancellationToken))
                .WithMessage($"Пользователь с таким логином уже существует")
                ;

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать пароль пользователя")
                .MinimumLength(MinPasswordLength)
                .WithMessage($"Пароль пользователя слишком короткий (менее {MinPasswordLength} символов)")
                .MaximumLength(MaxPasswordLength)
                .WithMessage($"Пароль пользователя слишком длинный (более {MaxPasswordLength} символов)")
                .Matches(@"^[aA-zZ0-9_]*$")
                .WithMessage($"Пароль пользователя может содержать только {PasswordAllowedSymbols}")
                ;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать имя пользователя")
                .MinimumLength(MinNameLength)
                .WithMessage($"Имя пользователя слишком короткое (менее {MinNameLength} символов)")
                .MaximumLength(MaxNameLength)
                .WithMessage($"Имя пользователя слишком длинное (более {MaxNameLength} символов)")
                .MustAsync(async (name, cancellationToken) => !await userValidatorsService
                    .CheckIfUsernameExistsAsync(name, cancellationToken))
                ;

            RuleFor(x => x.ShortName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать короткое имя пользователя")
                .MinimumLength(2)
                .WithMessage("Короткое имя пользователя слишком короткое (менее 2 символов)")
                .MaximumLength(15)
                .WithMessage("Короткое имя пользователя слишком длинное (более 15 символов)")
                .MustAsync(async (name, cancellationToken) => !await userValidatorsService
                    .CheckIfShortNameExistsAsync(name, cancellationToken))
                ;

            RuleFor(x => x.Roles)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Необходимо указать хотя бы одну роль пользователя")
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("Нельзя указать одну роль несколько раз")
                .Must(roles => UserRole.GetAll().Select(x => x.Id).Intersect(roles).Count() == roles.Count)
                .WithMessage("Указан несуществующий Id роли")
                ;
        }
    }
}