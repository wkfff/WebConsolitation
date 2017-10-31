using System.Security.Principal;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers
{
    public class PermissionSettings
    {
        private readonly bool canDeleteProgram;
        private readonly bool canEditDetail;
        private readonly bool canApprove;
        
        public PermissionSettings(IPrincipal userCredentials, D_ExcCosts_ListPrg program)
        {
            this.canDeleteProgram = false;
            this.canEditDetail = false;
            this.canApprove = false;

            if (userCredentials.IsInRole(ProgramRoles.Creator))
            {
                this.canDeleteProgram = program != null 
                                        && userCredentials.Identity.Name == program.RefCreators.Login;
                this.canEditDetail = this.canDeleteProgram;
                
                this.canApprove = program != null 
                                  && userCredentials.Identity.Name == program.RefCreators.Login 
                                  && program.RefApYear == null;
            }

            if (userCredentials.IsInRole(ProgramRoles.Viewer))
            {
                this.canDeleteProgram = false;
                this.canEditDetail = false;
                this.canApprove = false;
            }
        }

        public bool CanDeleteProgram
        {
            get { return this.canDeleteProgram; }
        }

        public bool CanEditDetail
        {
            get { return this.canEditDetail; }
        }

        public bool CanApprove
        {
            get { return this.canApprove; }
        }
    }
}
