using System.Linq;
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
        /// <summary>
        /// Полоска пути
        /// </summary>
        private LineRenderer line = null;

        private void Awake() {
            //Получаем кольцо выделения
            this.selectionRend = this.transform.Find("SelectionCircle").GetComponent<SpriteRenderer>();
            if (this.selectionRend) {
                this.selectionRend.gameObject.SetActive(false);

                //Получаем ссылку на полоску
                this.line = this.selectionRend.GetComponent<LineRenderer>();
            }

            //Получаем компонент управления
            this.agent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Последняя точка пути
        /// </summary>
        private Vector2 lastPathPosition;
        private Vector2[] path = null;

        public override void OnUpdate() {
            if (this.element is null || this.element.isComplete) return;

            //Устанавливаем скорость движения агента
            this.agent.speed = this.element.speed;
            this.transform.rotation = Quaternion.identity;

            var destinationChanged = (this.element.destination - this.lastDestination).magnitude > 0.25f;
            if (destinationChanged)  this.lastDestination = this.agent.destination = this.element.destination;

            this.element.position = this.transform.position;

            //Двигаем полоску
            if (this.selectionRend.gameObject.activeSelf && this.line && this.agent && this.agent.path.corners != null) {
                var changed = destinationChanged;
                if (this.path?.Length != this.agent.path.corners?.Length) changed = true;
                else if ((this.path[0] - (Vector2)this.agent.path.corners[0]).magnitude > 1f) changed = true;
                else if ((this.path.Last() - (Vector2)this.agent.path.corners.Last()).magnitude > 1f) changed = true;

                if (changed) this.path = this.agent.path.corners.Select(p => (Vector2)p).ToArray();

                //Обновляем отрисвоку после прохождения 1ед расстояния
                var lastDist = (this.lastPathPosition - path[0]).magnitude;
                if (lastDist > 1f || changed) {
                    this.lastPathPosition = this.selectionRend.transform.position;

                    //Сбрасываем предыдущие точки
                    this.line.positionCount = this.path.Length;
                    
                    Vector2 pos = this.selectionRend.transform.position;
                    this.line.SetPosition(0, new Vector3(pos.x, pos.y, -1f));

                    //Выполняем перерасчет элементов
                    float fullDist = 0f;

                    for(int i = 1; i < this.path.Length; i++) {
                        pos = this.path[i];
                        this.line.SetPosition(i, new Vector3(pos.x, pos.y, -1));

                        //Замеряем длину всего пути
                        var lastPos = (Vector2)this.line.GetPosition(i - 1);
                        fullDist += (lastPos - pos).magnitude;
                    }

                    int count = 3 * (int)Mathf.Clamp(fullDist, 1, fullDist);
                    this.line.material.SetFloat("_RepeatCount", count);
                }
            }

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