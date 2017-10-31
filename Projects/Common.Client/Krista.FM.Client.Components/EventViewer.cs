using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using System.Data;
using System.CodeDom.Compiler;
using System.IO;

using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.Components
{
    public partial class EventViewer : Form
    {
        DataSet dsEvents = new DataSet();
        Dictionary<int, Event> dctEvents = new Dictionary<int, Event>();
        
        internal EventListener listener = new EventListener();

        public EventViewer()
        {
            InitializeComponent();
            // создаем объекты для хранений перехваченных сообщений
            dsEvents.BeginInit();
            DataTable dt = new DataTable("Events");
            dt.BeginInit();
            DataColumn parentColumn = dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("TIME", typeof(DateTime));
            dt.Columns.Add("NAME", typeof(string));
            dt.EndInit();
            dsEvents.Tables.Add(dt);

            dt = new DataTable("EventsParams");
            dt.BeginInit();
            DataColumn childColumn = dt.Columns.Add("REFEVENT", typeof(int));
            dt.Columns.Add("PARAMNAME", typeof(string));
            dt.EndInit();
            dsEvents.Tables.Add(dt);

            DataRelation dr = new DataRelation("relation", parentColumn, childColumn, false);
            dsEvents.Relations.Add(dr);

            dsEvents.EndInit();

            DataViewManager dvManager = new DataViewManager(dsEvents);
            dvManager.DataViewSettings[0].Sort = " ID DESC";
            ugEvents.SyncWithCurrencyManager = false;
            ugEvents.DataSource = dvManager.CreateDataView(dsEvents.Tables[0]);
        }

        ~EventViewer()
        {
            if (listener != null)
                listener.Detach();
        }

        private static Form FindParentForm(Control cnt)
        {
            if (cnt == null)
                return null;

            if (cnt is Form)
                return cnt as Form;

            return FindParentForm(cnt.Parent);
        }

        public static EventViewer StartEventTracking(Control cnt)
        {
            EventViewer viewer = new EventViewer();
            viewer.listener.AttachTo(new AddEventDelegate(viewer.OnAddEvent), cnt);
            Form frm = FindParentForm(cnt);
            viewer.Left = cnt.Left;
            viewer.Top = cnt.Bottom;
            viewer.Show(frm as IWin32Window);
            return viewer;
        }

        public void StopEventTracking()
        {
            if (listener != null)
                listener.Detach();
            this.Close();
        }

        public void ClearEvents()
        {
            pgProps.SelectedObject = null;
            dsEvents.Clear();
            dctEvents.Clear();
        }

        private void ugEvents_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.ScrollStyle = ScrollStyle.Immediate;
            e.Layout.MaxBandDepth = 2;
            e.Layout.Appearance.FontData.SizeInPoints = 7;
            e.Layout.Appearance.BackColor = Color.White;

            e.Layout.Override.BorderStyleRow = UIElementBorderStyle.None;
            e.Layout.Override.BorderStyleHeader = UIElementBorderStyle.Default;// None;
            e.Layout.Override.BorderStyleCell = UIElementBorderStyle.None;

            e.Layout.Override.DefaultRowHeight = 9;
            e.Layout.Override.CellMultiLine = DefaultableBoolean.False;
            e.Layout.Override.CellPadding = 0;
            e.Layout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.MultiBand;
            e.Layout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.Vertical;
            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;

            e.Layout.Override.AllowAddNew = AllowAddNew.No;
            e.Layout.Override.AllowDelete = DefaultableBoolean.False;
            e.Layout.Override.AllowColSizing = AllowColSizing.None;
            e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
            e.Layout.Override.AllowRowFiltering = DefaultableBoolean.False;
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;

            e.Layout.Override.HeaderClickAction = HeaderClickAction.Select;
            e.Layout.Override.HeaderPlacement = HeaderPlacement.FixedOnTop;

            e.Layout.InterBandSpacing = 0;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

            // настраиваем слой событий
            UltraGridBand band = e.Layout.Bands["Events"];
            UltraGridColumn clmn = band.Columns["ID"];
            clmn.CellAppearance.ForeColor = Color.DarkGray;
            //clmn.Hidden = true;
            clmn.Width = 35;
            //SortedColumns.Add(clmn, true, false);
            clmn = band.Columns["NAME"];
            clmn.Header.Caption = "Событие";
            clmn.CellAppearance.ForeColor = Color.Blue;
            clmn = band.Columns["TIME"];
            clmn.CellAppearance.ForeColor = Color.DarkGray;
            clmn.Header.Caption = "Время";
            clmn.Format = "HH:mm:ss:ffff";
            // настраиваем слой параметров
            band = e.Layout.Bands["Relation"];
            clmn = band.Columns["REFEVENT"];
            clmn.Hidden = true;
            clmn = band.Columns["PARAMNAME"];
            clmn.Header.Caption = "Параметр";
        }

        public void OnAddEvent(Event e)
        {
            // если снят флажок перехвата событий - выходим
            if (!cbActive.Checked)
                return;

            DataTable events = dsEvents.Tables[0];
            DataTable prms = dsEvents.Tables[1];
            int eventID = events.Rows.Count;
            events.Rows.Add(eventID, e.TimeRaised, e.EventName);
            foreach (object prm in e.parameters)
                prms.Rows.Add(eventID, prm.ToString());

            dctEvents.Add(eventID, e);

            if ((ugEvents.Rows != null) && (ugEvents.Rows[0] != null))
            {
                //ugEvents.Selected.Rows.Clear();
                //ugEvents.Rows[0].Selected = true;
                ugEvents.ActiveRow = ugEvents.Rows[0];
            }
        }

        private void ugEvents_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            pgProps.SelectedObject = null;

            if ((ugEvents.Selected.Rows.Count == 0) ||
                (ugEvents.Selected.Rows[0] == null))
                return;

            UltraGridRow row = ugEvents.Selected.Rows[0];

            if (row.Band.Key != "relation")
                return;

            int refEvent = Convert.ToInt32(row.Cells["REFEVENT"].Value);
            DataRow[] filtered = dsEvents.Tables[1].Select(String.Format("REFEVENT = {0}", refEvent));
            if (filtered.Length == 0)
                return;

            string paramName = Convert.ToString(row.Cells["PARAMNAME"].Value);
            Event ev = dctEvents[refEvent];
            object paramObj = null;
            foreach (object obj in ev.parameters)
            {
                if (obj.ToString() == paramName)
                {
                    paramObj = obj;
                    break;
                }
            }
            if (paramObj != null)
                pgProps.SelectedObject = paramObj;
        }

        #region Перехват событий
        // оригинал где то здесь http://www.codeproject.com/csharp/#Delegates+and+Events
        // изменено:
        // 1) сборка с объектом-перехватчиком генерируется в памяти
        // 2) передача событий в объект просмотра осуществляется через делегат
        // 3) отключено освобождение делегатов событий (проверить на утечки)

        public struct Event
        {
            public DateTime TimeRaised;
            public string EventName;
            public object[] parameters;
        }

        public delegate void AddEventDelegate(Event e);

        public class EventListener
        {
            private object instance;
            private object m_target;

            private ArrayList delegates = new ArrayList();
            private AddEventDelegate handler = null;

            private bool IsApplicableEvent(string eventName)
            {
                return !((eventName.IndexOf("Mouse") >= 0) ||
                        (eventName.IndexOf("Invalidate") >= 0) ||
                        (eventName.IndexOf("Paint") >= 0)
                        );
            }

            public void RaiseEvent(string eventName, object[] args)
            {
                // если обработчик не указан - выходим
                if (handler == null)
                    return;
                // передаем событие для отображения
                Event e = new Event();
                e.EventName = eventName;
                e.TimeRaised = DateTime.Now;
                e.parameters = args;
                handler(e);
            }

            public void AttachTo(AddEventDelegate addEventHandler, object target)
            {
                m_target = target;
                handler = addEventHandler;
                Type targetType = target.GetType();

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                //Initialize the Code DOM engine
                CSharpCodeProvider csp = new CSharpCodeProvider();
                ICodeGenerator csgen = csp.CreateGenerator(sw);
                CodeGeneratorOptions cop = new CodeGeneratorOptions();

                //Add the class using statements
                CodeSnippetCompileUnit usingSystem = new CodeSnippetCompileUnit("using System;");

                csgen.GenerateCodeFromCompileUnit(usingSystem, sw, cop);
                sw.WriteLine();

                //Add the EventSpy namespace
                CodeNamespace ns = new CodeNamespace("Krista.FM.Client.Components");

                //Add the EventSpyReceiver class
                CodeTypeDeclaration ctd = new CodeTypeDeclaration();
                ctd.IsClass = true;
                ctd.Name = "EventSpyReceiver";
                ctd.TypeAttributes = TypeAttributes.Public;

                //Add the EventListener property

                CodeMemberField eventListenerField = new CodeMemberField("Krista.FM.Client.Components.EventViewer.EventListener", "m_EventListener");
                eventListenerField.Attributes = MemberAttributes.Private;
                ctd.Members.Add(eventListenerField);

                CodeMemberProperty eventListenerProperty = new CodeMemberProperty();
                eventListenerProperty.Name = "EventListener";
                eventListenerProperty.HasGet = true;
                eventListenerProperty.HasSet = true;
                eventListenerProperty.Attributes = MemberAttributes.Public;
                eventListenerProperty.Type = new CodeTypeReference("Krista.FM.Client.Components.EventViewer.EventListener");

                eventListenerProperty.GetStatements.Add(new CodeSnippetStatement("return m_EventListener;"));
                eventListenerProperty.SetStatements.Add(new CodeSnippetStatement("m_EventListener = value;"));
                ctd.Members.Add(eventListenerProperty);

                foreach (EventInfo nfo in targetType.GetEvents())
                {
                    // мышь и перерисовку обрабатывать не будем
                    if (!IsApplicableEvent(nfo.Name))
                        continue;


                    CodeMemberMethod eventMethod = new CodeMemberMethod();
                    eventMethod.Name = targetType.Name + nfo.Name;
                    eventMethod.ReturnType = new CodeTypeReference(typeof(void));
                    eventMethod.Attributes = MemberAttributes.Public;

                    Type eventHandler = nfo.EventHandlerType;
                    MethodInfo invokeMethod = eventHandler.GetMethod("Invoke");

                    foreach (ParameterInfo param in invokeMethod.GetParameters())
                    {
                        eventMethod.Parameters.Add(new CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                    }

                    StringBuilder raiseEventStatement = new StringBuilder();

                    raiseEventStatement.Append("if (EventListener != null)");
                    raiseEventStatement.Append("EventListener.RaiseEvent(\"");
                    raiseEventStatement.Append(nfo.Name);
                    raiseEventStatement.Append("\", new object[] {");

                    foreach (ParameterInfo param in invokeMethod.GetParameters())
                    {
                        if (param.Position == 0)
                            raiseEventStatement.Append(param.Name);
                        else
                            raiseEventStatement.Append(", " + param.Name);
                    }

                    raiseEventStatement.Append("});");

                    eventMethod.Statements.Add(new CodeSnippetExpression(raiseEventStatement.ToString()));

                    ctd.Members.Add(eventMethod);
                }

                ns.Types.Add(ctd);
                csgen.GenerateCodeFromNamespace(ns, sw, cop);
                sw.Close();

                //ICodeCompiler compiler = csp.CreateCompiler();
                CompilerParameters compilerParams = new CompilerParameters();

                compilerParams.GenerateExecutable = false;
                compilerParams.GenerateInMemory = true;

                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    compilerParams.ReferencedAssemblies.Add(asm.Location);
                }

                CompilerResults result = csp.CompileAssemblyFromSource(compilerParams, sb.ToString());

                if (result.Errors.Count > 0)
                {
                    StringBuilder errorList = new StringBuilder();

                    foreach (CompilerError error in result.Errors)
                    {
                        errorList.Append(error.ErrorText);
                        errorList.Append(" Line: ");
                        errorList.Append(error.Line);
                        errorList.Append("\r\n");
                    }

                    System.Windows.Forms.MessageBox.Show(errorList.ToString(), "Compilation Error",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    Assembly asm = result.CompiledAssembly;
                    instance = asm.CreateInstance("Krista.FM.Client.Components.EventSpyReceiver");
                    Type receiver = instance.GetType();

                    receiver.GetProperty("EventListener").SetValue(instance, this, null);

                    foreach (EventInfo nfo in targetType.GetEvents())
                    {
                        // мышь и перерисовку обрабатывать не будем
                        if (!IsApplicableEvent(nfo.Name))
                            continue;

                        Delegate d = Delegate.CreateDelegate(nfo.EventHandlerType, instance, targetType.Name + nfo.Name);

                        nfo.AddEventHandler(target, d);
                        delegates.Add(d);
                    }
                }

                //eventViewer = new frmEventViewer(target);
                //eventViewer.Show();
                //eventViewer.Closing += new System.ComponentModel.CancelEventHandler(eventViewer_Closing);
            }

            public void Detach()
            {
                Type targetType = m_target.GetType();

                for (int i = 0; i < targetType.GetEvents().Length; i++)
                {

                    EventInfo nfo = targetType.GetEvents()[i];

                    // мышь и перерисовку обрабатывать не будем
                    if (!IsApplicableEvent(nfo.Name))
                        continue;

                    try
                    {
                        nfo.RemoveEventHandler(m_target, (Delegate)delegates[i]);
                    }
                    catch { };
                }
                delegates.Clear();
                Type receiver = instance.GetType();
                receiver.GetProperty("EventListener").SetValue(instance, null, null);
            }
        }
        #endregion


    }
}