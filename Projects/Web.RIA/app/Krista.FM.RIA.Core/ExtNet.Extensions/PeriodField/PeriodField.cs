using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Ext.Net;
using Ext.Net.Utilities;

[assembly: WebResource("Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField.js.PeriodField.js", "text/javascript")]

namespace Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField
{
    // TODO Горячие клавиши, фокусировка контрола при его открытии (чтобы можно было без мышки обходиться при выборе периода)
    public sealed class PeriodField : DropDownField
    {
        private readonly string controlIDSuffix;

        /// <summary>
        /// Список месяцев.
        /// </summary>
        private static readonly List<string> months = new List<string>(12)
        {
            "Январь", 
            "Февраль", 
            "Март", 
            "Апрель", 
            "Май", 
            "Июнь", 
            "Июль", 
            "Август", 
            "Сентябрь", 
            "Октябрь", 
            "Ноябрь", 
            "Декабрь"
        };

        private readonly string Scope = "ExtExtensions.PeriodField";
        private TreePanel treePanel;
        private DatePicker datePicker;

        private PeriodFieldConfig config;

        public PeriodField(string controlIDSuffix)
        {
            this.controlIDSuffix = controlIDSuffix;
            InitField(new PeriodFieldConfig());
        }

        public PeriodField(string controlIDSuffix, PeriodFieldConfig config)
        {
            this.controlIDSuffix = controlIDSuffix;
            InitField(config);
        }

        protected override List<ResourceItem> Resources
        {
            get
            {
                List<ResourceItem> baseList = base.Resources;
                baseList.Capacity += 1;
                baseList.Add(new ClientScriptItem(
                        typeof(PeriodField),
                        "Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField.js.PeriodField.js",
                        "ux/extensions/maximgb/TreeGrid.js"));
                return baseList;
            }
        }

        public static string GetPeriodText(int value, bool renderQuartersAsMonths = false)
        {
            if (value <= 0)
            {
                return String.Empty;
            }

            var day = value % 100;
            var month = ((value - day) % 10000) / 100;
            var year = (value - (month * 100) - day) / 10000;

            // выбран год
            if (month == 0 && day == 1)
            {
                return String.Empty + year + " год";
            }

            // выбран квартал
            if (month == 99)
            {
                return FormatQuarterNodeText(day % 10, year, renderQuartersAsMonths);
            }

            // выбран месяц
            if (month >= 1 && month <= 12 && day == 0)
            {
                return String.Empty + months[month - 1] + ' ' + year + " года";
            }

            // выбрана точная дата
            return (day < 10 ? "0" : String.Empty) + day + (month < 10 ? ".0" : ".") + month + '.' + year;
        }

        public string GetRenderer()
        {
            return config.RenderQuartersAsMonths ? Scope + ".rendererMonthsInQuartersFn" : Scope + ".rendererFn";
        }

        private static string FormatQuarterNodeText(int quarter, int year, bool renderQuartersAsMonths)
        {
            string result;
            if (renderQuartersAsMonths)
            {
                var month = quarter * 3;
                var text = string.Empty;
                switch (month)
                {
                    case 3:
                        text = "месяца";
                        break;
                    case 6:
                    case 9:
                    case 12:
                        text = "месяцев";
                        break;
                }   
             
                result = "{0} {1} {2} года".FormatWith(month, text, year);
            }
            else
            {
                result = "{0} квартал {1} года".FormatWith(quarter, year);    
            }

            return result;
        }

        private void InitField(PeriodFieldConfig config)
        {
            this.config = config;

            treePanel = new TreePanel
            {
                Icon = Icon.Accept,
                Shadow = ShadowMode.None,
                UseArrows = true,
                Lines = true,
                SingleExpand = true,
                Width = 200,
                AutoScroll = true,
                Animate = true,
                EnableDD = true,
                ContainerScroll = true,
                NoLeafIcon = true,
                RootVisible = false,
                Height = 195,
            };

            if (!config.ShowMonth && !config.ShowQuarter && !config.ShowYear)
            {
                treePanel.Width = 0;
            }
            else
            {
                var root = new TreeNode("0", "ddd", Icon.None);
                treePanel.Root.Add(root);

                var startYear = config.MinDate == null ? 2000 : ((DateTime)config.MinDate).Year;
                var endYear = config.MaxDate == null ? DateTime.Today.Year : ((DateTime)config.MaxDate).Year;
                AddYearNodes(config, root, startYear, endYear);

                var dblClickHandler = new StringBuilder()
                    .Append("if (")
                    .Append(Scope).Append(".periodChecked(this, node, {0}, {1}, {2}) == true)"
                                .FormatWith(
                                    config.YearSelectable ? "true" : "false",
                                    config.QuarterSelectable ? "true" : "false",
                                    config.MonthSelectable ? "true" : "false"))
                    .Append(@"{
                    ")
                    .Append(config.AfterSelectHandler)
                    .Append(@"
                    }");

                if (config.ShowDay)
                {
                    treePanel.Listeners.DblClick.Handler = dblClickHandler.ToString();
                }
                else
                {
                    treePanel.Listeners.Click.Handler = dblClickHandler.ToString();
                }

                treePanel.SelectionModel.Add(new DefaultSelectionModel());
            }

            var panel = new Panel
            {
                Layout = "ColumnLayout",
                AutoWidth = true
            };

            if (!config.ShowDay)
            {
                panel.Items.Add(treePanel);
            }
            else
            {
                var selectHandler = Scope + ".dateSelected(item, date); " + config.AfterSelectHandler;

                // добавление календаря
                datePicker = new DatePicker
                {
                    ID = "PeriodDatePicker_{0}".FormatWith(controlIDSuffix),
                    AutoFocus = true,
                    Listeners =
                        {
                            Select =
                                {
                                    Handler = (!config.DaySelectable) ? String.Empty : selectHandler
                                }
                        }
                };

                // при клике на месяц - отображение соотв. месяца в календаре
                treePanel.Listeners.Click.AddAfter(Scope + ".periodClicked(node, {0});".FormatWith(datePicker.ID)); 
                
                if (config.MinDate != null)
                {
                    datePicker.MinDate = (DateTime)config.MinDate;
                }

                if (config.MaxDate != null)
                {
                    datePicker.MaxDate = (DateTime)config.MaxDate;
                }

                if (config.ShowMonth || config.ShowQuarter || config.ShowYear)
                {
                    panel.Items.Add(treePanel);
                }

                panel.Items.Add(datePicker);
            }

            Component.Add(panel);

            Mode = DropDownMode.ValueText;
            Editable = false;
            
            AutoFocus = true;
            AutoShow = true;
            SelectOnFocus = true;
        }

        private void AddYearNodes(PeriodFieldConfig config, TreeNode root, int startYear, int endYear)
        {
            if (config.ShowYear)
            {
                if (config.SortASC)
                {
                    for (int year = startYear; year <= endYear; year++)
                    {
                        AddYearAndChilds(config, year, root);
                    }
                }
                else
                {
                    for (int year = endYear; year >= startYear; year--)
                    {
                        AddYearAndChilds(config, year, root);
                    }
                }
            }
            else
            {
                if (config.SortASC)
                {
                    for (int year = startYear; year <= endYear; year++)
                    {
                        AddQuarterNodes(config, year, root);
                    }
                }
                else
                {
                    for (int year = endYear; year >= startYear; year--)
                    {
                        AddQuarterNodes(config, year, root);
                    }
                }
            }
        }

        private void AddYearAndChilds(PeriodFieldConfig config, int year, TreeNode root)
        {
            var yearNode = new TreeNode(
                ((year * 10000) + 1).ToString(),
                "{0} год".FormatWith(year),
                Icon.None)
                               {
                                   Leaf = false,
                                   AllowChildren = true
                               };
            root.Nodes.Add(yearNode);
            AddQuarterNodes(config, year, yearNode);
        }

        private void AddQuarterNodes(PeriodFieldConfig config, int year, TreeNode parentNode)
        {
            if (config.ShowQuarter)
            {
                if (config.SortASC)
                {
                    for (int quarter = 1; quarter <= 4; quarter++)
                    {
                        AddQuarterAndChilds(config, quarter, year, parentNode);
                    }
                }
                else
                {
                    for (int quarter = 4; quarter >= 1; quarter--)
                    {
                        AddQuarterAndChilds(config, quarter, year, parentNode);
                    }
                }
            }
            else
            {
                AddMonthNodes(config, year, parentNode, 1, 12);
            }
        }

        private void AddQuarterAndChilds(PeriodFieldConfig config, int quarter, int year, TreeNode parentNode)
        {
            var quarterNode = new TreeNode(
                ((year * 10000) + (9990 + quarter)).ToString(),
                 FormatQuarterNodeText(quarter, year, config.RenderQuartersAsMonths),
                Icon.None)
                                  {
                                      Leaf = false,
                                      AllowChildren = true
                                  };
            parentNode.Nodes.Add(quarterNode);
            var startMonth = ((quarter - 1) * 3) + 1;
            var endMonth = quarter * 3;

            AddMonthNodes(config, year, quarterNode, startMonth, endMonth);
        }

        private void AddMonthNodes(PeriodFieldConfig config, int year, TreeNode parentNode, int startMonth, int endMonth)
        {
            if (config.ShowMonth)
            {
                if (config.SortASC)
                {
                    for (int month = startMonth; month <= endMonth; month++)
                    {
                        AddMonthNode(month, year, parentNode);
                    }
                }
                else
                {
                    for (int month = endMonth; month >= startMonth; month--)
                    {
                        AddMonthNode(month, year, parentNode);
                    }
                }
            }
        }

        private void AddMonthNode(int month, int year, TreeNode parentNode)
        {
            var monthNode = new TreeNode(
                ((year * 10000) + (month * 100)).ToString(),
                "{0} {1} года".FormatWith(months[month - 1], year),
                Icon.None)
                                {
                                    Leaf = true
                                };

            parentNode.Nodes.Add(monthNode);
        }
    }
}
