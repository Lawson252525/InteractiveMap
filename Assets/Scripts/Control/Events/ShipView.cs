using InteractiveMap.Models;
using InteractiveMap.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент для отображения данных события Корабль
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class ShipView : MoveEventView<Ship>, ISelectionView {
        /// <summary>
        /// Максимальная скорость перемещения Корабля
        /// </summary>
        [Range(0.1f, 5f)]
        public float maxMoveSpeed = 5f;
        /// <summary>
        /// Минимальная скорость перемещения Корабля
        /// </summary>
        [Range(0.1f, 5f)]
        public float minMoveSpeed = 3f;
        /// <summary>
        /// Поле максимального размера команды
        /// </summary>
        [Range(1, 15)]
        public int maxCrewSize = 6;
        /// <summary>
        /// Поле минимального размера команды
        /// </summary>
        [Range(1, 15)]
        public int minCrewSize = 4;
        /// <summary>
        /// Поле элемента флага
        /// </summary>
        public Image iconElement;
        /// <summary>
        /// Поле для компонента управления событием Метка
        /// </summary>
        public ShipViewPanel viewPanel;

        /// <summary>
        /// Поле выделения корабля
        /// </summary>
        private SpriteRenderer selectionRend = null;
        /// <summary>
        /// Агент навигации
        /// </summary>
        private NavMeshAgent agent = null;
        private Vector2 lastDestination;

        private void Awake() {
            //Получаем кольцо выделения
            this.selectionRend = this.transform.Find("SelectionCircle").GetComponent<SpriteRenderer>();
            if (this.selectionRend) this.selectionRend.gameObject.SetActive(false);

            //Получаем компонент управления
            this.agent = GetComponent<NavMeshAgent>();
        }

        public override void OnUpdate() {
            if (this.element is null || this.element.isComplete) return;

            //Устанавливаем скорость движения агента
            this.agent.speed = this.element.speed;
            this.transform.rotation = Quaternion.identity;

            var changed = (this.element.destination - this.lastDestination).magnitude > 0.25f;
            if (changed) this.lastDestination = this.agent.destination = this.element.destination;

            this.element.position = this.transform.position;

            //Двигаем событие по карте в точку назначения
            // var normal = (this.element.destination - this.element.position).normalized;
            // var newPosition = this.element.position + normal * this.element.speed * Time.deltaTime;
            // if (this.borders.Contains(newPosition) && (newPosition - this.element.destination).magnitude > 0.1f) {
            //     this.element.position = newPosition;
            //     this.transform.position = newPosition;
            // }

            base.OnUpdate();
        }

        public ViewPanel CreateViewPanel(RectTransform parent = null) {
            //Создаем новую панель управления
            var view = Instantiate<ShipViewPanel>(this.viewPanel, Vector2.zero, Quaternion.identity, parent);
            view.element = this.element;
            return view;
        }

        public void OnPointerClick(PointerEventData eventData) {
            //Выделяем элемент
            if (this.selectionRend) this.selectionRend.gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {
            //Снимаем выделение
            if (this.selectionRend) this.selectionRend.gameObject.SetActive(false);
        }

    }
}