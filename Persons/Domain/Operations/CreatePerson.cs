namespace ChatBackend.Persons.Domain.Operations
{
    /// <summary>
    /// Создаёт запись о военнослужащем
    /// </summary>
    public sealed class CreatePerson
    {
        /// <summary>
        /// ФИО
        /// </summary>
        public CreateFullName? FullName { get; set; }
        
        /// <summary>
        /// Id воинского звания
        /// </summary>
        /// <example>1</example>
        public int? MilitaryRankId { get; set; }
    }
}