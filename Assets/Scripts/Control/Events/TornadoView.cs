using System;
using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент для отображения данных события Вихрь
    /// </summary>
    public sealed class TornadoView : MoveEventView<Tornado>, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
        /// <summary>
        /// Максимальное время жизни Вихря в секундах
        /// </summary>
        [Range(1, 180)]
        public int maxTimeDelay = 100;
        /// <summary>
        /// Минимальное время жизни Вихря в секундах
        /// </summary>
        [Range(1, 180)]
        public int minTimeDelay = 50;
        /// <summary>
        /// Максимальная скорость перемещения вихря
        /// </summary>
        [Range(0.1f, 5f)]
        public float maxMoveSpeed = 5f;
        /// <summary>
        /// Минимальная скорость перемещения вихря
        /// </summary>
        [Range(0.1f, 5f)]
        public float minMoveSpeed = 3f;

        public override void Update() {
            if (this.element is null || this.element.isComplete) return;

            //Запоминаем предыдущую пози
            var oldPosition = this.element.position;

            //Двигаем событие по карте в точку назначения
            var normal = (this.element.destination - this.element.position).normalized;
            var newPosition = this.element.position + normal * this.element.speed * Time.deltaTime;
            if (this.borders.Contains(newPosition)) {
                this.element.position = newPosition;
                this.transform.position = this.element.position;
            }

            //Если позиция события изменилась то объявить событие изменившимся
            if ((oldPosition - this.element.position).sqrMagnitude > 0.1f) this.element.SetDirty();

            //Проверка времени жизни Вихря
            var nowTime = DateTime.Now;
            var complete = nowTime >= this.element.expiresDate;
            if (complete) this.element.Complete();

            //Вращение вихря
            var rotation = this.transform.rotation.eulerAngles;
            rotation.z += this.element.speed * Time.deltaTime * 100f;
            this.transform.rotation = Quaternion.Euler(rotation);

            base.Update();
        }

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {}

    }
}
