using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Класс для обработки секций
    /// Секция содержит свои границы на карте
    /// </summary>
    public sealed class Section : System.IEquatable<Section> { 

        /// <summary>
        /// Индекс секции на карте
        /// Индекс задается как номер секции на карте x:y 0:0 0:1 и тд.
        /// </summary>
        public readonly Vector2Int index;
        /// <summary>
        /// Реальные размеры секции на карте
        /// </summary>
        public readonly Rect size;

        public Section(Vector2Int index, Rect size) {
            this.index = index;
            this.size = size;
        }

        public Section(int x, int y, Rect size) {
            this.index = new Vector2Int(x, y);
            this.size = size;
        }

        public bool Equals(Section other)
        {
            return other && other.index == this.index;
        }

        public static implicit operator bool(Section section) {
            return Equals(section, null) == false;
        }
    }
}