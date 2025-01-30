using System;

namespace InteractiveMap.Models {
    /// <summary>
    /// Интерфейс события
    /// </summary>
    interface IEventElement {
        /// <summary>
        /// Создатель события, может быть Null
        /// </summary>
        User creator {get;}
        /// <summary>
        /// Дата создания события
        /// </summary>
        DateTime date {get;}
        /// <summary>
        /// Секция где находится событие
        /// </summary>
        Section section {get;}
    }
}