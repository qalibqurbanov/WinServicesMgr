using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace WinServicesMgr.Helpers
{
    public class UserHelper
    {
        public static bool IsUserAdministrator()
        {
            bool isAdmin = false;

            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);

                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException error)
            {
                MessageBox.Show(error.Message);
                Environment.Exit(0);

                isAdmin = false;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                Environment.Exit(0);

                isAdmin = false;
            }

            return isAdmin;
        }
    }
}