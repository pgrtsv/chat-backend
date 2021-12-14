namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Параметры пагинации
    /// </summary>
    public sealed class PagingSettings
    {
        /// <summary>
        /// Количество сообщений, выводимых на одной странице
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Page { get; set; }
    }
}