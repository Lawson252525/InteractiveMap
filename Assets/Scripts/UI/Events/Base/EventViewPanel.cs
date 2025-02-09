using InteractiveMap.Models;

namespace InteractiveMap.UI {
    /// <summary>
    /// Типизированный компоненту правления событием
    /// </summary>
    /// <typeparam name="T">Тип события</typeparam>
    public abstract class EventViewPanel<T> : ViewPanel where T : BaseEvent {
        /// <summary>
        /// Поле элемента события
        /// </summary>
        private T objElement;
        /// <summary>
        /// Свойство возвращает тип элемента
        /// </summary>
        public T element {
            get {return this.element;}
            set {this.objElement = value;}
        }

    }
}
