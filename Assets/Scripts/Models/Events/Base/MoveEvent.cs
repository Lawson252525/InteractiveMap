using System;
using UnityEngine;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для события которое может перемещаться по карте
    /// </summary>
    public abstract class MoveEvent : PointEvent {
        /// <summary>
        /// Точка перемещения события
        /// </summary>
        private Vector2 objDestination;
        /// <summary>
        /// Скорость перемещения к точке назначения
        /// </summary>
        private float objSpeed;

        protected MoveEvent(string id, DateTime creationTime) : base(id, creationTime) {}

        /// <summary>
        /// Свойство устанавливает скорость перемещения события
        /// </summary>
        public float speed {
            get {return this.objSpeed;}
            set {
                if (this.objSpeed != value) {
                    this.objSpeed = value;
                    SetDirty();
                }
            }
        }
        
        /// <summary>
        /// Свойство устаналивает точку назначения события
        /// </summary>
        public Vector2 destination {
            get {return this.objDestination;}
            set {
                if (this.objDestination != value) {
                    this.objDestination = value;
                    SetDirty();
                }
            }
        }
    }
}