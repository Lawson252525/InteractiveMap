﻿using System;
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
        public static BaseEvent[] GetAllEvents(bool openClose = true) {
            HashSet<BaseEvent> results = new HashSet<BaseEvent>();

            if (openClose) Open();

            var request = "SELECT * FROM events";
            using(var command = new NpgsqlCommand(request, Connection)) {
                using(var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        try {
                            //Читаем данные событий из базы данных
                            string id = reader.GetValue(reader.GetOrdinal("id")).ToString();
                            string owner = reader.GetValue(reader.GetOrdinal("owner")).ToString();
                            DateTime creationTime = reader.GetTimeStamp(reader.GetOrdinal("creationTime"));
                            string typeName = reader.GetString(reader.GetOrdinal("type"));
                            string json = reader.GetValue(reader.GetOrdinal("data")).ToString();

                            //Конвертируем полученные данные в события
                            var eventType = Type.GetType(typeName);
                            var element = Activator.CreateInstance(eventType, id, owner, creationTime, json) as BaseEvent;
                            if (element) results.Add(element);

                        } catch(Exception e) {  
                            Debug.Log($"Не удалось прочитать событие\n{e}");
                        }
                    }
                }
            }

            if (openClose) Close();

            return results.ToArray();
        }

        /// <summary>
        /// Метод возвращает все события для определенного пользователя
        /// </summary>
        /// <param name="owner">Ключ пользователя</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Массив событий</returns>
        public static BaseEvent[] GetEventsFor(string owner, bool openClose = true) {
            return GetAllEvents(openClose).Where(e => e.owner == owner).ToArray();
        }

        /// <summary>
        /// Метод возвращает все события для определенного пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Массив событий</returns>
        public static BaseEvent[] GetEventsFor(User user, bool openClose) {
            return GetEventsFor(user.id, openClose);
        }

        /// <summary>
        /// Метод устанавливает данные события
        /// </summary>
        /// <param name="element">Элемент события</param>
        /// <param name="container">Данные события</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SetEvent(BaseEvent element, IEventContainer container, bool openClose = true) {
            bool result = false;

            if (openClose) Open();

            var typeName = container.typeName;

            try {
                var request = "UPDATE events SET owner=@owner, type=@type, data=@data WHERE id=@id";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавление параметров
                    command.Parameters.AddWithValue("id", element.id);
                    command.Parameters.AddWithValue("owner", element.owner);
                    command.Parameters.AddWithValue("type", typeName);

                    var json = container.Serialize();
                    command.Parameters.AddWithValue("data", json);

                    command.ExecuteNonQuery();

                    result = true;
                }
            } catch(Exception e) {
                Debug.Log($"Не удалось установить данные события {typeName}\n{e}");
            }

            if (openClose) Close();

            return result;
        }

        /// <summary>
        /// Метод установки значений событию в базе данных
        /// </summary>
        /// <param name="id">Ключ события</param>
        /// <param name="container">Данные события</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SetEvent(string id, IEventContainer container, bool openClose = true) {
            bool result = false;

            if (openClose) Open();

            var typeName = container.typeName;

            try {
                var request = "UPDATE events SET data=@data WHERE id=@id";
                var json = container.Serialize();

                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавления параметров команду
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("data", json);
                    command.ExecuteNonQuery();

                    result = true;
                }
            } catch(Exception e) {
                Debug.Log($"Не удалось установить данные события {typeName}\n{e}");
            }

            if (openClose) Close();

            return result;
        }

        /// <summary>
        /// Метод удаления элемента события
        /// </summary>
        /// <param name="element">Элемент события</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool RemoveEvent(BaseEvent element, bool openClose = true) {
            return RemoveEvent(element.id, openClose);
        }

        /// <summary>
        /// Метод удаляет событие из базы данных
        /// </summary>
        /// <param name="id">Ключ события</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool RemoveEvent(string id, bool openClose = true) {
            bool result = false;

            if (openClose) Open();

            try {
                var request = "DELETE FROM events WHERE id=@id";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавляем параметры в команду
                    command.Parameters.AddWithValue("id", id);

                    command.ExecuteNonQuery();

                    result = true;
                }
            } catch(Exception e) {
                Debug.Log($"Не удалось стереть событие \n{e}");

                result = false;
            }

            if (openClose) Close();

            return result;
        }

        /// <summary>
        /// Метод создает в базе данных новое событие по параметрам
        /// </summary>
        /// <param name="container">Контейнер данных</param>
        /// <param name="user">Пользователь</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Элемент события</returns>
        public static BaseEvent CreateEvent(IEventContainer container, User user, bool openClose) {
            var type = Type.GetType(container.typeName);
            return CreateEvent(type, container, user, openClose);
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
                    
                    var json = container.Serialize();
                    command.Parameters.AddWithValue("data", json);

                    using(var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            //Получаем уникальный ключ события
                            string id = reader.GetValue(reader.GetOrdinal("id")).ToString();
                            DateTime creationTime = reader.GetTimeStamp(reader.GetOrdinal("creationTime"));
                            string owner = reader.GetValue(reader.GetOrdinal("owner")).ToString();

                            //Создаем новый элемент события через активатор
                            result = Activator.CreateInstance(eventType, id, user.id, creationTime, container) as BaseEvent;

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
                            User user = new User(name, id, status, isAdmin);
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
        /// <returns>Возвращает ключ пользователя</returns>
        public static string SignUp(string name, string password, bool openClose = true) {
            string result = string.Empty;

            if (openClose) Open();

            try {
                var request = "UPDATE users SET status=@status WHERE name=@name AND password=@password RETURNING id";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавляем параметры для команды
                    command.Parameters.AddWithValue("status", "online");
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("password", password);
                    
                    using(var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            //Читаем ключ пользователя
                            result = reader.GetValue(reader.GetOrdinal("id")).ToString();
                        }
                    }
                }
            } catch(Exception e) {
                Debug.Log($"Ошибка входа пользователя {name}\n{e}");
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