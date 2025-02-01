namespace InteractiveMap.Models {
    /// <summary>
    /// Интерфейс события который помещается в базу данных
    /// </summary>
    public interface IEventBase : IEventElement {
        /// <summary>
        /// Свойство изменения события
        /// </summary>
        bool isChanged {get;}
        /// <summary>
        /// Сбрасывает изменения у события
        /// </summary>
        void Reset();
        /// <summary>
        /// Метод возвращает контейнер с данными события
        /// </summary>
        IEventContainer GetContainer();
        /// <summary>
        /// Метод применят данные из контейнера к событию
        /// </summary>
        /// <param name="container">Контейнер с данными</param>
        void ApplyContainer(IEventContainer container);
    }
}