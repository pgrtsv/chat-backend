using System.Diagnostics.CodeAnalysis;

namespace ChatBackend.Chat.Domain.Operations
{
    /// <summary>
    /// Заполняет плейсхолдеры в шаблоне сообщения
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global",
        Justification = "Публичные сеттеры используются фреймворком")]
    public sealed class FillPlaceholders
    {
        /// <summary>
        /// Шаблон сообщения
        /// </summary>
        public string? MessageTemplateContent { get; set; }

        /// <summary>
        /// Контекст посыльного
        /// </summary>
        public CreateMessengerContext? MessengerContext { get; set; }
    }
}