using ChatBackend.Persons.Domain.Operations;
using ChatBackend.Persons.Domain.Validators;
using FluentValidation;

namespace ChatBackend.Persons.Domain
{
    /// <summary>
    /// ФИО
    /// </summary>
    public sealed class FullName
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; }
        
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; }
        
        /// <summary>
        /// Отчество
        /// </summary>
        public string ParentName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="parentName"></param>
        [ForbidUsage]
        public FullName(string firstName, string lastName, string parentName)
        {
            FirstName = firstName;
            LastName = lastName;
            ParentName = parentName;
        }

        /// <summary>
        /// Возвращает полное ФИО
        /// </summary>
        public string GetFullName() => $"{LastName} {FirstName} {ParentName}";

        /// <summary>
        /// Возвращает фамилию и инициалы
        /// </summary>
        /// <returns></returns>
        public string GetShortName() => $"{LastName} {FirstName[0]}.{ParentName[0]}.";

        /// <summary>
        /// Создаёт ФИО
        /// </summary>
        /// <param name="createFullName"></param>
        public static FullName New(CreateFullName createFullName)
        {
            new CreateFullNameValidator().ValidateAndThrow(createFullName);
            return new FullName(
                createFullName.FirstName!,
                createFullName.LastName!,
                createFullName.ParentName!);
        }
    }
}