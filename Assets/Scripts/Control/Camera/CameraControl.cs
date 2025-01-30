using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент для управления камерой на карте
    /// Камерой можно управлять правой кнопкой мыши.
    /// Чтобы двигать камеру необходимо зажать правую кнопку мыши и перемещать ее.
    /// Для зума используется колесико мышки
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Camera), typeof(Physics2DRaycaster))]
    public sealed class CameraControl : MonoBehaviour {
        /// <summary>
        /// Минимальное значение зума камеры
        /// </summary>
        public const float MIN_ZOOM = 3f;
        /// <summary>
        /// Максимальное значения зума камеры
        /// </summary>
        public const float MAX_ZOOM = 8f;
        /// <summary>
        /// Минимальная скорость движения камеры
        /// </summary>
        private const float MIN_MOVE_DELTA = .1f;

        /// <summary>
        /// Синглтон компонента камеры, 
        /// так как камера на сцене одна можно к ней обращаться на прямую 
        /// </summary>
        public static CameraControl Instance {get; private set;}
        /// <summary>
        /// Сглаживаем движения камеры
        /// </summary>
        [Range(1f, 10f)]
        public float smoothFollow = 5f;
        /// <summary>
        /// Чувствительность зума
        /// </summary>
        [Range(0.1f, 1f)]
        public float zoomSensetive = 0.25f;

        //Компонент камеры
        private new Camera camera = null;
        /// <summary>
        /// Целевая точка движения камеры
        /// </summary>
        private Vector3 targetPoint;
        /// <summary>
        /// Границы карты и видимости камеры
        /// </summary>
        private Rect borderRect, viewRect;
        /// <summary>
        /// Позиция камеры
        /// </summary>
        private Vector3 position;
        /// <summary>
        /// Последняя позиция камеры
        /// </summary>
        private Vector3 lastPoint;
        /// <summary>
        /// Текущий зум камеры
        /// </summary>
        private float currentZoom;
        /// <summary>
        /// Целевой зум камеры
        /// </summary>
        private float targetZoom;
        /// <summary>
        /// Скорость изменения зума
        /// </summary>
        private float zoomSensetiveDelta;

        private void Awake() {
            //Устанвливаем ссылку на синглтон
            Instance = this;

            //Получаем на старте компонент камеры
            this.camera = GetComponent<Camera>();

            //Устанавливм стартовую позицию камеры на карте
            this.targetPoint = this.position = this.lastPoint = this.transform.position;

            //Устанавливаем стартовый зум на камере
            this.currentZoom = this.targetZoom = this.camera.orthographicSize;
        }

        private void Start() {
            //Получаем границы карты
            this.borderRect = Map.Borders;
        }

        private void Update() {
            //Получаем значение события прокрутки колесика мыши
            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0f || scroll < 0f) OnZooming(scroll);

            //Получаем позицию мыши на карте
            Vector2 mousePosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(1)) OnScreenPointDown(mousePosition);
            else if (Input.GetMouseButton(1)) OnScreenPointMove(mousePosition);
            else if (Input.GetMouseButtonUp(1)) OnScreenPointUp(mousePosition);

            //Обновляем каждый кадр зум камеры
            UpdateZoom();
            //Обновляем каждый кадр позицию камеры на карте
            UpdateCameraPosition();
        }

        /// <summary>
        /// Метод перемещает камеру в указанную точку
        /// </summary>
        public void MoveTo(Vector2 point) {
            this.targetPoint = new Vector3(point.x, point.y, this.targetPoint.z);
        }

        /// <summary>
        /// Метод обновляет зум камеры
        /// </summary>
        private void UpdateZoom() {
            this.zoomSensetiveDelta = Mathf.Lerp(this.zoomSensetiveDelta, 0f, Time.deltaTime * 10f);

            this.currentZoom = Mathf.Lerp(this.currentZoom, this.targetZoom, this.smoothFollow * Time.deltaTime);
            this.camera.orthographicSize = this.currentZoom;
        }

        /// <summary>
        /// Метод обновляет позицию камеры на карте
        /// </summary>
        private void UpdateCameraPosition() {
            this.position = Vector3.Lerp(this.position, this.targetPoint, this.smoothFollow * Time.deltaTime);

            this.viewRect = RecalculateViewRect();

            float xMin = this.borderRect.xMin + this.viewRect.width / 2f;
            float yMin = this.borderRect.yMin + this.viewRect.height / 2f;
            Vector2 min = new Vector2(xMin, yMin);

            float xMax = this.borderRect.xMax - this.viewRect.width / 2f;
            float yMax = this.borderRect.yMax - this.viewRect.height / 2f;
            Vector2 max = new Vector2(xMax, yMax);

            Rect moveRect = new Rect();
            moveRect.min = min;
            moveRect.max = max;

            this.position.x = Mathf.Clamp(this.position.x, moveRect.xMin, moveRect.xMax);
            this.position.y = Mathf.Clamp(this.position.y, moveRect.yMin, moveRect.yMax);

            this.transform.position = this.position;
        }

        /// <summary>
        /// Метод возвращает обновленные размеры границы карты и видимость камеры
        /// </summary>
        public Rect RecalculateViewRect()   {
            Rect result = new Rect();

            float minHeight = this.currentZoom * 2f;
            float minWidth = minHeight * (float)this.camera.pixelWidth / (float)this.camera.pixelHeight;

            result.xMin = this.transform.position.x - minWidth / 2f;
            result.xMax = this.transform.position.x + minWidth / 2f;
            result.yMin = this.transform.position.y - minHeight / 2f;
            result.yMax = this.transform.position.y + minHeight / 2f;

            return result;
        }

        private void OnScreenPointDown(Vector2 mousePosition)   {
            Vector3 newPoint = ToWorldPoint(mousePosition);
            this.lastPoint = newPoint;
            this.targetPoint = this.transform.position;
        }

        private void OnScreenPointMove(Vector2 mousePosition)   {
            Vector3 newPoint = ToWorldPoint(mousePosition);
            float sqrDist = (newPoint - this.lastPoint).sqrMagnitude;
            if (sqrDist > MIN_MOVE_DELTA)
            {
                Vector3 direction = -(newPoint - this.lastPoint).normalized;
                float dist = Mathf.Sqrt(sqrDist);
                this.targetPoint += (direction * dist);
                this.lastPoint = newPoint;
            }
        }

        private void OnScreenPointUp(Vector2 mousePosition) {
            Vector3 newPoint = ToWorldPoint(mousePosition);
            this.lastPoint = newPoint;
        }

        /// <summary>
        /// Метод возвращает мировую позицию точку на экране
        /// </summary>
        private Vector3 ToWorldPoint(Vector2 mousePosition) {
            Vector3 pos = this.camera.ScreenToWorldPoint(mousePosition);
            return new Vector3(pos.x, pos.y, this.transform.position.z);
        }

        /// <summary>
        /// Метод возвращает максимальный возможный зум камеры
        /// </summary>
        /// <param name="borders"></param>
        /// <param name="cameraRect"></param>
        /// <returns></returns>
        public float GetMaxZoomValue(Rect borders, Rect cameraRect) {
            return borders.width * cameraRect.height / cameraRect.width * 0.5f;
        }

        private void OnZooming(float value) {
            float maxZoom = MAX_ZOOM;
            float minZoom = MIN_ZOOM;
            this.zoomSensetiveDelta += this.zoomSensetive;
            float delta = (this.zoomSensetiveDelta + this.zoomSensetive) * -value;
            this.targetZoom = Mathf.Clamp(this.targetZoom + delta, minZoom, maxZoom);
        }

        /// <summary>
        /// Метод возвращает еткущее значение зума камеры
        /// </summary>
        public float GetZoom() {
            return this.currentZoom;
        }

    }

}