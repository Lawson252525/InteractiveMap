using UnityEngine;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент отображения страниц в книге элементов
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
    public sealed class PageElement : BaseUI {
        /// <summary>
        /// Поле активации элемента на старте
        /// </summary>
        public bool activeOnStart = true;

        /// <summary>
        /// Поле отображения элемента
        /// </summary>
        private CanvasGroup group;
        /// <summary>
        /// Поле блокировки окна
        /// </summary>
        private bool objIsLocked;

        protected override void Awake() {
            base.Awake();

            this.group = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Свойство возвращает блокировку поля
        /// </summary>
        public bool isLocked {
            get {return this.objIsLocked;}
            set {this.objIsLocked = value;}
        }

        public bool interactable {
            get {return this.group.interactable;}
            set {this.group.interactable = value;}
        }

        /// <summary>
        /// Свойсвто возвращает видимость страницы
        /// </summary>
        public bool isVisible {
            get {return this.interactable && this.group.blocksRaycasts;}
        }

        /// <summary>
        /// Метод активирует страницу с флагом
        /// </summary>
        /// <param name="flag">Показать\Скрыть</param>
        /// <returns>Результат операции</returns>
        public bool Activate(bool flag = true) {
            if (this.isLocked) return false;

            bool result = false;

            if (flag && this.isVisible == false) {
                //Показать страницу если возможно
                this.interactable = this.group.blocksRaycasts = true;
                this.group.alpha = 1f;

                result = true;
            } else if (flag == false && this.isVisible) {
                //Скрыть страницу
                this.interactable = this.group.blocksRaycasts = false;
                this.group.alpha = 0f;

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Метод показывает страницу
        /// </summary>
        public void Show() {
            Activate(true);
        }

        /// <summary>
        /// Метод скрывает страницу
        /// </summary>
        public void Hide() {
            Activate(false);
        }

    }
}