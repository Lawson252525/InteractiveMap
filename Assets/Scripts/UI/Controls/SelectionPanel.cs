using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент управления выделением элементов на сцене
    /// </summary>
    public sealed class SelectionPanel : BlockPanel {
        /// <summary>
        /// Синглтон панели выделения
        /// </summary>
        public static SelectionPanel Instance {get; private set;}
        
        /// <summary>
        /// Поле контейнера данных элемента
        /// </summary>
        public Image contentContainer;

        /// <summary>
        /// Поле текущей панели отображения
        /// </summary>
        private ISelectionView objCurrent;
        /// <summary>
        /// Поле панели интерфейса
        /// </summary>
        private ViewPanel view;
        /// <summary>
        /// Поле выделения элементов
        /// </summary>
        private Physics2DRaycaster mapRaycaster;
        /// <summary>
        /// Поле выделения элементов интерфейса
        /// </summary>
        private GraphicRaycaster uiRaycaster;

        /// <summary>
        /// Свойство возвращает текущую панель управления
        /// </summary>
        public ISelectionView current {
            get {return this.objCurrent;}
        }

        protected override void Awake() {
            Instance = this;

            base.Awake();

            //Получаем ссылку на компонент выделения элементов
            this.mapRaycaster = Camera.main.GetComponent<Physics2DRaycaster>();
            this.uiRaycaster = Camera.main.GetComponentInChildren<GraphicRaycaster>();
        }

        private void Update() {
            //Операция выделения элнеметов на сцене
            if (Input.GetMouseButtonUp(0)) {
                var mousePosition = Input.mousePosition;
                var pointer = new PointerEventData(EventSystem.current);
                pointer.position = mousePosition;
                var results = new List<RaycastResult>();

                //Сначала пытаемся выделить элементы интрефейса
                var result = false;
                this.uiRaycaster.Raycast(pointer, results);
                if (results.Count > 0) {
                    //Попался элемент интерфейса
                    var first = results.Select(e => e.gameObject.GetComponent<ISelectionView>()).FirstOrDefault();
                    if (first != null) result = Select(first);
                } else {
                    //Пробуем выделить элемент сцены
                    results = new List<RaycastResult>();
                    this.mapRaycaster.Raycast(pointer, results);
                    if (results.Count > 0) {
                        //Попался элемент сцены
                        var first = results.Select(e => e.gameObject.GetComponent<ISelectionView>()).FirstOrDefault();
                        if (first != null) result = Select(first);
                    }
                }
            }
        }

        /// <summary>
        /// Метод выделения элемента
        /// </summary>
        /// <param name="selection">Интерфейс элемента</param>
        /// <returns>Результат выделения</returns>
        public bool Select(ISelectionView selection) {
            if (selection != this.current) {
                this.objCurrent = selection;

                //Удаляем прерыдущую панель
                if (this.view) Destroy(this.view.gameObject);
                this.view = null;

                if (this.current != null) {
                    var parent = this.contentContainer.transform as RectTransform;
                    this.view = this.current.CreateViewPanel(parent);
                    if (this.view) {
                        //Инициализируем новую панель
                        this.view.Initialize();

                        //Перестраиваем форму оторажения панели
                        if (this.view.transform.parent != parent) this.view.transform.SetParent(parent);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(parent);

                        //Открываем панель
                        Open();

                        return true;
                    }
                }

                //Закрывам окно если панели для отображения нет
                if (this.view is null) Close();
            }

            return false;
        }

        protected override void OnChanged(bool flag) {
            base.OnChanged(flag);

            //При закрытие панели удалить панели
            if (flag == false) Select(null);
        }

    }

}