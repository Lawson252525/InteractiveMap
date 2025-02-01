using System.Collections;
using InteractiveMap.Models;
using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Главный компонент проекта
    /// Содержит настройки и глобальные параметры приложения
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour {
        /// <summary>
        /// Синглот главного компонента
        /// </summary>
        public static Main Instance {get; private set;}
        /// <summary>
        /// Локальный пользователь
        /// Для тестов используется учетка глобального администратора
        /// </summary>
        public static User LocalUser {get; private set;} = new User("admin", "de5d051b-6343-4b57-ab9c-7414bc1d70a6", true);

        private void Awake() {
            Instance = this;
        }

        //TEST
        private IEnumerator Start() {
            yield return new WaitForSeconds(1f);

            //Создание пользователя
            // string name = "Lawson";
            // string password = "1234";
            // var result = Server.CreateUser(name, password);
            // print($"Результат создания пользователя {name} ключ: {result}");

            //Вход пользователя
            // string name = "admin";
            // string password = "admin";
            // var result = Server.SignUp(name, password);
            // print($"Вход пользователя {name} выполнен: {result}");

            // yield return new WaitForSeconds(5f);

            // result = Server.SignOut(LocalUser.id, name, password);
            // print($"Выход пользователя {name} выполнен: {result}");
        }
        //

    }
}