using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Xml;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Shared;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.Win32;
using System.Windows.Forms;
using ADODB;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct CommonUtils
    {
        //Separator ��� ������� ArrayToString and ArrayFromString
        public const char separator = ';';

        /// <summary>
        /// ���������� ���������� ������� � ���������
        /// </summary>
        /// <param name="executablePath"></param>
        static public void AssociationReportExtension(string executablePath)
        {
            try
            {
                RegistryKey rootKey = Registry.ClassesRoot;
                //������ ������ �������� �� �������
                rootKey.DeleteSubKey(Consts.reportExt, false);
                RegistryKey keyForDelete = null;
                keyForDelete = rootKey.OpenSubKey(Consts.applicationNameRegKey);
                if (keyForDelete != null)
                {
                    rootKey.DeleteSubKeyTree(Consts.applicationNameRegKey);
                    keyForDelete = null;
                }
                keyForDelete = rootKey.OpenSubKey(Consts.autoExtensionKey);
                if (keyForDelete != null)
                {
                    rootKey.DeleteSubKeyTree(Consts.autoExtensionKey);
                    keyForDelete = null;
                }

                RegistryKey extensionKey = rootKey.CreateSubKey(Consts.reportExt);
                extensionKey.SetValue(string.Empty, Consts.applicationNameRegKey);
                extensionKey.SetValue("Content Type", Consts.mimeType);



                RegistryKey applNameKey = rootKey.CreateSubKey(Consts.applicationNameRegKey);
                applNameKey.SetValue(string.Empty, Consts.reportInform);
                RegistryKey tempKey = applNameKey.CreateSubKey("shell");
                tempKey = tempKey.CreateSubKey("open");
                tempKey = tempKey.CreateSubKey("command");
                //���������� ������� ����� ����������� ��� ������� ����� �� ������
                tempKey.SetValue(string.Empty, string.Format("\"{0}\" \"{1}\"", executablePath, "%1"));

                tempKey.Close();
                applNameKey.Close();
                extensionKey.Close();
                rootKey.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// ���������� ������������� ����� � �����������, ����� ��������� �� ��������,
        /// ���� ���������� ������, ���� ��������� ����������
        /// </summary>
        /// <param name="e"></param>
        public static void ProcessException(Exception e)
        {
            ProcessException(e, false);
        }

        /// <summary>
        /// ���������� ������������� ����� � �����������, ����� ��������� �� ��������,
        /// ���� ���������� ������, ���� ��������� ����������
        /// </summary>
        /// <param name="e"></param>
        /// <param name="needKillProcess">true - ��� ��������������� �������� ���������� :). 
        /// ����� ����� Application.Exit() �� �������� (�������� �� ����� �������� ������� �����)
        /// </param>
        public static void ProcessException(Exception e, bool needKillProcess)
        {
            ErrorFormResult res = FormException.ShowErrorForm(e);
            switch (res)
            {
                case ErrorFormResult.efrClose:
                    if (needKillProcess)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                    else
                    {
                        Application.Exit();
                    }
                    break;
                case ErrorFormResult.efrRestart:
                    Application.Restart();
                    break;
            }
        }


        #region ������ �� ��������
        /// <summary>
        /// ���� ������ ������, ��������� �� ������, � ������ ������...
        /// </summary>
        /// <param name="g"></param>
        /// <param name="measuredString"></param>
        /// <param name="font"></param>
        /// <param name="rectangleWidth"></param>
        /// <returns>int(������ ������)</returns>
        public static int GetStringHeight(Graphics g, string measuredString, Font font, int rectangleWidth)
        {
            SizeF sizeF = g.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);
            return rect.Height;
        }

        /// <summary>
        /// ������ ������ ������
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns>int(������ ������)</returns>
        public static float GetStringWidth(Graphics g, string text, Font font)
        {
            return g.MeasureString(text, font).Width;
        }

        //�������� ������������ ������ �� �������������� �����
        public static int GetStringMaxWidth(Graphics graphics, string[] strings, Font font)
        {
            int result = 0;
            foreach (string str in strings)
            {
                int width = (int)graphics.MeasureString(str, font).Width + 1;
                result = Math.Max(result, width);
            }
            return result;
        }

        //�������� ������ �����
        public static int[] GetStringWidths(Graphics graphics, string[] strings, Font font)
        {
            int[] result = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                result[i] = (int)graphics.MeasureString(strings[i], font).Width + 1;
            }
            return result;
        }

        /// <summary>
        /// ������������ ������ ����� � ������
        /// </summary>
        /// <param name="array">������ ����� �����</param>
        /// <param name="separator">������ �����������</param>
        /// <returns></returns>
        public static string ArrayToString(int[] array, char separator)
        {
            StringBuilder result = new StringBuilder();
            foreach (int item in array)
            {
                result.Append(item.ToString() + separator);
            }
            if (result.Length > 0)
                result[result.Length - 1] = ' ';
            return result.ToString().Trim();
        }

        /// <summary>
        /// ������ ������ (�� separator) � ���������� ������ ����� ����� ������������ � ���
        /// </summary>
        /// <param name="source">������ � �������</param>
        /// <param name="separator">�����������</param>
        /// <returns></returns>
        public static int[] ArrayFromString(string source, char separator)
        {

            List<int> intArray = new List<int>();
            string[] strArray = source.Split(separator);
            int i;
            foreach (string item in strArray)
            {
                if (item == string.Empty)
                    continue;

                if (Int32.TryParse(item, out i))
                {
                    intArray.Add(i);
                }
                else
                {
                    intArray.Add(0);
                }
            }
            return intArray.ToArray();
        }

        /// <summary>
        /// �� ������� ����, �������� ��� �����
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        static public string SeparateFileName(string fullFileName, bool isIncludeExtension)
        {
            if (fullFileName == string.Empty)
                return string.Empty;
            string[] strArray = fullFileName.Split('\\');
            string fileName = strArray[strArray.Length - 1];
            if (!isIncludeExtension && (fileName.LastIndexOf('.') != -1))
                fileName = fileName.Remove(fileName.LastIndexOf('.'));
            return fileName;
        }

        static public string SetExtension(string fileName, string extension)
        {
            if (extension == string.Empty)
                return fileName;

            //������� ������ ����������
            if (fileName.LastIndexOf('.') != -1)
                fileName = fileName.Remove(fileName.LastIndexOf('.'));
            //���������� �����
            return fileName + extension;
        }

        static private string CutPart(ref string tail, string separator)
        {
            string result = "0";
            if (!string.IsNullOrEmpty(tail))
            {
                int p = tail.IndexOf(separator);
                if (p == -1)
                {
                    result = tail;
                    tail = String.Empty;
                }
                else
                {
                    result = tail.Substring(0, p);
                    tail = tail.Remove(0, p + separator.Length);
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// ��������� ������� ������� � �������� � ������ ����
        /// </summary>
        /// <param name="poses"></param>
        /// <param name="path"></param>
        public static void WritePoses(PositionCollection poses, string path)
        {
            try
            {
                List<string> str = new List<string>();
                int posNum = 0;
                foreach (Position pos in poses)
                {
                    //str.Add(posNum.ToString().PadRight(40, '-'));
                    //str.Add(string.Empty.PadRight(40, '-'));
                    //int memNum = 0;
                    string s = string.Empty; ;
                    foreach (Member mem in pos.Members)
                    {
                        s += string.Format("{0} | {1}    ||   ", mem.LevelDepth, mem.UniqueName);
                        //str.Add("    " + memNum + "  " + mem.UniqueName);
                        //str.Add(string.Format("    {0} | {1}", mem.LevelDepth, mem.UniqueName));
                        // memNum++;
                    }
                    str.Add(s);
                    //str.Add(string.Empty.PadRight(40, '-'));
                    //str.Add(string.Empty);
                    posNum++;
                }

                System.IO.File.WriteAllLines(path, str.ToArray());
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"���� �� ������ ��� ���������, ���������� � ������������,
                    �.�. �� ����� ������� ���, ������������ ��� �������");
            }
        }

        /// <summary>
        /// �������� ��� ���������� ����������� �������� ����, � ������� ������ ����������� �������������
        /// �������� � ��������� ���������, ���� newValue �� ������������� ���� �� ������ ����������, ����� 
        /// ���������� ���������� c ������� ��������� � ��������� "exceptionText"
        /// </summary>
        /// <param name="newValue">����� ��������</param>
        /// <param name="oldValidValue">��������� ���������� ��������</param>
        /// <param name="lowerBound">������ ������ ��������</param>
        /// <param name="upperBound">������� ������ ��������</param>
        /// <param name="isExitEdiValue">���� ������� �� ������ ��������������</param>
        /// <param name="sException">����� ����������</param>
        /// <param name="outValue">�������� ��������</param>
        /// <returns></returns>
        static public bool IsValidIntValue(string newValue, int oldValidValue, int lowerBound, int upperBound,
            bool isExitEditMode, string exceptionText, out string outValue)
        {
            bool result = true;
            outValue = newValue;

            int i;
            if (!int.TryParse(newValue, out i) || !(i >= lowerBound) || !(i <= upperBound))
            {
                result = false;
                //���� ������� �� ������ �������������� � �������, �������� �������� ��������, 
                //��������� ����������... ����� �� ����� ����� ������, ��� ����� ����� ������������ �����������
                //��������� ��������� �������
                if (isExitEditMode)
                    outValue = oldValidValue.ToString();

                FormException.ShowErrorForm(new Exception(exceptionText),
                    ErrorFormButtons.WithoutTerminate);
            }

            return result;
        }

        #region ������ � ������������
        /// <summary>
        /// �������� ����������� ��������� ������� ������
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Bitmap GetBitMap(Rectangle bounds)
        {

            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics bmpGraphics = Graphics.FromImage(bitmap))
            {
                bmpGraphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, new Size(bounds.Width, bounds.Height));
            }

            return bitmap;
        }

        /// <summary>
        /// ������� ����� Bitmap � �������� � ���� ���������� Graphics
        /// </summary>
        /// <param name="graphicsSource"></param>
        /// <param name="x">X ���������� ������ �������� ����</param>
        /// <param name="y">Y ���������� ������ �������� ����</param>
        /// <param name="width">������ ���������� �������</param>
        /// <param name="height">������ ���������� �������</param>
        static public Bitmap GetBitmapFromGraphics(Graphics graphicsSource, int x, int y, int width, int height)
        {
            IntPtr hDCSource = graphicsSource.GetHdc();
            IntPtr hDCDestination = Win32.CreateCompatibleDC(hDCSource);
            IntPtr hBitmap = Win32.CreateCompatibleBitmap(hDCSource, width, height);
            IntPtr hPreviousBitmap = Win32.SelectObject(hDCDestination, hBitmap);
            Win32.BitBlt(hDCDestination, 0, 0, width, height, hDCSource, x, y, Win32.SRCCOPY);
            Bitmap bitmap = Bitmap.FromHbitmap(hBitmap);
            Win32.DeleteObject(hDCDestination);
            Win32.DeleteObject(hBitmap);
            graphicsSource.ReleaseHdc(hDCSource);
            return bitmap;
        }
        #endregion

        static public VersionRelation GetVersionRelation(string reportVersionStr, string comparedVersionStr)
        {
            int partCount = Consts.versionPartNumber;
            int[] reportVersion = new int[partCount];
            int[] comparedVersion = new int[partCount];

            int i;
            for (i = 0; i < partCount; i++)
            {
                Int32.TryParse(CutPart(ref reportVersionStr, "."), out reportVersion[i]);
                Int32.TryParse(CutPart(ref comparedVersionStr, "."), out comparedVersion[i]);
            }

            i = 0;

            while (i < partCount)
            {
                if (reportVersion[i] < comparedVersion[i])
                {
                    return VersionRelation.Ancient;
                }
                else
                {
                    if (reportVersion[i] > comparedVersion[i])
                    {
                        return VersionRelation.Future;
                    }
                }
                i++;
            }
            return VersionRelation.Modern;
        }

        //��������� ������ ��� � �������
        static public List<string> GetCatalogList(string connStr)
        {

            ADODB.Connection con = new ADODB.Connection();
            ADODB.Recordset catalogRecordset = null;
            List<string> result = new List<string>();

            try
            {
                con.Open(connStr, "", "", -1);

                if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                {
                    catalogRecordset = con.OpenSchema(SchemaEnum.adSchemaCatalogs, Missing.Value, Missing.Value);

                    while (!catalogRecordset.EOF)
                    {
                        result.Add(catalogRecordset.Fields["CATALOG_NAME"].Value.ToString());
                        catalogRecordset.MoveNext();
                    }

                }
            }
            finally
            {
                if (catalogRecordset != null)
                {
                    if (catalogRecordset.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        catalogRecordset.Close();
                    }
                    catalogRecordset = null;
                }

                if (con != null)
                {
                    if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        con.Close();
                    }
                    con = null;
                }

            }
            return result;
        }

        /// <summary>
        /// ����������� �� ������ ����
        /// </summary>
        /// <param name="set">���</param>
        /// <param name="value">������</param>
        /// <returns></returns>
        static public bool IsInSet(object[] set, object value)
        {
            if ((set != null) && (value != null))
            {
                foreach (object setItem in set)
                {
                    if (setItem.ToString() == value.ToString())
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// ���������� �������� �����
        /// </summary>
        /// <param name="number">�����</param>
        /// <param name="digitCount">���-�� ������ ����� �������</param>
        /// <returns>����� � �������� ���������</returns>
        public static double SetNumberPrecision(double number, int digitCount)
        {
            try
            {
                number = Math.Round(number, digitCount);
                /*
                number *= Math.Pow(10, digitCount);
                number = Math.Truncate(number);
                number /= Math.Pow(10, digitCount);*/
                return number;
            }
            catch
            {
                return number;
            }
        }

        /// <summary>
        /// ����������� ����� �� ���� ����������
        /// </summary>
        /// <param name="src">�����, ������� ��������</param>
        /// <param name="dst">�����, � ������� ��������</param>
        public static void CopyDirectory(string src, string dst)
        {
            try
            {
                String[] Files;
                if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                    dst += Path.DirectorySeparatorChar;
                if (!Directory.Exists(dst)) Directory.CreateDirectory(dst);
                Files = Directory.GetFileSystemEntries(src);
                foreach (string element in Files)
                {
                    if (Directory.Exists(element))
                        CopyDirectory(element, dst + Path.GetFileName(element));
                    else
                        File.Copy(element, dst + Path.GetFileName(element), true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// �������� �������� �������� ������������� ���� (��������� �������� ��������� [Description])
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumValueDescription(Type enumType, object value)
        {
            FieldInfo fi = value.GetType().GetField(Enum.GetName(enumType, value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();

        }

        /// <summary>
        /// ��������� ��������� �������� ��� ID
        /// </summary>
        /// <param name="mbr">�������</param>
        /// <returns></returns>
        static public string GetMemberCaptionWithoutID(Member mbr)
        {
            string memCaption = mbr.Caption;
            //���� ����� ���������� �������� �������� ID �������� - ������� ���� ID
            string memID = GetMemberIDFromCaption(mbr.Caption);
            if (!String.IsNullOrEmpty(memID))
            {
                mbr.FetchAllProperties();
                if ((mbr.Properties.Find("PKID") != null) && (mbr.Properties["PKID"].Value != null))
                {
                    //���� � ��������� ����� ID ������ ���,  �� ��������� ��������� ��� ����
                    if (memCaption == mbr.Properties["PKID"].Value.ToString())
                    {
                        return memCaption;
                    }

                    if (mbr.Properties["PKID"].Value.ToString() == memID)
                    {
                        bool isDataMember = false;
                        if ((mbr.Properties.Find("IS_DATAMEMBER") != null) && (mbr.Properties["IS_DATAMEMBER"].Value != null))
                        {
                            isDataMember = (mbr.Properties["IS_DATAMEMBER"].Value.ToString().ToUpper() == "TRUE");
                        }
                        memCaption = isDataMember
                                         ? memCaption.Remove(1, memID.Length)
                                         : memCaption.Remove(0, memID.Length);
                    }
                }
            }
            return memCaption;
        }

        /// <summary>
        /// ��������� ID �������� �� ���������
        /// </summary>
        /// <param name="caption">��������� ��������</param>
        /// <returns>ID</returns>
        static public string GetMemberIDFromCaption(string caption)
        {
            string memberID = String.Empty;
            //���� ����� ���������� � ������� "(" - �������� ��� ���������� (����� �������� IS_DATAMEMBER 
            //��������� ������ ������ �����������)
            int startMemberID = 0;
            if ((caption.Length > 0) && (caption[0] == '('))
                startMemberID = 1;

            for (int i = startMemberID; i < caption.Length; i++)
            {
                if (Char.IsNumber(caption[i]))
                {
                    memberID += caption[i];
                }
                else
                {
                    break;
                }
            }
            return memberID;
        }
    }

    //��������� ����� ������� ��������
    public enum VersionRelation
    {
        //������
        Ancient,
        //���������
        Modern,
        //�����
        Future
    }

    public enum VersionType
    {
        Release,
        Test
    }

    public interface ICompositeLegendParams
    {
        LegendLocation CompositeLegendLocation
        {
            get; set;
        }

        int CompositeLegendExtent
        {
            get; set;
        }
    }


}