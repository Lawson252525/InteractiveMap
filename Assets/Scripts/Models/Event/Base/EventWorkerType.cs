
namespace InteractiveMap.Models {
    /// <summary>
    /// Класс управления событием с типизацией
    /// </summary>
    /// <typeparam name="T">Тип события</typeparam>
    public abstract class EventWorkerType<T> : EventWorker where T : BaseEvent {
        /// <summary>
        /// Свойство возвращает типизированное событие обработчика
        /// </summary>
        public new T element {
            get {return base.element as T;}
            protected set {base.element = value;}
        }

    }
}