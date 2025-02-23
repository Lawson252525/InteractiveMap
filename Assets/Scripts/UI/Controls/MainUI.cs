using System.Collections;
using InteractiveMap.Control;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Клавный компонент управления интерфейсом
    /// </summary>
    public sealed class MainUI : BaseUI {
        /// <summary>
        /// Синглот компонента
        /// </summary>
        public static MainUI Instance {get; private set;}

        /// <summary>
        /// Поле области панели управления пользователя
        /// </summary>
        public RectTransform userPanelContainer;

        /// <summary>
        /// Поле панели управления пользователя
        /// </summary>
        private BlockPanel userPanel;

        protected override void Awake() {
            Instance = this;

            base.Awake();
        }

        private new IEnumerator Start() {
            //Ожидаем подключения пользователя
            while(Main.LocalUser is null) yield return new WaitForEndOfFrame();

            //Загружаем панель управления пользователя
            var panelName = Main.LocalUser.isAdmin ? "UI/User/AdminPanel" : "UI/User/UserPanel";
            this.userPanel = Resources.Load<BlockPanel>(panelName);
            if (this.userPanel) {
                this.userPanel = Instantiate<BlockPanel>(this.userPanel, Vector2.zero, Quaternion.identity, this.userPanelContainer);
                var rect = this.userPanel.transform;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.userPanelContainer);
            }
        }

        /// <summary>
        /// Метод открывает текущюу панель управления пользователя
        /// </summary>
        public void OpenUserPanel() {
            if (this.userPanel) this.userPanel.Open();
        }

    }
}