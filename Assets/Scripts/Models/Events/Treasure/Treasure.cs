using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для события сокровище
    /// </summary>
    public sealed class Treasure : PointEvent {
        /// <summary>
        /// Время истченеия срока события
        /// </summary>
        public DateTime objExpiresTime;
        /// <summary>
        /// Значение
        /// </summary>
        public int amount = 100;
        /// <summary>
        /// Поле имения
        /// </summary>
        public string name;
        /// <summary>
        /// Поле описания
        /// </summary>
        public string description;

        public DateTime expires => this.objExpiresTime;

        public Treasure(string id, string owner, DateTime creationTime) : base(id, owner, creationTime) {}

        public Treasure(string id, string owner, DateTime creationTime, string containerData) : this(id, owner, creationTime, JsonUtility.FromJson<TreasureContainer>(containerData)) {}

        public Treasure(string id, string owner, DateTime creationTime, TreasureContainer container) : base(id, owner, creationTime) {
            this.position = container.position;
            this.amount = container.amount;
            this.objExpiresTime = container.expiresDate;

            //Сбрасываем изменения события
            Reset();
        }

        public override void ApplyContainer(IEventContainer container) {
            if (container is TreasureContainer) {
                var con = (TreasureContainer)container;
                this.amount = con.amount;
                this.objExpiresTime = con.expiresDate;

                this.name = con.name;
                this.description = con.description;

                //Сбрасываем изменения события
                Reset();
            }
        }

        public override IEventContainer GetContainer() {
            var result = new TreasureContainer();

            result.position = this.position;
            result.amount = this.amount;

            result.name = this.name;
            result.description = this.description;

            //Сбрасываем изменения события
            Reset();

            return result;
        }
    }
}