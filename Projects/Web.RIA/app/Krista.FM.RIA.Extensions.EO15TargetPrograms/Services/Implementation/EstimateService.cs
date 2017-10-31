using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Models;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class EstimateService : IEstimateService
    {
        private readonly ILinqRepository<F_ExcCosts_EstPrg> factRepository;
        private readonly ILinqRepository<D_ExcCosts_Crit> criteriaRepository;
        private readonly IDatasourceService datasourceService;
        private readonly IAdditionalService additionalService;

        public EstimateService(
                              ILinqRepository<F_ExcCosts_EstPrg> factRepository,
                              ILinqRepository<D_ExcCosts_Crit> criteriaRepository,
                              IDatasourceService datasourceService,
                              IAdditionalService additionalService)
        {
            this.factRepository = factRepository;
            this.criteriaRepository = criteriaRepository;
            this.datasourceService = datasourceService;
            this.additionalService = additionalService;
        }

        public IList<EstimateModel> GetReportTable(D_ExcCosts_ListPrg program, int year, ProgramStage stage)
        {
            int criteriasSourceId = datasourceService.GetCriteriasSourceId(stage);
            var refYear = additionalService.GetRefYear(year);

            // Находим в таблице фактов сохраненные данные
            var data = factRepository.FindAll().Where(x => x.RefProg == program
                                                        && x.RefCrit.SourceID == criteriasSourceId
                                                        && x.RefUNV == refYear)
                                               .ToList();

            var criterias = new List<EstimateModel>();

            decimal? estimateSum = null;

            // Корневые критерии
            var lvl1Criterias = criteriaRepository.FindAll().Where(x => x.ID > 0 
                                                                     && x.ParentID == null 
                                                                     && x.Code > 0 
                                                                     && x.SourceID == criteriasSourceId)
                                                            .ToList();
            foreach (D_ExcCosts_Crit critLvl1 in lvl1Criterias)
            {
                var criteriaLevel1 = new EstimateModel
                                       {
                                           ID = critLvl1.ID,
                                           Level = 0,
                                           CritName = critLvl1.Name,
                                           Weight = critLvl1.Weight,
                                           Point = "-"
                                       };
                criterias.Add(criteriaLevel1);

                var lvl2Criterias = criteriaRepository.FindAll().Where(x => x.ParentID == critLvl1.ID).ToList();
                foreach (D_ExcCosts_Crit critLvl2 in lvl2Criterias)
                {
                    var criteriaLevel2 = new EstimateModel
                                             {
                                                 ID = critLvl2.ID,
                                                 Level = 1,
                                                 CritName = critLvl2.Name,
                                                 Weight = critLvl2.Weight,
                                                 Point = "-"
                                             };

                    criterias.Add(criteriaLevel2);

                    var critLvl3 = data.Where(x => x.RefCrit.ParentID == critLvl2.ID).Select(x => new { x.RefCrit.ID, x.RefCrit.Point, x.Comment }).FirstOrDefault();
                    if (critLvl3 != null)
                    {
                        criteriaLevel2.SelectedId = critLvl3.ID;
                        criteriaLevel2.Point = critLvl3.Point.ToString("G29");
                        criteriaLevel2.Estimate = criteriaLevel2.Weight * critLvl3.Point;
                        criteriaLevel2.Comment = critLvl3.Comment;

                        if (criteriaLevel2.Estimate != null)
                        {
                            criteriaLevel1.Estimate = criteriaLevel1.Estimate ?? 0;
                            criteriaLevel1.Estimate += criteriaLevel2.Estimate;
                        }
                    }
                }

                if (criteriaLevel1.Estimate != null)
                {
                    estimateSum = estimateSum ?? 0;
                    estimateSum += criteriaLevel1.Weight * criteriaLevel1.Estimate;
                }
            }

            if (criterias.Any())
            {
                // Корневой критерий ИТОГО
                var critLvl0 =
                    criteriaRepository.FindAll().FirstOrDefault(x => x.ID > 0 && x.ParentID == null && x.Code == 0);

                criterias.Add(new EstimateModel
                                  {
                                      ID = critLvl0.ID,
                                      Level = 0,
                                      CritName = "ИТОГО:",
                                      Weight = null,
                                      Estimate = estimateSum,
                                      Point = String.Empty
                                  });
            }

            return criterias;
        }

        public void SaveReportFactData(D_ExcCosts_ListPrg program, int year, int selectedCritId, string comment)
        {
            var critLvl3 = criteriaRepository.FindOne(selectedCritId);
            if (critLvl3.ParentID == null)
            {
                throw new Exception("Выбранный критерий не имеет родителя");
            }

            var critLvl2 = criteriaRepository.FindOne((int)critLvl3.ParentID);
            if (critLvl2.ParentID == null)
            {
                throw new Exception("Выбранный критерий должен быть критерием третьего уровня");
            }
            
            var refYear = additionalService.GetRefYear(year);
            
            // Находим существующий факт
            var facts = factRepository.FindAll()
                                      .Where(x => x.RefProg == program 
                                               && x.RefUNV == refYear
                                               && x.RefCrit.ParentID == critLvl3.ParentID)
                                      .ToList();
            var factsCount = facts.Count();
            
            // Очищаем некорректные дубликаты
            if (factsCount > 1)
            {
                foreach (F_ExcCosts_EstPrg row in facts)
                {
                    factRepository.Delete(row);
                }

                factsCount = 0;
            }

            F_ExcCosts_EstPrg entity;
            if (factsCount == 0)
            {
                entity = new F_ExcCosts_EstPrg
                           {
                               SourceID = -1,
                               TaskID = -1,
                               RefProg = program,
                               RefUNV = refYear
                           };
            }
            else
            {
                entity = facts.First();
            }

            entity.RefCrit = critLvl3;
            entity.Comment = comment;

            factRepository.Save(entity);
            factRepository.DbContext.CommitChanges();

            // Страховка от дублей при многопользовательском режиме
            factsCount = factRepository.FindAll().Count(x => x.RefProg == program
                                                          && x.RefUNV == refYear
                                                          && x.RefCrit.ParentID == critLvl3.ParentID);
            if (factsCount > 1)
            {
                throw new Exception("Дубликаты! Попробуйте ещё раз.");
            }
        }

        public IList GetSubcriteriasList(int criteriaId)
        {
            var result = criteriaRepository.FindAll()
                                                     .Where(x => x.ParentID == criteriaId)
                                                     .OrderBy(x => x.Code)
                                                     .Select(x => new { x.ID, x.Name, x.Point }).ToList();
            return result;
        }
    }
}
