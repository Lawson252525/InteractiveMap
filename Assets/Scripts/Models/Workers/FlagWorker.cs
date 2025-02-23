using System.Linq;
using InteractiveMap.Control;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.Models {
    /// <summary>
    /// Обработчик события Метка
    /// </summary>
    public sealed class FlagWorker : EventWorkerType<Flag> {
        /// <summary>
        /// Презентер события Метка
        /// </summary>
        private FlagView view;
        /// <summary>
        /// Поле текстового изображения
        /// </summary>
        private Image imageView;
        /// <summary>
        /// Поле завершения обработки
        /// </summary>
        private bool isComplete = false;

        public FlagWorker() {
            //Загружаем презентер с данными события Метка
            string prefabName = "Events/Flag";
            this.view = Resources.Load<FlagView>(prefabName);
        }

        public override IEventContainer GenerateSettings() {
            IEventContainer result = null;

            if (this.view) {
                //Устанавливаем время жизни события
                System.DateTime startTime = System.DateTime.Now;
                float maxTime = this.view.maxTimeDelay;
                float minTime = this.view.minTimeDelay;
                float time = Random.Range(minTime, maxTime);
                var expiresTime = startTime.AddSeconds(time);
                
                //Установливаем случайную точку назначения
                var border = Map.Borders;
                var xMin = border.min.x;
                var xMax = border.max.x;
                var yMin = border.min.y;
                var yMax = border.max.y;

                var xPos = Random.Range(xMin, xMax);
                var yPos = Random.Range(yMin, yMax);
                var position = new Vector2(xPos, yPos);

                //Получение имени владельца метки, по умолчанию это локальный пользователь
                var localUser = Main.LocalUser;
                var name = localUser ? localUser.id : "Unknown";

                result = new FlagContainer() {position = position, expiresDate = expiresTime, creatorId = name};
            }

            return result;
        }

        public override void Initialize(BaseEvent element) {
            if (this.element) throw new System.Exception($"Нельзя инициализировать обработчик больше одного раза для типа {element.GetType()}");

            this.element = element as Flag;

            if (this.view) {
                //Инициализируем презентер события на карту
                var parent = Map.Instance.transform;
                this.view = GameObject.Instantiate<FlagView>(this.view, this.element.position, Quaternion.identity, parent);
                this.view.element = this.element;
                var size = this.view.GetComponent<SpriteRenderer>().bounds.size;

                //Инициализация элемента интерфейса
                var canvasRect = Map.Instance.canvasContainer;
                var iconPosition = this.element.position;
                iconPosition.y += size.y / 1.6f;
                this.imageView = GameObject.Instantiate<Image>(this.view.iconElement, iconPosition, Quaternion.identity, canvasRect);
                var textElement = this.imageView.GetComponentInChildren<Text>();
                var creator = Main.Instance.GetUsers().FirstOrDefault(u => u.id == this.element.creatorId);
                textElement.text = creator ? creator.name : "Unknown";

                //Вызываем событие создания обработчика
                OnWorkerCreated();
            } else throw new System.Exception($"Презентер события {element.GetType()} на задан");
        }

        public override void OnUpdate() {
            if (this.element is null || this.isComplete) return;
            else if (this.element.isComplete && this.isComplete == false) {
                this.isComplete = true;

                //Вызов события завершения обработки
                OnWorkerComplete();

                return;
            }

            //Обрабатываем презентер события Метка
            if (this.view) this.view.OnUpdate();
        }

        public override void Dispose() {
            //Удалем презентер
            if (this.view) GameObject.Destroy(this.view.gameObject);
            if (this.imageView) GameObject.Destroy(this.imageView.gameObject);
        }

    }
}