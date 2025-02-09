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

        public Tornado(string id, string owner, DateTime creationTime) : base(id, owner, creationTime) {}

        public Tornado(string id, string owner, DateTime creationTime, string containerData) :this(id, owner, creationTime, JsonUtility.FromJson<TornadoContainer>(containerData)) {}

        public Tornado(string id, string owner, DateTime creationTime, TornadoContainer container) :base(id, owner, creationTime) {
            this.speed = (float)container.speed;
            this.position = container.position;
            this.destination = container.destination;
            this.sectionIndex = container.sectionIndex;
            this.objExpiresDate = container.expiresDate;

            //Сбрасываем изменения события
            Reset();
        }

        public override bool SetOwner(string newOwner) {
            return base.SetOwner(newOwner);
        }

        public override void ApplyContainer(IEventContainer container) {
            if (container is TornadoContainer) {
                var con = (TornadoContainer)container;
                this.speed = (float)con.speed;
                this.destination = con.destination;

                //При обновлении данных события из контейнера не нужно устанавливать позицию и индекс секции
                //this.sectionIndex = con.sectionIndex;
                //this.position = con.position;
                //Достаточно передавать точку назначения и скорость с временем жизни

                this.objExpiresDate = con.expiresDate;

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
            container.expiresDate = this.expiresDate;

            //Сбрасываем изменения события
            Reset();

            return container;
        }
    }
}