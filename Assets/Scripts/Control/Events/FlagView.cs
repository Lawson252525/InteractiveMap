using System;
using InteractiveMap.Models;
using InteractiveMap.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент для отображения события Метка
    /// </summary>
    public sealed class FlagView : BaseEventViewType<Flag>, ISelectionView {
        /// <summary>
        /// Максимальное время жизни Вихря в секундах
        /// </summary>
        [Range(1, 600)]
        public int maxTimeDelay = 540;
        /// <summary>
        /// Минимальное время жизни Вихря в секундах
        /// </summary>
        [Range(1, 600)]
        public int minTimeDelay = 240;
        /// <summary>
        /// Поле элемента флага
        /// </summary>
        public Image iconElement;
        /// <summary>
        /// Поле для компонента управления событием Метка
        /// </summary>
        public FlagViewPanel viewPanel;

        public override void OnUpdate() {
            if (this.element is null || this.element.isComplete) return;

            //Проверка времени жизни Вихря только если владелец события это локальный пользователь
            var nowTime = DateTime.Now;
            var complete = nowTime >= this.element.expires;
            if (complete) this.element.Complete();
        }

        public ViewPanel CreateViewPanel(RectTransform parent = null) {
            //Создаем новую панель управления
            var view = Instantiate<FlagViewPanel>(this.viewPanel, Vector2.zero, Quaternion.identity, parent);
            view.element = this.element;
            return view;
        }

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {}

    }
}