using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class MarksDataInitializer : IMarksDataInitializer
    {
        private static readonly List<string> RegionsInitialized = new List<string>();

        private readonly IRegion10MarksOivExtension extension;
        private readonly ILinqRepository<F_OIV_REG10Qual> repository;
        private readonly IRepository<FX_OIV_StatusData> statusRepository;
        private readonly IMarksRepository marksRepository;

        public MarksDataInitializer(
            IRegion10MarksOivExtension extension,
            ILinqRepository<F_OIV_REG10Qual> repository,
            IRepository<FX_OIV_StatusData> statusRepository,
            IMarksRepository marksRepository)
        {
            this.extension = extension;
            this.repository = repository;
            this.statusRepository = statusRepository;
            this.marksRepository = marksRepository;
        }

        public void CreateMarksForTerritory(int year, D_Territory_RF territory, bool onlyMO)
        {
            string key = String.Format("{0}_{1}", year, territory.ID);
            if (!RegionsInitialized.Contains(key))
            {
                InitializeRowsForTerritory(territory, onlyMO);
                RegionsInitialized.Add(key);
            }
        }

        private void InitializeRowsForTerritory(D_Territory_RF territory, bool onlyMO)
        {
            repository.DbContext.BeginTransaction();
            try
            {
                var year = extension.CurrentYear;
                var sourceId = extension.DataSourceOiv.ID;

                List<F_OIV_REG10Qual> existingFacts;
                List<D_OIV_Mark> marks;
                if (!onlyMO)
                {
                    existingFacts = repository.FindAll()
                        .Where(x => x.RefTerritory == territory
                                    && x.RefYear == year
                                    && x.RefOIV.RefTypeMark.ID == (int)TypeMark.Gather
                                    && x.SourceID == sourceId)
                        .ToList();

                    marks = marksRepository.FindAll()
                                           .Where(x => x.RefTypeMark.ID == (int)TypeMark.Gather)
                                           .ToList();
                }
                else
                {
                    existingFacts = repository.FindAll()
                        .Where(x => x.RefTerritory == territory
                                    && x.RefYear == year
                                    && x.RefOIV.RefTypeMark.ID == (int)TypeMark.Gather
                                    && x.RefOIV.MO == true
                                    && x.SourceID == sourceId)
                        .ToList();

                    marks = marksRepository.FindAll()
                                           .Where(x => x.RefTypeMark.ID == (int)TypeMark.Gather
                                                       && x.MO == true)
                                           .ToList();
                }

                var status = statusRepository.Get((int)OivStatus.OnEdit);

                foreach (var mark in marks)
                {
                    D_OIV_Mark currentMark = mark;
                    if (!existingFacts.Any(x => x.RefOIV == currentMark))
                    {
                        var fact = new F_OIV_REG10Qual
                        {
                            TaskID = -1,
                            SourceID = sourceId,
                            RefOIV = mark,
                            RefTerritory = territory,
                            RefYear = year,
                            RefStatusData = status
                        };
                        repository.Save(fact);
                    }
                }

                repository.DbContext.CommitChanges();

                repository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());
                repository.DbContext.RollbackTransaction();
            }
        }
    }
}
