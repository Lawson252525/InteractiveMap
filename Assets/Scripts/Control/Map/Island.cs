using System.Linq;
using InteractiveMap.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент управления островами
    /// Остров наследует НО не реализует интерфейсы обработки кликов мыши.
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(PolygonCollider2D))]
    public sealed class Island : MonoBehaviour, ISelectionView {
        /// <summary>
        /// Поле текстового отображения названия
        /// </summary>
        public TextViewResize textLabel;
        /// <summary>
        /// Поле описания острова
        /// </summary>
        public string description = "Описание";
        public IslandViewPanel viewPanel;
        /// <summary>
        /// Компонент полигонов острова
        /// Чтобы определить его границы
        /// </summary>
        private new PolygonCollider2D collider = null;
        /// <summary>
        /// Границы острова в прямоугольнике
        /// </summary>
        public Rect border {get; private set;}
        /// <summary>
        /// Массив точек острова
        /// </summary>
        public Vector2[] points {get; private set;}

        private void Awake() {
            //Получаем компонент коллайдера
            this.collider = GetComponent<PolygonCollider2D>();
            this.points = this.collider.GetPath(0);

            //Формируем прямоугольную фигуру сотрова
            float xMin = 0f;
            float yMin = 0f;
            float xMax = xMin;
            float yMax = yMin;
            foreach(var point in this.points) {
                var x = point.x;
                var y = point.y;

                if (x <= xMin) xMin = x;
                if (y <= yMin) yMin = y;
                if (x >= xMax) xMax = x;
                if (y >= yMax) yMax = y;
            }
            var min = (Vector2)this.transform.position + new Vector2(xMin, yMin);
            var max = (Vector2)this.transform.position + new Vector2(xMax, yMax);
            this.border = Rect.MinMaxRect(min.x, min.y, max.x, max.y); 

            //Формируем глобальный точки границы
            this.points = this.points.Select(p => (Vector2)this.transform.position + p).ToArray();
        }

        /// <summary>
        /// Проверка что точка находится в границах острова
        /// </summary>
        public bool Contains(Vector2 point) {
            bool result = false;
            int j = this.points.Length - 1;

            for (int i = 0; i < this.points.Length; i++) {
                if (this.points[i].y < point.y && this.points[j].y >= point.y || 
                    this.points[j].y < point.y && this.points[i].y >= point.y) {
                    if (this.points[i].x + (point.y - this.points[i].y) /
                        (this.points[j].y - this.points[i].y) *
                        (this.points[j].x - this.points[i].x) < point.x) result = !result;
                }
                j = i;
            }

            return result;
        }

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {}

        public ViewPanel CreateViewPanel(RectTransform parent = null) {
            //Устанавливаем данные панели
            var view = Instantiate<IslandViewPanel>(this.viewPanel, Vector2.zero, Quaternion.identity, parent);
            view.island = this;
            return view;
        }


#region Отрисовка прямоугольных границ острова
  #if UNITY_EDITOR
  private void OnDrawGizmos() {
   if (Application.isPlaying == false) return;

   Color c = Gizmos.color;

   //Отрисовка линий 
   Gizmos.color = Color.green;
   var max = this.border.max;
   var min = this.border.min;
   var p1 = min;
   var p2 = new Vector2(min.x, max.y);
   var p3 = max;
   var p4 = new Vector2(max.x, min.y);
   Gizmos.DrawLine(p1, p2);
   Gizmos.DrawLine(p2, p3);
   Gizmos.DrawLine(p3, p4);
   Gizmos.DrawLine(p4, p1);

   Gizmos.color = c;
  }
#endif
#endregion

    }

}