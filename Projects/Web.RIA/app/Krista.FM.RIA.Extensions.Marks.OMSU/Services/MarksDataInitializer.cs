using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class MarksDataInitializer : IMarksDataInitializer
    {
        private static readonly List<string> RegionsInitialized = new List<string>();

        private readonly IMarksOmsuExtension extension;
        private readonly ILinqRepository<F_OMSU_Reg16> repository;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;
        private readonly IMarksRepository marksRepository;
        private readonly IRegionsRepository regionRepository;

        public MarksDataInitializer(
            IMarksOmsuExtension extension,
            ILinqRepository<F_OMSU_Reg16> repository,
            IRepository<FX_OMSU_StatusData> statusRepository,
            IMarksRepository marksRepository,
            IRegionsRepository regionRepository)
        {
            this.extension = extension;
            this.repository = repository;
            this.statusRepository = statusRepository;
            this.marksRepository = marksRepository;
            this.regionRepository = regionRepository;
        }

        public virtual void CreateMarksForRegion(string key)
        {
            if (!RegionsInitialized.Contains(key))
            {
                InitializeRows();

                RegionsInitialized.Add(key);
            }
        }

        public void CreateRegionsForMark(string key, int markId)
        {
            if (!RegionsInitialized.Contains(key))
            {
                InitializeRowsRegionsForMark(markId);

                RegionsInitialized.Add(key);
            }
        }

        private void InitializeRows()
        {
            repository.DbContext.BeginTransaction();
            try
            {
                InitializeEntityRows();

                repository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());
                repository.DbContext.RollbackTransaction();
            }
        }

        private void InitializeEntityRows()
        {
            var year = extension.CurrentYearUNV;
            var sourceId = extension.DataSourceOmsu.ID;

            var facts = repository.FindAll().Where(x => x.RefRegions == extension.UserRegionCurrent && x.RefYearDayUNV == year && x.SourceID == sourceId).ToList();

            var status = statusRepository.Get((int)OMSUStatus.OnEdit);

            foreach (var mark in marksRepository.FindAll().ToList())
            {
                D_OMSU_MarksOMSU currentMark = mark;
                if (!facts.Any(x => x.RefMarksOMSU == currentMark))
                {
                    var fact = new F_OMSU_Reg16
                    {
                        TaskID = -1,
                        SourceID = extension.DataSourceOmsu.ID,
                        RefMarksOMSU = mark,
                        RefRegions = extension.UserRegionCurrent,
                        RefYearDayUNV = extension.CurrentYearUNV,
                        RefStatusData = status
                    };
                    repository.Save(fact);
                }
            }

            repository.DbContext.CommitChanges();
        }

        private void InitializeRowsRegionsForMark(int markId)
        {
            repository.DbContext.BeginTransaction();

            try
            {
                InitializeEntityRowsRegionsForMark(markId);

                repository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());
                repository.DbContext.RollbackTransaction();
            }
        }

        private void InitializeEntityRowsRegionsForMark(int markId)
        {
            var marksOmsu = marksRepository.FindOne(markId);

            var year = extension.CurrentYearUNV;
            var sourceId = extension.DataSourceOmsu.ID;

            var facts = repository.FindAll().Where(x => x.RefMarksOMSU == marksOmsu && x.RefYearDayUNV == year && x.SourceID == sourceId).ToList();

            var status = statusRepository.Get((int)OMSUStatus.OnEdit);

            foreach (var region in regionRepository.FindAll()
                .Where(x => x.RefTerr.ID == (int)TerrytoryType.MR || x.RefTerr.ID == (int)TerrytoryType.GO)
                .ToList())
            {
                D_Regions_Analysis currentRegion = region;
                if (!facts.Any(x => x.RefRegions == currentRegion))
                {
                    var fact = new F_OMSU_Reg16
                    {
                        TaskID = -1,
                        SourceID = extension.DataSourceOmsu.ID,
                        RefMarksOMSU = marksOmsu,
                        RefRegions = region,
                        RefYearDayUNV = extension.CurrentYearUNV,
                        RefStatusData = status
                    };
                    repository.Save(fact);
                }
            }

            repository.DbContext.CommitChanges();
        }
    }
}
