using System;
using System.Runtime.InteropServices;
using Krista.FM.Common.RegistryUtils;

namespace Krista.FM.Common.OfficePluginServices.FMOfficeAddin
{
    public class SOAPDimChooserHelper : DisposableObject
    {
        private ISOAPDimChooser soapDimChooserItf;

        public SOAPDimChooserHelper()
        {
            if (Utils.CheckLibByProgID(FMOfficeAddinConsts.SOAPDimChooser_ProgID, true))
            {
                Type objectType = Type.GetTypeFromProgID(FMOfficeAddinConsts.SOAPDimChooser_ProgID);
                if (objectType == null)
                    throw new Exception("���������� ������� ������ " + FMOfficeAddinConsts.SOAPDimChooser_ProgID);
                try
                {
                    soapDimChooserItf = Activator.CreateInstance(objectType) as ISOAPDimChooser;
                }
                catch { }//�� �64 ������-�� ������ �������� ���-��������, ����������� ����
            }
        }

        protected override void Dispose(bool disposing)
        {
            if ((disposing) && (soapDimChooserItf != null))
            {
                try
                {
                    Marshal.ReleaseComObject(soapDimChooserItf);
                    soapDimChooserItf = null;
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }

        #region ������� ��� �������� ISOAPDimChooser
        public bool SelectDimension(int parentWnd, string url, string schemeName, ref string dimensionName)
        {
            if (soapDimChooserItf == null)
                return false;
            return soapDimChooserItf.SelectDimension(parentWnd, url, schemeName, ref dimensionName);
        }

        public bool RefreshOnShow
        {
            set
            {
                if (soapDimChooserItf == null)
                    return;
                soapDimChooserItf.RefreshOnShow = value;
            }
        }

        public string LastError
        {
            get
            {
                if (soapDimChooserItf == null)
                    return String.Empty;
                return soapDimChooserItf.LastError;
            }
        }

        public void SetAuthenticationInfo(int authType, string login, string pwdHash)
        {
            if (soapDimChooserItf != null)
                soapDimChooserItf.SetAuthenticationInfo(authType, login, pwdHash);
        }
        #endregion
    }
}
