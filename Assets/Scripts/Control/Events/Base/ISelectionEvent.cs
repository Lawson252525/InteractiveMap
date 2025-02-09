using UnityEngine;
using UnityEngine.EventSystems;

namespace InteractiveMap.UI {
    /// <summary>
    /// Интерфейс управления элементом события
    /// </summary>
    public interface ISelectionView : IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
        /// <summary>
        /// Метод возвращает панель управления элементом события
        /// </summary>
        /// <returns>Новая панель управления</returns>
        ViewPanel CreateViewPanel(RectTransform parent = null);
    }
}