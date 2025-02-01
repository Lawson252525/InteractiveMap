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
    public sealed class EventControl : MonoBehaviour {
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
        private float dataBaseUpdateTimed = 0f;
        /// <summary>
        /// Массив всех обработчиков событий
        /// </summary>
        private List<EventWorker> workers = new List<EventWorker>();
        /// <summary>
        /// Массив новых созданных событий локального пользователя
        /// </summary>
        private Dictionary<Type, EventWorker> localCreatingWorkers = new Dictionary<Type, EventWorker>();

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

        private IEnumerator UpdateRoutine() {
            while(true) {
                //Обновляем базу данных через определенное кол-во времени
                this.dataBaseUpdateTimed += Time.deltaTime;
                if (this.dataBaseUpdateTimed >= this.dataBaseUpdateDelay) {
                    this.dataBaseUpdateTimed = 0f;

                    //Открываем данные сервера
                    Server.Open();

                    //Локальный пользователь
                    User localUser = Main.LocalUser;

                    //Загружаем все события из базы данных
                    BaseEvent[] globalEvents = new BaseEvent[0];
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
                        var owner = localEvent.owner;
                        
                        if (globalEventNames.Contains(name) == false) {
                            //Удаляем обработчики которых больше нет в базе данных без подчищения базы данных
                            this.workers.RemoveAt(i);
                            localWorker.Dispose();
                            i -= 1;
                        } else if (owner.id == localUser.id) {
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
                        }

                        //Удаляем существующие элементы из глобального списка всех элементов базы данных
                        //Это нужно чтобы исключить уже добавленные элементы из глобального списка и оставить только те что еще не добавлены локальному пользователю
                        globalNewEvents.Remove(localEvent);
                    }

                    //Удаляем обработчики локального пользователя и подчищаем базу данных
                    foreach(var worker in localRemoveWorkers) {
                        var element = worker.element;
                        this.workers.Remove(worker);
                        worker.Dispose();

                        //Удаляем данные из базы данных
                    }

                    //Добавляем новые события в базу данных и сооздаем их обработчики на карте
                    foreach(var pair in this.localCreatingWorkers) {
                        //Создаем новый элемент события с случайными настройками
                        var eventType = pair.Key;
                        var worker = pair.Value;
                        var settings = worker.GetSettings();
                        var element = Server.CreateEvent(eventType, settings, localUser, false);

                        if (element && worker) {
                            //Добавляем новый обработчик в массив всех обработчиков
                            this.workers.Add(worker);

                            //Инициализируем обработчик события
                            worker.Initialize(element);
                        }
                    }
                    this.localCreatingWorkers = new Dictionary<Type, EventWorker>();

                    //Обновляем обработчики локального пользователя в базе данных
                    foreach(var element in localUpdateEvents) {
                        var container = element.GetContainer();
                        //Конвертируем контейнер с данными 
                        var typeName = container.typeName;
                        var data = container.Serialize();

                        //Обновляем данные в базе данных
                    }

                    //Добавляем обработчики глобального списка которых еще нет у локального пользователя
                    foreach(var element in globalNewEvents) CreateEvent(element);

                    //Закрываем сервер
                    Server.Close();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void OnEnable() {
            //Запускаем корутину обновления базы данных
            StartCoroutine(UpdateRoutine());
        }

        private void OnDisable() {
            //останаливаем корутину обновления базы данных
            StopAllCoroutines();
        }

        /// <summary>
        /// Метод создает новый элемент события
        /// Событие добавляется в массив соыбтий ожидающих добавление
        /// </summary>
        /// <param name="eventType">Тип события</param>
        public void CreateEvent(string eventType) {
            EventWorker worker = null;
            Type type = null;

            if (eventType == typeof(Tornado).ToString()) {
                //Создаем новый обработчик для события Вихрь
                worker = new TornadoWorker();
                type = typeof(Tornado);
            } else throw new Exception($"Не удалось создать событие {eventType}");

            //Добавляем новое событие и его обработчик в массив ожидания
            if (worker && type != null) this.localCreatingWorkers.Add(type, worker);
        }

        /// <summary>
        /// Метод создает новый обработчик для готового события на карте для локального пользователя
        /// </summary>
        /// <param name="element">Элемент события</param>
        /// <returns>Обработчик события</returns>
        public EventWorker CreateEvent(BaseEvent element) {
            //Новый обработчик события
            EventWorker result = null;

            if (element is Tornado) {
                result = new TornadoWorker();
            } else throw new Exception($"Не удалось создать событие {element.GetType()} по ключу {element.id}");

            if (result) {
                //Добавляем новый обработчик в массив
                this.workers.Add(result);

                //Сразу же инициализируем обработчик события
                result.Initialize(element);
            }

            return result;
        }
    }
}