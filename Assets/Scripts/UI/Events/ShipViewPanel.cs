using System.Collections.Generic;
using System.Linq;
using InteractiveMap.Control;
using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент управления событие Корабль
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class ShipViewPanel : EventViewPanel<Ship> {
        /// <summary>
        /// Поле имени владельца, размера команды, размера груза, имена всех членов команды
        /// </summary>
        public Text ownerLabel, crewSizeLabel, cargoSizeLabel, crewNamesLabel, nameLabel;
        /// <summary>
        /// Поле кнопок покинуть и присоедениться
        /// </summary>
        public Button joinButton, leaveButton, moveButton;
        /// <summary>
        /// Поле указателя
        /// </summary>
        public SpriteRenderer pointImage;

        /// <summary>
        /// Поле блокиратора интерфейса
        /// </summary>
        private Image blocker = null;
        /// <summary>
        /// Поле поиска точки назначения
        /// </summary>
        private bool objIsSearchingLocation = false;
        /// <summary>
        /// Поле группы элементов
        /// </summary>
        private CanvasGroup canvasGroup = null;
        /// <summary>
        /// Поле выделения элементов интерфейса
        /// </summary>
        private GraphicRaycaster uiRaycaster;
        /// <summary>
        /// Тело указателя
        /// </summary>
        private Transform pointer;

        protected override void Awake() {
            base.Awake();

            this.canvasGroup = GetComponent<CanvasGroup>();

            //Получаем ссылку на компонент выделения элементов
            this.uiRaycaster = Camera.main.GetComponentInChildren<GraphicRaycaster>();
        }

        private void Update() {
            if (this.isSearchingLocation && this.blocker) {
                var mousePosition = Input.mousePosition;
                //Переноси точку мыши в току назначения
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);

                //Двигаем указатель
                if (this.pointer) this.pointer.transform.position = worldPos;
                
                //Поиск точки назначения
                if (Input.GetMouseButtonUp(0)) {
                    var pointer = new PointerEventData(EventSystem.current);
                    pointer.position = mousePosition;
                    var results = new List<RaycastResult>();
                    var canCast = true;

                    //Сначала получаем элементы интерфейса
                    this.uiRaycaster.Raycast(pointer, results);
                    //Если попали только в элемент блокиратора то можем 
                    canCast = (results.Count <= 0) || (results.Count == 1 && results[0].gameObject.name == this.blocker?.gameObject.name);

                    if (canCast) {
                        //Сбрасываем событие поиск точки назначения
                        ResetSearchLocation();
                        SetDestinationPoint(worldPos);
                    }   
                }
            }
        }

        /// <summary>
        /// Метод назначает точку назначения для корабля
        /// </summary>
        /// <param name="destination"></param>
        private void SetDestinationPoint(Vector2 destination) {
            var localUser = Main.LocalUser;
            //Менять точку назначения может только владелец судна
            if (localUser && this.element && this.element.shipOwner == localUser.id) {
                this.element.destination = destination;
                if (this.element.owner != localUser.id) this.element.SetOwner(localUser.id);
            }
        }

        /// <summary>
        /// Метод завершает поиск точки назначения
        /// </summary>
        private void ResetSearchLocation() {
            this.isSearchingLocation = false;
            this.canvasGroup.interactable = true;
            if (this.blocker) Destroy(this.blocker.gameObject);
            this.blocker = null;
            SelectionPanel.Instance.enabled = true;
            if (this.pointer) Destroy(this.pointer.gameObject);
            this.pointer = null;
        }

        public override void Initialize() {
            this.isSearchingLocation = false;

            var localUser = Main.LocalUser;
            if (localUser) {
                this.ownerLabel.text = "Unknown";

                //Устанавливаем настройки относительно локального пользователя
                this.nameLabel.text = this.element.name;
                this.descriptionLabel.text = this.element.description;
                this.crewSizeLabel.text = $"Размер команды {this.element.crewSize} чел.";

                var cargoSize = 0;

                //Записываем состав команды
                var memberIds = this.element.GetMembers();
                var users = Main.Instance.GetUsers();
                var members = users.Where(u => memberIds.Any(m => m.Equals(u.id)));
                var text = $"\n\nСписок всех членов команды";
                foreach(var member in members) {
                    text += $"\n - {member.name}";

                    //Записываем кол-во награды каждого члена команды
                    cargoSize += member.value;

                    //Устанавлиаем имя владельца
                    if (member.id == this.element.shipOwner) this.ownerLabel.text = $"{member.name}";
                }
                this.crewNamesLabel.text = text;

                //Устанавливаем размер груза
                this.cargoSizeLabel.text = $"Общий груз {cargoSize} зол.";

                //Настройки только для владельца и других игроков
                var isOwner = localUser.id.Equals(this.element.shipOwner);
                if (isOwner) {
                    this.moveButton.interactable = true;
                    this.leaveButton.gameObject.SetActive(false);
                    this.joinButton.gameObject.SetActive(false);
                } else {
                    //Если локальный игрок на борту, то отобразить ему кнопку покинуть команду
                    var onBoard = memberIds.Any(m => m.Equals(localUser.id));
                    this.joinButton.gameObject.SetActive(onBoard == false);
                    this.leaveButton.gameObject.SetActive(onBoard);
                }
            }
        }

        /// <summary>
        /// Метод присодеинения к команде локального пользователя
        /// </summary>
        public void JoinClick() {
            var localUser = Main.LocalUser;
            var result = this.element.AddMember(localUser.id);
            if (result) {
                //Устанавливаем вледльца СОБЫТИЯ-КОРАБЛЬ текущего пользователя чтобы он сохранил свои данные
                this.element.SetOwner(localUser.id);

                SelectionPanel.Instance.Select(null);
            }
        }

        /// <summary>
        /// Метод покидания команды локального пользователя
        /// </summary>
        public void LeaveClick() {
            var localUser = Main.LocalUser;
            //Удаляем из команды локального пользователя который НЕ является владельцем
            var result = this.element.RemoveMember(localUser.id);
            if (result) {
                //Устанавливаем вледльца СОБЫТИЯ-КОРАБЛЬ текущего пользователя чтобы он сохранил свои данные
                this.element.SetOwner(localUser.id);

                SelectionPanel.Instance.Select(null);
            }
        }

        /// <summary>
        /// Метод переносит камеру в точку нахождения корабля
        /// </summary>
        public void FindLocation() {
            CameraControl.Instance.MoveTo(this.element.position);
        }

        /// <summary>
        /// Метод устанавливает новую точку назначения корабля
        /// </summary>
        public void MoveToLocation() {
            if (this.blocker is null) {
                this.blocker = new GameObject("blocker").AddComponent<Image>();
                this.blocker.rectTransform.SetParent(this.canvas.transform);
                this.blocker.raycastTarget = true;
                this.blocker.rectTransform.anchorMin = Vector2.zero;
                this.blocker.rectTransform.anchorMax = Vector2.one;
                this.blocker.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                this.blocker.rectTransform.offsetMax = this.blocker.rectTransform.offsetMin = Vector2.zero;
                Color c = Color.black;
                c.a = 0.25f;
                this.blocker.color = c;
            }

            //Создаем указатель
            if (this.pointImage) {
                var pos = Vector2.zero;
                this.pointer = Instantiate<Transform>(this.pointImage.transform, pos, Quaternion.identity, Map.Instance.transform);
            }

            this.isSearchingLocation = true;
            this.canvasGroup.interactable = false;
            SelectionPanel.Instance.enabled = false;
        }

        /// <summary>
        /// Свойство возвращает поиск точки назначения
        /// </summary>
        public bool isSearchingLocation {
            get{ return this.objIsSearchingLocation;}
            private set {this.objIsSearchingLocation = value;}
        }

    }
}