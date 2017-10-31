using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.Client.ViewObjects.MessagesUI.Tests
{
    [TestFixture]
    public class MessageManagerTest
    {
        private IScheme scheme;

        [SetUp]
        void SetUp()
        {
            // connect to scheme

            // Настройка среды .NET Remoting
            RemotingConfiguration.Configure(@"y:\Debug\Krista.FM.Client.Workplace\Krista.FM.Client.Workplace.exe.config ", false);

            LogicalCallContextData.SetAuthorization("fmadmin");
            ClientSession.CreateSession(SessionClientType.WindowsNetClient);

            string url = String.Format("{0}://{1}:{2}/{3}", "tcp", "localhost", "8008", "FMServer/Server.rem");

            // Создаем прозрачный прокси объект для взаимодействия с сервером
            IServer proxy = (IServer)Activator.GetObject(typeof(IServer), url);

            try
            {
                // Делаем первое обращение к удаленному серверу для проверки соединения(доступности сервера)
                proxy.Activate();
            }
            catch
            {
                return;
            }

            string err = String.Empty;
            proxy.Connect(out scheme, AuthenticationType.atWindows, "krista2\\tsvetkov",
                                PwdHelper.GetPasswordHash(PwdHelper.GetPasswordHash("")), ref err);
        }

        [Test]
        public void Connect_to_scheme_test()
        {
            Assert.NotNull(scheme);
        }

        [Ignore]
        [Test]
        public void Get_nhibernate_session_test()
        {
            object session = LogicalCallContextData.GetContext()["NHibernateCallSession"];
            Assert.NotNull(session);
        }

        [Ignore]
        [Test]
        public void Get_Messages_for_user()
        {
            // arrange
            IMessageManager messageManager = scheme.MessageManager;
            //act
            IList<MessageDTO> messages =  messageManager.ReceiveMessages(100);
            // assert
            Assert.AreEqual(messages.Count, 8);
        }

        [Test]
        [Ignore]
        public void Get_not_null_message_attachment_test()
        {
            // arrange
            IMessageManager messageManager = scheme.MessageManager;
            // act
            MessageAttachmentDTO messageAttachmentDto = messageManager.GetMessageAttachment(18);
            // assert
            Assert.IsNotNull(messageAttachmentDto);

            // open attachment
            string path = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), messageAttachmentDto.DocumentFileName);
            File.WriteAllBytes(path, messageAttachmentDto.Document);
            Process.Start(path);
        }

        [Ignore]
        [Test]
        public void Create_new_message_witch_attachment()
        {
            string documentName = "image";
            string documentFileName = "x_4a23b1998e.jpg";
            byte[] document = StreamFile(@"d:\Files\images\x_4a23b1998e.jpg");

            // arrange
            IMessageManager messagesManager = scheme.MessageManager;
            // action
            /*messagesManager.SendMessage("Новое побщение от пользователя tsvetkov", DateTime.Now, DateTime.Now.AddDays(10), MessageType.AdministratorMessage,
                                        MessageStatus.New,
                                        MessageImportance.Regular, 100, null, null, true, documentName, documentFileName,
                                        document);*/
        }

        private byte[] StreamFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] imageData = new byte[fs.Length];
            fs.Read(imageData, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return imageData;
        }
    }
}
