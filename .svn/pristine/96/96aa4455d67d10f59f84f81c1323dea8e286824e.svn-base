using System;
using System.Diagnostics;
using System.IO;
using Krista.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using NUnit.Framework;

namespace Krista.FM.Common.Tests.IoC
{
    [TestFixture]
    public class LogInterceptorTests
    {
        private UnityContainer container;

        [SetUp]
        public void SetUp()
        {
            container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.Configure<Interception>()
                .AddPolicy("logging")
                .AddMatchingRule(new LogMatchingRule())
                .AddCallHandler<LogInterceptor>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor());
            
            container.RegisterType<TestClass, TestClass>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<VirtualMethodInterceptor>());

            container.RegisterType<TestLevelsClass, TestLevelsClass>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<VirtualMethodInterceptor>());

            container.RegisterType<TestAssemblyLevelsClass, TestAssemblyLevelsClass>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<VirtualMethodInterceptor>());
        }

        [Test]
        public void LogTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestClass>();
            var result = obj.Sum(1, 0.5);

            Assert.AreEqual(new decimal(1.5), result);

            Assert.AreEqual(
@"Krista.FM.Common Verbose: 0 : Sum(a:[1] b:[0,5])
Krista.FM.Common Verbose: 0 : Sum Returns:[1,5]
", 
 sw.ToString());
        }

        [Test]
        public void LogVoidResultTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestClass>();
            obj.ReturnVoid();

            var log = sw.ToString();

            Assert.True(log.Contains("ReturnVoid Returns:[]"));
        }

        [Test]
        public void LogExceptionTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestClass>();
            try
            {
                obj.ThrowException();
            }
            catch
            {
            }

            var log = sw.ToString();

            Assert.True(log.Contains("Error: 0 : ThrowException ThrowException:[---------- System.Exception ----------"));
            Assert.True(log.Contains("Message=Error message"));
        }

        [Test]
        public void EntrySuccessLevelsTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestLevelsClass>();
            obj.Dummy();

            Assert.AreEqual(
@"Krista.FM.Common Information: 0 : Dummy()
Krista.FM.Common Warning: 0 : Dummy Returns:[]
",
 sw.ToString());
        }

        [Test]
        public void ExceptionLevelsTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestLevelsClass>();
            try
            {
                obj.ThrowException();
            }
            catch
            {
            }

            var log = sw.ToString();

            Assert.True(log.Contains("Information: 0 : ThrowException ThrowException:[---------- System.Exception ----------"));
            Assert.True(log.Contains("Message=Error message"));
        }

        [Test]
        public void EnterOffLevelTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestLevelsClass>();
            obj.EnterOff();

            Assert.AreEqual(
@"Krista.FM.Common Error: 0 : EnterOff Returns:[]
",
 sw.ToString());
        }

        [Test]
        public void AssemblyLevelTest()
        {
            StringWriter sw = SetupTrace();

            var obj = container.Resolve<TestAssemblyLevelsClass>();
            try
            {
                obj.ThrowException();
            }
            catch
            {
            }

            var log = sw.ToString();

            Assert.False(log.Contains("Krista.FM.Common Verbose: 0 : Dummy()"));
            Assert.True(log.Contains("Error: 0 : ThrowException ThrowException:[---------- System.Exception ----------"));
            Assert.True(log.Contains("Message=Error message"));
        }

        private static StringWriter SetupTrace()
        {
            StringWriter sw = new StringWriter();

            var logListener = new TextWriterTraceListener(sw);
            Trace.Listeners.Add(logListener);

            var source = KristaDiagnostics.GetTraceSource("Krista.FM.Common");
            source.Switch.Level = SourceLevels.All;
            source.Listeners.Add(logListener);

            return sw;
        }

        [Log]
        public class TestClass
        {
            public virtual decimal Sum(int a, double b)
            {
                return Convert.ToDecimal(a + b);
            }

            public virtual void ThrowException()
            {
                throw new Exception("Error message");
            }

            public virtual void ReturnVoid()
            {
            }
        }

        public class TestLevelsClass
        {
            [Log(EntryLevel = TraceLevel.Info, SuccessLevel = TraceLevel.Warning)]
            public virtual void Dummy()
            {
            }

            [Log(EntryLevel = TraceLevel.Off, SuccessLevel = TraceLevel.Error)]
            public virtual void EnterOff()
            {
            }

            [Log(EntryLevel = TraceLevel.Off, ExceptionLevel = TraceLevel.Info)]
            public virtual void ThrowException()
            {
                throw new Exception("Error message");
            }
        }

        public class TestAssemblyLevelsClass
        {
            public virtual void ThrowException()
            {
                throw new Exception("Error message");
            }
        }
    }
}
