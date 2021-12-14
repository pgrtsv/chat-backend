using System;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Плейсхолдер в шаблоне сообщения посыльного по роте
    /// </summary>
    public sealed class Placeholder
    {
        /// <summary>
        /// Текст плейсхолдера
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Функция, возвращающая текст-заполнение плейсхолдера исходя из контекста посыльного
        /// </summary>
        private readonly Func<MessengerContext, string> _replacement;

        /// <summary>
        /// Возвращает текст шаблона сообщения, в котором все вхождения плейсхолдера заменены в соответствии с контекстом
        /// посыльного
        /// </summary>
        /// <param name="text">Текст шаблона сообщения</param>
        /// <param name="messengerContext">Контекст посыльного</param>
        public string FillPlaceholder(string text, MessengerContext messengerContext) =>
            text.Replace(Content, _replacement(messengerContext));

        private Placeholder(string content, Func<MessengerContext, string> replacement)
        {
            Content = content;
            _replacement = replacement;
        }

        /// <summary>
        /// Номер роты
        /// </summary>
        public static Placeholder Company { get; } = new("{рота}", x => x.Company.ToString());

        /// <summary>
        /// Налицо
        /// </summary>
        public static Placeholder Present { get; } = new("{налицо}", x => x.Present.ToString());

        /// <summary>
        /// В наряде
        /// </summary>
        public static Placeholder Duty { get; } = new("{наряд}", x => x.GetDuty().ToString());

        /// <summary>
        /// В увольнении
        /// </summary>
        public static Placeholder Leave { get; } = new("{увольнение}", x => x.Leave.ToString());

        /// <summary>
        /// В госпитале
        /// </summary>
        public static Placeholder Hospital { get; } = new("{госпиталь}", x => x.Hospitalized.ToString());

        /// <summary>
        /// В командировке
        /// </summary>
        public static Placeholder Business { get; } = new("{командировка}", x => x.Business.ToString());

        /// <summary>
        /// Ответственный
        /// </summary>
        public static Placeholder Responsible { get; } =
            new("{ответственный}", x => x.Responsible?.GetShortName() ?? "{ответственный}");

        /// <summary>
        /// Посыльный по роте дневной смены
        /// </summary>
        public static Placeholder DailyMessenger { get; } =
            new("{дневной_посыльный}", x => x.DailyMessenger?.GetShortName() ?? "{дневной_посыльный}");

        /// <summary>
        /// Посыльный по роте ночной смены
        /// </summary>
        public static Placeholder NightlyMessenger { get; } =
            new("{ночной_посыльный}", x => x.NightlyMessenger?.GetShortName() ?? "{ночной_посыльный}");

        /// <summary>
        /// Помощник дежурного по ЦОД
        /// </summary>
        public static Placeholder DpcHelper { get; } =
            new("{цод_посыльный}", x => x.DpcHelper?.GetShortName() ?? "{цод_посыльный}");

        /// <summary>
        /// Посыльный по технополису
        /// </summary>
        public static Placeholder TechnopolisMessenger { get; } =
            new("{технополис_посыльный}", x => x.TechopolisMessenger?.GetShortName() ?? "{технополис_посыльный}");

        /// <summary>
        /// Помощник дежурного по технополису
        /// </summary>
        public static Placeholder TechnopolisHelper { get; } =
            new("{технополис_помощник}", x => x.TechnopolisHelper?.GetShortName() ?? "{технополис_помощник}");

        /// <summary>
        /// Возвращает все доступные плейсхолдеры
        /// </summary>
        public static Placeholder[] GetAll() => new[]
        {
            Company,
            Present,
            Duty,
            Leave,
            Hospital,
            Business,
            Responsible,
            DailyMessenger,
            NightlyMessenger,
            DpcHelper,
            TechnopolisMessenger,
            TechnopolisHelper,
        };
    }
}