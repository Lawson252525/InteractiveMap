using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент навигации по меню
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class RouterElement : BaseUI, IPointerClickHandler {
        /// <summary>
        /// Поле страницы
        /// </summary>
        public PageElement page;
        /// <summary>
        /// Цвета отображения
        /// </summary>
        public Color enableColor, disableColor;

        /// <summary>
        /// Поле книжного элемента
        /// </summary>
        private BookPanel book;
        /// <summary>
        /// Поле отображения картинки
        /// </summary>
        private Image backImage;

        protected override void Awake() {
            base.Awake();

            this.backImage = GetComponent<Image>();

            //Получаем ссылку на элемент книги
            this.book = GetComponentInParent<BookPanel>();
        }

        protected override void OnEnable() {
            base.OnEnable();

            //Подключаем обработку события смены страницы
            if (this.book) this.book.OnPageChanged += OnPageChanged;
        }

        protected override void OnDisable() {
            base.OnDisable();

            //Отключаем обработку изменений
            if (this.book) this.book.OnPageChanged -= OnPageChanged;
        }

        private void OnPageChanged(PageElement newPage) {
            if (newPage == this.page) {
                if (this.backImage) this.backImage.color = this.enableColor;
            } else {
                if (this.backImage) this.backImage.color = this.disableColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (this.page && this.book) {
                if (this.page.isVisible == false && this.page.isLocked == false) this.book.ChangePage(this.page);
            }
        }
    }
}