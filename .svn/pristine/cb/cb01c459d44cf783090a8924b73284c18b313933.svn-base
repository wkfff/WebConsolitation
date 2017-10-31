using System;
using System.Runtime.InteropServices;
using Krista.FM.Common.RegistryUtils;

namespace Krista.FM.Common.OfficePluginServices.FMOfficeAddin
{
    public class SOAPDimEditorHelper : DisposableObject
    {
        private ISOAPDimEditor soapDimEditorItf;

        public SOAPDimEditorHelper()
        {
            if (Utils.CheckLibByProgID(FMOfficeAddinConsts.SOAPDimEditor_ProgID, true))
            {
                Type objectType = Type.GetTypeFromProgID(FMOfficeAddinConsts.SOAPDimEditor_ProgID);
                if (objectType == null)
                    throw new Exception("Невозможно создать объект " + FMOfficeAddinConsts.SOAPDimEditor_ProgID);
                try
                {
                    soapDimEditorItf = Activator.CreateInstance(objectType) as ISOAPDimEditor;
                }
                catch { }
            }
        }

        #region Обертки над методами ISOAPDimEditor
        public bool EditMemberTree(int parentWnd, string url, string schemeName, string dimensionName, ref string value)
        {
            if (soapDimEditorItf == null)
                return false;
            return soapDimEditorItf.EditMemberTree(parentWnd, url, schemeName, dimensionName, ref value);
        }

        public string LastError
        {
            get
            {
                if (soapDimEditorItf != null)
                    return soapDimEditorItf.LastError;
                return String.Empty;
            }
        }


        public string GetTextMemberList(string xmlValue)
        {
            if (soapDimEditorItf == null)
                return String.Empty;
            return soapDimEditorItf.GetTextMemberList(xmlValue);
        }

        public void SetAuthenticationInfo(int authType, string login, string pwdHash)
        {
            if (soapDimEditorItf != null)
                soapDimEditorItf.SetAuthenticationInfo(authType, login, pwdHash);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if ((disposing) && (soapDimEditorItf != null))
            {
                try
                {
                    Marshal.ReleaseComObject(soapDimEditorItf);
                    soapDimEditorItf = null;
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }
    }
}
