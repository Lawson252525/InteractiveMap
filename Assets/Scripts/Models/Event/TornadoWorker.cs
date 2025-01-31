using InteractiveMap.Control;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для обработки события Вихрь
    /// </summary>
    public sealed class TornadoWorker : EventWorkerType<Tornado> {
        /// <summary>
        /// Поле завершения обработки
        /// </summary>
        private bool isComplete = false;
        /// <summary>
        /// Прещентер события Вихрь
        /// </summary>
        private TornadoView view;

        public TornadoWorker(BaseEvent element) : base(element) {
            //Загружаем префаб презентер события
            string prefabName = "Events/Tornado";
            this.view = Resources.Load<TornadoView>(prefabName);
            if (this.view) {
                //Инициализируем презентер события
                var parent = Map.Instance.transform;
                var position = this.element.position;
                this.view = GameObject.Instantiate<TornadoView>(this.view, position, Quaternion.identity, parent);

                //Вызов события создания обработчика
                OnCreated?.Invoke(this);
            }
        }

        public override void OnUpdate() {
            if (this.isComplete) return;
            else if (this.element.isComplete && this.isComplete == false) {
                this.isComplete = true;

                //Вызов события завершения обработки
                OnComplete?.Invoke(this);

                return;
            }

            //Обрабатываем презентер события Вихрь
            if (this.view) this.view.OnUpdate();
        }

        public override void Dispose() {
            //Удалем презентер
            if (this.view) GameObject.Destroy(this.view.gameObject);
        }

    }
}