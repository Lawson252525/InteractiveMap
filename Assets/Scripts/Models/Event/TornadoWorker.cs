using System;
using System.Linq;
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

        public TornadoWorker() {
            //Загружаем презентер события Вихрь
            this.view = LoadPrefab();
        }

        public override IEventContainer GetSettings() {
            IEventContainer result = null;

            //Генерируем настройки события случайно
            if (this.view) {
                //Устанавливаем скорость перемещения события
                float maxSpeed = this.view.maxMoveSpeed;
                float minSpeed = this.view.minMoveSpeed;
                float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

                //Устанавливаем время жизни события
                DateTime startTime = DateTime.Now;
                float maxTime = this.view.maxTimeDelay;
                float minTime = this.view.minTimeDelay;
                float time = UnityEngine.Random.Range(minTime, maxTime);
                DateTime expiresTime = startTime.AddSeconds(time);

                //Устанавливаем точку создания события
                var section = Map.Instance.GetSections();
                Vector2 position = Map.Instance.transform.position;

                //Устанавливаем номер секции события
                Vector2Int sectionIndex = section.FirstOrDefault(s => s.size.Contains(position)).index;

                //Установливаем случайную точку назначения
                Vector2 destination = section[UnityEngine.Random.Range(0, section.Length - 1)].size.center;

                //Создаем контейнер с настройками события
                var container = new TornadoContainer();
                container.position = position;
                container.destination = destination;
                container.sectionIndex = sectionIndex;
                container.expires = expiresTime;
                container.speed = speed;

                result = container;
            }

            return result;
        }

        public override void Initialize(BaseEvent element) {
            if (this.element) throw new Exception($"Нельзя инициализировать обработчик больше одного раза для типа {element.GetType()}");

            this.element = element as Tornado;

            if (this.view) {
                //Инициализируем презентер события на карту
                var parent = Map.Instance.transform;
                this.view = GameObject.Instantiate<TornadoView>(view, this.element.position, Quaternion.identity, parent);
                this.view.element = this.element;

                //Вызываем событие создания обработчика
                OnCreated?.Invoke(this);
            } else throw new Exception($"Презентер события {element.GetType()} на задан");
        }

        /// <summary>
        /// Метод возвращает префаб презентера события Вихрь
        /// </summary>
        /// <returns>Презенетер события</returns>
        private TornadoView LoadPrefab() {
            string prefabName = "Events/Tornado";
            return Resources.Load<TornadoView>(prefabName);
        }

        public override void OnUpdate() {
            if (this.element is null || this.isComplete) return;
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