
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
        private System.Action<EventWorker> objOnComplete;
        /// <summary>
        /// Событие запуска обработки
        /// </summary>
        private System.Action<EventWorker> objOnCreated;

        /// <summary>
        /// Подключение события завершения обработки
        /// </summary>
        public event System.Action<EventWorker> OnComplete {
            add {
                this.objOnComplete -= value;
                this.objOnComplete += value;
            } 
            remove {
                this.objOnComplete -= value;
            }
        }
        /// <summary>
        /// Подключение события начала обработки
        /// </summary>
        public event System.Action<EventWorker> OnCreated {
            add {
                this.objOnCreated -= value;
                this.objOnCreated += value;
            } 
            remove {
                this.objOnCreated -= value;
            }
        }

        /// <summary>
        /// Событие обработчика
        /// </summary>
        private BaseEvent objElement;

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
        public BaseEvent element {
            get {return this.objElement;}
            protected set {this.objElement = value;}
        }

        /// <summary>
        /// Метод инициализаирует событие
        /// </summary>
        /// <param name="element">Элемент события</param>
        public abstract void Initialize(BaseEvent element);

        /// <summary>
        /// Метод возвращает случайные настройки события
        /// </summary>
        public abstract IEventContainer GenerateSettings();

        /// <summary>
        /// Вызов метода завершения обработки
        /// </summary>
        protected void OnWorkerComplete() {
            this.objOnComplete?.Invoke(this);
        }
        /// <summary>
        /// Вызов метода начала обработки
        /// </summary>
        protected void OnWorkerCreated() {
            this.objOnCreated?.Invoke(this);
        }

        public static implicit operator bool(EventWorker worker) {
            return Equals(worker, null) == false;
        }

    }
}