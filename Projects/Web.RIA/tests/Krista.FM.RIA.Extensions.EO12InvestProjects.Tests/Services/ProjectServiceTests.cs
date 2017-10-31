using System;
using System.Collections.Generic;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Tests.Services
{
    [TestFixture]
    public class ProjectServiceTests
    {
        private MockRepository mocks;
        private ILinqRepository<D_InvProject_Reestr> projectsRepository;
        private ILinqRepository<FX_InvProject_Part> invProjPartRepository;
        private ILinqRepository<F_InvProject_Data> factRepository;
        private ILinqRepository<D_InvProject_Vizual> filesRepository;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            projectsRepository = mocks.DynamicMock<ILinqRepository<D_InvProject_Reestr>>();
            invProjPartRepository = mocks.DynamicMock<ILinqRepository<FX_InvProject_Part>>();
            factRepository = mocks.DynamicMock<ILinqRepository<F_InvProject_Data>>();
            filesRepository = mocks.DynamicMock<ILinqRepository<D_InvProject_Vizual>>();
        }

        [Test]
        public void GetInitialProjectModelTest()
        {
            var projectService = new ProjectService(projectsRepository, invProjPartRepository, factRepository, filesRepository);
            var initialProjectModel = projectService.GetInitialProjectModel(InvProjPart.Part1);
            Assert.NotNull(initialProjectModel);
            Assert.NotNull(initialProjectModel.Name);
            Assert.NotNull(initialProjectModel.RefBeginDateVal);
            Assert.NotNull(initialProjectModel.RefEndDateVal);
            Assert.IsTrue(initialProjectModel.RefStatusId == (int)InvProjStatus.Edit);
            Assert.IsTrue(initialProjectModel.RefPartId == (int)InvProjPart.Part1);
            
            initialProjectModel = projectService.GetInitialProjectModel(InvProjPart.Part2);
            Assert.NotNull(initialProjectModel);
            Assert.NotNull(initialProjectModel.Name);
            Assert.Null(initialProjectModel.RefBeginDateVal);
            Assert.Null(initialProjectModel.RefEndDateVal);
            Assert.IsTrue(initialProjectModel.RefStatusId == (int)InvProjStatus.Edit);
            Assert.IsTrue(initialProjectModel.RefPartId == (int)InvProjPart.Part2);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException), UserMessage = "Проект не найден.")]
        public void GetProjectTest()
        {
            const int ProjectId = -1;
            Expect.Call(projectsRepository.FindOne(ProjectId)).Return(null).Repeat.Once();
            mocks.ReplayAll();
            var projectService = new ProjectService(projectsRepository, invProjPartRepository, factRepository, filesRepository);
            var project = projectService.GetProject(ProjectId);
            Assert.Fail("а должна была возникнуть ошибка...");
        }

        [Ignore]
        [Test]
        public void GetProjectsTableTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
                "Oracle",
                "10");

            var projectsRepositoryReal = new NHibernateLinqRepository<D_InvProject_Reestr>();
            var projectService = new ProjectService(projectsRepositoryReal, invProjPartRepository, factRepository, filesRepository);

            var model = projectService.GetProjectsTable(InvProjPart.Part1, new bool[] { true, true, true });
            Assert.NotNull(model);
            Assert.GreaterOrEqual(model.Count, 0);
            Console.Write(String.Format("Выбрано записей: {0}", model.Count));

            model = projectService.GetProjectsTable(InvProjPart.Part1, new bool[] { false, false, false });
            Assert.NotNull(model);
            Assert.IsTrue(model.Count == 0);

            try
            {
                model = projectService.GetProjectsTable(InvProjPart.Part1, new bool[] { true, true });
                Assert.NotNull(model);
                model = projectService.GetProjectsTable(InvProjPart.Part1, null);
                Assert.NotNull(model);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}