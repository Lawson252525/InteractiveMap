using System;

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

        public override void ApplyContainer(IEventContainer container) {
            if (container is TornadoContainer) {
                var con = (TornadoContainer)container;
                this.speed = con.speed;
                this.destination = con.destination;
                this.sectionIndex = con.sectionIndex;
                this.position = con.position;
                this.objExpiresDate = con.expires;
            }
        }

        public override IEventContainer GetContainer() {
            TornadoContainer container = new TornadoContainer();
            container.speed = this.speed;
            container.destination = this.destination;
            container.sectionIndex = this.sectionIndex;
            container.position = this.position;
            container.expires = this.expiresDate;

            return container;
        }
    }
}