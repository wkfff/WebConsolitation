using System;
using System.Diagnostics;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.InteropServices;


namespace Krista.FM.Common
{
	/// <summary>
    /// Базовый класс для поддержки детерменированного уничтожения объектов
	/// </summary>
    [DebuggerStepThrough()]
    [ComVisible(true)]
    public abstract class DisposableObject : MarshalByRefObject, IDisposable, ISponsor
	{
        /// <summary>
        /// Показывать ли отладочный вывод
        /// </summary>
        public static bool ShowTrace = false;
        
        private bool disposed = false;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		public DisposableObject()
		{
            if (ShowTrace)
            {
                //Console.ForegroundColor = ConsoleColor.Yellow;
                //System.Diagnostics.Trace.TraceInformation(this.ToString());
                //Console.ResetColor();
            }
        }

		/// <summary>
		/// Деструктор класса
		/// </summary>
        ~DisposableObject()
		{
            if (!this.disposed)
                Dispose(false);
        }

		
        // Вызавается когда данный объект разрушается для уведомления
        // унаследованных классов когда объект был удален.
//		protected abstract void OnDispose();


		/// <summary>
        /// Вызывается для детерменированного уничтожения объекта
		/// </summary>
		public void Dispose()
		{
            //Trace.WriteLine("Dispose() вызван", "DisposableObject");
            if (!this.disposed)
			{
                // Вызываем метод, реально выполняющий очистку.
                Dispose(true);

                // Поскольку очистка объекта выполняется явно,
				// запрещаем сборщику мусора вызов метода Finalize
				//GC.SuppressFinalize(this);
				
				this.disposed = true;


				// уведомляем унаследованные классы о том, что данный объект был уничтожен
//				this.OnDispose();   
			}
            //Trace.WriteLine("Dispose() завершон", "DisposableObject");
        }


		/// <summary>
		/// Этот открытый метод можно вызывать вместо Dispose
		/// </summary>
		public virtual void Close()
		{
			Dispose();
		}

		/// <summary>
		/// Общий метод, реально выполняющий очистку.
		/// Его вызывают методы Finalize, Dispose и Close
		/// </summary>
		/// <param name="disposing">
		/// true - явное уничтожение/закрытие объекта; 
		/// false - неявное уничтожение при сборке мусора
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
            //Trace.WriteLine("Dispose(" + Convert.ToString(disposing)  + ") вызван", "DisposableObject");
            // Синхронизируем потоки для запрета одновременного вызова Dispose/Close
			lock(this)
			{                
//                if (ListenerContainer.textWriter != null)
//                    if (Debug.Listeners.Contains(ListenerContainer.textWriter))
//                    {
                        /*Debug.WriteLine(String.Format("~{0}({1})", GetType().FullName, disposing));*/
//                    }
//                    else
//                        throw new Exception("Debug.Listeners.Contains(ListenerContainer.textWriter)");
                if (disposing)
                {
                    if (ShowTrace)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        try
                        {
                            Trace.WriteLine(String.Format("Dispose(true) {0} {1}", DateTime.Now, this));
                        }
                        catch
                        {
                        }
                        finally
                        {
                            Console.ResetColor();
                        }
                    }

                    // Здесь еще можно обращаться к полям, ссылающимся 
                    // на другие объекты - это безопасно для кода, так как для
                    // этих объектов метод Finalize еще не вызван
                }
                else
                {
                    if (ShowTrace)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        try
                        {
                            Trace.WriteLine(String.Format("Dispose(false) {0} {1}", DateTime.Now, this));
                        }
                        catch
                        {
                        }
                        finally
                        {
                            Console.ResetColor();
                        }
                    }
                }

				// Здесь должно выполняться уничножение/закрытие неупровляемых ресурсов
			}
            //Trace.WriteLine("Dispose(" + Convert.ToString(disposing) + ") завершон", "DisposableObject");
        }


		/// <summary>
		/// Возвращает true если объект уже был уничтожен
		/// </summary>
        [System.ComponentModel.Browsable(false)]
		public bool Disposed
		{
			get	{ return this.disposed; }
        }


        /// <summary>
        /// Генерит исключение ObjectDisposedException
        /// если объект был уничтожен.
        /// </summary>
        public void VerifyNotDisposed()
        {
            // если объект был уничтожен, то генерим исключение 
            // и передаем тип объекта в качестве сообщения
            if (this.Disposed)
                throw new ObjectDisposedException(this.GetType().ToString());
        }

        #region Аренда времени жизни для удаленных объектов

        // Внимание!!! Важно!!!
        // До настоящего времени (24.08.2006 ver.2.1.12.9) для удаленных объектов этого класса всегда 
        // устанавливалось бесконечное время жизни. Т.е. любой потомок класса DisposableObject ссылка
        // на который была передана клиенту через Remoting навсегда оставался в памяти сервера 
        // (до перезагрузки). Также в памяти оставались все его дочерние объекты (на которые имелись ссылки).
        // В частности, такими объектами являлись DataUpdater, Session, Task, TaskParam и т.п.
        // Это приводило к утечкам памяти.
        // Попробуем сделать следующим образом:
        //
        // 1) При активации объекта через Remoting зададим ему ограниченную аренду и объявим его 
        // своим же спонсором.
        // 2) При запросе на продолжение аренды будем отдавать 60 сек если для объекта не был вызван Dispose(),
        // если был вызван - аннулируем аренду. 
        //
        // Таким образом Remoting отпустит объект и он будет уничтожен при следующем сборе мусора.
        // Следует также заметить что данный механизм работает только при активации через Remoting,
        // при создании объектов только на сервере он активирован не будет.


        /// <summary>
        /// Первоначальная аренда времени жизни
        /// </summary>
        /// <returns>Аренда</returns>
        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                if (DisposableObject.ShowTrace)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Trace.WriteLine(String.Format("InitializeLifetimeService {0} {1}", DateTime.Now, this));
                    Console.ResetColor();
                }

                // устснавливаем параметры арнды
                lease.InitialLeaseTime = TimeSpan.FromSeconds(30);
                lease.RenewOnCallTime = TimeSpan.FromSeconds(5);
                lease.SponsorshipTimeout = TimeSpan.FromSeconds(3);
                // регистрируем себя в качестве спонсора
                lease.Register(this as ISponsor);
            }
            return lease;
        }

        /// <summary>
        /// Обработка запроса на продление аренды
        /// </summary>
        /// <param name="lease">Аренда</param>
        /// <returns>Время на которое следует продлить аренду</returns>
        public TimeSpan Renewal(ILease lease)
        {
            if (this.Disposed)
            {
                // если был вызван Dispose - аннулируем аренду
                lease.Unregister(this as ISponsor);
                if (DisposableObject.ShowTrace)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Trace.WriteLine(String.Format("Аренда аннулирована {0} {1}", DateTime.Now, this));
                    Console.ResetColor();
                }

                return TimeSpan.Zero;
            }
            else
            {
                // если объект еще жив - продляем аренду

                if (DisposableObject.ShowTrace)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Trace.WriteLine(String.Format("Аренда продлена {0} {1}", DateTime.Now, this));
                    Console.ResetColor();
                }

                return TimeSpan.FromSeconds(60);
            }
        }
        #endregion

    }
}
