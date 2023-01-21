using System;
using System.Data;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;
using System.ServiceProcess;

namespace WinServicesMgr
{
    public partial class MainForm : Form
    {
        #region Vars
        /// <summary>
        /// Servisleri ('DisplayName'-i A-dan Z-ye dogru siralanmiw bir wekilde) ozunde saxlayir.
        /// </summary>
        private static readonly ServiceController[] services = ServiceController.GetServices().OrderBy(x => x.DisplayName).ToArray<ServiceController>();
        #endregion Vars

        public MainForm()
        {
            InitializeComponent();

            this.Text = "WinServicesMgr";
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Font = new Font("Segoe UI", 12);

            lvServices.View = View.Details;
            lvServices.GridLines = true;
            lvServices.FullRowSelect = true;
            lvServices.MultiSelect = false;
            lvServices.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        private void WinServicesMgr_Load(object sender, EventArgs e)
        {
            foreach (ServiceController service in services)
            {
                ListViewItem listViewItem = new ListViewItem(new string[] { service.ServiceName, service.Status.ToString(), service.DisplayName });
                switch (service.Status)
                {
                    case ServiceControllerStatus.Stopped:
                    case ServiceControllerStatus.StopPending: listViewItem.BackColor = Color.OrangeRed; break;

                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.Running: listViewItem.BackColor = Color.LawnGreen; break;

                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.Paused: listViewItem.BackColor = Color.Yellow; break;

                    default: listViewItem.BackColor = Color.White; break;
                }

                lvServices.Items.Add(listViewItem);
            }

            lvServices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvServices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void lvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lvServices.SelectedIndices.Count > 0)
                {
                    string selectedServiceName = lvServices.SelectedItems[0].SubItems[0].Text;
                    services.ToList().ForEach((service) =>
                    {
                        if (service.ServiceName == selectedServiceName)
                        {
                            var servis = new ManagementObject(new ManagementPath(string.Format($"Win32_Service.Name='{service.ServiceName}'")));
                            if (servis["Description"] != null) { rtbDescription.Text = servis["Description"].ToString(); }
                            else { rtbDescription.Text = "No Description Found"; }
                        }
                    });
                }
            }
            catch { }
        }

        private void lvServices_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string selectedServiceName = lvServices.SelectedItems[0].SubItems[0].Text;

            /* Regedit aciq idise, birbawa acilmasini istediyimiz hedef Keye yonlendirib gostere bilmerik, cunki artiq Regedit aciqdir. Yonlendirmemiwden qabaq regediti baglamaliyiq, baglamasaq, aciq qalsa regedit, yeni acmaga caliwdigimiz regedit acilmayaq ve evvelceden aciq olan regedit penceresine fokuslanacayiq. Ona gorede regediti baglayiriq Keyi acmamiwdan qabaq: */
            foreach (Process proc in Process.GetProcessesByName("regedit")) proc.Kill();

            /* Regedit acilanda fokuslanmagini istediyim Key: */
            var Path = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{selectedServiceName}");

            /* Reyestrda son aciq qalan yolu saxlayan Keydir awagidaki, bu Keyde olan yol acilir Regedit proqrami acilanda. Eger bu keyin deyerini acilmasini istediyimiz Key ile deyiwsek, demeli regedit acilanda son aciq qalan Key olaraq bizim verdiyimiz Keyi bilecek: */
            Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Applets\\Regedit").SetValue("LastKey", Path);

            /* Regedit-i bawlat: */
            Process.Start("regedit");
        }
    }
}