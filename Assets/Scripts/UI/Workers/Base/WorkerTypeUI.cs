using InteractiveMap.Models;

namespace InteractiveMap.UI {
    public abstract class WorkerTypeUI<T> : WorkerUI where T : EventWorker, new() {
        /// <summary>
        /// Поле обработчика события
        /// </summary>
        private T objWorker;

        /// <summary>
        /// Свойство возвращает тип обработчика события
        /// </summary>
        public override EventWorker worker => this.objWorker;

        public override void Reset() {
            base.Reset();

            //Создаем новый экземпляр обработчика события
            this.objWorker = new T();
        }

    }
}