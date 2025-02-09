using InteractiveMap.Control;
using InteractiveMap.Models;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Базовый компонент для интерфейса обработчика события
    /// </summary>
    public abstract class WorkerUI : BaseUI {
        /// <summary>
        /// Поля заполнения имени и описания события
        /// </summary>
        public InputField nameField, descriptionField;

        /// <summary>
        /// Свойство возвращает обработчик события
        /// </summary>
        public abstract EventWorker worker {get;}

        /// <summary>
        /// Метод возвращает контейнер с настройками события
        /// </summary>
        /// <returns>Контейнер с данными</returns>
        public abstract IEventContainer GetContainer();

        /// <summary>
        /// Метод сбрасывает настройки редактора
        /// </summary>
        public new virtual void Reset() {}

    }
}