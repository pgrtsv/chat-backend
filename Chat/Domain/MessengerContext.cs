using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Chat.Domain.Operations;
using ChatBackend.Chat.Domain.Validators;
using ChatBackend.Persons.Domain;
using FluentValidation;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Контекст посыльного
    /// </summary>
    public sealed class MessengerContext
    {
        private MessengerContext(int company, int present, int leave, int hospitalized, int business,
            Person? dailyMessenger, Person? nightlyMessenger, Person? dpcHelper, Person? techopolisMessenger,
            Person? technopolisHelper, Person? responsible)
        {
            Company = company;
            Present = present;
            Leave = leave;
            Hospitalized = hospitalized;
            Business = business;
            DailyMessenger = dailyMessenger;
            NightlyMessenger = nightlyMessenger;
            DpcHelper = dpcHelper;
            TechopolisMessenger = techopolisMessenger;
            TechnopolisHelper = technopolisHelper;
            Responsible = responsible;
        }

        /// <summary>
        /// Номер роты
        /// </summary>
        public int Company { get; }

        /// <summary>
        /// Количество л/с налицо
        /// </summary>
        public int Present { get; }

        /// <summary>
        /// Количество л/с в увольнении
        /// </summary>
        public int Leave { get; }

        /// <summary>
        /// Количество л/с в госпитале
        /// </summary>
        public int Hospitalized { get; }

        /// <summary>
        /// Количество л/с в командировке
        /// </summary>
        public int Business { get; }

        /// <summary>
        /// Посыльный дневной смены
        /// </summary>
        public Person? DailyMessenger { get; }

        /// <summary>
        /// Посыльный ночной смены
        /// </summary>
        public Person? NightlyMessenger { get; }

        /// <summary>
        /// Помощник дежурного по ЦОД
        /// </summary>
        public Person? DpcHelper { get; }

        /// <summary>
        /// Посыльный по технополису
        /// </summary>
        public Person? TechopolisMessenger { get; }

        /// <summary>
        /// Помощник дежурного по технополису
        /// </summary>
        public Person? TechnopolisHelper { get; }

        /// <summary>
        /// Ответственный по роте
        /// </summary>
        public Person? Responsible { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createMessengerContext"></param>
        /// <param name="chatValidatorsRepository"></param>
        /// <param name="personsRepository"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ValidationException"></exception>
        public static async Task<MessengerContext> NewAsync(
            CreateMessengerContext createMessengerContext,
            IChatValidatorsRepository chatValidatorsRepository,
            IPersonsRepository personsRepository,
            CancellationToken cancellationToken)
        {
            await new CreateMessengerContextValidator(chatValidatorsRepository)
                .ValidateAndThrowAsync(createMessengerContext, cancellationToken);
            return new(
                createMessengerContext.Company,
                createMessengerContext.Present,
                createMessengerContext.Leave,
                createMessengerContext.Hospitalized,
                createMessengerContext.Business,
                createMessengerContext.DailyMessenger == default
                    ? null
                    : await personsRepository.GetByGuidAsync(createMessengerContext.DailyMessenger, cancellationToken),
                createMessengerContext.NightlyMessenger == default
                    ? null
                    : await personsRepository.GetByGuidAsync(createMessengerContext.NightlyMessenger,
                        cancellationToken),
                createMessengerContext.DpcHelper == default
                    ? null
                    : await personsRepository.GetByGuidAsync(createMessengerContext.DpcHelper, cancellationToken),
                createMessengerContext.TechopolisMessenger == default
                    ? null
                    : await personsRepository.GetByGuidAsync(createMessengerContext.TechopolisMessenger,
                        cancellationToken),
                createMessengerContext.TechnopolisHelper == default
                    ? null
                    : await personsRepository.GetByGuidAsync(createMessengerContext.TechnopolisHelper,
                        cancellationToken),
                createMessengerContext.Responsible == default
                    ? null
                    : await personsRepository.GetByGuidAsync(createMessengerContext.Responsible, cancellationToken));
        }

        /// <summary>
        /// Возвращает количество личного состава в суточном наряде
        /// </summary>
        public int GetDuty() =>
            (DailyMessenger == null ? 0 : 1) +
            (NightlyMessenger == null ? 0 : 1) +
            (DpcHelper == null ? 0 : 1) +
            (TechopolisMessenger == null ? 0 : 1) +
            (TechnopolisHelper == null ? 0 : 1);

        /// <summary>
        /// Возвращает количество л/с
        /// </summary>
        public int GetTotal() => Present + Leave + Hospitalized + Business + GetDuty();
    }
}