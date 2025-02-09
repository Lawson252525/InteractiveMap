using UnityEngine;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент управления блокирующими панеля
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
    public class BlockPanel : BaseUI {
        /// <summary>
        /// Поле группы элементов
        /// </summary>
        private CanvasGroup group;

        protected override void Awake() {
            base.Awake();

            //Получаем ссылку на управление группой
            this.group = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Свойство возвращает значения видимости окна
        /// </summary>
        public bool visible {
            get {
                return this.group.interactable && this.group.blocksRaycasts;
            }
            set {
                if (this.visible != value) {
                    this.group.interactable = this.group.blocksRaycasts = value;
                    this.group.alpha = (value) ? 1f : 0f;

                    OnChanged(value);
                }
            }
        }

        /// <summary>
        /// Событие открытия или закрытия панели
        /// </summary>
        /// <param name="flag">Значение открытия\закрытия</param>
        protected virtual void OnChanged(bool flag) {}

        /// <summary>
        /// Метод открытия панели
        /// </summary>
        public void Open(){ 
            this.visible = true;
        }

        /// <summary>
        /// Метод закрытия панели
        /// </summary>
        public void Close() {
            this.visible = false;
        }

    }
}