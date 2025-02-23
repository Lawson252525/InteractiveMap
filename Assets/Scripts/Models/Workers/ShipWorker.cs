using System.Linq;
using InteractiveMap.Control;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для обработки события Корабль
    /// </summary>
    public sealed class ShipWorker : EventWorkerType<Ship> {
        /// <summary>
        /// Поле тип корабля
        /// </summary>
        private Ship.Types shipType;
        /// <summary>
        /// Поле текстового изображения
        /// </summary>
        private Image imageView;
        /// <summary>
        /// Прещентер события Корабль
        /// </summary>
        private ShipView view;

        public ShipWorker() : this(Ship.Types.Small) {}

        public ShipWorker(Ship.Types type) {
            this.shipType = type;
            this.view = LoadPrefab();
        }

        public override void OnUpdate() {
            if (this.element is null) return;

            //Обрабатываем презентер события Корабль
            if (this.view) {
                this.view.OnUpdate();

                //Обрабатываем элемент заголовка
                if (this.imageView) {
                    var size = this.view.GetComponent<SpriteRenderer>().bounds.size;
                    var iconPosition = this.view.transform.position;
                    iconPosition.y += size.y / 1.6f;
                    this.imageView.transform.position = iconPosition;

                    //Обрабатываем имя
                    var label = this.imageView.GetComponentInChildren<Text>();
                    if (label) label.text = this.element.name;
                }
            }
        }

        public override IEventContainer GenerateSettings() {
            //Генерируем случаынйе настройки появления корабля на карте
            if (this.view) {
                //Генерируем размер команды
                var maxCrewSize = this.view.maxCrewSize;
                var minCrewSize = this.view.minCrewSize;
                var crewSize = Random.Range(minCrewSize, maxCrewSize);

                //Генерируем скорость корабля
                float maxSpeed = this.view.maxMoveSpeed;
                float minSpeed = this.view.minMoveSpeed;
                float speed = Random.Range(minSpeed, maxSpeed);

                //Генерируем случайную точку появления
                Vector2 position = Vector2.zero;
                if (Map.Instance) {
                    var islands = Map.Instance.GetIslands().Select(i => i.border);
                    var border = Map.Borders;
                    var xMin = border.min.x;
                    var xMax = border.max.x;
                    var yMin = border.min.y;
                    var yMax = border.max.y;
                    var index = 0;
                    while(index < 10) {
                        var xPos = Random.Range(xMin, xMax);
                        var yPos = Random.Range(yMin, yMax);
                        var newPosition = new Vector2(xPos, yPos);
                        if (islands.Any(i => i.Contains(newPosition)) == false) {
                            position = newPosition;
                            break;
                        }

                        index += 1;
                        if (index > 10) break;
                    }
                }

                //Возвращаем контейнер с настройками
                return new ShipContainer() {
                    name = "Корабль",
                    description = "Описание",
                    crewSize = crewSize,
                    position = position,
                    speed = speed,
                    type = this.type
                };
            }

            return null;
        }

        /// <summary>
        /// Свойство возвращает тип корабля
        /// </summary>
        public Ship.Types type {
            get {
                return this.element ? this.element.type : this.shipType;
            }
        }

        /// <summary>
        /// Метод загружает префаб элемент с условием
        /// </summary>
        private ShipView LoadPrefab() {
            var prefabName = "Events/Ships/";
            switch(this.type) {
                case Ship.Types.Small: prefabName += "ShipSmall";
                break;
                case Ship.Types.Middle: prefabName += "ShipMiddle";
                break;
                case Ship.Types.Big: prefabName += "ShipBig";
                break;
            }

            return Resources.Load<ShipView>(prefabName);
        }

        public override void Initialize(BaseEvent element) {
            if (this.element) throw new System.Exception($"Нельзя инициализировать обработчик больше одного раза для типа {element.GetType()}");

            this.element = element as Ship;
            this.view = LoadPrefab();

            if (this.view) {
                //Инициализируем презентер события на карту
                var parent = Map.Instance.transform;
                this.view = GameObject.Instantiate<ShipView>(this.view, this.element.position, Quaternion.identity, parent);
                this.view.element = this.element;

                //Инициализация элемента интерфейса
                var canvasRect = Map.Instance.canvasContainer;
                this.imageView = GameObject.Instantiate<Image>(this.view.iconElement, this.element.position, Quaternion.identity, canvasRect);
                var textElement = this.imageView.GetComponentInChildren<Text>();
                textElement.text = this.element.name;

                //Вызываем событие создания обработчика
                OnWorkerCreated();
                
            } else throw new System.Exception($"Презентер события {element.GetType()} на задан");
        }

        public override void Dispose() {
            //Удаляем презентер события
            if (this.view) GameObject.Destroy(this.view.gameObject);
            if (this.imageView) GameObject.Destroy(this.imageView.gameObject);
        }

    }
}