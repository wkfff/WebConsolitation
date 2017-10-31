using System.Web.Security;

namespace Krista.FM.RIA.Core.Controllers
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        
        string LastError();
    }
}