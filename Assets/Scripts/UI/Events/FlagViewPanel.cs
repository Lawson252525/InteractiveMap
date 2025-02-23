using InteractiveMap.Models;
using UnityEngine.UI;
using InteractiveMap.Control;
using System.Linq;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент управления событие Метка
    /// </summary>
    public sealed class FlagViewPanel : EventViewPanel<Flag> {
        /// <summary>
        /// Поле имени владельца метки
        /// </summary>
        public Text ownerLabel;
        /// <summary>
        /// Поле списка подключившихся пользователей
        /// </summary>
        public Text membersLabel;
        /// <summary>
        /// Поле кнопки подключения
        /// </summary>
        public Button enterButton;

        public override void Initialize() {
            var users = Main.Instance.GetUsers();

            this.descriptionLabel.text = $"{this.element.description}";
            var creator = users.FirstOrDefault(u => u.id == this.element.creatorId);
            var creatorName = creator ? creator.name : "Unknown";
            this.ownerLabel.text = $"{creatorName}";

            var localUser = Main.LocalUser;
            if (localUser) {
                //Отключаем кнопку если владелец Метки локальный пользователь или он уже состоит в группе
                this.enterButton.interactable = (this.element.creatorId == localUser.id || this.element.ContainsMember(localUser.id)) == false;
            }

            //Выбираем имена пользователей что подключились к Метке
            var userNames = users.Where(u => this.element.ContainsMember(u.id)).Select(u => u.name);
            var connectedNames = "\nОткликнулись: ";
            foreach(var name in userNames) connectedNames += $"\n - {name}";
            this.membersLabel.text = connectedNames;
        }

        /// <summary>
        /// Метод подключает локального пользователя в метку
        /// </summary>
        public void EnterFlag() {
            var localUser = Main.LocalUser;
            if (localUser) {
                //Подключаем локального пользователя к Метке
                if (this.element.creatorId != localUser.id && this.element.ContainsMember(localUser.id) == false) {
                    //Добавляем пользователя к группе
                    var result = this.element.AddMember(localUser.id);
                    if (result) {
                        this.element.SetOwner(localUser.id);
                        SelectionPanel.Instance.Select(null);
                    }
                }
            }
        }

    }
}