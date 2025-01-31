using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент управленяи событиями на карте
    /// </summary>
    [DisallowMultipleComponent]    
    public sealed class EventControl : MonoBehaviour {
        /// <summary>
        /// Синглот компонента, так как контроллер будет один карте
        /// </summary>
        public static EventControl Instance {get; private set;}

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            //Обрабатывать события здесь
        }

    }
}