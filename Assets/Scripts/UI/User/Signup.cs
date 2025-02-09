using InteractiveMap.Control;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InteractiveMap.UI {
    /// <summary>
    /// Компонент для работы со входом в приложение
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
    public class Signup : BaseUI {
        /// <summary>
        /// После заполнения имени
        /// </summary>
        public InputField nameField = null;
        /// <summary>
        /// После заполнения пароля
        /// </summary>
        public InputField passwordField = null;
        /// <summary>
        /// Обработчик группы
        /// </summary>
        protected CanvasGroup group {get; private set;}

        protected override void Start() {
            base.Start();

            //Получаем компонент обработки группы
            this.group = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Метод обработки входа в систему
        /// </summary>
        public void MakeSignup() {
            Main.Instance.TrySignup(this.nameField.text, this.passwordField.text, (flag) => {
                if (flag) SceneManager.LoadScene("Map");
                else {
                    //Отобразить что вход не удался

                    this.group.interactable = true;
                    this.group.alpha = 1f;
                }
            });

            this.group.interactable = false;
            this.group.alpha = 0.5f;
        }

    }
}