using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityNpgsql;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс для имитации работы сервера с базой данных
    /// </summary>
    public static class Server {
        /// <summary>
        /// Имя сервера на котором расположена база postgres
        /// </summary>
        static string ServerName = "localhost";
        /// <summary>
        /// Порт на котором расположена база postgres
        /// </summary>
        static string Port = "5432";
        /// <summary>
        /// Имя пользователя для доступа к базе данных
        /// </summary>
        static string User = "postgres";
        /// <summary>
        /// Пароль для доступа к базе данных
        /// </summary>
        static string Password = "!Lawson252525";
        /// <summary>
        /// Имя базы данных
        /// </summary>
        static string Base = "postgres";
        /// <summary>
        /// Полные параметры подключения к базе данных
        /// </summary>
        static string ConnectionParameters = $"Port={Port}; Server={ServerName}; Database={Base}; User ID={User}; Password={Password}";

        /// <summary>
        /// Свойство возвращает статус соединения с базой
        /// </summary>
        public static bool IsOpen {get; private set;}

        /// <summary>
        /// Поле соединения с базой
        /// </summary>
        private static NpgsqlConnection Connection = null;

        /// <summary>
        /// Метод возвращает массив всех событий в базе данных
        /// </summary>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Массив событий</returns>
        public static IEventBase[] GetAllEvents(bool openClose = true) {
            if (openClose) Open();

            var request = "SELECT * FROM events";
            using(var command = new NpgsqlCommand(request, Connection)) {
                using(var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        try {
                            //Читаем данные событий из базы данных
                            string id = reader.GetValue(reader.GetOrdinal("id")).ToString();
                            string owner = reader.GetString(reader.GetOrdinal("owner"));
                            DateTime creationTime = reader.GetTimeStamp(reader.GetOrdinal("creationTime"));
                            string typeName = reader.GetString(reader.GetOrdinal("type"));
                            string json = reader.GetValue(reader.GetOrdinal("data")).ToString();

                            //Конвертируем полученные данные в события

                        } catch(Exception e) {  
                            Debug.Log($"Не удалось прочитать одно из событий\n{e}");
                        }
                    }
                }
            }

            if (openClose) Close();

            return null;
        }

        /// <summary>
        /// Метод создает в базе данных новое событие по указанному типу
        /// </summary>
        /// <param name="eventType">Тип события</param>
        /// <param name="container">Настройки события</param>
        /// <param name="user">Пользователь для которого создается событие</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Элемент события</returns>
        public static BaseEvent CreateEvent(Type eventType, IEventContainer container, User user, bool openClose = true) {
            BaseEvent result = null;

            if (openClose) Open();

            try {
                var request = "INSERT INTO events(owner, type, data) VALUES(@owner, @type, @data) RETURNING *";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавляем параметры команды
                    command.Parameters.AddWithValue("type", eventType.ToString());
                    command.Parameters.AddWithValue("owner", user.id);
                    
                    var data = container.Serialize();
                    command.Parameters.AddWithValue("data", data);

                    using(var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            //Получаем уникальный ключ события
                            string id = reader.GetString(reader.GetOrdinal("id"));
                            DateTime creationTime = reader.GetTimeStamp(reader.GetOrdinal("creationTime"));

                            //Создаем новый элемент события через активатор
                            result = Activator.CreateInstance(eventType, id, creationTime, container) as BaseEvent;

                            break;
                        }
                    }
                }
            } catch(Exception e) {
                Debug.LogError($"Не удалось добавить новое событие {eventType}\n{e}");
            }

            if (openClose) Close();

            return result;
        }

        /// <summary>
        /// Метод создает нового пользоватля с правами и возвращает его ключ или пустую строку
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="password">Пароль</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Уникальный ключ</returns>
        public static string CreateUser(string name, string password, bool openClose = true) {
            if (openClose) Open();

            string result = string.Empty;

            try {
                //Добавляем пользователя в базу данных
                var request = "INSERT INTO users(name, password, status) VALUES(@name, @password, @status) RETURNING id";
                using(var command = new NpgsqlCommand(request, Connection)) {

                    //Добавляем параметры в команду заполнения
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("password", password);
                    command.Parameters.AddWithValue("status", "offline");

                    using(var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            //Читаем новый уникальный ключи пользователя
                            result = reader.GetValue(reader.GetOrdinal("id")).ToString();
                            break;
                        }
                    }
                }
            } catch(Exception e) {
                Debug.LogError($"Ошибка добавления нового пользователя {name}\n{e}");
            }

            if (openClose) Close();

            return result;
        }
        /// <summary>
        /// Метод возвращает пользователя с указанным именем и паролем или null
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Пользователь</returns>
        public static User GetUser(string name, bool openClose = true) {
            return GetAllUsers(openClose).FirstOrDefault(u => u.name == name);
        }
        /// <summary>
        /// Метод возвращает всех пользователей в базе данных
        /// </summary>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Массив пользователей</returns>
        public static User[] GetAllUsers(bool openClose = true) {
            if (openClose) Open();

            HashSet<User> users = new HashSet<User>();

            //Загружаем данные всех пользователей из базы данных
            var request = "SELECT * FROM users";
            using(var command = new NpgsqlCommand(request, Connection)) {
                using(var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        try {
                            //Читаем каждый параметр данных пользователя
                            string id = reader.GetValue(reader.GetOrdinal("id")).ToString();
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            string status = reader.GetString(reader.GetOrdinal("status"));
                            bool isAdmin = reader.GetBoolean(reader.GetOrdinal("admin"));

                            //Добавляем нового пользователя в список
                            User user = new User(name, id, isAdmin);
                            users.Add(user);
                        } catch(Exception e) {
                            Debug.Log($"Ошибка получения данных пользователя\n{e}");
                        }
                    }
                }
            }

            if (openClose) Close();

            return users.ToArray();
        }
        /// <summary>
        /// Метод проводит вход пользователя и устанавливает его статус
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="password">Пароль</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SignUp(string name, string password, bool openClose = true) {
            bool result = false;

            if (openClose) Open();

            try {
                var request = @"UPDATE users SET status=@status WHERE name=@name AND password=@password";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавляем параметры для команды
                    command.Parameters.AddWithValue("status", "online");
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("password", password);
                    command.ExecuteNonQuery();

                    result = true;
                }
            } catch(Exception e) {
                Debug.Log($"Ошибка входа пользователя {name}\n{e}");
                result = false;
            }

            if (openClose) Close();

            return result;
        }
        /// <summary>
        /// Метод выполняет выход пользователя и устанавливает его статус
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <param name="name">Имя</param>
        /// <param name="password">Пароль</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SignOut(string id, string name, string password, bool openClose = true) {
            bool result = false;

            if (openClose) Open();

            try {
                var request = "UPDATE users SET status=@status WHERE id=@id AND name=@name AND password=@password";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавляем параметры для команды
                    command.Parameters.AddWithValue("status", "offline");
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("password", password);
                    command.ExecuteNonQuery();

                    result = true;
                }
            } catch(Exception e) {
                Debug.Log($"Ошибка выхода пользователя {name}\n{e}");
                result = false;
            }

            if (openClose) Close();

            return result;
        }
        /// <summary>
        /// Метод возвращает статус пользователя
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Статус</returns>
        public static string GetUserStatus(string name, bool openClose = true) {
            string result = string.Empty;

            if (openClose) Open();

            try {
                var request = "SELECT status FROM users WHERE name=@name";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавление параметров команды
                    command.Parameters.AddWithValue("name", name);

                    using(var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            result = reader.GetString(reader.GetOrdinal("status"));
                            break;
                        }
                    }
                }
            } catch(Exception e) {
                Debug.Log($"Не удалось получить статус пользователя {name}\n{e}");
            }

            if (openClose) Close();

            return result;
        }

        /// <summary>
        /// Метод открывает новое соединения с базой
        /// </summary>
        public static void Open() {
            if (IsOpen || Connection != null) return; 

            try {
                Connection = new NpgsqlConnection(ConnectionParameters);
                Connection.Open();
                IsOpen = Connection != null;
            } catch(Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Метод закрывает соединение
        /// </summary>
        public static void Close() {
            if (Connection != null) {
                try {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                } catch(Exception e) {
                    Debug.LogError(e);
                }

                IsOpen = false;
            }
        }

    }
}