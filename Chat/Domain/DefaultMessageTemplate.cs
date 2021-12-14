namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Стандартный (неизменяемый) шаблон сообщений
    /// </summary>
    public sealed class DefaultMessageTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleId"></param>
        /// <param name="content"></param>
        [ForbidUsage]
        public DefaultMessageTemplate(int id, int roleId, string content)
        {
            Id = id;
            RoleId = roleId;
            Content = content;
        }

        /// <summary>
        /// Id шаблона
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// Id роли пользователей
        /// </summary>
        public int RoleId { get; }
        
        /// <summary>
        /// Содержимое шаблона
        /// </summary>
        public string Content { get; }
    }
}