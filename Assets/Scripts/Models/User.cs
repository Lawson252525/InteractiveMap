using System;

namespace InteractiveMap.Models {
    /// <summary>
    /// Класс пользователя
    /// Хранит настройки пользователя
    /// </summary>
    public sealed class User : IEquatable<User> {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public readonly string name;
        /// <summary>
        /// Уникальный идентификатор пользователя uuid
        /// </summary>
        public readonly string id;
        /// <summary>
        /// Является ли пользователь админом
        /// </summary>
        public readonly bool isAdmin = false;
        /// <summary>
        /// Поле статуса пользователя
        /// </summary>
        public readonly bool isOnline = false;

        public User(string name, string id, string status, bool isAdmin = false) : this(name, id, status == "online", isAdmin) {}

        public User(string name, string id, bool isOnline = false, bool isAdmin = false) {
            this.name = name;
            this.id = id;
            this.isAdmin = isAdmin;
            this.isOnline = isOnline;
        }

        /// <summary>
        /// Хэш код элемента является его ключом
        /// </summary>
        public override int GetHashCode() {
            return this.id.GetHashCode();
        }

        public bool Equals(User other) {
            return other && other.id == this.id;
        }

        public static implicit operator bool(User user) {
            return Equals(user, null) == false;
        }
    }
}