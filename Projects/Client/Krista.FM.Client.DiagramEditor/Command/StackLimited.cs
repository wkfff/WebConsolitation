using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.DiagramEditor
{
    public class StackLimited
    {
        /// <summary>
        /// Лист для хранения команд 
        /// </summary>
        ArrayList list;

        /// <summary>
        /// Ограничиваем размер стека
        /// </summary>
        int maxSize;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="maxSize"></param>
        public StackLimited(int maxSize)
        {
            list = new ArrayList(32);
            this.maxSize = maxSize;
        }

        public StackLimited(int maxSize, int initialCapacity)
        {
            list = new ArrayList(initialCapacity);
            this.maxSize = maxSize;
        }

        /// <summary>
        /// Верхний объект
        /// </summary>
        /// <returns></returns>
        public object Peek()
        {
            return list[list.Count - 1];
        }

        /// <summary>
        /// Вставляем
        /// </summary>
        /// <param name="obj"></param>
        public void Push(object obj)
        {
            if (list.Count == maxSize)
                list.RemoveAt(0);
            this.list.Add(obj);
        }

        /// <summary>
        /// Извлекаем
        /// </summary>
        /// <returns></returns>
        public object Pop()
        {
            object obj = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return obj;
        }

        /// <summary>
        /// Количество элементов в стеке
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return list.Count; } 
        }

        /// <summary>
        /// Очищаем
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }
    }
}
