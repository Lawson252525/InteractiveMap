using System;

namespace InteractiveMap.Models {
    /// <summary>
    /// Интерфейс события
    /// </summary>
    public interface IEventElement {
        /// <summary>
        /// Ключ события
        /// </summary>
        string id {get;}
        /// <summary>
        /// Дата создания события
        /// </summary>
        DateTime creationTime {get;}
        /// <summary>
        /// Владелец события
        /// </summary>
        string owner {get;}
        /// <summary>
        /// Метод завершения события
        /// </summary>
        void Complete();
        /// <summary>
        /// Событие завершено или нет
        /// </summary>
        bool isComplete {get;}
    }
}