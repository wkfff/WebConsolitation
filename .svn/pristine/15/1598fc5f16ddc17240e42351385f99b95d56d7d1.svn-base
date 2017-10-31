using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.FO41.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.FO41.Tests
{
    [TestFixture]
    public class ExportServiceTests
    {
        private MockRepository mocks;
        private IAppPrivilegeService requestsRepository;
        private IAppFromOGVService requestsFormOGVRepository;
        private IFactsService factsRepository;
        private ILinqRepository<T_Doc_ApplicationOGV> docsOGVRepository;
        private ILinqRepository<T_Doc_ApplicationOrg> docsOrgRepository;
        private IRepository<D_Marks_NormPrivilege> normRepository;
        private ILinqRepository<F_Marks_Privilege> factsEstimateRepository;
        private ICategoryTaxpayerService categoryRepository;

        [Test]
        public void CanExportForOGVTest()
        {
            mocks = new MockRepository();
            requestsRepository = mocks.DynamicMock<IAppPrivilegeService>();
            requestsFormOGVRepository = mocks.DynamicMock<IAppFromOGVService>();
            factsRepository = mocks.DynamicMock<IFactsService>();
            docsOGVRepository = mocks.DynamicMock<ILinqRepository<T_Doc_ApplicationOGV>>();
            docsOrgRepository = mocks.DynamicMock<ILinqRepository<T_Doc_ApplicationOrg>>();
            normRepository = mocks.DynamicMock<IRepository<D_Marks_NormPrivilege>>();
            factsEstimateRepository = mocks.DynamicMock<ILinqRepository<F_Marks_Privilege>>();
            categoryRepository = mocks.DynamicMock<ICategoryTaxpayerService>();

            const int appFromOGVId = 1;
            const int applicationId = 1;

            var ogv = new D_OMSU_ResponsOIV
                          {
                              ID = 1,
                              Code = 1,
                              Name = "OGVName",
                              Role = "ОГВ",
                              ShortName = "OGVShort",
                              UserID = 1
                          };

            var category = new D_Org_CategoryTaxpayer
                               {
                                   ID = 1,
                                   Name = "Category"
                               };

            var state = new FX_State_ApplicOGV
                            {
                                ID = 1,
                                Name = "Created"
                            };

            var period = new FX_Date_YearDayUNV
                             {
                                 ID = 20110000
                             };

            var appFromOGV = new D_Application_FromOGV
                                 {
                                     ID = appFromOGVId,
                                     Executor = "Executor",
                                     RowType = 0,
                                     RefOGV = ogv,
                                     RefOrgCategory = category,
                                     RefStateOGV = state,
                                     RefYearDayUNV = period,
                                     RequestDate = DateTime.Today
                                 };

            var region = new B_Regions_Bridge
            {
                Name = "Region"
            };

            var application = new D_Application_Privilege
            {
                ID = applicationId,
                Executor = "Executor",
                RequestDate = DateTime.Today,
                RefOrgCategory = category,
                RefOrgPrivilege =  new D_Org_Privilege
                          {
                              ID = 1,
                              Name = "Org",
                              RefBridgeRegions = region
                          },
                RefYearDayUNV = period,
                RefApplicOGV = null,
                RefStateOrg = new FX_State_ApplicOrg { ID = 1 },
                RowType = 0
            };

            Expect.Call(requestsFormOGVRepository.FindOne(appFromOGVId)).Return(appFromOGV);

            var mark = new D_Marks_Privilege
            {
                ID = 1,
                Code = 1,
                NumberString = "1",
                Name = "Mark",
                RefOKEI = new D_Units_OKEI
                {
                    ID = 383,
                    Name = "okei",
                    Designation = "okei_Designation",
                    Symbol = "okei_Symbol",
                    Code = 2
                }
            };

            Expect.Call(factsRepository.GetFactsForOGV(appFromOGVId)).Return(new List<F_Marks_DataPrivilege>
            {
                new F_Marks_DataPrivilege { ID = 1, RefApplication = application, RefMarks = mark, PreviousFact = -1, Fact = 0, Estimate = 1, Forecast = 2},
                new F_Marks_DataPrivilege { ID = 2, RefApplication = application, RefMarks = mark, PreviousFact = -11, Fact = 10, Estimate = 11, Forecast = 12},
                new F_Marks_DataPrivilege { ID = 3, RefApplication = application, RefMarks = mark, PreviousFact = -21, Fact = 20, Estimate = 21, Forecast = 22},
            });

            var doc = new T_Doc_ApplicationOGV
            {
                RefApplicOGV = appFromOGV,
                Name = "Doc",
                Note = "Note",
                Executor = "Executor",
                DateDoc = DateTime.Today
            };

            Expect.Call(docsOGVRepository.FindAll())
                .Return(new List<T_Doc_ApplicationOGV>
                            {
                                doc,
                                doc
                            }.AsQueryable());

            Expect.Call(requestsRepository.FindAll()).Return(new List<D_Application_Privilege>
                                                                 {
                                                                     application,
                                                                     application,
                                                                     application
                                                                 }.AsQueryable());
            
            mocks.ReplayAll();

            var stream = new ExportService(
                requestsRepository, 
                requestsFormOGVRepository, 
                factsRepository, 
                docsOrgRepository, 
                docsOGVRepository, 
                normRepository, 
                factsEstimateRepository,
                categoryRepository
                ).ExportForOGV(appFromOGVId);

            mocks.VerifyAll();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            /*using (var file = File.Create(@"x:\Exports\testOGV.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

        [Test]
        public void CanExportForTaxpayerTest()
        {
            mocks = new MockRepository();
            requestsRepository = mocks.DynamicMock<IAppPrivilegeService>();
            requestsFormOGVRepository = mocks.DynamicMock<IAppFromOGVService>();
            factsRepository = mocks.DynamicMock<IFactsService>();
            docsOGVRepository = mocks.DynamicMock<ILinqRepository<T_Doc_ApplicationOGV>>();
            docsOrgRepository = mocks.DynamicMock<ILinqRepository<T_Doc_ApplicationOrg>>();
            normRepository = mocks.DynamicMock<IRepository<D_Marks_NormPrivilege>>();
            factsEstimateRepository = mocks.DynamicMock<ILinqRepository<F_Marks_Privilege>>();

            const int applicationId = 29;

            var category = new D_Org_CategoryTaxpayer
                               {
                                   Name = "Category Обобщенные сведения для расчета бюджетной и социальной эффективности предоставляемых (планируемых к предоставлению) налоговых льгот Category Обобщенные сведения для расчета бюджетной и социальной эффективности предоставляемых (планируемых к предоставлению) налоговых льгот"
                               };

            var region = new B_Regions_Bridge
                             {
                                 Name = "Region"
                             };

            var org = new D_Org_Privilege
                          {
                              Name = "Org",
                              RefBridgeRegions = region
                          };

            var period = new FX_Date_YearDayUNV
                             {
                                 ID = 20110000
                             };

            var state = new FX_State_ApplicOrg
                            {
                                ID = 1,
                                Name = "Created"
                            };

            var application = new D_Application_Privilege
                                  {
                                      ID = applicationId,
                                      Executor = "Executor",
                                      RequestDate = DateTime.Today,
                                      RefOrgCategory = category,
                                      RefOrgPrivilege = org,
                                      RefYearDayUNV = period,
                                      RefApplicOGV = null,
                                      RefStateOrg = state,
                                      RowType = 0
                                  };

            Expect.Call(requestsRepository.Get(applicationId)).Return(application);

            var mark = new D_Marks_Privilege
                           {
                               ID = 1,
                               Code = 1,
                               NumberString = "1",
                               Name = "Mark \r\n в т.ч. трям",
                               RefOKEI = new D_Units_OKEI
                                             {
                                                 ID = 383,
                                                 Name = "okei",
                                                 Designation = "okei_Designation",
                                                 Symbol = "okei_Symbol",
                                                 Code = 2
                                             }
                           };

            Expect.Call(factsRepository.GetFactsForOrganization(applicationId))
                .Return(new List<F_Marks_DataPrivilege>
                            {
                                new F_Marks_DataPrivilege 
                                    {
                                        ID = 1,
                                        RefApplication = application,
                                        RefMarks = mark,
                                        PreviousFact = -1,
                                        Fact = 0,
                                        Estimate = 1,
                                        Forecast = 2
                                    },
                                new F_Marks_DataPrivilege
                                    {
                                        ID = 2,
                                        RefApplication = application,
                                        RefMarks = mark,
                                        PreviousFact = -11,
                                        Fact = 10,
                                        Estimate = 11,
                                        Forecast = 12
                                    },
                                new F_Marks_DataPrivilege
                                    {
                                        ID = 3,
                                        RefApplication = application,
                                        RefMarks = mark,
                                        PreviousFact = -21,
                                        Fact = 20,
                                        Estimate = 21,
                                        Forecast = 22
                                    },
                            });

            var doc = new T_Doc_ApplicationOrg
                          {
                              RefApplicOrg = application,
                              Name = "Doc",
                              Note = "Note",
                              Executor = "Executor",
                              DateDoc = DateTime.Today
                          };

            Expect.Call(docsOrgRepository.FindAll())
                .Return(new List<T_Doc_ApplicationOrg>
                            {
                                doc,
                                doc
                            }.AsQueryable());

            mocks.ReplayAll();

            var stream =
                new ExportService(
                    requestsRepository, 
                    requestsFormOGVRepository, 
                    factsRepository, 
                    docsOrgRepository,
                    docsOGVRepository, 
                    normRepository, 
                    factsEstimateRepository,
                    categoryRepository)
                    .ExportForTaxpayer(applicationId);

            mocks.VerifyAll();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            /*using (var file = File.Create(@"x:\Exports\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

        [Test]
        public void CanExportResultTest()
        {
            mocks = new MockRepository();
            requestsFormOGVRepository = mocks.DynamicMock<IAppFromOGVService>();
            factsRepository = mocks.DynamicMock<IFactsService>();
            normRepository = mocks.DynamicMock<IRepository<D_Marks_NormPrivilege>>();
            factsEstimateRepository = mocks.DynamicMock<ILinqRepository<F_Marks_Privilege>>();

            const int appFromOGVId = 1;
            const int sourceId = 1;

            var category = new D_Org_CategoryTaxpayer
            {
                ID = 1,
                Name = "CategoryCategoryCategoryCategoryCategoryCategoryCategoryCategoryCategory Длиннноооооеее такооооое оооочень наименовааааниеее кааатееегориииии....",
                CorrectIndex = (decimal?)0.87
            };

            const int year = 2011;

            var period = new FX_Date_YearDayUNV
            {
                ID = year * 10000
            };

            var appFromOGV = new D_Application_FromOGV
            {
                ID = appFromOGVId,
                RefOrgCategory = category,
                RefYearDayUNV = period
            };

            var normMin = new D_Marks_NormPrivilege
            {
                ID = 1,
                PreviousFact = -1,
                Fact = 0,
                Estimate = 1,
                Forecast = 2,
                Name = "Величина прожиточного минимума в расчете на душу населения",
                Symbol = "MIN",
                Year = year
            };

            Expect.Call(requestsFormOGVRepository.FindOne(appFromOGVId)).Return(appFromOGV);
            Expect.Call(normRepository.GetAll()).Return(new List<D_Marks_NormPrivilege> {normMin});

            var typeMark = new FX_FX_TypeMark
            {
                ID = 1,
                Code = 4
            };

            var okei = new D_Units_OKEI
            {
                ID = 383,
                Name = "okei",
                Designation = "okei_Designation",
                Symbol = "okei_Symbol",
                Code = 2
            };

            var mark = new D_Marks_Privilege
            {
                ID = 1,
                Code = 1,
                RefTypeMark = typeMark,
                NumberString = "1",
                Name = "Показатель для таблицы итоговые показатели - тестовый длинный вариант",
                RefOKEI = okei
            };

            Expect.Call(factsEstimateRepository.FindAll()).Return(new List<F_Marks_Privilege>
                {
                    new F_Marks_Privilege { ID = 1, RefMarks = mark, PreviousFact = -1, Fact = 0, Estimate = 1, Forecast = 2, RefCategory = category, RefYearDayUNV = period, SourceID = sourceId},
                    new F_Marks_Privilege { ID = 1, RefMarks = mark, PreviousFact = -1, Fact = 0, Estimate = 1, Forecast = 2, RefCategory = category, RefYearDayUNV = period, SourceID = sourceId},
                    new F_Marks_Privilege { ID = 2, RefMarks = mark, PreviousFact = -11, Fact = 10, Estimate = 11, Forecast = 12, RefCategory = category, RefYearDayUNV = period, SourceID = sourceId},
                    new F_Marks_Privilege { ID = 3, RefMarks = mark, PreviousFact = -21, Fact = 20, Estimate = 21, Forecast = 22, RefCategory = category, RefYearDayUNV = period, SourceID = sourceId}
                }.AsQueryable());

            mocks.ReplayAll();

            var stream = new ExportService(
                requestsRepository, 
                requestsFormOGVRepository, 
                factsRepository, 
                docsOrgRepository, 
                docsOGVRepository, 
                normRepository, 
                factsEstimateRepository,
                categoryRepository)
                .ExportResult(appFromOGVId, sourceId);

            mocks.VerifyAll();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            /*using (var file = File.Create(@"x:\Exports\testResult.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }
    }
}
