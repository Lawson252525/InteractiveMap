
namespace InteractiveMap.Models {
    /// <summary>
    /// Класс обработчик события
    /// Именно этот класс инициализирует и обрабатывает событие.
    /// Подбирает необходимые настройки события и любых его обработчиков
    /// </summary>
    public abstract class EventWorker : System.IDisposable {
        /// <summary>
        /// Событие завершения обработки
        /// </summary>
        public static System.Action<EventWorker> OnComplete;
        /// <summary>
        /// Событие запуска обработки
        /// </summary>
        public static System.Action<EventWorker> OnCreated;

        /// <summary>
        /// Событие обработчика
        /// </summary>
        private BaseEvent objElement;
        
        protected EventWorker(BaseEvent element) {
            this.objElement = element;
        }

        /// <summary>
        /// Метод обработки
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// Метод очистки обработчика
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Свойство возвращает событие обработчика
        /// </summary>
        public BaseEvent element => this.objElement;

        public static implicit operator bool(EventWorker worker) {
            return Equals(worker, null) == false;
        }

    }
}