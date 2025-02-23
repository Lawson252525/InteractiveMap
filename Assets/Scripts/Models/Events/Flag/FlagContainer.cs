using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Контейнер для события Метка
    /// </summary>
    [System.Serializable]
    public struct FlagContainer : IEventContainer {
        /// <summary>
        /// После имени события
        /// </summary>
        [SerializeField]
        public string name;
        /// <summary>
        /// Описание события
        /// </summary>
        [SerializeField]
        public string description;
        /// <summary>
        /// Поле пользователя - создателя
        /// </summary>
        [SerializeField]
        public string creatorId;
        /// <summary>
        /// Поле позиции
        /// </summary>
        [SerializeField]
        public Vector2 position;
        /// <summary>
        /// Поле участников метки
        /// </summary>
        [SerializeField]
        public string[] members;
        /// <summary>
        /// Время завершения жизни Вихря
        /// </summary>
        [SerializeField]
        private string expires;

        public DateTime expiresDate {
            get {return DateTime.Parse(this.expires);}
            set {this.expires = value.ToString();}
        }

        public string typeName => typeof(Flag).ToString();

        public string Serialize() {
            return JsonUtility.ToJson(this);
        }
    }
}