namespace ChatBackend.Persons.Domain.Operations
{
    /// <summary>
    /// Создаёт ФИО
    /// </summary>
    public sealed class CreateFullName
    {
        /// <summary>
        /// Имя
        /// </summary>
        /// <example>Иван</example>
        public string? FirstName { get; set; }
        
        /// <summary>
        /// Фамилия
        /// </summary>
        /// <example>Иванов</example>
        public string? LastName { get; set; }
        
        /// <summary>
        /// Отчество
        /// </summary>
        /// <example>Иванович</example>
        public string? ParentName { get; set; }
    }
}