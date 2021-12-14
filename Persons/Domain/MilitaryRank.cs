using System.Linq;

namespace ChatBackend.Persons.Domain
{
    /// <summary>
    /// Воинское звание
    /// </summary>
    public sealed class MilitaryRank
    {
        private MilitaryRank(int id, string fullName, string shortName)
        {
            Id = id;
            FullName = fullName;
            ShortName = shortName;
        }

        /// <summary>
        /// Id воинского звания
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// Полное название звания
        /// </summary>
        public string FullName { get; }
        
        /// <summary>
        /// Сокращенное название звания
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Без воинского звания (гражданский персонал)
        /// </summary>
        public static MilitaryRank None = new (0, string.Empty, string.Empty);

        /// <summary>
        /// Рядовой
        /// </summary>
        public static MilitaryRank Private = new(1, "рядовой", "ряд.");

        /// <summary>
        /// Ефрейтор
        /// </summary>
        public static MilitaryRank Corporal = new(2, "ефрейтор", "ефр.");

        /// <summary>
        /// Младший сержант
        /// </summary>
        public static MilitaryRank JrSergeant = new(3, "младший сержант", "мл. с-т");
        
        /// <summary>
        /// Сержант
        /// </summary>
        public static MilitaryRank Sergeant = new(4, "сержант", "с-т");

        /// <summary>
        /// Старший сержант
        /// </summary>
        public static MilitaryRank SrSergeant = new(5, "старший сержант", "ст. с-т");

        /// <summary>
        /// Прапорщик
        /// </summary>
        public static MilitaryRank Ensign = new(7, "прапорщик", "пр-к");

        /// <summary>
        /// Старший прапорщик
        /// </summary>
        public static MilitaryRank SrEnsign = new(8, "старший прапорщик", "ст. пр-к");

        /// <summary>
        /// Младший лейтенант
        /// </summary>
        public static MilitaryRank JrLeutenant = new(9, "младший лейтенант", "мл. л-т");

        /// <summary>
        /// Лейтенант
        /// </summary>
        public static MilitaryRank Leutenant = new(10, "лейтенант", "л-т");

        /// <summary>
        /// Старший лейтенант
        /// </summary>
        public static MilitaryRank SrLeutenant = new(11, "старший лейтенант", "ст. л-т");

        /// <summary>
        /// Капитан
        /// </summary>
        public static MilitaryRank Captain = new(12, "капитан", "к-н");

        /// <summary>
        /// Майор
        /// </summary>
        public static MilitaryRank Major = new(13, "майор", "м-р");

        /// <summary>
        /// Подполковник
        /// </summary>
        public static MilitaryRank LeutenantColonel = new(14, "подполковник", "пп-к");

        /// <summary>
        /// Полковник
        /// </summary>
        public static MilitaryRank Colonel = new(15, "полковник", "п-к");

        /// <summary>
        /// Генерал-майор
        /// </summary>
        public static MilitaryRank MajorGeneral = new(16, "генерал-майор", "ген. м-р");

        /// <summary>
        /// Генерал-лейтенант
        /// </summary>
        public static MilitaryRank LieutenantGeneral = new(17, "генерал-лейтенант", "ген. л-т");

        /// <summary>
        /// Генерал-полковник
        /// </summary>
        public static MilitaryRank ColonelGeneral = new(18, "генерал-полковник", "ген. п-к");

        /// <summary>
        /// Возвращает все звания
        /// </summary>
        public static MilitaryRank[] GetAll() => new[]
        {
            None,
            Private,
            Corporal,
            JrSergeant,
            Sergeant,
            SrSergeant,
            Ensign,
            SrEnsign,
            JrLeutenant,
            Leutenant,
            SrLeutenant,
            Captain,
            Major,
            LeutenantColonel,
            Colonel,
            MajorGeneral,
            LieutenantGeneral,
            ColonelGeneral,
        };

        /// <summary>
        /// Возвращает воинское звание по Id
        /// </summary>
        /// <param name="id"></param>
        public static MilitaryRank GetById(int id) => GetAll().First(x => x.Id == id);
    }
}