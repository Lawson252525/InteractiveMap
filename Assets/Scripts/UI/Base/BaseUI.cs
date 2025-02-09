using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.UI {
    /// <summary>
    /// Базовый класс для всех UI элементов
    /// </summary>
    public abstract class BaseUI : UIBehaviour {
        /// <summary>
        /// Холст родительский
        /// </summary>
        public Canvas canvas {get; private set;}

        protected override void Awake() {
            base.Awake();

            this.canvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// Компонент RectTransform элемента
        /// </summary>
        public new RectTransform transform {
            get {return base.transform as RectTransform;}
        }

    }
}