using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public class MonitoringPlacementInfoReportItem
    {
        /// <summary>
        /// ������������ ���
        /// </summary>
        public string NamePpo { get; set; }

        /// <summary>
        /// ������������ ����
        /// </summary>
        public string NameGrbs { get; set; }

        /// <summary>
        /// ID ����������
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ��� ����������
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// ��� ����������
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// ������������ ����������
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// ��������� ����������
        /// </summary>
        public List<MonitoringPlacementInfoReportDocItem> Docs { get; set; }
    }
}