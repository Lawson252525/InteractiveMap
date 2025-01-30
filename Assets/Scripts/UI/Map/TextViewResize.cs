using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InteractiveMap.Control;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент для обработки изменяемого текста
    /// Компонент меняет текст в заданном диапазона в зависимости от дальности камеры
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Text))]
    public sealed class TextViewResize : UIBehaviour {

        /// <summary>
        /// Максимальное значение зума под который будет меняться размер элемента
        /// </summary>
        [Range(1f, CameraControl.MAX_ZOOM)]
        public float maxZoom = CameraControl.MAX_ZOOM;
        /// <summary>
        /// Максимально возможный размер элемента 
        /// </summary>
        [Range(0.01f, 0.1f)]
        public float maxSize = 0.03f;

        /// <summary>
        /// Текущий размер элемента
        /// </summary>
        private Vector2 scale;
        /// <summary>
        /// Изначальный размер элемента
        /// </summary>
        private Vector2 originScale;
        /// <summary>
        /// Компонент формы элемента
        /// </summary>
        private new RectTransform transform = null;

        protected override void Awake() {
            base.Awake();

            //Получаем компонент формы элемента
            this.transform = base.transform as RectTransform;
            this.originScale = this.scale = this.transform.localScale;
        }

        private void Update() {
            //Получаем текущий размер зума камеры
            float currentZoom = CameraControl.Instance.GetZoom();
            if (currentZoom < this.maxZoom) {
                //Если текущий зум меньше максимального то продолжаем адаптировать размеры элемента
                float minZoom = CameraControl.MIN_ZOOM;
                float maxZoom = CameraControl.MAX_ZOOM;
                float percent = (currentZoom - minZoom) / (maxZoom - minZoom);

                Vector2 newScale = new Vector2(this.maxSize, this.maxSize) - this.originScale;
                this.scale = (newScale * percent) + this.originScale;
                this.transform.localScale = this.scale;
            }
        }

    }
}