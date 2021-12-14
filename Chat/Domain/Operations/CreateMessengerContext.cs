using System;
using System.Diagnostics.CodeAnalysis;

namespace ChatBackend.Chat.Domain.Operations
{
    /// <summary>
    /// Создаёт контекст посыльного
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public sealed class CreateMessengerContext
    {
        /// <summary>
        /// Номер роты
        /// </summary>
        public int Company { get; set; }
        
        /// <summary>
        /// Количество л/с налицо
        /// </summary>
        public int Present { get; set; }
        
        /// <summary>
        /// Количество л/с в увольнении
        /// </summary>
        public int Leave { get; set; }
        
        /// <summary>
        /// Количество л/с в госпитале
        /// </summary>
        public int Hospitalized { get; set; }
        
        /// <summary>
        /// Количество л/с в командировке
        /// </summary>
        public int Business { get; set; }

        /// <summary>
        /// Посыльный дневной смены
        /// </summary>
        public Guid DailyMessenger { get; set; }
        
        /// <summary>
        /// Посыльный ночной смены
        /// </summary>
        public Guid NightlyMessenger { get; set; }
        
        /// <summary>
        /// Помощник дежурного по ЦОД
        /// </summary>
        public Guid DpcHelper { get; set; }
        
        /// <summary>
        /// Посыльный по технополису
        /// </summary>
        public Guid TechopolisMessenger { get; set; }
        
        /// <summary>
        /// Помощник дежурного по технополису
        /// </summary>
        public Guid TechnopolisHelper { get; set; }
        
        /// <summary>
        /// Ответственный по роте
        /// </summary>
        public Guid Responsible { get; set; }
    }
}