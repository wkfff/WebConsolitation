using System;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.MessagesUI.Evaluators
{
    public class GroupByEvaluator : IGroupByEvaluator
    {
        public object GetGroupByValue(UltraGridGroupByRow groupByRow, UltraGridRow row)
        {
            if (groupByRow.Value is DateTime)
            {
                DateTime groupByDate = (DateTime)groupByRow.Value;

                if (groupByDate.Date == DateTime.Today)
                {
                    return groupByDate;
                }

                if (groupByDate.Date == DateTime.Today.AddDays(-1f))
                {
                    return groupByDate;
                }

                if (groupByDate.Date > DateTime.Today.AddDays(-7f))
                {
                    return groupByDate;
                }

                return null;
            }

            return groupByRow.Value;
        }

        public bool DoesGroupContainRow(UltraGridGroupByRow groupByRow, UltraGridRow row)
        {
            DateTime receivedDate = ((DateTime)row.Cells["ReceivedDate"].Value).Date;

            if (groupByRow.Value is DateTime)
            {
                DateTime groupByDate = (DateTime)groupByRow.Value;

                if (groupByDate.Date == DateTime.Today &&
                     receivedDate.Date == DateTime.Today)
                {
                    return true;
                }

                if (groupByDate.Date == DateTime.Today.AddDays(-1f) &&
                    receivedDate.Date == DateTime.Today.AddDays(-1f))
                {
                    return true;
                }

                if (receivedDate.Date == ((DateTime)groupByRow.Value).Date &&
                    receivedDate.Date > DateTime.Today.AddDays(-7f))
                {
                    return true;
                }
            }
            else
            {
                if (groupByRow.Value is DayOfWeek &&
                    receivedDate.DayOfWeek == (DayOfWeek)groupByRow.Value)
                {
                    if (receivedDate > DateTime.Today.AddDays(-7f))
                    {
                        return true;
                    }
                }
            }

            if (groupByRow.Value == null)
            {
                if (receivedDate <= DateTime.Today.AddDays(-7f))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
