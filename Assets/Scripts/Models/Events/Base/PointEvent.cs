using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для событий привязанных к точке на карте
    /// </summary>
    public abstract class PointEvent : BaseEvent {
        /// <summary>
        /// Индекс секции события
        /// </summary>
        private Vector2Int objSectionIndex;
        /// <summary>
        /// Позиция события на карте
        /// </summary>
        private Vector2 objPosition;

        public PointEvent(string id, string owner, DateTime creationTime) : base(id, owner, creationTime) {}

        /// <summary>
        /// Свойство устанавливает индекс секции
        /// </summary>
        public Vector2Int sectionIndex {
            get {return this.objSectionIndex;}
            set {
                if (this.objSectionIndex != value) {
                    this.objSectionIndex = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Свойство устанавливает позицию события на карте
        /// </summary>
        public Vector2 position {
            get {return this.objPosition;}
            set {
                if (this.objPosition != value) {
                    this.objPosition = value;
                    SetDirty();
                }
            }
        }
    }

}