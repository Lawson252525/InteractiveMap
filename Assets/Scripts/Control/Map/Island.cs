using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент управления островами
    /// Остров наследует НО не реализует интерфейсы обработки кликов мыши.
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(PolygonCollider2D))]
    public sealed class Island : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {

        /// <summary>
        /// Компонент полигонов острова
        /// Чтобы определить его границы
        /// </summary>
        private new PolygonCollider2D collider = null;

        private void Awake() {
            //Получаем компонент коллайдера
            this.collider = GetComponent<PolygonCollider2D>();
        }

        /// <summary>
        /// Метод возвращает массив всех точек границ острова
        /// </summary>
        public Vector2[] GetPoints() {
            return this.collider.GetPath(0);
        }

        /// <summary>
        /// Проверка что точка находится в границах острова
        /// </summary>
        public bool Contains(Vector2 point) {
            var points = GetPoints();
            var j = points.Length - 1;
            var isInside = false;
            
            for (int i = 0; i < points.Length; j = i++) {
                var pi = points[i];
                var pj = points[j];
                if (((pi.y <= point.y && point.y < pj.y) || (pj.y <= point.y && point.y < pi.y)) &&
                    (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x))
                    isInside = !isInside;
                }
            return isInside;
        }

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {}
    }

}