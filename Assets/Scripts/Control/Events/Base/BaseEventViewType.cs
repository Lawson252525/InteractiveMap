using InteractiveMap.Models;

namespace InteractiveMap.Control {
    /// <summary>
    /// Базовый компонент для типизированного события
    /// </summary>
    /// <typeparam name="T">Тип события</typeparam>
    public abstract class BaseEventViewType<T> : BaseEventView where T : BaseEvent {
        /// <summary>
        /// Свойсво устанавливает данные события
        /// </summary>
        public new T element {
            get {return base.element as T;}
            set {base.element = value;}
        }

    }
}