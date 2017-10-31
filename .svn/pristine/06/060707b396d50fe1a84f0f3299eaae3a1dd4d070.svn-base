using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Менеджер для управления операциями Undo\Redo
    /// </summary>
    public class UndoRedoManager
    {
        #region События

        private EventHandler _newEvent = null;
        private EventHandler _undoChanged = null;
        private EventHandler _redoChanged = null;
        private EventHandler _clearedEvents = null;

        /// <summary>
        /// Происходи при добавлении нового события
        /// </summary>
        public event EventHandler NewEvent
        {
            add { _newEvent += value; }
            remove { _newEvent -= value; }
        }

        /// <summary>
        /// Происходи при отмены действия
        /// </summary>
        public event EventHandler UndoChanged
        {
            add { _undoChanged += value; }
            remove { _undoChanged -= value; }
        }

        /// <summary>
        /// Происходи при возврате действия
        /// </summary>
        public event EventHandler RedoChanged
        {
            add { _redoChanged += value; }
            remove { _redoChanged -= value; }
        }

        /// <summary>
        /// Происходи при очистке всей истории
        /// </summary>
        public event EventHandler ClearedEvents
        {
            add { _clearedEvents += value; }
            remove { _clearedEvents -= value; }
        }

        #endregion

        #region Поля
        private bool _isRecordHistory;
        private Stack undoStack;
        private Stack redoStack;
        List<UndoRedoElementManager> elementsManager;
        #endregion

        #region Свойства
        /// <summary>
        /// Писать ли историю
        /// </summary>
        public bool IsRecordHistory
        {
            get { return _isRecordHistory; }
            set { _isRecordHistory = value; }
        }

        /// <summary>
        /// Можем ли мы пойти назад
        /// </summary>
        public bool IsMakeUndo
        {
            get 
            {
                foreach (string item in this.undoStack)
                {
                    UndoRedoElementManager elementManager = this.GetUndoRedoElementManager(item);
                    if (elementManager.IsMakeUndo)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Можем ли мы пойти в перед
        /// </summary>
        public bool IsMakeRedo
        {
            get { return this.redoStack.Count > 0; }
        }
        #endregion

        /// <summary>
        /// Создаем менеджер истории
        /// </summary>
        /// <param name="capacity">Вместимость истории (шагов)</param>
        public UndoRedoManager(int capacity)
        {
            this.undoStack = new Stack(capacity);
            this.redoStack = new Stack(capacity);
            this.elementsManager = new List<UndoRedoElementManager>();
            this.IsRecordHistory = true;
        }

        /// <summary>
        /// Добавляем информацию о состояние элемента, и тип произошедшего события
        /// </summary>
        /// <param name="element">элемент отчета</param>
        /// <param name="eventType">произошедшее событие</param>
        public void AddEvent(CustomReportElement element, UndoRedoEventType eventType)
        {
            this.AddEvent(element, eventType, false);
        }

        /// <summary>
        /// Добавляем информацию о состояние элемента, и тип произошедшего события
        /// </summary>
        /// <param name="element">элемент отчета</param>
        /// <param name="eventType">произошедшее событие</param>
        /// <param name="isForceClear">перед добавлением события, может принудительно
        /// очистить историю элемента</param>
        public void AddEvent(CustomReportElement element, UndoRedoEventType eventType,
            bool isForceClear)
        {

            if (this.IsRecordHistory && (element != null) &&
                ((eventType == UndoRedoEventType.AppearanceChange) ||
                 ((eventType == UndoRedoEventType.DataChange) && !element.PivotData.IsDeferDataUpdating)))
            {
                if (isForceClear)
                    this.RemoveElementEvents(element.UniqueName);

                UndoRedoElementManager elementManger = this.GetUndoRedoElementManager(element.UniqueName);

                if (elementManger == null)
                {
                    //если для указоного элемента еще нет своего менеджера, создадим
                    elementManger = new UndoRedoElementManager(element);
                    this.elementsManager.Add(elementManger);
                }

                if (elementManger.AddEvent(eventType))
                {
                    this.ClearRedoStack();
                    this.undoStack.Push(elementManger.UniqueName);
                }
                this.OnNewEvent();
            }
        }

        public void ClearRedoStack()
        {
            this.redoStack.Clear();
            foreach (UndoRedoElementManager elementManager in this.elementsManager)
            {
                elementManager.ClearRedoStack();
            }
        }

        /// <summary>
        /// Назад
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count > 1)
            {
                try
                {
                    this.IsRecordHistory = false;
                    //извлечем имя элемента
                    string elementUN = this.undoStack.Pop().ToString();
                    //добавим его в стэк redo
                    this.redoStack.Push(elementUN);

                    UndoRedoElementManager elementManager = this.GetUndoRedoElementManager(elementUN);
                    if (elementManager.IsMakeUndo)
                    {
                        this.OnUndoChanged();
                        elementManager.Undo();
                    }
                    else
                        this.Undo();
                }
                finally
                {
                    this.IsRecordHistory = true;
                }
            }
        }

        /// <summary>
        /// Вперед
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count > 0)
            {
                try
                {
                    this.IsRecordHistory = false;
                    //извлечем имя элемента
                    string elementUN = this.redoStack.Pop().ToString();
                    //добавим его в стэк undo
                    this.undoStack.Push(elementUN);

                    UndoRedoElementManager elementManager = this.GetUndoRedoElementManager(elementUN);
                    if (elementManager.IsMakeRedo)
                    {
                        this.OnRedoChanged();
                        elementManager.Redo();
                    }
                    else
                        this.Redo();
                }
                finally
                {
                    this.IsRecordHistory = true;
                }
            }
        }

        /// <summary>
        /// Удалить из стэков события указаного элемента
        /// </summary>
        /// <param name="elementUniqueName">уникальное имя элеменат отчета, события 
        /// которого надо удалить</param>
        public void RemoveElementEvents(string elementUniqueName)
        {
            this.RemoveElementEvents(this.undoStack, elementUniqueName);
            this.RemoveElementEvents(this.redoStack, elementUniqueName);
            
            UndoRedoElementManager elementManager = this.GetUndoRedoElementManager(elementUniqueName);
            this.elementsManager.Remove(elementManager);
        }

        /// <summary>
        /// Очистит всю историю
        /// </summary>
        public void ClearEvents()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();
            foreach (UndoRedoElementManager elementManager in this.elementsManager)
            {
                elementManager.Clear();
            }
            this.elementsManager.Clear();
            this.OnClearedEvents();
        }

        /// <summary>
        /// Из указаного стэка удалить события указаного элемента
        /// </summary>
        /// <param name="stack">стэк из которого удаляем</param>
        /// <param name="elementUniqueName">уникальное имя элеменат отчета, события 
        /// которого надо удалить</param>
        private void RemoveElementEvents(Stack stack, string elementUniqueName)
        {
            if ((stack.Count > 0) && (elementUniqueName != string.Empty))
            {
                List<Object> arrayInfo = new List<object>(stack.ToArray());
                //удалим события указаного элемента
                for (int i = arrayInfo.Count - 1; i >= 0; i--)
                {
                    string itemUN = arrayInfo[i].ToString();
                    if (itemUN == elementUniqueName)
                        arrayInfo.RemoveAt(i);
                }

                //инициализируем стэк заного, уже без удаленых объектов
                stack.Clear();
                for (int i = arrayInfo.Count - 1; i >= 0; i--)
                {
                    stack.Push(arrayInfo[i]);
                }
            }
        }

        /// <summary>
        /// Получить управляюшего историей элемента
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private UndoRedoElementManager GetUndoRedoElementManager(string elementUN)
        {
            if ((this.elementsManager.Count > 0) && (elementUN != string.Empty))
            {
                foreach (UndoRedoElementManager item in this.elementsManager)
                {
                    if (item.UniqueName == elementUN)
                        return item;
                }
            }
            return null;
        }

        private void OnNewEvent()
        {
            if (this._newEvent != null)
                this._newEvent(this, new EventArgs());
        }

        private void OnUndoChanged()
        {
            if (this._undoChanged != null)
                this._undoChanged(this, new EventArgs());
        }

        private void OnRedoChanged()
        {
            if (this._redoChanged != null)
                this._redoChanged(this, new EventArgs());
        }

        private void OnClearedEvents()
        {
            if (this._clearedEvents != null)
                this._clearedEvents(this, new EventArgs());
        }
    }

    public class UndoRedoElementManager
    {
        private string _uniqueName;
        private CustomReportElement _element;
        private Stack undoStack;
        private Stack redoStack;

        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        public CustomReportElement Element
        {
            get { return _element; }
            set { _element = value; }
        }

        /// <summary>
        /// Можем ли мы пойти назад
        /// </summary>
        public bool IsMakeUndo
        {
            get { return this.undoStack.Count > 1; }
        }

        /// <summary>
        /// Можем ли мы пойти в перед
        /// </summary>
        public bool IsMakeRedo
        {
            get { return this.redoStack.Count > 0; }
        }

        /// <summary>
        /// Создаем менеджер истории для элемента
        /// </summary>
        /// <param name="element"></param>
        public UndoRedoElementManager(CustomReportElement element)
        {
            this.undoStack = new Stack();
            this.redoStack = new Stack();
            this.Element = element;
            this.UniqueName = element.UniqueName;
        }

        /// <summary>
        /// Добавляем информацию о состояние элемента, и тип произошедшего события
        /// </summary>
        /// <param name="eventType">произошедшее событие</param>
        public bool AddEvent(UndoRedoEventType eventType)
        {
            UndoRedoInfo info = new UndoRedoInfo(this.Element, eventType);
            UndoRedoInfo prepareInfo = null; 
            if (this.undoStack.Count > 0) 
                prepareInfo = (UndoRedoInfo)this.undoStack.Peek();

            if (this.IsEqualEvents(info, prepareInfo))
            {
                //Если события совпадают, но у последнего событие связано с изменениями данных
                //то переписываем его у предыдущего
                if ((eventType == UndoRedoEventType.DataChange) && (prepareInfo != null))
                    prepareInfo.EventType = eventType;
            }
            else //Если это одно и тоже событие, записывать второй раз его не будем
            {
                this.redoStack.Clear();
                this.undoStack.Push(info);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Назад
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count > 1)
            {
                //извлечем событие
                UndoRedoInfo redoInfo = (UndoRedoInfo)this.undoStack.Pop();
                //добавим его в стэк redo
                this.redoStack.Push(redoInfo);

                UndoRedoInfo undoInfo = (UndoRedoInfo)this.undoStack.Peek();
                UndoRedoEventType undoInfoEventType = undoInfo.EventType;
                //тип берем у предыдущего события
                undoInfo.EventType = redoInfo.EventType;

                this.Element.DoUndoRedoEvent(undoInfo, EventType.Undo);

                if ((undoInfoEventType == UndoRedoEventType.AppearanceChange) && 
                    (this.Element is TableReportElement))
                    ((TableReportElement)this.Element).RefreshSyncCharts();

                //востанавливаем событию его тип
                undoInfo.EventType = undoInfoEventType;
            }
        }

        /// <summary>
        /// Вперед
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count > 0)
            {
                //извлечем событие
                UndoRedoInfo redoInfo = (UndoRedoInfo)this.redoStack.Pop();
                //добавим его в стэк undo
                this.undoStack.Push(redoInfo);

                this.Element.DoUndoRedoEvent(redoInfo, EventType.Redo);
            }
        }

        public void ClearRedoStack()
        {
            this.redoStack.Clear();
        }

        public void Clear()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();
            this.Element = null;
        }

        /// <summary>
        /// Эквиваленты ли события
        /// </summary>
        /// <param name="event1"></param>
        /// <param name="event2"></param>
        private bool IsEqualEvents(UndoRedoInfo event1, UndoRedoInfo event2)
        {
            if ((event1 == null) || (event2 == null))
                return false;
            return event1.ElementProperties.OuterXml == event2.ElementProperties.OuterXml;
        }
    }

    /// <summary>
    /// Информация необходимая для востановления элемента
    /// </summary>
    public class UndoRedoInfo
    {
        XmlNode _elementProperties;
        UndoRedoEventType _eventType;

        public XmlNode ElementProperties
        {
            get { return _elementProperties; }
            set { _elementProperties = value; }
        }

        public UndoRedoEventType EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }

        public UndoRedoInfo(CustomReportElement element, UndoRedoEventType eventType)
        {
            this.ElementProperties = element.Save();
            this.EventType = eventType;
        }
    }

    /// <summary>
    /// Режим обновления события
    /// </summary>
    public enum UndoRedoEventType
    {
        DataChange,
        AppearanceChange
    }

    /// <summary>
    /// Тип события
    /// </summary>
    public enum EventType
    {
        Undo,
        Redo
    }
}
