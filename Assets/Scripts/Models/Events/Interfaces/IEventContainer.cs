namespace InteractiveMap.Models {
    /// <summary>
    /// Интерфейс контейнера данных события
    /// </summary>
    public interface IEventContainer {
        /// <summary>
        /// Имя типа конвертации
        /// </summary>
        string typeName {get;}
        /// <summary>
        /// Метод сериализует данные контейнера в JSON
        /// </summary>
        /// <returns></returns>
        string Serialize();
    }
}