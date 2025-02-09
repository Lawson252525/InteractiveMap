using InteractiveMap.Control;
using InteractiveMap.Models;
using UnityEngine;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент для обработки процесса создания события
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WorkerCreator : BaseUI {
        /// <summary>
        /// Массив обработчиков
        /// </summary>
        private WorkerUI[] workers;
        /// <summary>
        /// Поле страницы обработчика
        /// </summary>
        private PageElement myPage;

        /// <summary>
        /// Текущий обработчик
        /// </summary>
        public WorkerUI current {get; private set;}

        protected override void Awake() {
            base.Awake();

            //Получаем массив всех обработчиков событий
            this.workers = GetComponentsInChildren<WorkerUI>();

            //Получаем ссылку на страницу обработчика
            this.myPage = GetComponentInParent<PageElement>();
        }

        protected override void Start() {
            base.Start();
    
            //Устанавливаем первый обработчик как текущий
            this.current = this.workers[0];
        }

        /// <summary>
        /// Метод создает текущий обработчик события
        /// </summary>
        public void Create() {
            if (this.current) {
                //Передаем событие и его обработчик в контроллер событий
                //Ожидаем щавершения добавления события
                //Перемещаем камеру в точку создания события если можем
                var container = this.current.GetContainer();
                var worker = this.current.worker;

                EventControl.Instance.CreateEvent(container, worker, OnEventCreated);

                //Блокируем окно от закрытия
                this.myPage.isLocked = true;
                this.myPage.interactable = false;
            }
        }

        /// <summary>
        /// Метод меняет обработчик нового события по указанному индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        public void ChangeWorker(int index) {
            if (index >= 0 && index < this.workers.Length) ChangeWorker(this.workers[index]);
        }

        /// <summary>
        /// Метод смены окна создания события
        /// </summary>
        /// <param name="newWorker">Новое окно</param>
        public void ChangeWorker(WorkerUI newWorker) {
            if (this.current != newWorker) {
                //Переключаем старый обработчик на новый
                this.current.gameObject.SetActive(false);
                this.current = newWorker;
                this.current.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Метод обработки завершения добавления события в базу данных
        /// </summary>
        /// <param name="worker">Обработчик события</param>
        private void OnEventCreated(EventWorker worker) {
            //Разблокировать окно если событие было добавлено
            this.myPage.isLocked = false;
            this.myPage.interactable = true;

            //Перемещаем камеру в место создания события
            var element = worker.element;
            var pointEvent = element as PointEvent;
            if (pointEvent) {
                var position = pointEvent.position;
                CameraControl.Instance.MoveTo(position);

                //Закрываем панель администратора
                var adminPanel = GetComponentInParent<BlockPanel>();
                if (adminPanel) adminPanel.Close();
            }   

            //Сбрасываем текущий редактор события
            this.current.Reset();
        }

        /// <summary>
        /// Метод генерирует настройки текущего обработчика события
        /// </summary>
        public void Generate() {
            if (this.current) this.current.Reset();
        }

    }
}