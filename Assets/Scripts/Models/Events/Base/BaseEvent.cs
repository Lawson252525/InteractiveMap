﻿using System;

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
        private string objOwner;
        /// <summary>
        /// Поле завершения события
        /// </summary>
        private bool objIsComplete = false;
        /// <summary>
        /// Поле изменения события
        /// </summary>
        private bool objIsChanged = false;

        public BaseEvent(string id, string owner, DateTime creationTime) {
            this.objId = id;
            this.objDate = creationTime;
            this.objOwner = owner;
        }

        public BaseEvent(string id, string owner, DateTime creationTime, IEventContainer container) : this(id, owner, creationTime) {
            ApplyContainer(container);
        }

        public string id => this.objId;

        public DateTime creationTime => this.objDate;

        public string owner => this.objOwner;

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
        public void SetDirty() {
            this.objIsChanged = true;
        }

        /// <summary>
        /// Перегрузка метода получения хэш кода события в виде его уникального ключа
        /// </summary>
        /// <returns>Хэш код</returns>
        public override int GetHashCode() {
            return this.id.GetHashCode();
        }

        /// <summary>
        /// Метод устанавливает новго владельца события и возвращает true если владалец сменился
        /// </summary>
        /// <param name="newOwner">Новый владелец события</param>
        /// <returns>Возвращает true или false</returns>
        public virtual bool SetOwner(string newOwner) {
            if (this.isComplete) return false;

             this.objOwner = newOwner;
             
            return true;
        }

        public static implicit operator bool(BaseEvent element) {
            return Equals(element, null) == false;
        }

    }
}