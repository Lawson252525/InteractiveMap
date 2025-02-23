using InteractiveMap.Control;
using InteractiveMap.Utils;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Обработчик события Сокровище
    /// </summary>
    public sealed class TreasureWorker : EventWorkerType<Treasure> {
        /// <summary>
        /// Прещентер события Вихрь
        /// </summary>
        private TreasureView view;
        /// <summary>
        /// Поле завершения обработки
        /// </summary>
        private bool isComplete = false;

        public TreasureWorker() {
            //Загружаем презентер с данными события сокровище
            string prefabName = "Events/Treasure";
            this.view = Resources.Load<TreasureView>(prefabName);
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

        public override IEventContainer GenerateSettings() {
            IEventContainer result = default;

            if (this.view) {
                //Устанавливаем кол-во вознаграждения
                var maxAmount = this.view.maxAmount;
                var minAmount = this.view.minAmount;
                var amount = Random.Range(minAmount, maxAmount);

                //Устанавливаем время жизни события
                System.DateTime startTime = System.DateTime.Now;
                float maxTime = this.view.maxTimeDelay;
                float minTime = this.view.minTimeDelay;
                float time = Random.Range(minTime, maxTime);
                var expiresTime = startTime.AddSeconds(time);

                //Устанавливам случайную точку появления на любом острове
                var islands = Map.Instance.GetIslands();
                var island = islands[Random.Range(0, islands.Length - 1)];  
                var points = island.points;
                Vector2 position = island.transform.position;
                int index = 0;
                while(true) {
                    try {
                        position = Triangulator.GetRandomPointWithin(points);
                    } catch {}

                    index += 1;
                    if (index > 10) break;
                }

                //Формируем контейнер с данными события Сокровище
                var container = new TreasureContainer();
                container.amount = amount;
                container.expiresDate = expiresTime;
                container.position = position;
                result = container;
            }

            return result;
        }

        public override void Initialize(BaseEvent element) {
            if (this.element) throw new System.Exception($"Нельзя инициализировать обработчик больше одного раза для типа {element.GetType()}");

            this.element = element as Treasure;

            if (this.view) {
                //Инициализируем презентер события на карту
                var parent = Map.Instance.transform;
                this.view = GameObject.Instantiate<TreasureView>(this.view, this.element.position, Quaternion.identity, parent);
                this.view.element = this.element;

                //Вызываем событие создания обработчика
                OnWorkerCreated();
            } else throw new System.Exception($"Презентер события {element.GetType()} на задан");
        }

        public override void Dispose() {
            //Удалем презентер
            if (this.view) GameObject.Destroy(this.view.gameObject);
        }
    }
}