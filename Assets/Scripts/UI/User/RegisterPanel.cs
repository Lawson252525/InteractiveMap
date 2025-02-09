using InteractiveMap.Control;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент регистрации нового пользователя
    /// </summary>
    public sealed class RegisterPanel : Signup {
        /// <summary>
        /// Поле выбора типа коробля
        /// </summary>
        private Toggle[] toggles;

        protected override void Start(){
            base.Start();

            //Все поля переключателей
            this.toggles = GetComponentsInChildren<Toggle>();
        }

        /// <summary>
        /// Метод обработки регистрации
        /// </summary>
        public void MakeRegistration() {
            var selected = this.toggles.FirstOrDefault(t => t.isOn);
            var index = this.toggles.ToList().IndexOf(selected);

            Main.Instance.TryRegister(this.nameField.text, this.passwordField.text, index, (flag) => {
                if (flag) SceneManager.LoadScene("Map");
                else {
                    //Отобразить что регистрация не удалась

                    this.group.interactable = true;
                    this.group.alpha = 1f;
                }
            });

            this.group.interactable = false;
            this.group.alpha = 0.5f;
        }

    }
}
