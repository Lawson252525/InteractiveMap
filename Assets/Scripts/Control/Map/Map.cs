using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveMap.Models;
using UnityEngine;

namespace InteractiveMap.Control {
    /// <summary>
    /// Компонент для работы с картой
    /// Содержит границы карты
    /// Разбивает карту на секции по заданному кол-ву ячеек по длине и высоте
    /// Карта содержит секции
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer))]
    public sealed class Map : MonoBehaviour, IEnumerable<Section> {

        /// <summary>
        /// Синглтон карты
        /// Так как карта всегда одна на сцене то можно к ней будет обращаться напрямую
        /// </summary>
        public static Map Instance {get; private set;}
        /// <summary>
        /// Свойство возвращает границы карты на сцене
        /// </summary>
        public static Rect Borders {get; private set;}

        /// <summary>
        /// Кол-во делений карты по длине
        /// </summary>
        [Range(1, 10)]
        public int rows = 3;
        /// <summary>
        /// Кол-во делений карты по высоте
        /// </summary>
        [Range(1, 10)]
        public int collums = 3;

        /// <summary>
        /// Массив секций на карте
        /// </summary>
        private Section[] sections = new Section[0];
        /// <summary>
        /// Массив островов на карте
        /// </summary>
        private Island[] islands = new Island[0];

        private new SpriteRenderer renderer = null;

        private void Awake() {
            Instance = this;

            //Получаем компонент отображения спрайта карты
            this.renderer = GetComponent<SpriteRenderer>();

            //Устанавливаем область карты по размерам спрайта карты
            var bounds = this.renderer.bounds;
            var max = bounds.max;
            var min = bounds.min;
            Borders = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

            //Разбиваем карту на секции согласно кол-ву ячеек
            int count = rows * collums;
            int tableIndex = 0;
            if (count > 1) {

                //Инициализируем массив секций
                this.sections = new Section[count];

                //Создаем секции по длине и высоте карты
                float width = Borders.width;
                float heigth = Borders.height;

                //Размеры каждой секции
                float sectionWidth = width / (float)rows;
                float sectionHeigth = heigth / (float)collums;

                //Начальные позиции секции
                float xMin = Borders.min.x;
                float yMax = Borders.max.y - sectionHeigth;

                //Цикл построения секций
                //Строиться по алгоритму
                // 0:0  1:0   2:0
                // 0:1  1:1   2:1  
                for(int i = 0; i < collums; i++) {
                    for(int j = 0; j < rows; j++) {
                        //Индекс секции на карте
                        var index = new Vector2Int(j, i);

                        //Размеры секции на карте
                        var x = xMin + sectionWidth * (float)j;
                        var y = yMax - sectionHeigth * (float)i;
                        var rect = new Rect(x, y, sectionWidth, sectionHeigth);

                        //Заносим новую секцию в массив
                        var section = new Section(index, rect);
                        this.sections[tableIndex] = section;
                        tableIndex += 1;
                    }
                }

            } else {
                //Если карта состоит из одной секции то создаем нулевую секцию которая охватывает всю карту 0:0
                Section section = new Section(0, 0, Borders);
                this.sections = new Section[1]{section};
            }

            //Получаем массив островов на карте
            this.islands = GetComponentsInChildren<Island>();
        }

        //TEST
        // private void Start() {
        //     var tornadoPrefab = Resources.Load<TornadoView>("Events/Tornado");
        //     if (tornadoPrefab) {
        //         var minTime = tornadoPrefab.minTimeDelay;
        //         var maxTime = tornadoPrefab.maxTimeDelay;
        //         var time = UnityEngine.Random.Range(minTime, maxTime);
        //         var minSpeed = tornadoPrefab.minMoveSpeed;
        //         var maxSpeed = tornadoPrefab.maxMoveSpeed;
        //         var speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        //         var tornadoEvent = new Tornado("123", DateTime.Now, DateTime.Now.AddSeconds(time));
        //         tornadoEvent.speed = speed;
        //         tornadoEvent.position = this.transform.position;

        //         //Устанавливаем тестовую точку назначения как точку крайней секции
        //         tornadoEvent.destination = this.sections[this.sections.Length - 1].size.center;

        //         var tornadoView = Instantiate<TornadoView>(tornadoPrefab, this.transform.position, Quaternion.identity, this.transform);
        //         tornadoView.element = tornadoEvent;

        //         //Сбрасываем все изменения
        //         tornadoEvent.Reset();
        //     }
        // }
        //

        public IEnumerator<Section> GetEnumerator() {
            foreach(var section in this.sections) yield return section;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Метод возвращает массив всех секций на карте
        /// </summary>
        public Section[] GetSections() {
            return this.sections;
        }

#region  Отрисовка секицй на карте через Gizmos только в редакторе
#if UNITY_EDITOR
  private void OnDrawGizmos() {
   if (Application.isPlaying == false && this.sections.Length > 0) return;

   Color c = Gizmos.color;

   //Отрисовка каждой секции 
   for(int i = 0; i < this.sections.Length; i++) {
        Section section = this.sections[i];
        Rect r = section.size;
        Vector2 p1 = r.min;
        Vector2 p2 = new Vector2(r.max.x, p1.y);
        Vector2 p3 = r.max;
        Vector2 p4 = new Vector2(p1.x, r.max.y);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
   }

   Gizmos.color = c;
  }
#endif
#endregion

    }

}