﻿using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;

namespace Krista.FM.Update.Framework.Utils
{
    public static class PermissionsCheck
    {
        private static IdentityReferenceCollection groups = WindowsIdentity.GetCurrent().Groups;
        private static string sidCurrentUser = WindowsIdentity.GetCurrent().User.Value;

        public static bool HaveWritePermissionsForFolder(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                return true;

            var rules = Directory.GetAccessControl(Path.GetDirectoryName(path)).GetAccessRules(true, true, typeof(SecurityIdentifier));

            bool allowwrite = false;
            bool denywrite = false;
            foreach (FileSystemAccessRule rule in rules)
            {
                if (rule.AccessControlType == AccessControlType.Deny &&
                    ((rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData || (int)rule.FileSystemRights == -1610612736) &&
                    (groups.Contains(rule.IdentityReference) || rule.IdentityReference.Value == sidCurrentUser)
                    )
                {
                    denywrite = true;
                }
                if (rule.AccessControlType == AccessControlType.Allow &&
                    ((rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData || (int)rule.FileSystemRights == -1610612736) &&
                    (groups.Contains(rule.IdentityReference) || rule.IdentityReference.Value == sidCurrentUser)
                    )
                {
                    allowwrite = true;
                }
            }

            if (allowwrite && !denywrite)
                return true;

            return false;
        }
    }
}
