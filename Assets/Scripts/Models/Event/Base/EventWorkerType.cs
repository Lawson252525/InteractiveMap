
namespace InteractiveMap.Models {
    /// <summary>
    /// Класс управления событием с типизацией
    /// </summary>
    /// <typeparam name="T">Тип события</typeparam>
    public abstract class EventWorkerType<T> : EventWorker where T : BaseEvent
    {
        protected EventWorkerType(BaseEvent element) : base(element){}

        public EventWorkerType(T element) : base(element) {}

        /// <summary>
        /// Свойство возвращает типизированное событие обработчика
        /// </summary>
        public new T element => base.element as T;

    }
}