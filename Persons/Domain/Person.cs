using System;
using ChatBackend.Persons.Domain.Operations;
using ChatBackend.Persons.Domain.Validators;
using FluentValidation;

namespace ChatBackend.Persons.Domain
{
    /// <summary>
    /// Военнослужащий
    /// </summary>
    public sealed class Person
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="fullName"></param>
        /// <param name="rank"></param>
        [ForbidUsage]
        public Person(Guid guid, FullName fullName, MilitaryRank rank)
        {
            Guid = guid;
            FullName = fullName;
            Rank = rank;
        }

        /// <summary>
        /// Guid военнослужащего
        /// </summary>
        public Guid Guid { get; }
        
        /// <summary>
        /// ФИО
        /// </summary>
        public FullName FullName { get; }
        
        /// <summary>
        /// Воинское звание
        /// </summary>
        public MilitaryRank Rank { get; }

        /// <summary>
        /// Возвращает короткую запись военнослужащего
        /// </summary>
        /// <example>ряд. Иванов И.И.</example>
        public string GetShortName() => $"{Rank.ShortName} {FullName.GetShortName()}";

        /// <summary>
        /// Возвращает полную запись военнослужащего
        /// </summary>
        /// <example>рядовой Иванов Иван Иванович</example>
        public string GetFullName() => $"{Rank.FullName} {FullName.GetFullName()}";

        /// <summary>
        /// Создаёт новую запись о военнослужащем
        /// </summary>
        /// <param name="createPerson"></param>
        public static Person New(CreatePerson createPerson)
        {
            new CreatePersonValidator().ValidateAndThrow(createPerson);
            return new Person(
                Guid.NewGuid(), 
                FullName.New(createPerson.FullName!),
                MilitaryRank.GetById(createPerson.MilitaryRankId ?? 0));
        }
        
        /// <summary>
        /// Заглушка, означающая неизвестного военнослужащего
        /// Только для старых сообщений
        /// </summary>
        public static Person Dummy { get; } = new(
            Guid.Parse("1277e7a2-2956-4079-a1ae-aeb896cbda0b"),
            new FullName(
                "Неизвестный",
                "Неизвестный",
                "Неизвестный"),
            MilitaryRank.None);
    }
}