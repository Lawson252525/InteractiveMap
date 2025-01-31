using System.Linq;
using InteractiveMap.Models;
using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Базовый компонент для управления событием которое перемещается по карте
    /// </summary>
    public abstract class MoveEventView<T> : BaseEventViewType<T> where T : MoveEvent {
        /// <summary>
        /// Массив секций на карте
        /// </summary>
        protected Section[] sections;
        /// <summary>
        /// Границы карты
        /// </summary>
        protected Rect borders {get; private set;}

        protected override void Start() {
            //Получаем массив всех секций на карте
            this.sections = Map.Instance.GetSections();

            //Получаем границы карты
            this.borders = Map.Borders;

            base.Start();
        }

        public override void Update() {
            if (this.element is null || this.element.isComplete) return;

            //Регистрируем секцию в которой находится событие
            if (this.sections.Length > 0 && this.element.isChanged) {
                var section = this.sections.FirstOrDefault(s => s.size.Contains(this.element.position));
                if (section) this.element.sectionIndex = section.index;
            }
        }

    }
}