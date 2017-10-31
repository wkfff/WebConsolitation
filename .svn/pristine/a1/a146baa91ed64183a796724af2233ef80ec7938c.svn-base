using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Helpers
{
    public class PermissionSettings
    {
        private readonly bool canDeleteProject;

        public PermissionSettings(IUserCredentials userCredentials, D_InvArea_Reestr project)
        {
            this.canDeleteProject = false;

            if (userCredentials.IsInRole(InvAreaRoles.Creator))
            {
                this.canDeleteProject = project != null 
                                        && userCredentials.User.Name == project.CreateUser
                                        && project.RefStatus.ID == (int)InvAreaStatus.Edit;
            }
            
            if (userCredentials.IsInRole(InvAreaRoles.Coordinator))
            {
                this.canDeleteProject = this.canDeleteProject 
                                        || (project != null
                                            && userCredentials.User.Name == project.CreateUser
                                            && project.RefStatus.ID == (int)InvAreaStatus.Edit);
            }
        }

        public bool CanDeleteProject
        {
            get { return this.canDeleteProject; }
        }
    }
}
