using System.Linq;

namespace ChatBackend.Users.Domain
{
    /// <summary>
    /// Роль пользователя чата
    /// </summary>
    public sealed class UserRole 
    {
        /// <summary>
        /// Id роли
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Нормализованное название роли
        /// </summary>
        public string NormalizedName { get; }
        
        private UserRole(int id, string name, string normalizedName)
        {
            Id = id;
            Name = name;
            NormalizedName = normalizedName;
        }

        /// <summary>
        /// Пользователь
        /// </summary>
        public static UserRole User = new(1, "Пользователь", nameof(User));
        
        /// <summary>
        /// Администратор
        /// </summary>
        public static UserRole Administrator = new(2, "Администратор", nameof(Administrator));
        
        /// <summary>
        /// Посыльный
        /// </summary>
        public static UserRole Messenger = new(3, "Посыльный", nameof(Messenger));

        /// <summary>
        /// Возвращает все роли
        /// </summary>
        public static UserRole[] GetAll() => new[] {User, Administrator, Messenger};

        /// <summary>
        /// Возвращает роль с указанным Id
        /// </summary>
        /// <param name="id">Id роли</param>
        public static UserRole GetById(int id) => GetAll().First(x => x.Id == id);
        
        /// <summary>
        /// Возвращает роль с указанным именем
        /// </summary>
        /// <param name="name">Имя роли</param>
        public static UserRole GetByName(string name) => GetAll().First(x => x.Name == name);

        /// <summary>
        /// Возвращает роль с указанным нормализованным именем
        /// </summary>
        /// <param name="normalizedName">Нормализованное имя</param>
        public static UserRole GetByNormalizedName(string normalizedName) =>
            GetAll().First(x => x.NormalizedName == normalizedName);
    }
}