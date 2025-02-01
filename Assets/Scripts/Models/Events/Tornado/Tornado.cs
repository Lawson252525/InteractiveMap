using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для события Вихрь
    /// </summary>
    public sealed class Tornado : MoveEvent {
        /// <summary>
        /// Время завершения жизни события
        /// </summary>
        private DateTime objExpiresDate;

        /// <summary>
        /// Свойство возвращает время завершения жизни события
        /// </summary>
        public DateTime expiresDate => this.objExpiresDate;

        public Tornado(string id, DateTime creationTime) : base(id, creationTime) {}

        public Tornado(string id, DateTime creationTime, TornadoContainer container) :base(id, creationTime) {
            this.speed = container.speed;
            this.position = container.position;
            this.destination = container.destination;
            this.sectionIndex = container.sectionIndex;
            this.objExpiresDate = container.expires;

            //Сбрасываем изменения события
            Reset();
        }

        public override void ApplyContainer(IEventContainer container) {
            if (container is TornadoContainer) {
                var con = (TornadoContainer)container;
                this.speed = con.speed;
                this.destination = con.destination;

                //При обновлении данных события из контейнера не нужно устанавливать позицию и индекс секции
                //this.sectionIndex = con.sectionIndex;
                //this.position = con.position;
                //Достаточно передавать точку назначения и скорость с временем жизни

                this.objExpiresDate = con.expires;

                //Сбрасываем изменения события
                Reset();
            }
        }

        public override IEventContainer GetContainer() {
            TornadoContainer container = new TornadoContainer();
            container.speed = this.speed;
            container.destination = this.destination;
            container.sectionIndex = this.sectionIndex;
            container.position = this.position;
            container.expires = this.expiresDate;

            //Сбрасываем изменения события
            Reset();

            return container;
        }
    }
}