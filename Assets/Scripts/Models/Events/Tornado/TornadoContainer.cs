using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Структура контейнер для данных события Вихрь
    /// </summary>
    [System.Serializable]
    public struct TornadoContainer : IEventContainer {
        /// <summary>
        /// Скорость события
        /// </summary>
        [SerializeField]
        public float speed;
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
        public DateTime expires;

        public string typeName => typeof(Tornado).ToString();

        public string Serialize() {
            return JsonUtility.ToJson(this);
        }
    }
}