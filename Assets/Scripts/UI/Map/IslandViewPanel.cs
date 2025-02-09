using InteractiveMap.Control;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент управления элементом Остров
    /// </summary>
    public sealed class IslandViewPanel : ViewPanel {
        /// <summary>
        /// Поле имени элемента
        /// </summary>
        public Text nameLabel;
        /// <summary>
        /// Свойство острова
        /// </summary>
        public Island island {get; set;}

        public override void Initialize() {
            this.nameLabel.text = this.island.textLabel.textElement.text;
        }

    }
}