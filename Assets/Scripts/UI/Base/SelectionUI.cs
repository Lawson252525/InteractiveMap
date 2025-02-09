using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Базовы компонент для работы с интерфейсом объекта карты
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class SelectionUI : BaseUI {
        /// <summary>
        /// Поле изображения объекта
        /// </summary>
        public Image icon;
        /// <summary>
        /// Тектовые поля названия и описания
        /// </summary>
        public Text nameField, descriptionField;

    }
}