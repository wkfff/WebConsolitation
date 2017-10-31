using System;
using System.Collections;
using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Models;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface IProgramService
    {
        IList GetProgramsTable(
                                bool filterTypeDCP, 
                                bool filterTypeVCP, 
                                bool filterUnapproved, 
                                bool filterApproved, 
                                bool filterRunning, 
                                bool filterFinished);

        IList GetParentProgramListForLookup(); 
        
        D_ExcCosts_ListPrg GetProgram(int id);

        ProgramViewModel GetProgramModel(int id);

        ProgramViewModel GetInitialProgramModel();

        ProgramStatus GetStatus(FX_Date_YearDayUNV approveDate, FX_Date_YearDayUNV startDate, FX_Date_YearDayUNV finishDate);

        void SaveProject(D_ExcCosts_ListPrg entityNew, D_ExcCosts_ListPrg entityOld);

        void DeleteProgram(int id);

        /// <summary>
        /// Формирует список c годами, начиная с года начала и заканчивая годом окончания реализации проекта
        /// </summary>
        /// <param name="programId">id проекта</param>
        IList<int> GetYears(int programId);

        /// <summary>
        /// Формирует список c годами, начиная с предыдущего от начала года и заканчивая последующим годом после окончания реализации программы
        /// </summary>
        /// <param name="programId">id проекта</param>
        IList<int> GetYearsWithPreviousAndFollowing(int programId);

        void ApproveProgram(D_ExcCosts_ListPrg program, DateTime approvementDate);
    }
}