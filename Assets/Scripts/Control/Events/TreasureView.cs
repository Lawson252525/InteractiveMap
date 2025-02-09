using InteractiveMap.Models;
using InteractiveMap.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент для отображения события сокровище
    /// </summary>
    public sealed class TreasureView : BaseEventViewType<Treasure>, ISelectionView {
        /// <summary>
        /// Максимальное время жизни Вихря в секундах
        /// </summary>
        [Range(1, 600)]
        public int maxTimeDelay = 100;
        /// <summary>
        /// Минимальное время жизни Вихря в секундах
        /// </summary>
        [Range(1, 600)]
        public int minTimeDelay = 50;
        /// <summary>
        /// Максимамльное вознаграждение
        /// </summary>
        [Range(1, 1000)]
        public int maxAmount = 500;
        /// <summary>
        /// Минимальное вознаграждение
        /// </summary>
        [Range(1, 1000)]
        public int minAmount = 100;
        /// <summary>
        /// Поле для компонента управления событием
        /// </summary>
        public TreasureViewPanel viewPanel;

        public override void OnUpdate() {
            if (this.element is null || this.element.isComplete) return;

            //Проверка времени жизни Вихря только если владелец события это локальный пользователь
            var nowTime = System.DateTime.Now;
            var complete = nowTime >= this.element.expires;
            if (complete) this.element.Complete();
        }

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {}

        public ViewPanel CreateViewPanel(RectTransform parent = null) {
            //Создаем новую панель управления
            var view = Instantiate<TreasureViewPanel>(this.viewPanel, Vector2.zero, Quaternion.identity, parent);
            view.element = this.element;
            return view;
        }

    }
}