using InteractiveMap.Control;
using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент отображения интерфейса создания события Метка
    /// </summary>
    public sealed class FlagWorkerUI : WorkerTypeUI<FlagWorker> {
        /// <summary>
        /// Поле префаба с настройками события Сокровище
        /// </summary>
        public FlagView view;

        /// <summary>
        /// Поля задания времени существования
        /// </summary>
        public Slider lifeTimeSlider;
        /// <summary>
        /// Текстовые поля значения и времени сущестования
        /// </summary>
        public Text lifeTimeLabel;

        /// <summary>
        /// Поле стартовых настроек
        /// </summary>
        private FlagContainer settings;

        protected override void Start() {
            base.Start();

            Reset();
        }
        
        public override IEventContainer GetContainer() {
            //Получаем имя локального пользователя
            var localUser = Main.LocalUser;
            var ownerName = localUser ? localUser.id : "unknown";

            //Формируем контейнер с данными для добавляения события
            return new FlagContainer() {
                creatorId = ownerName,
                name = this.nameField.text,
                description = this.descriptionField.text,
                position = this.settings.position,
                expiresDate = System.DateTime.Now.AddSeconds(this.lifeTimeSlider.value)
            };
        }

        public override void Reset() {
            base.Reset();

            //Получаем стартовые случайные настройки события
            this.settings = (FlagContainer)this.worker.GenerateSettings();

            //Задаем настройки времени существования
            this.lifeTimeSlider.minValue = this.view.minTimeDelay;
            this.lifeTimeSlider.maxValue = this.view.maxTimeDelay;
            this.lifeTimeSlider.value = Random.Range(this.view.minTimeDelay, this.view.maxTimeDelay);

            this.nameField.text = "Метка";
            this.descriptionField.text = string.Empty;
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