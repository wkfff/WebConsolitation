using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;

using Krista.FM.Client.Common;

using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;

namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	/// <summary>
	/// По хмл-параметрам закачки создает интерфейс пользователя
	/// </summary>
	public class PumpParamsDisplaying
    {
        #region Поля

        private XmlDocument xml_doc;
        public IWorkplace workplace;

        #endregion Поля


        #region Константы

        // Тэги хмл-документа
		//private const string tagDataPumpParams	= "DataPumpParams";
		private const string tagControlsGroup	= "ControlsGroup";
		private const string tagControl			= "Control";
		private const string tagRadio			= "Radio";
		private const string tagCheck			= "Check";

		// Атрибуты тэгов хмл-документа
		private const string attrName			= "Name";
		private const string attrText			= "Text";
		private const string attrLocationX		= "LocationX";
		private const string attrLocationY		= "LocationY";
		private const string attrWidth			= "Width";
		private const string attrHeight			= "Height";
		private const string attrType			= "Type";
		private const string attrPropertyValue	= "PropertyValue";
		private const string attrValue			= "Value";
		private const string attrParamsKind		= "ParamsKind";
        private const string attrFontBold       = "FontBold";

		// Значения атрибута Type
		private const string typeLabel			= "Label";
		private const string typeCombo			= "Combo";
		private const string typeEdit			= "Edit";
		private const string typeMaskEdit		= "MaskEdit";
		private const string typeCombo_Months	= "Combo_Months";
		private const string typeCombo_Years	= "Combo_Years";
		//private const string typeControl		= "Control";
		private const string typeGroupBox		= "GroupBox";
        private const string typeEditClsSelect  = "EditClsSelect";
        // множественный выбор
        private const string typeEditClsSelectEx = "EditClsSelectEx";
        private const string typeDateMaskedEdit = "DateMaskedEdit";

		// Значения атрибута ParamsKind
		private const string kindGeneral		= "General";
		//private const string kindIndividual		= "Individual";

        #endregion Константы


        #region Общие функции

        /// <summary>
        /// Возвращает значение атрибута с булевым значением
        /// </summary>
        /// <param name="xn">хмл-элемент</param>
        /// <param name="attrNm">Наименование атрибута</param>
        /// <param name="defaultValue">Значение атрибута по умолчанию (если он не найден)</param>
        /// <returns>Значение атрибута</returns>
        private static bool GetBoolAttrValue(XmlNode xn, string attrNm, bool defaultValue)
        {
            if (xn == null)
            {
                return defaultValue;
            }
            if (xn.Attributes.GetNamedItem(attrNm) == null)
            {
                return defaultValue;
            }
            if (xn.Attributes[attrNm].Value.ToUpper() == "TRUE")
            {
                return true;
            }
            else if (xn.Attributes[attrNm].Value.ToUpper() == "FALSE")
            {
                return false;
            }

            return defaultValue;
        }

        #endregion Общие функции


        #region Инициализация контролов

        /// <summary>
		/// Устанавливает общие свойства для всех контролов (ну, почти всех... :)
		/// </summary>
		/// <param name="control">Собственно контрол</param>
		/// <param name="node">хмл-параметры</param>
		private static void InitializeBaseControl(ref Control control, XmlNode node)
		{
			control.Name		= node.Attributes[attrName].Value;
			control.Text		= node.Attributes[attrText].Value;
			control.Location	= new Point(
				Convert.ToInt32(node.Attributes[attrLocationX].Value),
				Convert.ToInt32(node.Attributes[attrLocationY].Value));
			control.Size		= new Size(
				Convert.ToInt32(node.Attributes[attrWidth].Value),
				Convert.ToInt32(node.Attributes[attrHeight].Value));

            if (GetBoolAttrValue(node, attrFontBold, false))
            {
                Font font = new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Bold);
                control.Font = font;
            }
		}

        private void EditClsSelectButtonClick(object sender, EditorButtonEventArgs e)
        {
            if (workplace == null)
                return;
            UltraNumericEditor ed = (UltraNumericEditor)sender;
            int oldID = (int)ed.Value;
            object newID = 0;
            string clsName = (string)ed.Tag;
            if (workplace.ClsManager.ShowClsModal(clsName, oldID, -1, ref newID))
            {
                ed.Value = newID;
                //string val = workplace.ClsLookupManager.GetLookupValue(clsName, false, (int)newID);
            };
        }

        private void EditClsSelectExButtonClick(object sender, EditorButtonEventArgs e)
        {
            if (workplace == null)
                return;
            UltraTextEditor ed = (UltraTextEditor)sender;
            int oldID = -1;
            object newIDs = null;
            string clsName = (string)ed.Tag;
            if (!workplace.ClsManager.ShowClsModal(clsName, oldID, -1, 0, ref newIDs, false))
                return;
            List<int> idList = (List<int>)newIDs;
            int[] ids = idList.ToArray();
            string edValue = string.Empty;
            foreach (int id in ids)
                edValue += string.Format("{0},", id);
            ed.Value = edValue.Remove(edValue.Length - 1);
        }

        private void InitializeEditClsSelectControl(ref Control control, XmlNode node)
        {
            UltraNumericEditor ed = (UltraNumericEditor)control;
            ed.NumericType = NumericType.Integer;
            ed.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
            EditorButton editorButton = new EditorButton();
            ed.Tag = node.Attributes[attrPropertyValue].Value;
            ed.ButtonsRight.Add(editorButton);
            ed.ButtonStyle = UIElementButtonStyle.Button;
            ed.ReadOnly = true;
            ed.EditorButtonClick += new EditorButtonEventHandler(EditClsSelectButtonClick);
            ed.Value = node.Attributes[attrValue].Value;
        }

        private void InitializeEditClsSelectExControl(ref Control control, XmlNode node)
        {
            UltraTextEditor ed = (UltraTextEditor)control;
            ed.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
            EditorButton editorButton = new EditorButton();
            ed.Tag = node.Attributes[attrPropertyValue].Value;
            ed.ButtonsRight.Add(editorButton);
            //ed.ButtonStyle = UIElementButtonStyle.Button;
            ed.ReadOnly = true;
            ed.EditorButtonClick += new EditorButtonEventHandler(EditClsSelectExButtonClick);
            ed.Value = node.Attributes[attrValue].Value;
        }

        private void InitializeMaskedEditSelectControl(ref Control control, XmlNode node)
        {
            UltraMaskedEdit me = (UltraMaskedEdit)control;
            me.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
            me.InputMask = "99.99.9999";
            me.Value = node.Attributes[attrValue].Value;
        }

		private static GroupBox InitializeGroupBox(ref Control parentControl, XmlNode node)
		{
			GroupBox box = new GroupBox();
			parentControl.Controls.Add(box);

			// Свойства из хмл
			Control control = box;
			InitializeBaseControl(ref control, node);
			// Настройка стиля
			box.BackColor = Color.Transparent;
			box.FlatStyle = FlatStyle.Flat;

			return box;
		}

		private static void AddItemToCombo(string item_text, ref UltraComboEditor combo)
		{
			ValueListItem combo_item = new ValueListItem();
			combo_item.DataValue = item_text;
			combo.Items.Add(combo_item);
		}

		/// <summary>
		/// Добавляет в комбобокс элементы списка
		/// </summary>
		/// <param name="control">Комбобокс</param>
		/// <param name="node">хмл-настройки</param>
		private static void InitializeCombo(ref Control control, XmlNode node)
		{
			UltraComboEditor combo = (UltraComboEditor)control;
			XmlNodeList node_list = node.ChildNodes;

			foreach (XmlNode item in node_list)
			{
				AddItemToCombo(item.Attributes[attrValue].Value, ref combo);
			}
            if (node.Attributes[attrValue].Value == string.Empty)
            {
                combo.SelectedIndex = 0;
            }
            else
            {
                combo.SelectedIndex = Convert.ToInt32(node.Attributes[attrValue].Value);
            }

			control = combo;
		}

		/// <summary>
		/// Добавляет в комбобокс названия месяцев
		/// </summary>
		/// <param name="control">Комбобокс</param>
		/// <param name="node">хмл-настройки</param>
		private static void InitializeComboMonth(ref Control control, XmlNode node)
		{
			UltraComboEditor combo = (UltraComboEditor)control;
			
            AddItemToCombo("<Без ограничений>", ref combo);
			AddItemToCombo("Январь", ref combo);
			AddItemToCombo("Февраль", ref combo);
			AddItemToCombo("Март", ref combo);
			AddItemToCombo("Апрель", ref combo);
			AddItemToCombo("Май", ref combo);
			AddItemToCombo("Июнь", ref combo);
			AddItemToCombo("Июль", ref combo);
			AddItemToCombo("Август", ref combo);
			AddItemToCombo("Сентябрь", ref combo);
			AddItemToCombo("Октябрь", ref combo);
			AddItemToCombo("Ноябрь", ref combo);
			AddItemToCombo("Декабрь", ref combo);

            if (node.Attributes[attrValue].Value == string.Empty)
            {
                combo.SelectedIndex = 0;
            }
            else
            {
                if (Convert.ToInt32(node.Attributes[attrValue].Value) < 0)
                {
                    combo.SelectedIndex = 0;
                }
                else
                {
                    combo.SelectedIndex = Convert.ToInt32(node.Attributes[attrValue].Value);
                }
            }

			control = combo;
		}

		/// <summary>
		/// Добавляет в комбобокс названия годов
		/// </summary>
		/// <param name="control">Комбобокс</param>
		/// <param name="node">хмл-настройки</param>
		private static void InitializeComboYears(ref Control control, XmlNode node)
		{
			UltraComboEditor combo = (UltraComboEditor)control;
			
            AddItemToCombo("<Без ограничений>", ref combo);
            int maxYear = DateTime.Now.Year + 10;
            for (int i = 2000; i <= maxYear; i++)
            {
                AddItemToCombo(i.ToString(), ref combo);
            }

            if (node.Attributes[attrValue].Value == string.Empty)
            {
                combo.SelectedIndex = 0;
            }
            else
            {
                int year = Convert.ToInt32(node.Attributes[attrValue].Value);
                if (year >= 2000 && year <= maxYear)
                {
                    combo.SelectedIndex = year - 2000 + 1;
                }
                else
                {
                    combo.SelectedIndex = 0;
                }
            }

			control = combo;
		}

		/// <summary>
		/// Инициализация маскедита
		/// </summary>
		/// <param name="control">Маскедит</param>
		/// <param name="node">хмл-настройки</param>
		private static void InitializeMaskedEdit(ref Control control, XmlNode node)
		{
			UltraMaskedEdit m_edit = (UltraMaskedEdit)control;

            if (node.Attributes[attrPropertyValue].Value != string.Empty)
            {
                m_edit.InputMask = node.Attributes[attrPropertyValue].Value;
            }
            if (node.Attributes[attrValue].Value == string.Empty)
            {
                m_edit.Text = "0";
            }
            else
            {
                m_edit.Text = node.Attributes[attrValue].Value;
            }

			control = m_edit;
		}

		/// <summary>
		/// Создает контрол, устанавливает его свойства и помещает в групбокс
		/// </summary>
		/// <param name="parentControl">Родительский контрол</param>
		/// <param name="node">хмл-параметры</param>
		private void InitializeControl(ref Control parentControl, XmlNode node)
		{
			Control control;

			switch (node.Attributes[attrType].Value)
			{
				case typeLabel:
					control = new UltraLabel();
					break;

				case typeCombo:
				case typeCombo_Months:
				case typeCombo_Years:
					control = new UltraComboEditor();
					break;

                case typeEdit:
                    control = new UltraTextEditor();
                    break;

                case typeEditClsSelect:
                    control = new UltraNumericEditor(); ;
                    break;

                case typeEditClsSelectEx:
                    control = new UltraTextEditor(); ;
                    break;

				case typeMaskEdit:
					control = new UltraMaskedEdit();
					break;

                case typeDateMaskedEdit:
                    control = new UltraMaskedEdit();
                    break;

                default:
					throw (new Exception("[PumpParamsDisplaying] Неопределенный тип компонента"));
			}

			parentControl.Controls.Add(control);
			// Свойства из хмл
			InitializeBaseControl(ref control, node);

			// Настройка стиля
			switch (node.Attributes[attrType].Value)
			{
				case typeLabel:
					UltraLabel label = (UltraLabel)control;
					label.AutoSize = false;
                    label.WrapText = true;
					break;

				case typeCombo:
					UltraComboEditor combo = (UltraComboEditor)control;
					InitializeCombo(ref control, node);
					combo.AutoSize = true;
					combo.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
					break;

				case typeEditClsSelect:
                    InitializeEditClsSelectControl(ref control, node);
                    break;

                case typeEditClsSelectEx:
                    InitializeEditClsSelectExControl(ref control, node);
                    break;

                case typeDateMaskedEdit:
                    InitializeMaskedEditSelectControl(ref control, node);
                    break;
                case typeEdit:
					UltraTextEditor edit = (UltraTextEditor)control;
					edit.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
					break;

				case typeMaskEdit:
					UltraMaskedEdit mask_edit = (UltraMaskedEdit)control;
					InitializeMaskedEdit(ref control, node);
					mask_edit.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
					break;

				case typeCombo_Months:
					UltraComboEditor combo_month = (UltraComboEditor)control;
					InitializeComboMonth(ref control, node);
					combo_month.AutoSize = true;
                    combo_month.DropDownStyle = DropDownStyle.DropDownList;   
                    combo_month.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
					break;

				case typeCombo_Years:
					UltraComboEditor combo_years = (UltraComboEditor)control;
					InitializeComboYears(ref control, node);
					combo_years.AutoSize = true;
                    combo_years.DropDownStyle = DropDownStyle.DropDownList;
                    combo_years.DisplayStyle = EmbeddableElementDisplayStyle.VisualStudio2005;
					break;
            }
		}

		/// <summary>
		/// Создает радиогруппу, устанавливает ее свойства и помещает в групбокс
		/// </summary>
		/// <param name="parentControl">Родительский контрол</param>
		/// <param name="node">хмл-параметры</param>
		private static void InitializeRadio(ref Control parentControl, XmlNode node)
		{
			RadioButton radioBtn = new RadioButton();
            parentControl.Controls.Add(radioBtn);

			// Свойства из хмл
            Control control = radioBtn;
			InitializeBaseControl(ref control, node);

            if (node.Attributes[attrValue].Value != string.Empty)
            {
                radioBtn.Checked = Convert.ToBoolean(node.Attributes[attrValue].Value);
            }
		}

		/// <summary>
		/// Создает чекбокс, устанавливает ее свойства и помещает в групбокс
		/// </summary>
		/// <param name="parentControl">Родительский контрол</param>
		/// <param name="node">хмл-параметры</param>
		private static void InitializeCheck(ref Control parentControl, XmlNode node)
		{
			UltraCheckEditor check = new UltraCheckEditor();
			parentControl.Controls.Add(check);

			// Свойства из хмл
			Control control = check;
			InitializeBaseControl(ref control, node);

            if (node.Attributes[attrValue].Value != string.Empty)
            {
                check.Checked = Convert.ToBoolean(node.Attributes[attrValue].Value);
            }
		}

		/// <summary>
		/// Создает контролы и размещает их на панели
		/// </summary>
		/// <param name="GeneralContainer">Панель, где находятся контролы общих параметров</param>
		/// <param name="IndividualContainer">Панель, где находятся контролы индивидуальных параметров</param>
		/// <param name="DataPumpParams">хмл-параметры</param>
		/// <returns>Ошибка</returns>
		public string CreateControls(ref Panel GeneralContainer, ref Panel IndividualContainer, string DataPumpParams)
		{		
			try
			{
                GeneralContainer.Controls.Clear();
                IndividualContainer.Controls.Clear();
                xml_doc = new XmlDocument();
                xml_doc.LoadXml(DataPumpParams);


				// Получаем список всех групбоксов
				XmlNodeList group_list = xml_doc.GetElementsByTagName(tagControlsGroup);

				// Обходим все и помещаем необходимые компоненты в контейнер
				foreach (XmlNode group_node in group_list)
				{
                    Control parentControl;
                    switch (group_node.Attributes[attrParamsKind].Value)
					{
						case kindGeneral: parentControl = GeneralContainer;
							break;
						
						default: parentControl = IndividualContainer;
							break;
					}

					switch (group_node.Attributes[attrType].Value)
					{
						default:
							break;

                        case typeGroupBox:
							// Создаем групбокс
							GroupBox group_box = InitializeGroupBox(ref parentControl, group_node);
							parentControl = group_box;
							break;
					}

					// Список контролов данного групбокса
					XmlNodeList controls_list = group_node.ChildNodes;
					foreach (XmlNode control_node in controls_list)
					{
						switch(control_node.Name)
						{
							case tagControl:
								InitializeControl(ref parentControl, control_node);
								break;

							case tagRadio:
								InitializeRadio(ref parentControl, control_node);
								break;

							case tagCheck:
								InitializeCheck(ref parentControl, control_node);
								break;
						}
					}
				}
			}
			catch (Exception e)
			{
				return e.Message;
			}

			return string.Empty;
		}

		#endregion Инициализация контролов


		#region Сохранение контролов

		private void SaveControl(Control control, XmlNode node)
		{
			if (node.Attributes[attrValue] == null)
			{
				XmlAttribute attr = xml_doc.CreateAttribute(attrValue);
				node.Attributes.Append(attr);
			}
			node.Attributes[attrValue].Value = control.Text;

			switch (node.Attributes[attrType].Value)
			{
				case typeLabel:
					break;

				case typeCombo:
					UltraComboEditor combo = (UltraComboEditor)control;
					node.Attributes[attrValue].Value = Convert.ToString(combo.SelectedIndex);
					break;

                case typeEditClsSelect:
                    UltraNumericEditor ed = (UltraNumericEditor)control;
                    node.Attributes[attrValue].Value = ed.Value.ToString();
                    break;

                case typeEditClsSelectEx:
                    UltraTextEditor te = (UltraTextEditor)control;
                    node.Attributes[attrValue].Value = te.Value.ToString();
                    break;

                case typeEdit:
                    UltraTextEditor edit = (UltraTextEditor)control;
                    node.Attributes[attrText].Value = edit.Text;
					break;
                case typeDateMaskedEdit:
                    UltraMaskedEdit me = (UltraMaskedEdit)control;
                    node.Attributes[attrValue].Value = me.Value.ToString();
                    break;

				case typeMaskEdit:
					break;

				case typeCombo_Months:
					UltraComboEditor combo_months = (UltraComboEditor)control;
					node.Attributes[attrValue].Value = Convert.ToString(combo_months.SelectedIndex);
					break;

				case typeCombo_Years:
					UltraComboEditor combo_years = (UltraComboEditor)control;
					if (combo_years.SelectedIndex == 0) node.Attributes[attrValue].Value = "0";
					else node.Attributes[attrValue].Value = combo_years.Text;
					break;

				default:
					throw (new Exception("[PumpParamsDisplaying] Неопределенный тип компонента"));
			}
		}

		private void SaveRadio(Control control, XmlNode node)
		{
			if (node.Attributes[attrValue] == null)
			{
				XmlAttribute attr = xml_doc.CreateAttribute(attrValue);
				node.Attributes.Append(attr);
			}
            RadioButton radioBtn = (RadioButton)control;
            node.Attributes[attrValue].Value = Convert.ToString(radioBtn.Checked);
		}

		private void SaveCheck(Control control, XmlNode node)
		{
			if (node.Attributes[attrValue] == null)
			{
				XmlAttribute attr = xml_doc.CreateAttribute(attrValue);
				node.Attributes.Append(attr);
			}
			UltraCheckEditor check = (UltraCheckEditor)control;
			node.Attributes[attrValue].Value = Convert.ToString(check.Checked);
		}

		/// <summary>
		/// Сохраняет все контролы контейнера
		/// </summary>
		/// <param name="container">Контейнер</param>
		private void SaveContainer(Panel container/*, ref XmlDocument xmlDoc*/)
		{

			if (container.Controls.Count == 0) return;

			// Здесь обходим групбоксы
			for (int i = 0; i < container.Controls.Count; i++)
			{
                XmlNode control_node;
                // А здесь - то, что в этих групбоксах
				if (container.Controls[i].GetType() == typeof(GroupBox))
				{
					for (int j = 0; j < container.Controls[i].Controls.Count; j++)
					{
						control_node = xml_doc.SelectSingleNode(
							string.Format("//*[@Name = '{0}']", container.Controls[i].Controls[j].Name));
						switch (control_node.Name)
						{
							case tagControl:
								SaveControl(container.Controls[i].Controls[j], control_node);
								break;

							case tagRadio:
								SaveRadio(container.Controls[i].Controls[j], control_node);
								break;

							case tagCheck:
								SaveCheck(container.Controls[i].Controls[j], control_node);
								break;
						}
					}
				}
				else
				{
					control_node = xml_doc.SelectSingleNode(
							string.Format("//*[@Name = '{0}']", container.Controls[i].Name));
					switch (control_node.Name)
					{
						case tagControl:
							SaveControl(container.Controls[i], control_node);
							break;

						case tagRadio:
							SaveRadio(container.Controls[i], control_node);
							break;

						case tagCheck:
							SaveCheck(container.Controls[i], control_node);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Сохраняет значения контролов в хмл
		/// </summary>
		/// <param name="GeneralContainer">Панель, где находятся контролы общих параметров</param>
		/// <param name="IndividualContainer">Панель, где находятся контролы индивидуальных параметров</param>
		/// <param name="DataPumpParams">хмл-документ</param>
		/// <returns>Ошибка</returns>
		public string SaveControls(Panel GeneralContainer, Panel IndividualContainer, ref string DataPumpParams)
		{
			xml_doc = new XmlDocument();
			xml_doc.LoadXml(DataPumpParams);

			try
			{
				SaveContainer(GeneralContainer/*, ref xml_doc*/);
				SaveContainer(IndividualContainer/*, ref xml_doc*/);
				DataPumpParams = xml_doc.InnerXml;
			}
			catch (Exception e)
			{
				return e.Message;
			}

			return string.Empty;
		}

		#endregion Сохранение контролов
	}
}
