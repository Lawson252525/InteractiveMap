using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InteractiveMap.Control;
using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент отображения управления СОбытием корабль
    /// </summary>
    public sealed class ShipWorkerUI : WorkerTypeUI<ShipWorker> {

        /// <summary>
        /// Презентер события корбль
        /// </summary>
        public ShipView view;

        public Slider speedSlider;
        public Text speedLabel;
        public Button findButton, saveButton;

        /// <summary>
        /// Родительское окно
        /// </summary>
        private BlockPanel parentPanel;

        protected override void Awake() {
            base.Awake();

            this.parentPanel = GetComponentInParent<BlockPanel>();
        }

        protected override void OnEnable() {
            //При включении ищем корабль игрока
            var localUser = Main.LocalUser;
            if (localUser) {
                this.view = GameObject.FindObjectsOfType<ShipView>().FirstOrDefault(s => s.element?.shipOwner == localUser.id);
                Reset();
            }
        }

        public override void Reset() {
            base.Reset();

            if (this.view) {
                //Устанавливем параметры отображения
                this.nameField.text = this.view.element.name;
                this.descriptionField.text = this.view.element.description;
                
                //Устанавливаем параметры скорости
                this.speedSlider.maxValue = this.view.maxMoveSpeed;
                this.speedSlider.minValue = this.view.minMoveSpeed;
                this.speedSlider.value = this.view.element.speed;
            } else {
                this.nameField.text = this.descriptionField.text = string.Empty;
            }

            //Включаем кнопки
            this.findButton.interactable = this.saveButton.interactable = this.view;
        }

        public override IEventContainer GetContainer() {
            return default;
        }

        /// <summary>
        /// Метод обработки изменений скорости
        /// </summary>
        public void OnSpeedChanged() {
            var speed = this.speedSlider.value;
            this.speedLabel.text = $"{System.Math.Round(speed, 2)} узлов";
        }

        /// <summary>
        /// Метод переносит игрока к кораблю
        /// </summary>
        public void Find() {
            if (this.view) CameraControl.Instance.MoveTo(this.view.element.position);

            this.parentPanel.Close();
        }

        /// <summary>
        /// Метод сохраняет изменения
        /// </summary>
        public void Save() {
            var localUser = Main.LocalUser;
            if (localUser && this.view && this.view.element.shipOwner == localUser.id) {
                //Сохраняем данные
                var newName = this.nameField.text;
                if (string.IsNullOrEmpty(newName) == false) this.view.element.name = newName;

                var newDescription = this.descriptionField.text;
                if (string.IsNullOrEmpty(newDescription) == false) this.view.element.description = newDescription;

                var newSpeed = this.speedSlider.value;
                if (newSpeed <= this.view.minMoveSpeed && newSpeed <= this.view.maxMoveSpeed) this.view.element.speed = newSpeed;

                this.view.element.SetDirty();

                if (this.view.element.owner != localUser.id) this.view.element.SetOwner(localUser.id);

                this.parentPanel.Close();
            }
        }

    }
}