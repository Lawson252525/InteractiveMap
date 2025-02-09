using System.Collections;

namespace InteractiveMap.Models {
    /// <summary>
    /// Обработчик серверных событий
    /// </summary>
    public interface IServerHandle {
        /// <summary>
        /// Корутина обработки серверных данных
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateServerRoutine();
    }
}