using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Server.MessagesManager.Tests
{
    [TestFixture]
    public class MessageManagerTest
    {
        private MessageManager messagesManager;
        private MessageRepository messageRepository;
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();

            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                //@"Data Source=tsvetkov\sql2012;User ID=DV;Initial Catalog=DV;Password=DV;Persist Security Info=True",
                @"Data Source=dv;User ID=dv;Password=dv;Persist Security Info=True",
                "ORACLE",
                "10");

            ObjectRepository objectRepository = new ObjectRepository();
            MembershipsRepository membershipsRepository = new MembershipsRepository();
            PermissionRepository permissionRepository = new PermissionRepository();
            messageRepository = new MessageRepository(permissionRepository, objectRepository, membershipsRepository);

            IMessageExchangeProtocol log = mocks.DynamicMock<IMessageExchangeProtocol>();

            messagesManager = new MessageManager(
                log,
                new SimpleUnitOfWorkFactory(), 
                messageRepository, 
                new NHibernateRepository<Users>(), 
                membershipsRepository, 
                new NHibernateLinqRepository<MessageAttachment>(), 
                permissionRepository, 
                objectRepository);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_new_nessage_null_subject()
        {
            MessageWrapper message = new MessageWrapper();
            message.Subject = null;
            message.DateTimeOfActual = DateTime.Now.AddDays(10);
            message.RefUserSender = 1;
            message.RefGroupRecipient = 100;
            messagesManager.SendMessage(message);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_new_nessage_null_userreceived_and_groupreceived_and_not_sendallonline()
        {
            MessageWrapper message = new MessageWrapper();
            message.Subject = null;
            message.DateTimeOfActual = DateTime.Now.AddDays(10);
            message.RefUserRecipient = 100;
            messagesManager.SendMessage(message);
        }

        [Ignore]
        [Test]
        public void Exist_message_in_table()
        {
            // arrange
            // act
            int count = messageRepository.FindAll().Count();
            // assert
            Assert.AreNotEqual(count, 0);
        }

        [Ignore]
        [Test]
        public void Create_new_message_for_user_test()
        {
            // arrange
            int count = messageRepository.FindAll().Count();
            // action
            MessageWrapper message = new MessageWrapper();
            message.Subject = "Через 10 минут заменяю многомерную БД  Анализ и планирование_Подопытная_Oracle.";
            message.DateTimeOfActual = DateTime.Now.AddDays(10);
            message.RefUserSender = 1;
            message.RefUserRecipient = 100;
            messagesManager.SendMessage(message);

            int count2 = messageRepository.FindAll().Count();
            // assert
            Assert.AreEqual(count2 - count, 1);
        }

        [Ignore]
        [Test]
        public void Create_new_message_for_group_test()
        {
            // arrange
            int count = messageRepository.FindAll().Count();
            // action
            MessageWrapper message = new MessageWrapper();
            message.Subject = "Через 10 минут заменяю многомерную БД  Анализ и планирование_Подопытная_Oracle.";
            message.DateTimeOfActual = DateTime.Now.AddDays(10);
            message.RefUserSender = 1;
            message.RefGroupRecipient = 1;
            messagesManager.SendMessage(message);
            int count2 = messageRepository.FindAll().Count();
            // assert
            Assert.AreEqual(count2 - count, 2);
        }

        [Ignore]
        [Test]
        public void Create_new_message_for_all_test()
        {
            // arrange
            int count = messageRepository.FindAll().Count();
            // action
            MessageWrapper message = new MessageWrapper();
            message.Subject = "Через 10 минут заменяю многомерную БД  Анализ и планирование_Подопытная_Oracle.";
            message.DateTimeOfActual = DateTime.Now.AddDays(10);
            message.RefUserSender = 1;
            message.SendAll = true;
            messagesManager.SendMessage(message);
            int count2 = messageRepository.FindAll().Count();
            // assert
            Assert.AreEqual(count2 - count, 2);
        }

        [Ignore]
        [Test]
        public void Create_new_message_witch_attachment()
        {
            MessageAttachmentWrapper attachmentWrapper = new MessageAttachmentWrapper();
            attachmentWrapper.DocumentFileName = "x_4a23b1998e.jpg";
            attachmentWrapper.Document = StreamFile(@"d:\Files\images\x_4a23b1998e.jpg");

            // arrange
            int count = messageRepository.FindAll().Count();
            // action
            MessageWrapper message = new MessageWrapper();
            message.Subject = "Через 10 минут заменяю многомерную БД  Анализ и планирование_Подопытная_Oracle.";
            message.DateTimeOfActual = DateTime.Now.AddDays(10);
            message.RefUserSender = 1;
            message.SendAll = true;
            message.RefMessageAttachment = attachmentWrapper;
            messagesManager.SendMessage(message);
            int count2 = messageRepository.FindAll().Count();
            // assert
            Assert.AreEqual(count2 - count, 2);
        }

        [Ignore]
        [Test]
        public void Receive_messages_for_user_with_right()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //act
            int countMessage = messagesManager.ReceiveMessages(300).Count;
            stopWatch.Stop();

            TimeSpan timeSpan = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
            timeSpan.Milliseconds / 10);

            // assert 
            Assert.AreEqual(countMessage, 13);
        }

        [Ignore]
        [Test]
        public void Receive_messages_for_user_without_right()
        {
            //act
            int countMessage = messagesManager.ReceiveMessages(111).Count;

            // assert 
            Assert.AreEqual(countMessage, 0);
        }

        [Ignore]
        [Test]
        public void Change_message_status_test()
        {
            MessageDTO message = messagesManager.ReceiveMessages(100).FirstOrDefault();
            MessageStatus status1 = message.MessageStatus;
            //act
            messagesManager.UpdateMessage(message.ID, MessageStatus.Read);
            // assert 
            message = messagesManager.ReceiveMessages(100).FirstOrDefault();
            MessageStatus status2 = message.MessageStatus;

            Assert.AreNotEqual(status1, status2);
        }

        [Ignore]
        [Test]
        public void Delete_obsolete_messages()
        {
            // act
            messagesManager.RemoveObsoleteMessage();
            // assert
            int obsoleteMessages = messageRepository.FindAll().Where(message => message.DateTimeOfActual < DateTime.Now).Count();
            Assert.AreEqual(obsoleteMessages, 0);
        }

        [Test]
        [Ignore]
        public void Delete_message()
        {
            // act 
            messagesManager.DeleteMessage(24);
            // assert
            var message = messageRepository.FindOne(24);
            Assert.AreEqual((int)MessageStatus.Deleted, message.MessageStatus);
        }

        [Ignore]
        [Test]
        public void Mapping_test()
        {
            // act
            MessageDTO message = messagesManager.ReceiveMessages(100).FirstOrDefault();
            // assert
            Assert.AreEqual(message.RefUserSender, "FMADMIN");
        }

        [Test]
        [Ignore]
        public void Get_not_null_message_attachment_test()
        {
            // act
            MessageAttachmentDTO messageAttachmentDto = messagesManager.GetMessageAttachment(23);
            // assert
            Assert.IsNotNull(messageAttachmentDto);

            // open attachment
            /*string path = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), messageAttachmentDto.DocumentFileName);
            File.WriteAllBytes(path, messageAttachmentDto.Document);
            Process.Start(path);*/
        }

        [TestAttribute]
        [Ignore]
        public void Get_null_message_attachment_test()
        {
            // act
            MessageAttachmentDTO messageAttachmentDto = messagesManager.GetMessageAttachment(24);
            // assert
            Assert.IsNull(messageAttachmentDto);
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
