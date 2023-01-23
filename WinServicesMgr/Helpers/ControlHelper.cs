using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;

namespace WinServicesMgr.Helpers
{
    /// <summary>
    /// Form-un Control-lari ile elaqeli komekci sinif.
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
        public static void AddToListViewAndBeautify(ListView ListViewControl, string ServiceName, ServiceStartMode ServiceStatus, string DisplayName)
        {
            ListViewItem listViewItem = new ListViewItem(new string[] { ServiceName, ServiceStatus.ToString(), DisplayName });
            switch (ServiceStatus)
            {
                case ServiceStartMode.Disabled: listViewItem.BackColor = Color.Tomato; break;
                case ServiceStartMode.Manual: listViewItem.BackColor = Color.LimeGreen; break;
                case ServiceStartMode.Automatic: listViewItem.BackColor = Color.LawnGreen; break;
                // LightSkyBlue

                default: listViewItem.BackColor = Color.White; break;
            }

            ListViewControl.Items.Add(listViewItem);
        }
    }
}