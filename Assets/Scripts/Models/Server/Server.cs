using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using InteractiveMap.Utils;
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
        static string BaseName = "postgres";
        /// <summary>
        /// Полные параметры подключения к базе данных
        /// </summary>
        static string ConnectionParameters = $"Port={Port}; Server={ServerName}; Database={BaseName}; User ID={User}; Password={Password}";

        /// <summary>
        /// Поле настроек сервера
        /// </summary>
        private static Dictionary<string, string> EnvSettings = null;

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
            return CreateEvent(type, container, user.id, openClose);
        }

        /// <summary>
        /// Метод создает в базе данных новое событие по параметрам
        /// </summary>
        /// <param name="container">Контейнер данных</param>
        /// <param name="userId">Ключ пользователя</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Элемент события</returns>
        public static BaseEvent CreateEvent(IEventContainer container, string userId, bool openClose) {
            var type = Type.GetType(container.typeName);
            return CreateEvent(type, container, userId, openClose);
        }

        /// <summary>
        /// Метод создает в базе данных новое событие по указанному типу
        /// </summary>
        /// <param name="eventType">Тип события</param>
        /// <param name="container">Настройки события</param>
        /// <param name="user">Пользователь для которого создается событие</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Элемент события</returns>
        public static BaseEvent CreateEvent(Type eventType, IEventContainer container, string userId, bool openClose = true) {
            BaseEvent result = null;

            if (openClose) Open();

            try {
                var request = "INSERT INTO events(owner, type, data) VALUES(@owner, @type, @data) RETURNING *";
                using(var command = new NpgsqlCommand(request, Connection)) {
                    //Добавляем параметры команды
                    command.Parameters.AddWithValue("type", eventType.ToString());
                    command.Parameters.AddWithValue("owner", userId);
                    
                    var json = container.Serialize();
                    command.Parameters.AddWithValue("data", json);

                    using(var reader = command.ExecuteReader()) {
                        while(reader.Read()) {
                            //Получаем уникальный ключ события
                            string id = reader.GetValue(reader.GetOrdinal("id")).ToString();
                            DateTime creationTime = reader.GetTimeStamp(reader.GetOrdinal("creationTime"));
                            string owner = reader.GetValue(reader.GetOrdinal("owner")).ToString();

                            //Создаем новый элемент события через активатор
                            result = Activator.CreateInstance(eventType, id, userId, creationTime, container) as BaseEvent;

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

            //Кэшируем пароль
            password = GetStringHash(password);

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
                            int value = reader.GetInt32(reader.GetOrdinal("value"));

                            //Добавляем нового пользователя в список
                            User user = new User(name, id, status, value, isAdmin);
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

            //Кэшируем пароль
            password = GetStringHash(password);

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

            //Кэшируем пароль
            password = GetStringHash(password);

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
        /// Метод устанавливает параметры для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="values">Значения</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SetUser(User user, Dictionary<string, object> values, bool openClose = true) {
            return SetUser(user.id, values, openClose);
        }

        /// <summary>
        /// Метод устанавливает данные для указанной таблицы с параметрами и условиями
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="values">Значения</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SetTableValues(string tableName, Dictionary<string, object> values, bool openClose = true) {
            return SetTableValues(tableName, values, null, openClose);
        }

        /// <summary>
        /// Метод устанавливает данные для указанной таблицы с параметрами и условиями
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="values">Значения</param>
        /// <param name="conditionValues">Условия</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SetTableValues(string tableName, Dictionary<string, object> values, Dictionary<string, object> conditionValues = null, bool openClose = true) {
            bool result = false;

            var valuesContainer = string.Empty;
            var conditionsContainer = string.Empty;

            //Собираем данные в контейнер
            int index = 0;
            int count = values.Count;
            var num = values.Keys.GetEnumerator();
            while(num.MoveNext()) {
                var name = num.Current;

                valuesContainer += $" {name}=@{name}";
                if (index < count - 1) valuesContainer += ",";
                
                index += 1;
            }

            //Собираем условия
            if (conditionValues != null) {
                index = 0;
                count = conditionValues.Count;
                num = conditionValues.Keys.GetEnumerator();
                while(num.MoveNext()) {
                    var name = num.Current;

                    conditionsContainer += $" {name}=@{name}";
                    if (index < count - 1) conditionsContainer += " AND ";

                    index += 1;
                }
            }   

            if (string.IsNullOrEmpty(valuesContainer)) throw new ArgumentException($"Данные для записи не были обнаружены");
            else {                
                if (openClose) Open();

                try {
                    //Заканчиваем сборку условий
                    if (string.IsNullOrEmpty(conditionsContainer) == false) conditionsContainer = $"WHERE {conditionsContainer}";

                    var request = $"UPDATE {tableName} SET {valuesContainer} {conditionsContainer}";
                    using(var command = new NpgsqlCommand(request, Connection)) {
                        //Заполняем параметры команды
                        foreach(var pair in values) {
                            var name = pair.Key;
                            var value = pair.Value;
                            command.Parameters.AddWithValue(name, value);
                        }

                        //Заполняем условия
                        if (conditionValues != null) {
                            foreach(var pair in conditionValues) {
                                var name = pair.Key;
                                var value = pair.Value;
                                command.Parameters.AddWithValue(name, value);
                            }
                        }

                        result = command.ExecuteNonQuery() >= 0;
                    }

                } catch(Exception err) {
                    Debug.Log($"Не удалось заполнить данные таблицы {tableName}\n{err}");
                }

                if (openClose) Close();
            }

            return result;
        }

        /// <summary>
        /// Метод устанавливает параметры для пользователя
        /// </summary>
        /// <param name="id">Ключ пользователя</param>
        /// <param name="values">Значения</param>
        /// <param name="openClose">Автоматически открыть и закрыть соединение</param>
        /// <returns>Результат операции</returns>
        public static bool SetUser(string id, Dictionary<string, object> values, bool openClose = true) {
            bool result = false;

            if (openClose) Open();

            try {
                //Сортируем параметры данных
                string paramNames = string.Empty;
                var count = values.Keys.Count;
                var keys = values.Keys.GetEnumerator();
                var index = 0;
                while(keys.MoveNext()) {
                    var current = keys.Current;
                    paramNames = $" {current}=@{current}";

                    if (index < count - 1) paramNames += ',';
                    index += 1;
                }

                if (string.IsNullOrEmpty(paramNames) == false) {
                    var request = $"UPDATE users SET {paramNames} WHERE id=@id";
                    using(var command = new NpgsqlCommand(request, Connection)) {
                        //Добавление параметров
                        command.Parameters.AddWithValue("id", id);
                        foreach(var pair in values) {
                            var key = pair.Key;
                            var value = pair.Value;
                            command.Parameters.AddWithValue(key, value);
                        }

                        command.ExecuteNonQuery();

                        result = true;
                    }
                }
            } catch(Exception e) {
                Debug.LogError($"Не удалось записать пользовательские данные для {id}\n{e}");
            }

            if (openClose) Close();

            return result;
        }

        /// <summary>
        /// Метод открывает новое соединения с базой
        /// </summary>
        public static void Open() {
            //Загружаем данные файла настроек
            if (EnvSettings is null) {
                EnvSettings = EnvLoader.GetEnvData();

                //Разбераем настройки сервера
                EnvSettings.TryGetValue("SERVER", out ServerName);
                EnvSettings.TryGetValue("PORT", out Port);
                EnvSettings.TryGetValue("USER", out User);
                EnvSettings.TryGetValue("PASSWORD", out Password);
                EnvSettings.TryGetValue("DATABASE", out BaseName);
            }

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

        /// <summary>
        /// Метод хэширует строку с помощью md5
        /// </summary>
        /// <param name="stroke">Строка</param>
        /// <returns>Хэш строки</returns>
        private static string GetStringHash(string stroke) {
            if (string.IsNullOrEmpty(stroke)) return stroke;

            var result = stroke;

            //Хэшируем пароль
            using (var md5 = MD5.Create()) {
                var bytes = Encoding.ASCII.GetBytes(stroke);
                var hash = md5.ComputeHash(bytes);
                result = Convert.ToBase64String(hash);
            }

            return result;
        }

    }
}