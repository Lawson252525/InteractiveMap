using InteractiveMap.Models;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент управления событием Сокровище
    /// </summary>
    public sealed class TreasureViewPanel : EventViewPanel<Treasure> {
        /// <summary>
        /// Поле имени события и значения
        /// </summary>
        public Text nameLabel, amountLabel;

        public override void Initialize() {
            //Получаем данные события
            var changed = this.element.isChanged;
            var container = (TreasureContainer)this.element.GetContainer();
            if (changed) this.element.SetDirty();

            //Устанавливаем данные из контейнера
            this.nameLabel.text = container.name;
            this.descriptionLabel.text = container.description;
            this.amountLabel.text = $"{container.name} содержит {container.amount} золота";
        }

        /// <summary>
        /// Метод выполняет событие получения награды для локального пользователя
        /// </summary>
        public void GetTreasure() {

        }

    }
}