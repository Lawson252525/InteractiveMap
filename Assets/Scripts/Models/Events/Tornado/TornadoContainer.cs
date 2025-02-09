using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Структура контейнер для данных события Вихрь
    /// </summary>
    [Serializable]
    public struct TornadoContainer : IEventContainer {
        /// <summary>
        /// Поле описания
        /// </summary>
        [SerializeField]
        public string description;
        /// <summary>
        /// Скорость события
        /// </summary>
        [SerializeField]
        public double speed;
        /// <summary>
        /// Точка назначения вихря
        /// </summary>
        [SerializeField]
        public Vector2 destination;
        /// <summary>
        /// Секция нахождения вихря
        /// </summary>
        [SerializeField]
        public Vector2Int sectionIndex;
        /// <summary>
        /// Позиция события на карте
        /// </summary>
        [SerializeField]
        public Vector2 position;
        /// <summary>
        /// Время завершения жизни Вихря
        /// </summary>
        [SerializeField]
        private string expires;

        public DateTime expiresDate {
            get {return DateTime.Parse(this.expires);}
            set {this.expires = value.ToString();}
        }

        public string typeName => typeof(Tornado).ToString();

        public string Serialize() {
            this.speed = Math.Round(this.speed, 2);

            return JsonUtility.ToJson(this);
        }
    }
}