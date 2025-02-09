using System.Collections;
using System.Collections.Generic;
using InteractiveMap.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InteractiveMap.Control {
    /// <summary>
    /// Главный компонент проекта
    /// Содержит настройки и глобальные параметры приложения
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour, IServerHandle {
        /// <summary>
        /// Синглот главного компонента
        /// </summary>
        private static Main ObjInstance = null;
        /// <summary>
        /// Локальный пользователь
        /// Для тестов используется учетка глобального администратора
        /// </summary>
        public static User LocalUser {get; private set;}
        /// <summary>
        /// Пароль локального пользователя
        /// </summary>
        private static string LocalUserPassword;
        /// <summary>
        /// Операция входа в приложение
        /// </summary>
        private static SignupOperation SignOperation = null;
        /// <summary>
        /// Оперция регистрации нового пользователя
        /// </summary>
        private static RegisterOperation RegOperation = null;

        /// <summary>
        /// Периодичность обработки пользователей базы данных
        /// </summary>
        [Range(1f, 10f)]
        public float usersUpdateDelay = 1f;

        /// <summary>
        /// Время обновления пользователей базы данных
        /// </summary>
        private float usersUpdateTime = 0f;
        /// <summary>
        /// Массив всех пользователей из базы данных
        /// </summary>
        private User[] users = new User[0];
        /// <summary>
        /// Массив серверных обработчиков
        /// </summary>
        private static HashSet<IServerHandle> ServerHandles = new HashSet<IServerHandle>();

        private void Start() {
            //ТЕСТОВЫЙ ВХОД ПОД АДМИНИСТРАТОРОМ В РЕДАКТОРЕ ЕСЛИ РАНЕЕ НЕ БЫЛ ВЫПОЛНЕН ВХОД
            #if UNITY_EDITOR
            if (LocalUser is null && SceneManager.GetActiveScene().name == "Map") TrySignup("admin", "admin");
            #endif
        }

        public IEnumerator UpdateServerRoutine() {
            if (Time.time >= this.usersUpdateTime) {
                this.usersUpdateTime = Time.time + this.usersUpdateDelay;

                //Обработка пользовательских данных
                this.users = Server.GetAllUsers(false);

                //Процесс подключения локального пользователя
                if (SignOperation && LocalUser is null) {
                    string name = SignOperation.name;
                    string password = SignOperation.password;

                    var id = Server.SignUp(name, password, false);
                    var result = string.IsNullOrEmpty(id) == false;
                    if (result) {
                        //Записать нового локального пользователя
                        LocalUser = new User(name, id, true, name == "admin");

                        //Запоминаем пароль локального пользователя
                        LocalUserPassword = password;

                        print($"Пользователь {name} вошел в систему, имеет права администратора: {LocalUser.isAdmin}");
                    }

                    //Отправка события входа пользователя
                    SignOperation.OnComplete?.Invoke(result);

                    //Удаляем операцию входа пользователя
                    SignOperation = null;
                }

                //Процесс регистрации нового пользователя
                if (RegOperation && LocalUser is null) {
                    //Добавляем новго пользователя
                    var id = Server.CreateUser(RegOperation.name, RegOperation.password, false);
                    var result = string.IsNullOrEmpty(id) == false;
                    if (result) {
                        print($"Новый пользователь {RegOperation.name} зарегистрирован");

                        //Создаем корабль для нового пользователя
                        if (result) {
                            int shitType = RegOperation.shipType;
                        }

                        //Выполняем вход для нового пользователя
                        id = Server.SignUp(RegOperation.name, RegOperation.password, false);
                        result = string.IsNullOrEmpty(id) == false;
                        if (result) {
                            //Записываем нового зарегистрированного пользователя как локального
                            LocalUser = new User(RegOperation.name, id, true);

                            //Запонимаем пароль локального пользователя
                            LocalUserPassword = RegOperation.password;
                        }
                    }

                    //Вызываем событие заврешения операции регистрации
                    RegOperation.OnComplete?.Invoke(result);

                    //Удаляем операцию регистрации нового пользователя
                    RegOperation = null;
                }
            }

            yield return new WaitForEndOfFrame();
        }    

        /// <summary>
        /// Метод обрабатывает серверные обработчики
        /// </summary>
        private IEnumerator UpdateServerHandlesRoutine() {
            while(true) {
                yield return new WaitForEndOfFrame();

                //Открываем доступ к серверу
                Server.Open();

                foreach(var handle in ServerHandles) yield return handle.UpdateServerRoutine();

                //Закрываем доступ к серверу
                Server.Close();
            }
        }

        /// <summary>
        /// Метод возвращает данные всех пользователей в базе данных
        /// </summary>
        /// <returns>Массив пользователей</returns>
        public User[] GetUsers() {
            return this.users;
        }

        /// <summary>
        /// Метод добавляет серверный обработчик
        /// </summary>
        /// <param name="handle">Серверный обработчик</param>
        /// <returns>Результат операции</returns>
        public static bool RegisterServerHandle(IServerHandle handle) {
            return ServerHandles.Add(handle);
        }

        /// <summary>
        /// Метод удаляет серверный обработчик
        /// </summary>
        /// <param name="handle">Серверный обработчик</param>
        /// <returns>Результат операции</returns>
        public static bool UnregisterServerHandle(IServerHandle handle) {
            return ServerHandles.Remove(handle);
        }

        private void OnEnable() {
            //Регистрация серверного обработчика
            RegisterServerHandle(this);

            //Запуск обработки серверных обработчиков
            StartCoroutine(UpdateServerHandlesRoutine());

            //Подключение события закрытия приложения
            Application.quitting += OnTryingQuit;
        }

        /// <summary>
        /// Метод обработки закрытия приложения
        /// </summary>
        private void OnTryingQuit() {
            //Попытка выхода пользователя из приложения
            if (string.IsNullOrEmpty(LocalUserPassword) == false) {
                    var result = Server.SignOut(LocalUser.id, LocalUser.name, LocalUserPassword, Server.IsOpen == false);
                    print($"Пользователь {LocalUser.name} вышел из приложения {result}");
            }
        }

        private void OnDisable() {
            //Удаление серверного обработчика
            UnregisterServerHandle(this);

            //Отключаем обработку серверных обработчиков
            StopAllCoroutines();

            //Отключение события закрытия приложения
            Application.quitting -= OnTryingQuit;
        }    

        /// <summary>
        /// Метод выполняет попытку входа в приложение
        /// </summary>
        /// <param name="OnSigned">Событие завершение обработки входа</param>
        public void TrySignup(string name, string password, System.Action<bool> OnSigned = null) {
            if (LocalUser is null && SignOperation is null) {
                //Создаем новую операцию входа пользователя в приложение
                SignOperation = new SignupOperation() {name=name, password=password, OnComplete=OnSigned};
            } else throw new System.Exception($"Пользователь {name} не может войти в систему так как пользователь {LocalUser.name} уже находится в системе!");
        }

        /// <summary>
        /// Метод выполняет попытку зарегистрировать нового пользователя
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="password">Пароль</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="OnRegistered">Событие регистрации</param>
        public void TryRegister(string name, string password, int shipType, System.Action<bool> OnRegistered = null) {
            if (RegOperation is null) {
                RegOperation = new RegisterOperation(){name=name, password=password, shipType=shipType, OnComplete=OnRegistered};
            } else throw new System.Exception($"Операция регистрации уже выполняется для {RegOperation.name}");
        }

        /// <summary>
        /// Свойство возвращает главный контроллер приложения
        /// </summary>
        public static Main Instance {
            get {
                if (ObjInstance is null) {
                    ObjInstance = Camera.main.gameObject.GetComponent<Main>();
                    if (ObjInstance is null) ObjInstance = Camera.main.gameObject.AddComponent<Main>();
                }
                return ObjInstance;
            }
        }

        /// <summary>
        /// Класс обработки операции входа в систему
        /// </summary>
        private class SignupOperation {
            /// <summary>
            /// Событие заврешения обратки
            /// </summary>
            public System.Action<bool> OnComplete;
            /// <summary>
            /// Имя и пароль пользователя
            /// </summary>
            public string name, password;

            public static implicit operator bool(SignupOperation operation) {
                return Equals(operation, null) == false;
            }

        }

        /// <summary>
        /// Класс обработки оперции регистрации в системе
        /// </summary>
        private class RegisterOperation : SignupOperation {
            /// <summary>
            /// Поле типа корябля
            /// </summary>
            public int shipType;

        }

    }
}