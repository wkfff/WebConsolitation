using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators
{
    /// <summary>
    /// Класс для компилируемых обработчиков.
    /// </summary>
    public class RuntimeCompiledHandler
    {
        private string handlerNamespaceName = "Krista.FM.Server.FinSourcePlanning.Services.IBKKUIndicators";
        private string handlerClassName = "CalculateMacros";

        private string handlerText;

        public const string handlerErrorExceptionText = "Ошибки при компиляции обработчика вычислений";

        #region Свойства
        public string HandlerNamespaceName
        {
            get { return handlerNamespaceName; }
        }

        public string HandlerClassName
        {
            get { return handlerClassName; }
        }
        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="handlerContentText">Методы в обработчике.</param>
        public RuntimeCompiledHandler(string handlerContentText)
        {
            CompositionHandlerText(handlerContentText);
        }

        private void CompositionHandlerText(string handlerContentText)
        {
            string namespaceDeclaration = string.Format("namespace {0}{1}", handlerNamespaceName, Environment.NewLine);
            string classDeclaration = string.Format("public class {0}{1}", handlerClassName, Environment.NewLine);
            handlerText = string.Format("{0}{{{1}{{{2}}}}}", namespaceDeclaration, classDeclaration, handlerContentText);
            handlerText = handlerText.Replace("&lt;", "<");
            handlerText = handlerText.Replace("&gt;", ">");
        }

        /// <summary>
        /// Компилирует обработчик.
        /// </summary>
        /// <returns>Скомпилированная сборка.</returns>
        private CompilerResults CompileCalculateHandler()
        {
            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerParameters compilerParametrs = new CompilerParameters();
            compilerParametrs.GenerateInMemory = true;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                compilerParametrs.ReferencedAssemblies.Add(assembly.Location);
            }
            string[] source = new string[1];
            source[0] = handlerText;
            CompilerResults resultAssembly = provider.CompileAssemblyFromSource(compilerParametrs, source);
            if (resultAssembly.Errors.HasErrors)
                throw new Exception(handlerErrorExceptionText);
            return resultAssembly;
        }

        /// <summary>
        /// Выполняет обработчик.
        /// </summary>
        /// <param name="parametrs">Параметры.</param>
        /// <param name="entryMethodName">Имя метода точки входа.</param>
        /// <returns>Результат выполнения.</returns>
        public object ExecuteHandler(object[] parametrs, string entryMethodName)
        {
            CompilerResults resultAssembly = CompileCalculateHandler();
            Type calculatorType = resultAssembly.CompiledAssembly.GetType(string.Format("{0}.{1}", handlerNamespaceName, handlerClassName));
            object calculator = Activator.CreateInstance(calculatorType);
            MethodInfo mi = calculatorType.GetMethod(entryMethodName);
            return  mi.Invoke(calculator, parametrs);
        }
    }
}
