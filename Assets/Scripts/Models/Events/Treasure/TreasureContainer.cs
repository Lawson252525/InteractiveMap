using UnityEngine;
using DateTime = System.DateTime;

namespace InteractiveMap.Models {
    /// <summary>
    /// Структура данных для события сокровище
    /// </summary>
    [System.Serializable]
    public struct TreasureContainer : IEventContainer {
        /// <summary>
        /// Поле имени
        /// </summary>
        [SerializeField]
        public string name;
        /// <summary>
        /// Поле описания
        /// </summary>
        [SerializeField]
        public string description;
        /// <summary>
        /// Позиция события на карте
        /// </summary>
        [SerializeField]
        public Vector2 position;
        /// <summary>
        /// Значение
        /// </summary>
        [SerializeField]
        public int amount;
        /// <summary>
        /// Время завершения жизни Вихря
        /// </summary>
        [SerializeField]
        private string expires;

        public DateTime expiresDate {
            get {return DateTime.Parse(this.expires);}
            set {this.expires = value.ToString();}
        }

        public string typeName => typeof(Treasure).ToString();

        public string Serialize() {
            return JsonUtility.ToJson(this);
        }
    }
}