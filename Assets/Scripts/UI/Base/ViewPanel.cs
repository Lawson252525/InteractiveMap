using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Базовый компонент управления событием
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Image), typeof(VerticalLayoutGroup))]
    public abstract class ViewPanel : BaseUI {
        /// <summary>
        /// Поле описания события
        /// </summary>
        public Text descriptionLabel;

        /// <summary>
        /// Поле изображения панели
        /// </summary>
        protected Image panelImage {get; private set;}
        /// <summary>
        /// Поле группы элементов
        /// </summary>
        protected VerticalLayoutGroup group {get; private set;}

        protected override void Awake() {
            base.Awake();

            //Получаем изображение панели
            this.panelImage = GetComponent<Image>();
            //Получаем ссылку на группу элементов
            this.group = GetComponent<VerticalLayoutGroup>();
        }

        /// <summary>
        /// Событие инициализации панели в интерфейсе
        /// </summary>
        /// <param name="element">Элемент</param>
        public abstract void Initialize();

    }
}