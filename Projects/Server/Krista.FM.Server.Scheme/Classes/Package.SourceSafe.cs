using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    internal partial class Package
    {
        #region Работа с SourceSafe

        private string GetSourceSafeLocalSpec()
        {
            string path = GetLocalPath() + '\\' + Path.GetFileName(privatePath);
            return path.Replace('\\', '/');
        }

        private void CheckOut(string comments)
        {
            Trace.TraceVerbose("CheckOut");
            IVSSFacade vssFacade = SchemeClass.Instance.VSSFacade;
            if (vssFacade != null)
            {
                try
                {
                    string local = GetSourceSafeLocalSpec();
                    switch (vssFacade.IsCheckedOut(local))
                    {
                        case VSSFileStatus.VSSFILE_CHECKEDOUT:
                            throw new Exception(String.Format("В базе SourceSafe файл \"{0}\" заблокирован другим пользователем.", local));
                        case VSSFileStatus.VSSFILE_NOTCHECKEDOUT:
                            vssFacade.Checkout(local, comments); break;
                    }
                }
                finally
                {
                    vssFacade.Close();
                }
            }
            neelFlash = true;
        }

        private void CheckIn(string comments)
        {
            if (Name != SystemPackageName)
            {
                Trace.TraceVerbose("Try CheckIn \"{0}\"", this.PrivatePath);
                IVSSFacade vssFacade = SchemeClass.Instance.VSSFacade;
                if (vssFacade != null)
                {
                    try
                    {
                        string local = GetSourceSafeLocalSpec();
                        if (!vssFacade.Find(local))
                            vssFacade.Checkin(local, comments);
                        else if (vssFacade.IsCheckedOut(local) == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
                            vssFacade.Checkin(local, comments);
                    }
                    finally
                    {
                        vssFacade.Close();
                    }
                }
            }
        }

        private void UndoCheckOut()
        {
            Trace.TraceVerbose("UndoCheckOut");
            IVSSFacade vssFacade = SchemeClass.Instance.VSSFacade;
            if (vssFacade != null)
            {
                try
                {
                    string local = GetSourceSafeLocalSpec();
                    if (vssFacade.IsCheckedOut(local) == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
                        vssFacade.UndoCheckout(local);
                }
                finally
                {
                    vssFacade.Close();
                }
            }
        }

        public VSSFileStatus IsCheckedOut
        {
            get
            {
                try
                {
                    IVSSFacade vssFacade = SchemeClass.Instance.VSSFacade;
                    if (vssFacade != null)
                    {
                        try
                        {
                            if (State == ServerSideObjectStates.New)
                                return VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
                            else
                                return vssFacade.IsCheckedOut(GetSourceSafeLocalSpec());
                        }
                        finally
                        {
                            vssFacade.Close();
                        }
                    }
                    else
                        return VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    throw new Exception(e.Message, e);
                }
            }
        }

        #endregion Работа с SourceSafe
    }
}
