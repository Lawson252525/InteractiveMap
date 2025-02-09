using InteractiveMap.Control;
using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент отображения интерфейса создания события Сокровище
    /// </summary>
    public sealed class TreasureWorkerUI : WorkerTypeUI<TreasureWorker> {
        /// <summary>
        /// Поле префаба с настройками события Сокровище
        /// </summary>
        public TreasureView view;

        /// <summary>
        /// Поля задания значения и времени существования
        /// </summary>
        public Slider amountSlider, lifeTimeSlider;
        /// <summary>
        /// Текстовые поля значения и времени сущестования
        /// </summary>
        public Text amountLabel, lifeTimeLabel;

        /// <summary>
        /// Поле стартовых настроек
        /// </summary>
        private TreasureContainer settings;

        protected override void Start() {
            base.Start();

            Reset();
        }

        public override IEventContainer GetContainer() {
            //Формируем контейнер с данными для добавляения события
            return new TreasureContainer() {
                name = this.nameField.text,
                description = this.descriptionField.text,
                amount = (int)this.amountSlider.value,
                position = this.settings.position,
                expiresDate = System.DateTime.Now.AddSeconds(this.lifeTimeSlider.value)
            };
        }

        public override void Reset() {
            base.Reset();

            //Получаем стартовые случайные настройки события
            this.settings = (TreasureContainer)this.worker.GenerateSettings();

            //Задаем настройки значения
            this.amountSlider.minValue = this.view.minAmount;
            this.amountSlider.maxValue = this.view.maxAmount;
            this.amountSlider.value = this.settings.amount;

            //Задаем настройки времени существования
            this.lifeTimeSlider.minValue = this.view.minTimeDelay;
            this.lifeTimeSlider.maxValue = this.view.maxTimeDelay;
            this.lifeTimeSlider.value = Random.Range(this.view.minTimeDelay, this.view.maxTimeDelay);

            this.nameField.text = this.descriptionField.text = string.Empty;
        }

        /// <summary>
        /// Метод добработки изменения значения
        /// </summary>
        public void OnAmountChanged() {
            this.amountLabel.text = $"{(int)this.amountSlider.value} ед.";
        }

        /// <summary>
        /// Метод обработки изменения времени суещствования
        /// </summary>
        public void OnLifeTimeChanged() {
            var seconds = (int)this.lifeTimeSlider.value;
            this.lifeTimeLabel.text = $"{seconds} сек.";
        }

    }
}