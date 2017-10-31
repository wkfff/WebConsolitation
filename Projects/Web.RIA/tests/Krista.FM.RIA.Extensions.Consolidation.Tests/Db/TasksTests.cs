using System;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Db
{
    [TestFixture]
    public class TasksTests
    {
        [SetUp]
        public void SetUp()
        {
            NHibernateHelper.SetupNHibernate("Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV");
        }

        [Ignore]
        [Test]
        public void Test1()
        {
            string secretKey = "MySecretKey";
            string salt = "123";
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] preHash = System.Text.Encoding.UTF32.GetBytes(secretKey + salt);
            byte[] hash = sha.ComputeHash(preHash);
            string password = System.Convert.ToBase64String(hash, 0, 18);

            var query = NHibernateSession.Current.CreateSQLQuery("select NAME from sys.obj$ where owner# = 39");
            var result = query.List();

            System.Collections.Generic.Dictionary<string, string> data =
                new System.Collections.Generic.Dictionary<string, string>();
            System.Collections.Generic.List<string> collisions =
                new System.Collections.Generic.List<string>();
            int maxLength = 0;
            foreach (var name in result)
            {
                var sourceString = Convert.ToString(name);
                byte[] bytes = System.Text.Encoding.UTF32.GetBytes(sourceString);
                byte[] hashBytes = sha.ComputeHash(bytes);
                string hashString = Convert.ToBase64String(hashBytes, 0, 18);

                if (hashString.Length > maxLength)
                {
                    maxLength = hashString.Length;
                }

                if (data.ContainsKey(hashString))
                {
                    collisions.Add(data[hashString] + " -> " + sourceString);
                }
                else
                {
                    data.Add(hashString, sourceString);
                }
            }
        }

        [Ignore]
        [Test]
        public void Test2()
        {
            string secretKey = "MySecretKey";
            string salt = "123";
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] preHash = System.Text.Encoding.UTF32.GetBytes(secretKey + salt);
            byte[] hash = sha.ComputeHash(preHash);
            string password = System.Convert.ToBase64String(hash, 0, 18);

            var query = NHibernateSession.Current.CreateSQLQuery("select NAME from sys.obj$ where owner# = 39");
            var result = query.List();

            var data = new System.Collections.Generic.Dictionary<string, string>();
            var collisions = new System.Collections.Generic.List<string>();
            foreach (var name in result)
            {
                var sourceString = Convert.ToString(name);
                if (sourceString.Length > 20)
                {
                    byte[] bytes = System.Text.Encoding.UTF32.GetBytes(sourceString);
                    byte[] hashBytes = sha.ComputeHash(bytes);
                    string hashString = Convert.ToBase64String(hashBytes, 0, 5).Substring(0, 7);

                    string key = "{0}${1}${2}".FormatWith(
                        sourceString.Substring(0, 20),
                        hashString,
                        0);

                    if (data.ContainsKey(key))
                    {
                        collisions.Add(data[key] + " -> " + sourceString);
                    }
                    else
                    {
                        data.Add(key, sourceString);
                    }
                }
            }
        }

        [Ignore]
        [Test]
        public void Test3Speed()
        {
            var query = NHibernateSession.Current.CreateSQLQuery("select NAME from sys.obj$");
            var result = query.List();

            var sha = System.Security.Cryptography.MD5.Create();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var name in result)
            {
                var sourceString = Convert.ToString(name);
                byte[] bytes = System.Text.Encoding.UTF32.GetBytes(sourceString);
                byte[] hashBytes = sha.ComputeHash(bytes);
            }

            sw.Stop();
            var s = sw.ElapsedMilliseconds;
        }

        [Ignore]
        [Test]
        public void SelectSubjectTreeTest()
        {
            var list = new SubjectTreeRepository()
                .GetTree(1);

            var tree = list.ByHierarchy(t => t.ParentId == null, (parent, child) => parent.ID == child.ParentId).ToList();
        }

        [Ignore]
        [Test]
        public void GetTaskViewModelTest()
        {
            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            
            var model = new TaskService(taskRepository)
                .GetTaskViewModel(104);
        }
    }
}
