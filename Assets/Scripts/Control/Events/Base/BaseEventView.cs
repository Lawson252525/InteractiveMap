using InteractiveMap.Models;
using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Базовы компонент для отображения события на карте
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseEventView : MonoBehaviour {
        /// <summary>
        /// Поле данных события
        /// </summary>
        private IEventElement objElement;

        protected virtual void Start() {
            //Инициализация элемента
        }

        /// <summary>
        /// Метод обновления события
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Свойство устанавливает данные события
        /// </summary>
        public IEventElement element {
            get {return this.objElement;}
            set {this.objElement = value;}
        }

    }
}