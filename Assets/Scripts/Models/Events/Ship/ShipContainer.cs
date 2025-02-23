using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Контейнер данных для события Корабль
    /// </summary>
    [System.Serializable]
    public struct ShipContainer : IEventContainer {
        /// <summary>
        /// Поле владельца корабля, не изменяется
        /// </summary>
        public string ownerId;
        /// <summary>
        /// Название и описание корабля
        /// </summary>
        [SerializeField]
        public string name, description;
        /// <summary>
        /// Индекс секции
        /// </summary>
        [SerializeField]
        public Vector2Int sectionIndex;
        /// <summary>
        /// Позиция и точка назначения корабля
        /// </summary>
        [SerializeField]
        public Vector2 position, destination;
        /// <summary>
        /// Скорость корабля
        /// </summary>
        [SerializeField]
        public double speed;
        /// <summary>
        /// Список имен других пользователей
        /// </summary>
        [SerializeField]
        public string[] crew;
        /// <summary>
        /// Поле максимальное число команды
        /// </summary>
        [SerializeField]
        public int crewSize;
        /// <summary>
        /// Поле тип корабля
        /// </summary>
        [SerializeField]
        private int shipType;

        public string typeName => typeof(Ship).ToString();

        /// <summary>
        /// Свойство возвращает и устанавливает тип корабля
        /// </summary>
        public Ship.Types type {
            get {return (Ship.Types)this.shipType;}
            set {this.shipType = (int)value;}
        }

        public string Serialize() {
            this.speed = System.Math.Round(this.speed, 2);

            return JsonUtility.ToJson(this);
        }

    }
}