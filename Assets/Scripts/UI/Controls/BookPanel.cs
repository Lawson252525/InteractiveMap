using System.Linq;
using UnityEngine;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент для обработки постраничного отображения
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BookPanel : BaseUI {
        /// <summary>
        /// Событие изменения страницы книги
        /// </summary>
        public System.Action<PageElement> OnPageChanged;

        /// <summary>
        /// Массив страничных элементов
        /// </summary>
        private PageElement[] pages;

        /// <summary>
        /// Поле текущего окна
        /// </summary>
        private PageElement objCurrent;

        protected override void Awake() {
            base.Awake();

            //Получаем список всех окон
            this.pages = GetComponentsInChildren<PageElement>();
        }

        protected override void Start() {
            base.Start();

            //Скрываем все страницы
            PageElement first = null;
            foreach(var page in this.pages) {
                page.Hide();

                if (first is null && page.activeOnStart) first = page;
            }
            if (first is null) first = this.pages[0];

            //Отображаем первое окно
            ChangePage(first);
        }

        /// <summary>
        /// Свойство возвращает текущее открытое окно
        /// </summary>
        public PageElement current {
            get {return this.objCurrent;}
            private set {this.objCurrent = value;}
        }

        /// <summary>
        /// Метод меняет страницу отображения
        /// </summary>
        /// <param name="index">Индекс страницы</param>
        public void ChangePage(int index) {
            if (index >= 0 && index < this.pages.Length) ChangePage(this.pages[index]);
        }

        /// <summary>
        /// Метод менят страницу отображения
        /// </summary>
        /// <param name="newPage">Страница</param>
        public void ChangePage(PageElement newPage) {
            //Установить новое окно
            if (newPage != this.objCurrent) {
                //Скрыть все остальные окна если сможет
                var canChange = true;
                foreach(var other in this.pages) {
                    //Пытаемся скрыть все остальные окна
                    if (newPage != other && other.isVisible) {
                        other.Hide();
                        canChange = other.isVisible == false;

                        if (canChange == false) break;
                    }
                }

                //Активируем выбранное окно
                if (canChange && newPage.Activate()) {
                    this.current = newPage;

                    //Отправляем событие изменения страницы
                    this.OnPageChanged?.Invoke(this.current);
                }
            }
        }

    }
}