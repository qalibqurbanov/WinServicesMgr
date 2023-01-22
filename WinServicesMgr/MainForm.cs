using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using System.Management;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.ServiceProcess;
using WinServicesMgr.Helpers;
using WinServicesMgr.Entities;
using System.Collections.Generic;

namespace WinServicesMgr
{
    public partial class MainForm : Form
    {
        #region Vars
        /// <summary>
        /// Servisleri ('DisplayName'-i A-dan Z-ye dogru siralanmiw bir wekilde) ozunde saxlayir.
        /// </summary>
        private static readonly ServiceController[] services = ServiceController.GetServices().OrderBy(x => x.DisplayName).ToArray<ServiceController>();

        /// <summary>
        /// Servis haqqinda melumatlari yazacagim fayl.
        /// </summary>
        private static string DestinationFilePath = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(@"\")) + @"\ServicesList.json";
        #endregion Vars

        public MainForm()
        {
            InitializeComponent();

            #region Form settings
            this.Text = "WinServicesMgr";
            this.Icon = WinServicesMgr.Properties.Resources.app;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Font = new Font("Segoe UI", 12);
            this.StartPosition = FormStartPosition.CenterScreen;
            #endregion Form settings

            #region ListView settings
            lvServices.View = View.Details;
            lvServices.GridLines = true;
            lvServices.FullRowSelect = true;
            lvServices.MultiSelect = false;
            lvServices.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            #endregion ListView settings
        }

        private void WinServicesMgr_Load(object sender, EventArgs e)
        {
            if (File.Exists(DestinationFilePath))
            {
                if (new FileInfo(DestinationFilePath).Length > 0)
                {
                    try
                    {
                        List<ServiceEntity> resultEntity = JsonHelper<ServiceEntity>.Deserialize(DestinationFilePath);
                        foreach (var entity in resultEntity)
                        {
                            ControlHelper.AddToListViewAndBeautify(lvServices, entity.ServiceName, entity.ServiceStartMode, entity.DisplayName);
                        }
                    }
                    catch { lvServices.Items.Clear(); }
                }
            }
            else
            {
                List<ServiceEntity> listOfServiceEntity = new List<ServiceEntity>();

                foreach (ServiceController service in services)
                {
                    listOfServiceEntity.Add(new ServiceEntity() { DisplayName = service.DisplayName, ServiceName = service.ServiceName, ServiceStartMode = service.StartType });

                    ControlHelper.AddToListViewAndBeautify(lvServices, service.ServiceName, service.StartType, service.DisplayName);
                }

                JsonHelper<ServiceEntity>.Serialize
                (
                    Entity: listOfServiceEntity,
                    FilePath: DestinationFilePath
                );
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
                            rtbDescription.Text = servis["Description"] != null ? servis["Description"].ToString() : "No Description Found";

                            rtbDescription.Text = rtbDescription.Text.TrimEnd().EndsWith(".") ? rtbDescription.Text : rtbDescription.Text.Insert(rtbDescription.Text.Length, ".");
                        }
                    });
                }
            }
            catch { }
        }

        private void lvServices_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog SFD = new SaveFileDialog())
            {

            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog())
            {

            }
        }
    }
}
