using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Management;
using Microsoft.Win32;
using System.Diagnostics;

namespace WinServicesMgr
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static readonly ServiceController[] services = ServiceController.GetServices();

        private void WinServicesMgr_Load(object sender, EventArgs e)
        {
            lvServices.View = View.Details;
            lvServices.GridLines = true;
            lvServices.FullRowSelect = true;
            lvServices.MultiSelect = false;
            lvServices.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvServices.Sorting = SortOrder.Ascending;

            foreach (ServiceController service in services)
            {
                lvServices.Items.Add(new ListViewItem(new string[] { service.DisplayName, service.ServiceName, service.Status.ToString() }));
            }
        }

        private void lvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lvServices.SelectedIndices.Count > 0)
                {
                    string selectedServiceName = lvServices.SelectedItems[0].SubItems[1].Text;
                    services.ToList().ForEach((service) =>
                    {
                        if (service.ServiceName == selectedServiceName)
                        {
                            var servis = new ManagementObject(new ManagementPath(string.Format($"Win32_Service.Name='{service.ServiceName}'")));
                            if (servis["Description"] != null) { rtbDescription.Text = servis["Description"].ToString(); }
                            else { rtbDescription.Text = ""; }
                        }
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void lvServices_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string selectedServiceName = lvServices.SelectedItems[0].SubItems[1].Text;

            /* Regedit aciq idise, birbawa acilmasini istediyimiz hedef Keye yonlendirib gostere bilmerik, cunki artiq Regedit aciqdir. Yonlendirmemiwden qabaq regediti baglamaliyiq, baglamasaq, aciq qalsa regedit yeni acmaga caliwdigimiz regedit acilmayaq evvelceden aciq olan regedit penceresine fokuslanacayiq. Ona gorede regediti baglayiriq Keyi acmamiwdan qabaq: */
            foreach (Process proc in Process.GetProcessesByName("regedit")) proc.Kill();

            /* Regedit acilanda fokuslanmagini istediyim Key: */
            var Path = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{selectedServiceName}");

            /* Reyestrda son aciq qalan yolu saxlayan Keydir awagidaki, bu Keyde olan yol acilir Regedit proqrami acilanda. Eger bu keyin deyerini acilmasini istediyimiz Key ile deyiwsek, demeli regedit acilanda son aciq qalan Key olaraq bizim verdiyimiz Keyi bilecek: */
            Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit").SetValue("LastKey", Path);

            /* Regedit-i bawlat: */
            Process.Start("regedit");
        }
    }
}