using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class StackLimited
    {
        /// <summary>
        /// Лист для хранения команд 
        /// </summary>
        private ArrayList list;

        /// <summary>
        /// Ограничиваем размер стека
        /// </summary>
        private int maxSize;

        public StackLimited(int maxSize)
        {
            list = new ArrayList(64);
            this.maxSize = maxSize;
        }

        public StackLimited(int maxSize, int initialCapacity)
        {
            list = new ArrayList(initialCapacity);
            this.maxSize = maxSize;
        }

        /// <summary>
        /// Количество элементов в стеке
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// Верхний объект
        /// </summary>
        public object Peek()
        {
            return list[list.Count - 1];
        }

        /// <summary>
        /// Вставляем объект
        /// </summary>
        public void Push(object obj)
        {
            if (list.Count == maxSize)
            {
                list.RemoveAt(0);
            }

            this.list.Add(obj);
        }

        /// <summary>
        /// Извлекаем объект
        /// </summary>
        public object Pop()
        {
            object obj = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return obj;
        }

        /// <summary>
        /// Очищаем стэк
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }
    }
}
