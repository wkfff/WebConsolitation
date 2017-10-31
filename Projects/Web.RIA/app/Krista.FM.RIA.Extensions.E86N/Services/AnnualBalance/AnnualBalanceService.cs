using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance
{
    public class AnnualBalanceService : NewRestService, IAnnualBalanceService
    {
        #region IAnnualBalanceService Members

        public RestResult F0503730Read(int recId, int section)
        {
            СheckDocContent(recId);
            
            var data = from p in GetItems<F_Report_BalF0503730>()
                       where (p.RefParametr.ID == recId) && (p.Section == section)
                       select new
                       {
                           p.ID,
                           p.Name,
                           RefParametr = p.RefParametr.ID,
                           LineCode = p.lineCode,
                           TargetFundsBegin = p.targetFundsBegin,
                           TargetFundsEnd = p.targetFundsEnd,
                           ServicesBegin = p.servicesBegin,
                           ServicesEnd = p.servicesEnd,
                           TemporaryFundsBegin = p.temporaryFundsBegin,
                           TemporaryFundsEnd = p.temporaryFundsEnd,
                           p.StateTaskFundStartYear,
                           p.StateTaskFundEndYear,
                           p.RevenueFundsStartYear,
                           p.RevenueFundsEndYear,
                           TotalStartYear = p.totalStartYear,
                           TotalEndYear = p.totalEndYear,
                           NumberOffBalance = p.lineCode == "280" && section == (int) F0503130F0503730Details.Information 
                                ? "30"
                                : p.lineCode.Substring(0, 2)
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult F0503730Save(string data, int recId, int section)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, recId, section);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = new F_Report_BalF0503730
                    {
                        ID = Convert.ToInt32(dataUpdate[F0503730Fields.ID.ToString()]),
                        Name = Convert.ToString(dataUpdate[F0503730Fields.Name.ToString()]),
                        lineCode = Convert.ToString(dataUpdate[F0503730Fields.LineCode.ToString()]),
                        targetFundsBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.TargetFundsBegin.ToString(), Decimal.Zero),
                        targetFundsEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.TargetFundsEnd.ToString(), Decimal.Zero),
                        totalStartYear = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.TotalStartYear.ToString(), Decimal.Zero),
                        totalEndYear = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.TotalEndYear.ToString(), Decimal.Zero),
                    };

                if (Load<F_F_ParameterDoc>(recId).RefYearForm.ID + 1 < 2016)
                {
                    record.servicesBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.ServicesBegin.ToString(), Decimal.Zero);
                    record.servicesEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.ServicesEnd.ToString(), Decimal.Zero);
                    record.temporaryFundsBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.TemporaryFundsBegin.ToString(), Decimal.Zero);
                    record.temporaryFundsEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.TemporaryFundsEnd.ToString(), Decimal.Zero);
                }
                else
                {
                    record.StateTaskFundStartYear = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.StateTaskFundStartYear.ToString(), Decimal.Zero);
                    record.StateTaskFundEndYear = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.StateTaskFundEndYear.ToString(), Decimal.Zero);
                    record.RevenueFundsStartYear = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.RevenueFundsStartYear.ToString(), Decimal.Zero);
                    record.RevenueFundsEndYear = JsonUtils.GetFieldOrDefault(dataUpdate, F0503730Fields.RevenueFundsEndYear.ToString(), Decimal.Zero);
                }

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = GetItem<F_F_ParameterDoc>(recId);
                record.Section = section;

                Save(record);

                return new RestResult
                           {
                               Success = true,
                               Message = msg,
                               Data = from p in GetItems<F_Report_BalF0503730>()
                                      where p.ID == record.ID
                                      select new
                                                 {
                                                     p.ID,
                                                     p.Name,
                                                     RefParametr = p.RefParametr.ID,
                                                     LineCode = p.lineCode,
                                                     TargetFundsBegin = p.targetFundsBegin,
                                                     TargetFundsEnd = p.targetFundsEnd,
                                                     ServicesBegin = p.servicesBegin,
                                                     ServicesEnd = p.servicesEnd,
                                                     TemporaryFundsBegin = p.temporaryFundsBegin,
                                                     TemporaryFundsEnd = p.temporaryFundsEnd,
                                                     p.StateTaskFundStartYear,
                                                     p.StateTaskFundEndYear,
                                                     p.RevenueFundsStartYear,
                                                     p.RevenueFundsEndYear,
                                                     TotalStartYear = p.totalStartYear,
                                                     TotalEndYear = p.totalEndYear,
                                                     NumberOffBalance = p.lineCode.Substring(0, 2)
                                                 }
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public RestResult F0503121Read(int recId, int section)
        {
            СheckDocContent(recId);

            var data = from p in GetItems<F_Report_Bal0503121>()
                       where (p.RefParametr.ID == recId) && (p.Section == section)
                       select new
                       {
                           p.ID,
                           p.Name,
                           RefParametr = p.RefParametr.ID,
                           LineCode = p.lineCode,

                           IncomesRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                           IncomesRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                           ExpensesRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                           ExpensesRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                           OperatingResultRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                           OperatingResultRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                           OperationNonfinancialAssetsRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                           OperationNonfinancialAssetsRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                           OperationFinancialAssetsRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                           OperationFinancialAssetsRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                           AvailableMeans = p.availableMeans,
                           BudgetActivity = p.budgetActivity,
                           IncomeActivity = p.incomeActivity,
                           Total = p.total
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult F0503121Save(string data, int recId, int section)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, recId, section);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = new F_Report_Bal0503121
                    {
                        ID = Convert.ToInt32(dataUpdate[F0503121Fields.ID.ToString()]),
                        Name = Convert.ToString(dataUpdate[F0503121Fields.Name.ToString()]),
                        lineCode = Convert.ToString(dataUpdate[F0503121Fields.LineCode.ToString()]),
                        availableMeans = JsonUtils.GetFieldOrDefault(dataUpdate, F0503121Fields.AvailableMeans.ToString(), Decimal.Zero),
                        budgetActivity = JsonUtils.GetFieldOrDefault(dataUpdate, F0503121Fields.BudgetActivity.ToString(), Decimal.Zero),
                        incomeActivity = JsonUtils.GetFieldOrDefault(dataUpdate, F0503121Fields.IncomeActivity.ToString(), Decimal.Zero),
                        total = JsonUtils.GetFieldOrDefault(dataUpdate, F0503121Fields.Total.ToString(), Decimal.Zero),
                    };

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = GetItem<F_F_ParameterDoc>(recId);

                switch (section)
                {
                    case (int)F0503121Details.Incomes:
                        {
                            record.RefKosgy = Convert.ToString(dataUpdate["IncomesRefKosgy"]).IsNotNullOrEmpty() ? GetItem<D_KOSGY_KOSGY>(Convert.ToInt32(dataUpdate["IncomesRefKosgy"])) : null;
                            break;
                        }

                    case (int)F0503121Details.Expenses:
                        {
                            record.RefKosgy = Convert.ToString(dataUpdate["ExpensesRefKosgy"]).IsNotNullOrEmpty() ? GetItem<D_KOSGY_KOSGY>(Convert.ToInt32(dataUpdate["ExpensesRefKosgy"])) : null;
                            break;
                        }

                    case (int)F0503121Details.OperatingResult:
                        {
                            record.RefKosgy = Convert.ToString(dataUpdate["OperatingResultRefKosgy"]).IsNotNullOrEmpty() ? GetItem<D_KOSGY_KOSGY>(Convert.ToInt32(dataUpdate["OperatingResultRefKosgy"])) : null;
                            break;
                        }

                    case (int)F0503121Details.OperationFinancialAssets:
                        {
                            record.RefKosgy = Convert.ToString(dataUpdate["OperationFinancialAssetsRefKosgy"]).IsNotNullOrEmpty() ? GetItem<D_KOSGY_KOSGY>(Convert.ToInt32(dataUpdate["OperationFinancialAssetsRefKosgy"])) : null;
                            break;
                        }

                    case (int)F0503121Details.OperationNonfinancialAssets:
                        {
                            record.RefKosgy = Convert.ToString(dataUpdate["OperationNonfinancialAssetsRefKosgy"]).IsNotNullOrEmpty() ? GetItem<D_KOSGY_KOSGY>(Convert.ToInt32(dataUpdate["OperationNonfinancialAssetsRefKosgy"])) : null;
                            break;
                        }
                }

                record.Section = section;

                Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in GetItems<F_Report_Bal0503121>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               p.Name,
                               RefParametr = p.RefParametr.ID,
                               LineCode = p.lineCode,

                               IncomesRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                               IncomesRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                               ExpensesRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                               ExpensesRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                               OperatingResultRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                               OperatingResultRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                               OperationNonfinancialAssetsRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                               OperationNonfinancialAssetsRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                               OperationFinancialAssetsRefKosgy = p.RefKosgy != null ? p.RefKosgy.ID : -1,
                               OperationFinancialAssetsRefKosgyName = p.RefKosgy != null ? string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(p.RefKosgy.Code), p.RefKosgy.Name) : string.Empty,

                               AvailableMeans = p.availableMeans,
                               BudgetActivity = p.budgetActivity,
                               IncomeActivity = p.incomeActivity,
                               Total = p.total
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public RestResult F0503127Read(int recId, int section)
        {
            СheckDocContent(recId);

            var data = from p in GetItems<F_Report_Bal0503127>()
                       where (p.RefParametr.ID == recId) && (p.Section == section)
                       select new
                       {
                           p.ID,
                           p.Name,
                           RefParametr = p.RefParametr.ID,
                           LineCode = p.lineCode,
                           p.ApproveBudgAssign,
                           ExecFinAuthorities = p.execFinAuthorities,
                           ExecBankAccounts = p.execBankAccounts,
                           ExecNonCashOperation = p.execNonCashOperation,
                           ExecTotal = p.execTotal,
                           UnexecAssignments = p.unexecAssignments,
                           UnexecBudgObligatLimit = p.unexecBudgObligatLimit,
                           BudgObligatLimits = p.budgObligatLimits,
                           BudgClassifCode = p.budgClassifCode
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult F0503127Save(string data, int recId, int section)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, recId, section);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = new F_Report_Bal0503127
                    {
                        ID = Convert.ToInt32(dataUpdate[F0503127Fields.ID.ToString()]),
                        Name = Convert.ToString(dataUpdate[F0503127Fields.Name.ToString()]),
                        lineCode = Convert.ToString(dataUpdate[F0503127Fields.LineCode.ToString()]),
                        budgClassifCode = Convert.ToString(dataUpdate[F0503127Fields.BudgClassifCode.ToString()]),
                        ApproveBudgAssign = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.ApproveBudgAssign.ToString(), Decimal.Zero),
                        budgObligatLimits = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.BudgObligatLimits.ToString(), Decimal.Zero),
                        execFinAuthorities = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.ExecFinAuthorities.ToString(), Decimal.Zero),
                        execBankAccounts = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.ExecBankAccounts.ToString(), Decimal.Zero),
                        execNonCashOperation = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.ExecNonCashOperation.ToString(), Decimal.Zero),
                        execTotal = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.ExecTotal.ToString(), Decimal.Zero),
                        unexecAssignments = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.UnexecAssignments.ToString(), Decimal.Zero),
                        unexecBudgObligatLimit = JsonUtils.GetFieldOrDefault(dataUpdate, F0503127Fields.UnexecBudgObligatLimit.ToString(), Decimal.Zero),
                    };

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = GetItem<F_F_ParameterDoc>(recId);
                record.Section = section;

                Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in GetItems<F_Report_Bal0503127>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               p.Name,
                               RefParametr = p.RefParametr.ID,
                               LineCode = p.lineCode,
                               p.ApproveBudgAssign,
                               ExecFinAuthorities = p.execFinAuthorities,
                               ExecBankAccounts = p.execBankAccounts,
                               ExecNonCashOperation = p.execNonCashOperation,
                               ExecTotal = p.execTotal,
                               UnexecAssignments = p.unexecAssignments,
                               UnexecBudgObligatLimit = p.unexecBudgObligatLimit,
                               BudgObligatLimits = p.budgObligatLimits,
                               BudgClassifCode = p.budgClassifCode
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public RestResult F0503130Read(int recId, int section)
        {
            СheckDocContent(recId);

            var data = from p in GetItems<F_Report_BalF0503130>()
                       where (p.RefParametr.ID == recId) && (p.Section == section)
                       select new
                       {
                           p.ID,
                           p.Name,
                           RefParametr = p.RefParametr.ID,
                           LineCode = p.lineCode,
                           BudgetActivityBegin = p.budgetActivityBegin,
                           BudgetActivityEnd = p.budgetActivityEnd,
                           IncomeActivityBegin = p.incomeActivityBegin,
                           IncomeActivityEnd = p.incomeActivityEnd,
                           AvailableMeansBegin = p.availableMeansBegin,
                           AvailableMeansEnd = p.availableMeansEnd,
                           TotalBegin = p.totalBegin,
                           TotalEnd = p.totalEnd,
                           NumberOffBalance = p.lineCode == "280" && section == (int) F0503130F0503730Details.Information
                                ? "30"
                                : p.lineCode.Substring(0, 2)
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult F0503130Save(string data, int recId, int section)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, recId, section);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = new F_Report_BalF0503130
                    {
                        ID = Convert.ToInt32(dataUpdate[F0503130Fields.ID.ToString()]),
                        Name = Convert.ToString(dataUpdate[F0503130Fields.Name.ToString()]),
                        lineCode = Convert.ToString(dataUpdate[F0503130Fields.LineCode.ToString()]),
                        budgetActivityBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.BudgetActivityBegin.ToString(), Decimal.Zero),
                        budgetActivityEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.BudgetActivityEnd.ToString(), Decimal.Zero),
                        incomeActivityBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.IncomeActivityBegin.ToString(), Decimal.Zero),
                        incomeActivityEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.IncomeActivityEnd.ToString(), Decimal.Zero),
                        availableMeansBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.AvailableMeansBegin.ToString(), Decimal.Zero),
                        availableMeansEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.AvailableMeansEnd.ToString(), Decimal.Zero),
                        totalBegin = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.TotalBegin.ToString(), Decimal.Zero),
                        totalEnd = JsonUtils.GetFieldOrDefault(dataUpdate, F0503130Fields.TotalEnd.ToString(), Decimal.Zero),
                    };

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = GetItem<F_F_ParameterDoc>(recId);
                record.Section = section;
               
                Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in GetItems<F_Report_BalF0503130>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               p.Name,
                               RefParametr = p.RefParametr.ID,
                               LineCode = p.lineCode,
                               BudgetActivityBegin = p.budgetActivityBegin,
                               BudgetActivityEnd = p.budgetActivityEnd,
                               IncomeActivityBegin = p.incomeActivityBegin,
                               IncomeActivityEnd = p.incomeActivityEnd,
                               AvailableMeansBegin = p.availableMeansBegin,
                               AvailableMeansEnd = p.availableMeansEnd,
                               TotalBegin = p.totalBegin,
                               TotalEnd = p.totalEnd,
                               NumberOffBalance = p.lineCode.Substring(0, 2)
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public RestResult F0503137Read(int recId, int section)
        {
            СheckDocContent(recId);

            var data = from p in GetItems<F_Report_BalF0503137>()
                       where (p.RefParametr.ID == recId) && (p.Section == section)
                       select new
                       {
                           p.ID,
                           p.Name,
                           RefParametr = p.RefParametr.ID,
                           LineCode = p.lineCode,
                           BudgClassifCode = p.budgClassifCode,
                           ApproveEstimateAssign = p.approveEstimateAssign,
                           ExecFinancAuthorities = p.execFinancAuthorities,
                           ExecBankAccounts = p.execBankAccounts,
                           ExecNonCashOperation = p.execNonCashOperation,
                           ExecTotal = p.execTotal,
                           UnexecAssignments = p.unexecAssignments
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult F0503137Save(string data, int recId, int section)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, recId, section);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = new F_Report_BalF0503137
                    {
                        ID = Convert.ToInt32(dataUpdate[F0503137Fields.ID.ToString()]),
                        Name = Convert.ToString(dataUpdate[F0503137Fields.Name.ToString()]),
                        lineCode = Convert.ToString(dataUpdate[F0503137Fields.LineCode.ToString()]),
                        budgClassifCode = Convert.ToString(dataUpdate[F0503137Fields.BudgClassifCode.ToString()]),
                        approveEstimateAssign = JsonUtils.GetFieldOrDefault(dataUpdate, F0503137Fields.ApproveEstimateAssign.ToString(), Decimal.Zero),
                        execFinancAuthorities = JsonUtils.GetFieldOrDefault(dataUpdate, F0503137Fields.ExecFinancAuthorities.ToString(), Decimal.Zero),
                        execBankAccounts = JsonUtils.GetFieldOrDefault(dataUpdate, F0503137Fields.ExecBankAccounts.ToString(), Decimal.Zero),
                        execNonCashOperation = JsonUtils.GetFieldOrDefault(dataUpdate, F0503137Fields.ExecNonCashOperation.ToString(), Decimal.Zero),
                        execTotal = JsonUtils.GetFieldOrDefault(dataUpdate, F0503137Fields.ExecTotal.ToString(), Decimal.Zero),
                        unexecAssignments = JsonUtils.GetFieldOrDefault(dataUpdate, F0503137Fields.UnexecAssignments.ToString(), Decimal.Zero),
                    };

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = GetItem<F_F_ParameterDoc>(recId);
                record.Section = section;

                Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in GetItems<F_Report_BalF0503137>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               p.Name,
                               RefParametr = p.RefParametr.ID,
                               LineCode = p.lineCode,
                               BudgClassifCode = p.budgClassifCode,
                               ApproveEstimateAssign = p.approveEstimateAssign,
                               ExecFinancAuthorities = p.execFinancAuthorities,
                               ExecBankAccounts = p.execBankAccounts,
                               ExecNonCashOperation = p.execNonCashOperation,
                               ExecTotal = p.execTotal,
                               UnexecAssignments = p.unexecAssignments
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
        
        public RestResult F0503737Read(int recId, int section)
        {
            СheckDocContent(recId);

            var data = from p in GetItems<F_Report_BalF0503737>()
                       where (p.RefParametr.ID == recId) && (p.Section == section)
                       select new
                       {
                           p.ID,
                           p.Name,
                           RefParametr = p.RefParametr.ID,
                           LineCode = p.lineCode,
                           AnalyticCode = p.analyticCode,
                           ApprovePlanAssign = p.approvePlanAssign,
                           ExecPersonAuthorities = p.execPersonAuthorities,
                           ExecBankAccounts = p.execBankAccounts,
                           ExecNonCashOperation = p.execNonCashOperation,
                           ExecCashAgency = p.execCashAgency,
                           ExecTotal = p.execTotal,
                           UnexecPlanAssign = p.unexecPlanAssign,
                           RefTypeFinSupport = p.RefTypeFinSupport.ID,
                           RefTypeFinSupportName = p.RefTypeFinSupport.Name
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult F0503737Save(string data, int recId, int section)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = ValidateData(dataUpdate, recId, section);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = new F_Report_BalF0503737
                    {
                        ID = Convert.ToInt32(dataUpdate[F0503737Fields.ID.ToString()]),
                        Name = Convert.ToString(dataUpdate[F0503737Fields.Name.ToString()]),
                        lineCode = Convert.ToString(dataUpdate[F0503737Fields.LineCode.ToString()]),
                        analyticCode = Convert.ToString(dataUpdate[F0503737Fields.AnalyticCode.ToString()]),
                        approvePlanAssign = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.ApprovePlanAssign.ToString(), Decimal.Zero),
                        execPersonAuthorities = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.ExecPersonAuthorities.ToString(), Decimal.Zero),
                        execBankAccounts = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.ExecBankAccounts.ToString(), Decimal.Zero),
                        execNonCashOperation = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.ExecNonCashOperation.ToString(), Decimal.Zero),
                        execTotal = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.ExecTotal.ToString(), Decimal.Zero),
                        execCashAgency = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.ExecCashAgency.ToString(), Decimal.Zero),
                        unexecPlanAssign = JsonUtils.GetFieldOrDefault(dataUpdate, F0503737Fields.UnexecPlanAssign.ToString(), Decimal.Zero),
                    };

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefTypeFinSupport = GetItem<FX_FX_typeFinSupport>(Convert.ToInt32(dataUpdate[F0503737Fields.RefTypeFinSupport.ToString()]));
                record.RefParametr = GetItem<F_F_ParameterDoc>(recId);
                record.Section = section;

                Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in GetItems<F_Report_BalF0503737>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               p.Name,
                               RefParametr = p.RefParametr.ID,
                               LineCode = p.lineCode,
                               AnalyticCode = p.analyticCode,
                               ApprovePlanAssign = p.approvePlanAssign,
                               ExecPersonAuthorities = p.execPersonAuthorities,
                               ExecBankAccounts = p.execBankAccounts,
                               ExecNonCashOperation = p.execNonCashOperation,
                               ExecCashAgency = p.execCashAgency,
                               ExecTotal = p.execTotal,
                               UnexecPlanAssign = p.unexecPlanAssign,
                               RefTypeFinSupport = p.RefTypeFinSupport != null ? p.RefTypeFinSupport.ID : -1,
                               RefTypeFinSupportName = p.RefTypeFinSupport != null ? p.RefTypeFinSupport.Name : string.Empty
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Удаление документа
        /// </summary>
        /// <param name="recId">ID документа</param>
        public void DeleteDoc(int recId)
        {
            var typeDocName = Load<F_F_ParameterDoc>(recId).RefPartDoc.Name;
            var typeDoc = Load<F_F_ParameterDoc>(recId).RefPartDoc.ID;

            try
            {
                #region Удаляем шапочку(общие для всех документов атрибуты) F_Report_HeadAttribute

                var headAttribute = from p in GetItems<F_Report_HeadAttribute>()
                                    where p.RefParametr.ID == recId
                                    select p;

                headAttribute.Each(x => Delete<F_Report_HeadAttribute>(x.ID));
                #endregion

                switch (typeDoc)
                {
                    case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                        {
                            var doc = from p in GetItems<F_Report_BalF0503730>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_BalF0503730>(x.ID));

                            break;
                        }

                    case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                        {
                            var doc = from p in GetItems<F_Report_Bal0503121>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_Bal0503121>(x.ID));

                            break;
                        }

                    case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                        {
                            var doc = from p in GetItems<F_Report_Bal0503127>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_Bal0503127>(x.ID));

                            break;
                        }

                    case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                        {
                            var doc = from p in GetItems<F_Report_BalF0503130>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_BalF0503130>(x.ID));

                            break;
                        }

                    case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                        {
                            var doc = from p in GetItems<F_Report_BalF0503137>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_BalF0503137>(x.ID));

                            break;
                        }

                    case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                        {
                            var doc = from p in GetItems<F_Report_BalF0503721>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_BalF0503721>(x.ID));

                            break;
                        }

                    case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                        {
                            var doc = from p in GetItems<F_Report_BalF0503737>()
                                      where p.RefParametr.ID == recId
                                      select p;

                            doc.Each(x => Delete<F_Report_BalF0503737>(x.ID));

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа \"{0}\": ".FormatWith(typeDocName) + e.Message, e);
            }
        }

        /// <summary>
        /// Расчет сумм для заданных показателей
        /// </summary>
        /// <param name="docId">идентификатор документа</param>
        /// <param name="section">детализация в документе</param>
        public void CalculateSumm(int docId, int section)
        {
            var partDoc = GetItem<F_F_ParameterDoc>(docId).RefPartDoc.ID;

            switch (partDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503130Type:

                    var f0503130Rows = GetItems<F_Report_BalF0503130>().Where(p => (p.RefParametr.ID == docId) && (p.Section != (int)F0503130F0503730Details.Information)).ToList();

                    SaveRowSumF0503130(new[] { "030", "060", "070", "080", "090", "100", "140" }, f0503130Rows, "150");

                    SaveRowSumF0503130(new[] { "170", "210", "230", "260", "290", "310", "320", "330", "370", "380" }, f0503130Rows, "400");

                    SaveRowSumF0503130(new[] { "150", "400" }, f0503130Rows, "410");

                    SaveRowSumF0503130(new[] { "470", "490", "510", "530", "570", "580", "590" }, f0503130Rows, "600");

                    SaveRowSumF0503130(new[] { "600", "620" }, f0503130Rows, "900");

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:

                    var f0503730Rows = GetItems<F_Report_BalF0503730>().Where(p => (p.RefParametr.ID == docId) && (p.Section != (int)F0503130F0503730Details.Information)).ToList();

                    SaveRowSumF0503730(new[] { "030", "060", "070", "080", "090", "100", "140" }, f0503730Rows, "150");

                    SaveRowSumF0503730(new[] { "170", "210", "230", "260", "290", "310", "320", "330", "370", "380" }, f0503730Rows, "400");

                    SaveRowSumF0503730(new[] { "150", "400" }, f0503730Rows, "410");

                    SaveRowSumF0503730(new[] { "470", "490", "510", "530", "570", "580", "590" }, f0503730Rows, "600");

                    SaveRowSumF0503730(new[] { "623", "623.1", "624", "625", "626" }, f0503730Rows, "620");

                    SaveRowSumF0503730(new[] { "600", "620" }, f0503730Rows, "900");

                    break;

                 case FX_FX_PartDoc.AnnualBalanceF0503121Type:

                     var f0503121Rows = GetItems<F_Report_Bal0503121>().Where(p => (p.RefParametr.ID == docId && p.Section == section)).ToList();

                     SaveRowSumF0503121(new[] { "020", "030", "040", "050", "060", "080", "090", "100", "110" }, f0503121Rows, "010");

                     SaveRowSumF0503121(new[] { "160", "170", "190", "210", "230", "240", "260", "270", "280" }, f0503121Rows, "150");

                     SaveRowSumF0503121(new[] { "310", "380" }, f0503121Rows, "290");

                     SaveRowSumF0503121(new[] { "320", "330", "350", "360", "370" }, f0503121Rows, "310");

                     SaveRowSumF0503121(new[] { "410", "420", "440", "460", "470", "480" }, f0503121Rows, "390");

                     SaveRowSumF0503121(new[] { "520", "530", "540" }, f0503121Rows, "510");

                     SaveRowSumF0503121(new[] { "390", "510" }, f0503121Rows, "380");

                     break;

                 case FX_FX_PartDoc.AnnualBalanceF0503721Type:

                     var f0503721Rows = GetItems<F_Report_BalF0503721>().Where(p => (p.RefParametr.ID == docId && p.Section == section)).ToList();

                     SaveRowSumF0503721(new[] { "030", "040", "050", "060", "090", "100", "110" }, f0503721Rows, "010");

                     SaveRowSumF0503721(new[] { "160", "170", "190", "210", "230", "240", "250", "260", "290" }, f0503721Rows, "150");

                     SaveRowSumF0503721(new[] { "310", "380" }, f0503721Rows, "300");

                     SaveRowSumF0503721(new[] { "320", "330", "350", "360", "370" }, f0503721Rows, "310");

                     SaveRowSumF0503721(new[] { "410", "420", "440", "460", "470", "480" }, f0503721Rows, "390");

                     SaveRowSumF0503721(new[] { "520", "530", "540" }, f0503721Rows, "510");

                     SaveRowSumF0503721(new[] { "390", "510" }, f0503721Rows, "380");

                     break;
            }
        }

        /// <summary>
        /// Заполнение документа по умолчанию 
        /// </summary>
        [Transaction]
        public void СheckDocContent(int recId)
        {
            var docYear = GetItem<F_F_ParameterDoc>(recId).RefYearForm.ID;
            var partDoc = GetItem<F_F_ParameterDoc>(recId).RefPartDoc.ID;
            var docContent = GetItems<D_Marks_ItfSettings>()
                .Where(x => x.RefPartDoc.ID == partDoc &&
                    (!x.StartYear.HasValue || docYear >= x.StartYear) &&
                    (!x.EndYear.HasValue || docYear <= x.EndYear))
                .OrderBy(x => x.RefIndicators.LineCode)
                .Select(
                    x => new
                    {
                        x.RefIndicators.Name,
                        x.RefIndicators.LineCode,
                        x.Section,
                        x.Additional,

                        // связочка детализация-код строки для тех у кого дублируются кода на разных закладках
                        // идентификатор строчки на каждой детализации документа
                        SectionLineCode = string.Concat(x.Section, x.RefIndicators.LineCode)
                    });

            switch (partDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503130Type:

                    var docF0503130 = GetItems<F_Report_BalF0503130>().Where(p => (p.RefParametr.ID == recId)).ToList();
                    var docF0503130LineCodes = docF0503130.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!docF0503130LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_BalF0503130
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть
                            var settingse = itfSettings;
                            var existRecords = docF0503130.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var reportBalF0503130 in existRecords)
                            {
                                reportBalF0503130.Name = itfSettings.Name;

                                Save(reportBalF0503130);
                            }
                        }
                    }

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:

                    var docF0503730 = GetItems<F_Report_BalF0503730>().Where(p => (p.RefParametr.ID == recId) /* && (p.Section == section)*/).ToList();
                    var docF0503730LineCodes = docF0503730.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!docF0503730LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_BalF0503730
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть и у нее нет ссылки на справочник то проставляем ссылочку
                            var settingse = itfSettings;
                            var existRecords = docF0503730.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var reportBalF0503730 in existRecords)
                            {
                                reportBalF0503730.Name = itfSettings.Name;

                                Save(reportBalF0503730);
                            }
                        }
                    }

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:

                    var doc0503121 = GetItems<F_Report_Bal0503121>().Where(p => (p.RefParametr.ID == recId)).ToList();
                    var doc0503121LineCodes = doc0503121.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!doc0503121LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_Bal0503121
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode,
                                RefKosgy = GetItems<D_KOSGY_KOSGY>().FirstOrDefault(x => x.Code.Equals(itfSettings.Additional))
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть и у нее наименование параметра отличное от справочника то меняем наименование
                            var settingse = itfSettings;
                            var existRecords = doc0503121.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var item in existRecords)
                            {
                                item.Name = itfSettings.Name;

                                Save(item);
                            }
                        }
                    }

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:

                    var docF0503721 = GetItems<F_Report_BalF0503721>().Where(p => (p.RefParametr.ID == recId)).ToList();
                    var docF0503721LineCodes = docF0503721.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!docF0503721LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_BalF0503721
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode,
                                analyticCode = itfSettings.Additional
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть и у нее наименование параметра отличное от справочника то меняем наименование
                            var settingse = itfSettings;
                            var existRecords = docF0503721.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var item in existRecords)
                            {
                                item.Name = itfSettings.Name;

                                Save(item);
                            }
                        }
                    }

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:

                    var docF0503127 = GetItems<F_Report_Bal0503127>().Where(p => (p.RefParametr.ID == recId)).ToList();
                    var docF0503127LineCodes = docF0503127.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!docF0503127LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_Bal0503127
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode,
                                budgClassifCode = string.Empty
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть и у нее наименование параметра отличное от справочника то меняем наименование
                            var settingse = itfSettings;
                            var existRecords = docF0503127.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var item in existRecords)
                            {
                                item.Name = itfSettings.Name;

                                Save(item);
                            }
                        }
                    }

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:

                    var docF0503137 = GetItems<F_Report_BalF0503137>().Where(p => (p.RefParametr.ID == recId)).ToList();
                    var docF0503137LineCodes = docF0503137.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!docF0503137LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_BalF0503137
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode,
                                budgClassifCode = string.Empty
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть и у нее наименование параметра отличное от справочника то меняем наименование
                            var settingse = itfSettings;
                            var existRecords = docF0503137.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var item in existRecords)
                            {
                                item.Name = itfSettings.Name;

                                Save(item);
                            }
                        }
                    }

                    break;

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:

                    var docF0503737 = GetItems<F_Report_BalF0503737>().Where(p => (p.RefParametr.ID == recId)).ToList();
                    var docF0503737LineCodes = docF0503737.Select(x => string.Concat(x.Section, x.lineCode)).ToArray();
                    var grbs = GetItem<F_F_ParameterDoc>(recId).RefUchr.RefOrgGRBS.Code;

                    foreach (var itfSettings in docContent)
                    {
                        // если нет строчки то добавляем
                        if (!docF0503737LineCodes.Contains(itfSettings.SectionLineCode))
                        {
                            var record = new F_Report_BalF0503737
                            {
                                ID = 0,
                                RefParametr = GetItem<F_F_ParameterDoc>(recId),
                                Name = itfSettings.Name,
                                Section = itfSettings.Section,
                                lineCode = itfSettings.LineCode,
                                analyticCode = string.Empty,
                                RefTypeFinSupport = grbs.Equals("901")
                                                            ? GetItems<FX_FX_typeFinSupport>().SingleOrDefault(x => x.Code.Equals("7"))
                                                            : GetItems<FX_FX_typeFinSupport>().SingleOrDefault(x => x.Code.Equals("4"))
                            };

                            Save(record);
                        }
                        else
                        {
                            // если строчка есть и у нее наименование параметра отличное от справочника то меняем наименование
                            var settingse = itfSettings;
                            var existRecords = docF0503737.Where(x => string.Concat(x.Section, x.lineCode) == settingse.SectionLineCode && (x.Name != settingse.Name));

                            foreach (var item in existRecords)
                            {
                                item.Name = itfSettings.Name;

                                Save(item);
                            }
                        }
                    }

                    break;
            }
        }

        #endregion

        private string ValidateData(JsonObject record, int docId, int section)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Показатель с кодом строки \"{0}\" уже присутствует в документе <br>";
            const string Msg2 = "Показатель с кодом строки \"{0}\" ";
            
            var message = string.Empty;
            var typeDoc = Load<F_F_ParameterDoc>(docId).RefPartDoc.ID;

            switch (typeDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    {
                        if (Convert.ToString(record[F0503730Fields.Name.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.Name, (F0503130F0503730Details)section));
                        }

                        if (Convert.ToString(record[F0503730Fields.LineCode.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.LineCode, (F0503130F0503730Details)section));
                        }

                        var code = record[F0503730Fields.LineCode.ToString()];
                        var id = Convert.ToInt32(record[F0503730Fields.ID.ToString()]);

                        if (GetItems<F_Report_BalF0503730>().Any(x => x.RefParametr.ID == docId
                                                                    && x.Section == section
                                                                    && x.lineCode == (string)code
                                                                    && x.ID != id))
                        {
                            message += Msg1.FormatWith(code);
                        }
                       
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    {
                        if (Convert.ToString(record[F0503121Fields.Name.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503121NameMapping(F0503121Fields.Name, (F0503121Details)section));
                        }

                        if (Convert.ToString(record[F0503121Fields.LineCode.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503121NameMapping(F0503121Fields.LineCode, (F0503121Details)section));
                        }

                        var code = record[F0503121Fields.LineCode.ToString()];
                        var id = Convert.ToInt32(record[F0503121Fields.ID.ToString()]);

                        if (GetItems<F_Report_Bal0503121>().Any(x => x.RefParametr.ID == docId
                                                                    && x.Section == section
                                                                    && x.lineCode == (string)code
                                                                    && x.ID != id))
                        {
                            message += Msg1.FormatWith(code);
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    {
                        if (Convert.ToString(record[F0503127Fields.Name.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503127NameMapping(F0503127Fields.Name, (F0503127Details)section));
                        }

                        if (Convert.ToString(record[F0503127Fields.LineCode.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503127NameMapping(F0503127Fields.LineCode, (F0503127Details)section));
                        }

                        var code = record[F0503127Fields.LineCode.ToString()];
                        var id = Convert.ToInt32(record[F0503127Fields.ID.ToString()]);
                        var kbk = record[F0503127Fields.BudgClassifCode.ToString()];

                        if (GetItems<F_Report_Bal0503127>().Any(x => x.RefParametr.ID == docId
                                                                    && x.Section == section
                                                                    && x.lineCode == (string)code
                                                                    && x.budgClassifCode == (string)kbk
                                                                    && x.ID != id))
                        {
                            message += string.Concat(Msg2.FormatWith(code), "и КБК \"{0}\"".FormatWith(kbk), " уже присутствует в документе <br>");
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    {
                        if (Convert.ToString(record[F0503130Fields.Name.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.Name, (F0503130F0503730Details)section));
                        }

                        if (Convert.ToString(record[F0503130Fields.LineCode.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.LineCode, (F0503130F0503730Details)section));
                        }

                        var code = record[F0503130Fields.LineCode.ToString()];
                        var id = Convert.ToInt32(record[F0503130Fields.ID.ToString()]);

                        if (GetItems<F_Report_BalF0503130>().Any(x => x.RefParametr.ID == docId
                                                                    && x.Section == section 
                                                                    && x.lineCode == (string)code 
                                                                    && x.ID != id))
                        {
                            message += Msg1.FormatWith(code);
                        }
                        
                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    {
                        if (Convert.ToString(record[F0503137Fields.Name.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503137NameMapping(F0503137Fields.Name, (F0503137Details)section));
                        }

                        if (Convert.ToString(record[F0503137Fields.LineCode.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503137NameMapping(F0503137Fields.LineCode, (F0503137Details)section));
                        }

                        var code = record[F0503137Fields.LineCode.ToString()];
                        var id = Convert.ToInt32(record[F0503137Fields.ID.ToString()]);
                        var kbk = record[F0503137Fields.BudgClassifCode.ToString()];

                        if (GetItems<F_Report_BalF0503137>().Any(x => x.RefParametr.ID == docId
                                                                    && x.Section == section
                                                                    && x.lineCode == (string)code
                                                                    && x.budgClassifCode == (string)kbk
                                                                    && x.ID != id))
                        {
                            message += string.Concat(Msg2.FormatWith(code), "и КБК \"{0}\"".FormatWith(kbk), " уже присутствует в документе <br>");
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    {
                        if (Convert.ToString(record[F0503737Fields.Name.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.Name, (F0503737Details)section));
                        }

                        if (Convert.ToString(record[F0503737Fields.LineCode.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.LineCode, (F0503737Details)section));
                        }

                        if (Convert.ToString(record[F0503737Fields.RefTypeFinSupport.ToString()]).IsNullOrEmpty())
                        {
                            message += Msg.FormatWith(AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.RefTypeFinSupportName, (F0503737Details)section));
                        }

                        if (message.IsNullOrEmpty())
                        {
                            var code = record[F0503737Fields.LineCode.ToString()];
                            var id = Convert.ToInt32(record[F0503737Fields.ID.ToString()]);
                            var analytic = record[F0503737Fields.AnalyticCode.ToString()];
                            var finSupport = Convert.ToInt32(record[F0503737Fields.RefTypeFinSupport.ToString()]);

                            if (GetItems<F_Report_BalF0503737>().Any(x => x.RefParametr.ID == docId
                                                                        && x.Section == section
                                                                        && x.lineCode == (string)code
                                                                        && x.analyticCode == (string)analytic
                                                                        && x.RefTypeFinSupport.ID == finSupport
                                                                        && x.ID != id))
                            {
                                message += string.Concat(
                                    Msg2.FormatWith(code),
                          ", кодом аналитики \"{0}\" и финансовым обеспечением \"{1}\"".FormatWith(analytic, record[F0503737Fields.RefTypeFinSupportName.ToString()]),
                                    " уже присутствует в документе <br>");
                            }    
                        }

                        break;
                    }
            }

            return message;
        }
        
        private void SaveRowSumF0503130(IEnumerable<string> lineCodes, List<F_Report_BalF0503130> f0503130Rows, string sumRowLineCode)
        {
            if (f0503130Rows.Any(x => x.lineCode.Equals(sumRowLineCode)))
            {
                var result = f0503130Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                var sumList = f0503130Rows.Where(x => lineCodes.Contains(x.lineCode)).ToList();

                result.availableMeansBegin = sumList.Sum(x => x.availableMeansBegin);
                result.availableMeansEnd = sumList.Sum(x => x.availableMeansEnd);
                result.budgetActivityBegin = sumList.Sum(x => x.budgetActivityBegin);
                result.budgetActivityEnd = sumList.Sum(x => x.budgetActivityEnd);
                result.incomeActivityBegin = sumList.Sum(x => x.incomeActivityBegin);
                result.incomeActivityEnd = sumList.Sum(x => x.incomeActivityEnd);
                result.totalBegin = sumList.Sum(x => x.totalBegin);
                result.totalEnd = sumList.Sum(x => x.totalEnd);

                Save(result);
            }
        }

        private void SaveRowSumF0503730(IEnumerable<string> lineCodes, List<F_Report_BalF0503730> f0503730Rows, string sumRowLineCode)
        {
            if (f0503730Rows.Any(x => x.lineCode.Equals(sumRowLineCode)))
            {
                var result = f0503730Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                var sumList = f0503730Rows.Where(x => lineCodes.Contains(x.lineCode)).ToList();

                result.targetFundsBegin = sumList.Sum(x => x.targetFundsBegin);
                result.targetFundsEnd = sumList.Sum(x => x.targetFundsEnd);
                result.StateTaskFundStartYear = sumList.Sum(x => x.StateTaskFundStartYear);
                result.StateTaskFundEndYear = sumList.Sum(x => x.StateTaskFundEndYear);
                result.RevenueFundsStartYear = sumList.Sum(x => x.RevenueFundsStartYear);
                result.RevenueFundsEndYear = sumList.Sum(x => x.RevenueFundsEndYear);
                result.servicesBegin = sumList.Sum(x => x.servicesBegin);
                result.servicesEnd = sumList.Sum(x => x.servicesEnd);
                result.temporaryFundsBegin = sumList.Sum(x => x.temporaryFundsBegin);
                result.temporaryFundsEnd = sumList.Sum(x => x.temporaryFundsEnd);
                result.totalStartYear = sumList.Sum(x => x.totalStartYear);
                result.totalEndYear = sumList.Sum(x => x.totalEndYear);

                Save(result);
            }
        }

        private void SaveRowSumF0503121(IEnumerable<string> lineCodes, List<F_Report_Bal0503121> f0503121Rows, string sumRowLineCode)
        {
            if (f0503121Rows.Any(x => x.lineCode.Equals(sumRowLineCode)))
            {
                if (sumRowLineCode.Equals("290"))
                {
                    const string Code1 = "291";
                    const string Code2 = "292";

                    if (f0503121Rows.Any(x => x.lineCode.Equals(Code1))
                        && f0503121Rows.Any(x => x.lineCode.Equals(Code2)))
                    {
                        var row1 = f0503121Rows.Single(x => x.lineCode.Equals(Code1));
                        var row2 = f0503121Rows.Single(x => x.lineCode.Equals(Code2));

                        var result290 = f0503121Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                        result290.availableMeans = row1.availableMeans - row2.availableMeans;
                        result290.budgetActivity = row1.budgetActivity - row2.budgetActivity;
                        result290.incomeActivity = row1.incomeActivity - row2.incomeActivity;
                        result290.total = row1.total - row2.total;

                        Save(result290);

                        return;
                    }
                }

                if (sumRowLineCode.Equals("380"))
                {
                    const string Code1 = "390";
                    const string Code2 = "510";

                    if (f0503121Rows.Any(x => x.lineCode.Equals(Code1))
                        && f0503121Rows.Any(x => x.lineCode.Equals(Code2)))
                    {
                        var row1 = f0503121Rows.Single(x => x.lineCode.Equals(Code1));
                        var row2 = f0503121Rows.Single(x => x.lineCode.Equals(Code2));

                        var result380 = f0503121Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                        result380.availableMeans = row1.availableMeans - row2.availableMeans;
                        result380.budgetActivity = row1.budgetActivity - row2.budgetActivity;
                        result380.incomeActivity = row1.incomeActivity - row2.incomeActivity;
                        result380.total = row1.total - row2.total;

                        Save(result380);

                        return;
                    }
                }

                var result = f0503121Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                var sumList = f0503121Rows.Where(x => lineCodes.Contains(x.lineCode)).ToList();

                result.availableMeans = sumList.Sum(x => x.availableMeans);
                result.budgetActivity = sumList.Sum(x => x.budgetActivity);
                result.incomeActivity = sumList.Sum(x => x.incomeActivity);
                result.total = sumList.Sum(x => x.total);
                
                Save(result);    
            }
        }

        private void SaveRowSumF0503721(IEnumerable<string> lineCodes, List<F_Report_BalF0503721> f0503721Rows, string sumRowLineCode)
        {
            if (f0503721Rows.Any(x => x.lineCode.Equals(sumRowLineCode)))
            {
                if (sumRowLineCode.Equals("300"))
                {
                    const string Code1 = "301";
                    const string Code2 = "302";

                    if (f0503721Rows.Any(x => x.lineCode.Equals(Code1))
                        && f0503721Rows.Any(x => x.lineCode.Equals(Code2)))
                    {
                        var row1 = f0503721Rows.Single(x => x.lineCode.Equals(Code1));
                        var row2 = f0503721Rows.Single(x => x.lineCode.Equals(Code2));

                        var result290 = f0503721Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                        result290.targetFunds = row1.targetFunds - row2.targetFunds;
                        result290.StateTaskFunds = row1.StateTaskFunds - row2.StateTaskFunds;
                        result290.RevenueFunds = row1.RevenueFunds - row2.RevenueFunds;
                        result290.services = row1.services - row2.services;
                        result290.temporaryFunds = row1.temporaryFunds - row2.temporaryFunds;
                        result290.total = row1.total - row2.total;

                        Save(result290);

                        return;
                    }
                }

                if (sumRowLineCode.Equals("380"))
                {
                    const string Code1 = "390";
                    const string Code2 = "510";

                    if (f0503721Rows.Any(x => x.lineCode.Equals(Code1))
                        && f0503721Rows.Any(x => x.lineCode.Equals(Code2)))
                    {
                        var row1 = f0503721Rows.Single(x => x.lineCode.Equals(Code1));
                        var row2 = f0503721Rows.Single(x => x.lineCode.Equals(Code2));

                        var result380 = f0503721Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                        result380.targetFunds = row1.targetFunds - row2.targetFunds;
                        result380.StateTaskFunds = row1.StateTaskFunds - row2.StateTaskFunds;
                        result380.RevenueFunds = row1.RevenueFunds - row2.RevenueFunds;
                        result380.services = row1.services - row2.services;
                        result380.temporaryFunds = row1.temporaryFunds - row2.temporaryFunds;
                        result380.total = row1.total - row2.total;

                        Save(result380);

                        return;
                    }
                }

                var result = f0503721Rows.Single(x => x.lineCode.Equals(sumRowLineCode));

                var sumList = f0503721Rows.Where(x => lineCodes.Contains(x.lineCode)).ToList();

                result.targetFunds = sumList.Sum(x => x.targetFunds);
                result.StateTaskFunds = sumList.Sum(x => x.StateTaskFunds);
                result.RevenueFunds = sumList.Sum(x => x.RevenueFunds);
                result.services = sumList.Sum(x => x.services);
                result.temporaryFunds = sumList.Sum(x => x.temporaryFunds);
                result.total = sumList.Sum(x => x.total);

                Save(result);
            }
        }
    }
}