using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для события корабль
    /// </summary>
    public sealed class Ship : MoveEvent, IEnumerable<string> {
        /// <summary>
        /// Название и описание
        /// </summary>
        public string name, description;
        /// <summary>
        /// Поле списка пользователей команды
        /// </summary>
        private HashSet<string> objCrew = new HashSet<string>();
        /// <summary>
        /// Поле типа корабля
        /// </summary>
        private Types objType;
        /// <summary>
        /// Поле максимальное кол-во команды
        /// </summary>
        private int objCrewSize;
        /// <summary>
        /// Поле владельца корабля, никогда не меняется
        /// </summary>
        private string objShipOwnerId;

        public Ship(string id, string owner, DateTime creationTime) : base(id, owner, creationTime) {
            this.objShipOwnerId = owner;
        }

        public Ship(string id, string owner, DateTime creationTime, Types type) : this(id, owner, creationTime) {
            this.objType = type;
        }

        public Ship(string id, string owner, DateTime creationTime, string containerData) : this(id, owner, creationTime, JsonUtility.FromJson<ShipContainer>(containerData)) {}

        public Ship(string id, string owner, DateTime creationTime, ShipContainer container) : this(id, owner, creationTime) {
            this.position = container.position;
            this.destination = container.destination;
            this.name = container.name;
            this.description = container.description;
            this.speed = (float)container.speed;
            this.objCrew = new HashSet<string>(container.crew);
            this.objType = container.type;
            this.objCrewSize = container.crewSize > 0 ? container.crewSize : 1;
            this.objShipOwnerId = container.ownerId;

            //Сброс изменений данных
            Reset();
        }

        /// <summary>
        /// Свойство вовзвращает максимальное число команды
        /// </summary>
        public int crewSize {
            get {return this.objCrewSize;}
        }

        /// <summary>
        /// Свойство вовзвращает тип корабля
        /// </summary>
        public Types type {
            get {return (Types)this.objType;}
        }

        /// <summary>
        /// Метод добавляет члена команды
        /// </summary>
        /// <param name="userId">Ключ пользователя</param>
        /// <returns>Результат операции</returns>
        public bool AddMember(string userId) {
            if (this.objCrew.Count >= this.crewSize) return false;

            var result = this.objCrew.Add(userId);
            if (result) SetDirty();

            return result;
        }

        /// <summary>
        /// Метод возвращает результат если указанный пользователь есть в команде
        /// </summary>
        /// <param name="userId">Ключ пользователя</param>
        /// <returns>Результат операции</returns>
        public bool ContainsMember(string userId) {
            return this.objCrew.Contains(userId);
        }

        /// <summary>
        /// Метод удаляет члена команды
        /// </summary>
        /// <param name="userId">Ключ пользователя</param>
        /// <returns>Результат операции</returns>
        public bool RemoveMember(string userId) {
            //Нельзя удалить из команды владельца корабля
            if (this.owner == userId) return false;

            var result = this.objCrew.Remove(userId);
            if (result) SetDirty();

            return result;
        }

        /// <summary>
        /// Метод возвращает коллекцию членом команды
        /// </summary>
        /// <returns>Коллекция</returns>
        public IEnumerable<string> GetMembers() {
            return this.objCrew;
        }

        public override void ApplyContainer(IEventContainer container) {
            if (container is ShipContainer) {
                var con = (ShipContainer)container;
                this.speed = (float)con.speed;
                this.destination = con.destination;
                this.objCrew = new HashSet<string>(con.crew);

                //Сброс изменений данных
                Reset();
            }
        }

        /// <summary>
        /// Свойство возвращает владельца корабля
        /// </summary>
        public string shipOwner {
            get {return this.objShipOwnerId;}
        }

        public override IEventContainer GetContainer() {
            Reset();

            return new ShipContainer() {
                name = this.name,
                description = this.description,
                crewSize = this.crewSize,
                type = this.type,
                speed = this.speed,
                position = this.position,
                destination = this.destination,
                sectionIndex = this.sectionIndex,
                ownerId = this.objShipOwnerId,
                crew = this.objCrew.ToArray()
            };
        }

        public IEnumerator<string> GetEnumerator() {
            return this.objCrew.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Тип корабля
        /// </summary>
        public enum Types {
            /// <summary>
            /// Маленький корабль
            /// </summary>
            Small = 0,
            /// <summary>
            /// Средний корабль
            /// </summary>
            Middle,
            /// <summary>
            /// Крупный корабль
            /// </summary>
            Big
        }

    }
}