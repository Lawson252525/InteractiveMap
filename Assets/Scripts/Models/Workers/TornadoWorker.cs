using System;
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

        public override IEventContainer GenerateSettings() {
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

                var sections = Map.Instance.GetSections();
                var section = sections[UnityEngine.Random.Range(0, sections.Length - 1)];

                //Устанавливаем точку создания события
                Vector2 position = section.size.center;

                //Устанавливаем номер секции события
                Vector2Int sectionIndex = section.index;

                //Установливаем случайную точку назначения
                var border = Map.Borders;
                var xMin = border.min.x;
                var xMax = border.max.x;
                var yMin = border.min.y;
                var yMax = border.max.y;

                var xPos = UnityEngine.Random.Range(xMin, xMax);
                var yPos = UnityEngine.Random.Range(yMin, yMax);
                Vector2 destination = new Vector2(xPos, yPos);

                //Создаем контейнер с настройками события
                var container = new TornadoContainer();
                container.position = new Vector2(position.x, position.y);
                container.destination = new Vector2(destination.x, destination.y);;
                container.sectionIndex = sectionIndex;
                container.expiresDate = expiresTime;
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
                OnWorkerCreated();
                
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
                OnWorkerComplete();

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