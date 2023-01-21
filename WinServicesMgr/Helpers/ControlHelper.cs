using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;

namespace WinServicesMgr.Helpers
{
    /// <summary>
    /// Form Controllari ile elaqeli komekci sinif.
    /// </summary>
    static class ControlHelper
    {
        /// <summary>
        /// Servisleri ListView-ya elave edir ve elave edilmiw hemin servisleri statuslari esasinda arxaplanini renglendirir.
        /// </summary>
        /// <param name="ListViewControl">Icerisine element elave edeceyimiz ListView.</param>
        /// <param name="ServiceName">Elave edilecek 'ServiceName'.</param>
        /// <param name="ServiceStatus">Elave edilecek 'ServiceStatus'.</param>
        /// <param name="DisplayName">Elave edilecek 'DisplayName'.</param>
        public static void AddToListViewAndBeautify(ListView ListViewControl, string ServiceName, ServiceControllerStatus ServiceStatus, string DisplayName)
        {
            ListViewItem listViewItem = new ListViewItem(new string[] { ServiceName, ServiceStatus.ToString(), DisplayName });
            switch (ServiceStatus)
            {
                case ServiceControllerStatus.Stopped:
                case ServiceControllerStatus.StopPending: listViewItem.BackColor = Color.OrangeRed; break;

                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.Running: listViewItem.BackColor = Color.LawnGreen; break;

                case ServiceControllerStatus.PausePending:
                case ServiceControllerStatus.Paused: listViewItem.BackColor = Color.Yellow; break;

                default: listViewItem.BackColor = Color.White; break;
            }

            ListViewControl.Items.Add(listViewItem);
        }
    }
}