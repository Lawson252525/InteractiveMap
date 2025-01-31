using System;

namespace InteractiveMap.Models {
    /// <summary>
    /// Базовый класс для любого события
    /// </summary>
    public abstract class BaseEvent : IEventBase {
        /// <summary>
        /// Уникальный ключ события
        /// </summary>
        private readonly string objId;
        /// <summary>
        /// Дата создания события
        /// </summary>
        private readonly DateTime objDate;

        /// <summary>
        /// Владелец события
        /// </summary>
        private User objOwner;
        /// <summary>
        /// Поле завершения события
        /// </summary>
        private bool objIsComplete = false;
        /// <summary>
        /// Поле изменения события
        /// </summary>
        private bool objIsChanged = false;

        public BaseEvent(string id, DateTime creationTime) {
            this.objId = id;
            this.objDate = creationTime;
        }

        public string id => this.objId;

        public DateTime creationTime => this.objDate;

        public User owner => this.objOwner;

        public bool isComplete => this.objIsComplete;

        public bool isChanged => this.objIsChanged;

        public void Complete() {
            this.objIsComplete = true;
        }

        public abstract IEventContainer GetContainer();

        public abstract void ApplyContainer(IEventContainer container);

        public virtual void Reset() {
            this.objIsChanged = false;
        }
        /// <summary>
        /// Метод помечает событие как изменившееся
        /// </summary>
        protected void SetDirty() {
            this.objIsChanged = true;
        }

        /// <summary>
        /// Метод устанавливает новго владельца события и возвращает true если владалец сменился
        /// </summary>
        /// <param name="newOwner">Новый владелец события</param>
        /// <returns>Возвращает true или false</returns>
        public bool SetOwner(User newOwner) {
            if (this.isComplete || newOwner == this.owner) return false;

             this.objOwner = newOwner;
            return true;
        }
    }
}