using InteractiveMap.Control;
using UnityEngine;

namespace InteractiveMap.Models {
    public sealed class TreasureWorker : EventWorkerType<Treasure> {
        /// <summary>
        /// Поле завершения обработки
        /// </summary>
        private bool isComplete = false;
        /// <summary>
        /// Прещентер события Вихрь
        /// </summary>
        private TreasureView view;

        public TreasureWorker() {
            //Загружаем презентер с данными события сокровище
            string prefabName = "Events/Treasure";
            this.view = Resources.Load<TreasureView>(prefabName);
        }

        public override void OnUpdate() {}

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
                var position = points[Random.Range(0, points.Length - 1)];
                var border = island.border;
                var max = border.max;
                var min = border.min;
                var index = 0;
                while(true) {
                    var x = Random.Range(min.x, max.x);
                    var y = Random.Range(min.y, max.y);
                    position = new Vector2(x, y);
                    if (island.Contains(position)) break;

                    index += 1;
                    if (index >= 100) break;
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