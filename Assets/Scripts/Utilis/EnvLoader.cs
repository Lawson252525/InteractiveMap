using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace InteractiveMap.Utils {
    /// <summary>
    /// Класс для работы с файлом настроек env
    /// </summary>
    public static class EnvLoader {
        /// <summary>
        /// Метод возвращает список данных файла env
        /// </summary>
        /// <returns>Список с ключом и значением</returns>
        public static Dictionary<string, string> GetEnvData() {
            var result = new Dictionary<string, string>();

            var filePath = Path.Combine(StreamingAssetsPath, ".env");
            if (File.Exists(filePath)) {
                try {
                    //Загружаем файл
                    var lines = File.ReadAllLines(filePath);
                    foreach(var line in lines) {
                        var split = line.Split('=');
                        if (split.Length == 2) result.Add(split[0], split[1]);
                    }
                } catch {}
            }

            return result;
        }
        /// <summary>
        /// Свойство возвращает путь к папке StreamingAssets
        /// </summary>
        public static string StreamingAssetsPath {
            get { return Application.streamingAssetsPath;}
        }

    }
}