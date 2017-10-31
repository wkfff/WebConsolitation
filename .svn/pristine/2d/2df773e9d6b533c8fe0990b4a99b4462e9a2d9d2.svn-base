using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary.Validations;

using Krista.FM.ServerLibrary;

namespace Krista.FM.ServerLibrary.Forecast
{
	/// <summary>
	/// Прототип функции обратного вызова информирования клиента
	/// </summary>
	/// <param name="message">Текст сообщения</param>
	public delegate void ForecastListenerDelegate(string message);

	/// <summary>
	/// Статус сценария
	/// </summary>
	public enum ScenarioStatus
	{
		BaseScenario = -1, //Базовый сценарий
		NonCalculated = 0, //Нерасчитанный сценарий
		ReadyToCalc = 1,  //Сценарий готовый к расчету
		Calculated = 2  //Расчитанный сценарий
	}

	public interface IForecastService : IForecastBaseService
	{
		/// <summary>
		/// Копирование деталей из базового сценария (ID=0) в созданный (toID)
		/// </summary>
		/// <param name="toID">ID Сценария в который производится копирование праметров</param>
		/// <param name="listener">Функция обратного вызова информирования клиента</param>
		void CopyScenarioDetails(Int32 toID, ForecastListenerDelegate listener);

		/// <summary>
		/// Копирование деталей из сценария (fromID) в сценарий (toID)
		/// </summary>
		/// <param name="fromID">ID сценария из которого будут копироваться записи</param>
		/// <param name="toID">ID сценария в который будут копироваться записи</param>
		/// <param name="listener">Функция обратного вызова информирования клиента</param>
		void CopyScenarioDetails(Int32 fromID, Int32 toID, ForecastListenerDelegate listener);

		/// <summary>
		/// Рассчитывает индикаторы модели по заданному ID сценария
		/// </summary>
		/// <param name="id">ID сценария</param>
		IValidatorMessageHolder CalcModel(Int32 id);

		/// <summary>
		/// Индикативное плнирование
		/// </summary>
		/// <param name="id">id сценария</param>
		/// <param name="ind">перечень индикаторов id,Col</param>
		/// <param name="adj">перечень регуляторов id,Col</param>
		IValidatorMessageHolder IdicPlanning(Int32 id, Dictionary<int, string> ind, Dictionary<int, string> adj);

		/// <summary>
		/// Возвращает Data Updater для детали t_forecast_adjvalues из вьюшки для варианта расчета
		/// </summary>
		/// <param name="filtr">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		IDataUpdater GetValuationAdjustersUpdater(String filtr);

		/// <summary>
		/// Возвращает Data Updater для детали t_forecast_indvalues из вьюшки для варианта расчета
		/// </summary>
		/// <param name="filtr">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		IDataUpdater GetValuationIndicatorsUpdater(String filtr);

		/// <summary>
		/// Возвращает Data Updater для деталей сценария с переопределенным запросом select
		/// </summary>
		/// <param name="key">Ключ объекта</param>
		/// <param name="filter">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		IDataUpdater GetScenarioDetailsUpdater(String key, String filter);

		/// <summary>
		/// Устанавливает статус сценария с ID
		/// </summary>
		/// <param name="status"></param>
		/// <param name="id"></param>
		void SetScenarioStatus(Int32 id, ScenarioStatus status);


		/// <summary>
		/// Получает название родительского cценария по id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		String GetParentScenarioName(Int32 id);

		/// <summary>
		/// Получает процент готовности сценария по id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		void GetPercentOfComplete(Int32 id);
	}

	public interface IForecastBaseService : IServerSideObject
	{
		/// <summary>
		/// Таблица с данными.
		/// </summary>
		IFactTable Data { get; }

		/// <summary>
		/// Детальные данные блока.
		/// </summary>
		// TODO: Возвращаемую коллекцию обернуть в объект унаследованный от ISMOSerializable
		Dictionary<string, IEntityAssociation> Details { get; }
	}

	public interface IForm2pService : IForecastBaseService
	{
		/// <summary>
		/// Метод создает набор параметров для деталей формы 2п с id.
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="estYear">оценочный год</param>
		/// <returns></returns>
		Boolean CreateNewForm2p(Int32 id, Int32 estYear);

		/// <summary>
		/// Возвращает Data Updater для детали из вьюшки формы 2п 
		/// </summary>
		/// <param name="filtr">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		IDataUpdater GetForm2pDetailUpdater(String filtr);

		/// <summary>
		/// Метод рассчитывает модель и заполняет параметры формы 2п на основе этой модели
		/// </summary>
		/// <param name="scenId">id сценария рассчитываемого для формирования формы 2п</param>
		/// <param name="id2p">id из таблицы фактов для формы 2п</param>
		void FillFromScen(Int32 scenId, Int32 id2p);

		/// <summary>
		/// Метод получает имя формы2п на основе id
		/// </summary>
		/// <param name="id">id</param>
		/// <returns></returns>
		String GetNameForm2pByID(Int32 id);

		/// <summary>
		/// Сохранить форму 2п в формате МЭР
		/// </summary>
		/// <param name="v1">id формы для варианта 1</param>
		/// <param name="v2">id формы для варианта 2</param>
		/// <param name="year">Год формы</param>
		Byte[] SaveFormToExcel(Int32 v1, Int32 v2, Int32 year);

		/// <summary>
		/// Метод получает имя формы2п, год, территорию на основе id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="year"></param>
		/// <param name="reg"></param>
		void GetNameYearRegionForm2pByID(Int32 id, out String name, out Int32 year, out Int32 reg);

		/// <summary>
		/// Метод закачивает данные из других таблиц в форму 2п
		/// </summary>
		void PumpFromAnotherTable(Int32 id, Boolean replace);

		/// <summary>
		/// Метод реализует альтернативный прогноз по форме 2п
		/// </summary>
		/// <param name="id"></param>
		void AlternativeForecast(Int32 id);

	    /// <summary>
	    /// Метод получает XML файл с формулами авторасчета Формы-2п
	    /// </summary>
	    /// <returns>Содержимое XML файла</returns>
	    string GetXMLForm2pCalc();
	}

}