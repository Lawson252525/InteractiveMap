using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс события Метка
    /// </summary>
    public sealed class Flag : PointEvent {
        /// <summary>
        /// Время истченеия срока события
        /// </summary>
        public DateTime objExpiresTime;
        /// <summary>
        /// Свойство возвращает ключ пользователя - создателя
        /// </summary>
        public string creatorId {get; private set;}
        /// <summary>
        /// Поля имени и описания
        /// </summary>
        public string name, description;

        /// <summary>
        /// После участников метки
        /// </summary>
        private HashSet<string> members = new HashSet<string>();

        public DateTime expires => this.objExpiresTime;

        public Flag(string id, string owner, DateTime creationTime) : base(id, owner, creationTime) {}

        public Flag(string id, string owner, DateTime creationTime, string containerData) : this(id, owner, creationTime, JsonUtility.FromJson<FlagContainer>(containerData)) {}

        public Flag(string id, string owner, DateTime creationTime, FlagContainer container) : base(id, owner, creationTime) {
            this.position = container.position;
            this.objExpiresTime = container.expiresDate;
            this.creatorId = container.creatorId;
            this.name = container.name;
            this.description = container.description;
            this.members = container.members != null  ? new HashSet<string>(container.members) : new HashSet<string>();

            //Сбрасываем изменения события
            Reset();
        }

        /// <summary>
        /// Метод добавляет нового участника метки
        /// </summary>
        /// <param name="userId">Ключ пользователя</param>
        /// <returns>Результат операции</returns>
        public bool AddMember(string userId) {
            var result = this.members.Add(userId);

            if (result) SetDirty();

            return result;
        }

        /// <summary>
        /// Метод возвращает всех участников метки
        /// </summary>
        public IEnumerable<string> GetMembers() {
            return this.members;
        }

        /// <summary>
        /// Метод возвращает true если содержит участника
        /// </summary>
        /// <param name="userId">Ключ пользователя</param>
        /// <returns>Результат операции</returns>
        public bool ContainsMember(string userId) {
            return this.members.Contains(userId);
        }

        public override void ApplyContainer(IEventContainer container) {
            if (container is FlagContainer) {
                var con = (FlagContainer)container;
                this.position = con.position;
                this.members = new HashSet<string>(con.members);

                //При установке данных события не менять имя владельца метки
                //this.ownerName = con.ownerName;

                this.name = con.name;
                this.description = con.description;

                //Сбрасываем изменения события
                Reset();    
            }
        }

        public override IEventContainer GetContainer() {
            Reset();

            return new FlagContainer() {
                name = this.name,
                description = this.description,
                creatorId = this.creatorId,
                position = this.position,
                expiresDate = this.expires,
                members = this.members.ToArray()
            };
        }

    }
}