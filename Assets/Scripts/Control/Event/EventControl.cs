using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InteractiveMap.Models;
using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент управленяи событиями на карте
    /// </summary>
    [DisallowMultipleComponent]    
    public sealed class EventControl : MonoBehaviour, IServerHandle {
        /// <summary>
        /// Синглот компонента, так как контроллер будет один карте
        /// </summary>
        public static EventControl Instance {get; private set;}

        /// <summary>
        /// Периодичность обработки базы данных в секундах
        /// </summary>
        [Range(1f, 10f)]
        public float dataBaseUpdateDelay = 1f;

        /// <summary>
        /// Время обновления базы данных
        /// </summary>
        private float dataBaseUpdateTime = 0f;
        /// <summary>
        /// Массив всех обработчиков событий
        /// </summary>
        private List<EventWorker> workers = new List<EventWorker>();
        /// <summary>
        /// Массив новых созданных событий локального пользователя
        /// </summary>
        private Dictionary<IEventContainer, EventWorker> localCreatingWorkers = new Dictionary<IEventContainer, EventWorker>();

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            //Обрабатывать события каждый кадр здесь
            for(int i = 0; i < this.workers.Count; i++) {
                var worker = this.workers[i];
                worker.OnUpdate();
            }
        }

        public IEnumerator UpdateServerRoutine() {
                //Обновляем базу данных через определенное кол-во времени
                if (Time.time >= this.dataBaseUpdateTime) {
                    this.dataBaseUpdateTime = Time.time + this.dataBaseUpdateDelay;

                    //Локальный пользователь
                    User localUser = Main.LocalUser;

                    //Загружаем все события из базы данных
                    BaseEvent[] globalEvents = Server.GetAllEvents(false);

                    var globalEventNames = globalEvents.Select(e => e.id);
                    HashSet<BaseEvent> globalNewEvents = new HashSet<BaseEvent>(globalEvents);

                    //Формируем список событий который еще нет у локального пользователя
                    //Удаляем события у локального пользователя которых уже нет в базе
                    //Формируем список новых событий которых еще нет у локального пользователя
                    HashSet<EventWorker> localRemoveWorkers = new HashSet<EventWorker>();
                    HashSet<BaseEvent> localUpdateEvents = new HashSet<BaseEvent>();
                    for(int i = 0; i < this.workers.Count; i++) {
                        var localWorker = this.workers[i];
                        var localEvent = localWorker.element;
                        var name = localEvent.id;
                        var onwerId = localEvent.owner;
                        
                        if (globalEventNames.Contains(name) == false) {
                            //Удаляем обработчики которых больше нет в базе данных без подчищения базы данных
                            this.workers.RemoveAt(i);
                            localWorker.Dispose();

                            i -= 1;
                        } else if (onwerId == localUser.id) {
                            //Если владелец события - локальный пользователь то проводим дальнейшую обработку
                            if (localEvent.isComplete) {
                                //Если обработчик завершен то удаляем его с карты и помещаем в список подлежащих удалению из базы данных
                                localRemoveWorkers.Add(localWorker);
                            } else if (localEvent.isChanged) {
                                //Изменные события помещаем в массив подлежащих изменению в базе данных
                                localUpdateEvents.Add(localEvent);
                            }
                        } else {
                            //Обновляем все остальные обработчики глобального списка событий у локального пользователя
                            var otherEvent = globalEvents.FirstOrDefault(e => e.id == localEvent.id);
                            if (otherEvent) {
                                var otherContainer = otherEvent.GetContainer();
                                localEvent.ApplyContainer(otherContainer);
                            }

                            //Меняем пользователя здесь если он вышел
                            var users = Main.Instance.GetUsers();
                            var otherUserId = otherEvent.owner;
                            var otherUser = users.FirstOrDefault(u => u.id == otherUserId);
                            if (otherUser) {
                                if (otherUser.isOnline == false) {
                                    var result = otherEvent.SetOwner(localUser.id);

                                    //Если удалось сменть пользователя то обновляем событие в базе данных
                                    if (result) {
                                        otherEvent.SetDirty();
                                        localUpdateEvents.Add(otherEvent);
                                    }
                                }
                            }
                        }

                        //Удаляем существующие у локального пользователя элементы из глобального списка всех элементов базы данных
                        //Это нужно чтобы исключить уже добавленные элементы из глобального списка и оставить только те что еще не добавлены локальному пользователю
                        var removed = globalNewEvents.RemoveWhere(e => e.id == localEvent.id);
                    }

                    //Удаляем обработчики локального пользователя и подчищаем базу данных
                    foreach(var worker in localRemoveWorkers) {
                        var element = worker.element;

                        //Удаляем данные из базы данных
                        var result = Server.RemoveEvent(element, false);

                        if (result) {
                            //Удаляем обработчик если удалось удалить событие из базы
                            this.workers.Remove(worker);
                            worker.Dispose();
                        }
                    }

                    //Добавляем новые события в базу данных и сооздаем их обработчики на карте
                    foreach(var pair in this.localCreatingWorkers) {
                        var container = pair.Key;
                        var worker = pair.Value;

                        //Отправляем запрос на добавление нового события с данными
                        var eventType = Type.GetType(container.typeName);
                        var element = Server.CreateEvent(eventType, container, localUser, false);

                        if (element) {
                            //Добавляем новый обработчик в массив всех обработчиков
                            this.workers.Add(worker);

                            //Инициализируем обработчик события
                            worker.Initialize(element);

                            //Переместить камеру к событию если их создал администратор
                            if (localUser.isAdmin && localUser.id == element.owner && element is PointEvent) {
                                var pointEvent = element as PointEvent;
                                CameraControl.Instance.MoveTo(pointEvent.position);
                            }
                        }
                    }
                    this.localCreatingWorkers = new Dictionary<IEventContainer, EventWorker>();

                    //Обновляем обработчики локального пользователя в базе данных
                    foreach(var element in localUpdateEvents) {
                        var container = element.GetContainer();

                        //Обновляем данные в базе данных
                        var result = Server.SetEvent(element, container, false);
                        if (result) element.Reset();
                    }

                    //Добавляем обработчики глобального списка которых еще нет у локального пользователя
                    foreach(var element in globalNewEvents) {
                        CreateEvent(element);
                    }
                }

                yield return new WaitForEndOfFrame();
        }

        private void OnEnable() {
            //Регистрация серверного обработчика
            Main.RegisterServerHandle(this);
        }

        private void OnDisable() {
            //Удаления серверного обработчика
            Main.UnregisterServerHandle(this);
        }

        /// <summary>
        /// Метод создания события по его типу
        /// </summary>
        /// <param name="eventType">Тип события</param>
        public void CreateEventSimple(string eventType) {
            CreateEvent(eventType);
        }

        /// <summary>
        /// Метод создает новый элемент события с случайными настройками
        /// </summary>
        /// <param name="eventType">Тип события</param>
        public void CreateEvent(string eventType, Action<EventWorker> OnCreated = null, Action<EventWorker> OnComplete = null) {
            EventWorker worker = null;

            if (typeof(Tornado).ToString().EndsWith(eventType)) {
                //Создаем новый обработчик для события Вихрь
                worker = new TornadoWorker();
            } else if (typeof(Treasure).ToString().EndsWith(eventType)) {
                //Создаем новый обработчик длс события Сокровище
                worker = new TreasureWorker();
            } else throw new Exception($"Не удалось создать событие {eventType}");

            if (worker) {
                //Передаем настройки и обработчик события для дальнейшего добавления в базу данных
                var container = worker.GenerateSettings();
                CreateEvent(container, worker, OnCreated, OnComplete);
            }
        }

        /// <summary>
        /// Метод создает новый элемент события
        /// </summary>
        /// <param name="container">Контейнер данных события</param>
        public void CreateEvent(IEventContainer container, EventWorker worker, Action<EventWorker> OnCreated = null, Action<EventWorker> OnComplete = null) {
            //Подключение события инициализации обработки
            if (OnCreated != null) worker.OnCreated += OnCreated;
            if (OnComplete != null) worker.OnComplete += OnComplete;

            //Добавляем данные и обработчик в массив новых элементов
            this.localCreatingWorkers.Add(container, worker);
        }

        /// <summary>
        /// Метод создает новый обработчик для готового события на карте для локального пользователя
        /// </summary>
        /// <param name="element">Элемент события</param>
        /// <returns>Обработчик события</returns>
        public EventWorker CreateEvent(BaseEvent element, Action<EventWorker> OnCreated = null, Action<EventWorker> OnComplete = null) {
            //Новый обработчик события
            EventWorker result = null;

            if (element is Tornado) result = new TornadoWorker();
            else if (element is Treasure) result = new TreasureWorker();
            else throw new Exception($"Не удалось создать событие {element.GetType()} по ключу {element.id}");

            if (result) {
                //Добавляем новый обработчик в массив
                this.workers.Add(result);

                //Подключение события инициализации обработки
                if (OnCreated != null) result.OnCreated += OnCreated;
                if (OnComplete != null) result.OnComplete += OnComplete;

                //Сразу же инициализируем обработчик события
                result.Initialize(element);
            }

            return result;
        }

    }
}