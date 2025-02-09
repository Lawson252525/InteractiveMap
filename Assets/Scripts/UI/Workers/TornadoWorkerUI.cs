using InteractiveMap.Control;
using InteractiveMap.Models;
using InteractiveMap.UI;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.IU {
    /// <summary>
    /// Компонент отображения интерфейса создания события Вихрь
    /// </summary>
    public sealed class TornadoWorkerUI : WorkerTypeUI<TornadoWorker> {
        /// <summary>
        /// Поле префаба с настройками события Сокровище
        /// </summary>
        public TornadoView view;

        /// <summary>
        /// Поля задания значения и времени существования
        /// </summary>
        public Slider speedSlider, lifeTimeSlider;
        /// <summary>
        /// Текстовые поля значения и времени сущестования
        /// </summary>
        public Text speedLabel, lifeTimeLabel;

        private TornadoContainer settings;

        protected override void Start() {
            base.Start();

            Reset();
        }

        public override IEventContainer GetContainer() {
            //Формируем контейнер с данными события
            return new TornadoContainer() {
                description = this.descriptionField.text,
                speed = this.speedSlider.value,
                position = this.settings.position,
                destination = this.settings.destination,
                expiresDate = System.DateTime.Now.AddSeconds(this.lifeTimeSlider.value),
                sectionIndex = this.settings.sectionIndex,
            };
        }

        public override void Reset() {
            base.Reset();

            //Генерируем случайные настройки события
            this.settings = (TornadoContainer)this.worker.GenerateSettings();

            //Задаем значения скорости
            this.speedSlider.minValue = this.view.minMoveSpeed;
            this.speedSlider.maxValue = this.view.maxMoveSpeed;
            this.speedSlider.value = (float)this.settings.speed;

            //Задаем настройки времени существования
            this.lifeTimeSlider.minValue = this.view.minTimeDelay;
            this.lifeTimeSlider.maxValue = this.view.maxTimeDelay;
            this.lifeTimeSlider.value = Random.Range(this.view.minTimeDelay, this.view.maxTimeDelay);

            this.nameField.text = this.descriptionField.text = string.Empty;
        }

        /// <summary>
        /// Метод обработки изменения времени суещствования
        /// </summary>
        public void OnLifeTimeChanged() {
            var seconds = (int)this.lifeTimeSlider.value;
            this.lifeTimeLabel.text = $"{seconds} сек.";
        }

        /// <summary>
        /// Метод обработки изменений скорости
        /// </summary>
        public void OnSpeedChanged() {
            var speed = this.speedSlider.value;
            this.speedLabel.text = $"{System.Math.Round(speed, 2)} узлов";
        }

    }
}