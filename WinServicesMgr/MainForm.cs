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
    /// <summary>
    /// Proqramin esas penceresi.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Vars
        /// <summary>
        /// Servisleri ('DisplayName'-i A-dan Z-ye dogru siralanmiw bir wekilde) ozunde saxlayir.
        /// </summary>
        private static readonly ServiceController[] services = ServiceController.GetServices().OrderBy(x => x.DisplayName).ToArray<ServiceController>();

        /// <summary>
        /// Proqram acilanda ilk bawda servisleri cacheleyib, sonraki appin aciliwlarinda servisleri cachelediyim fayldan oxuyacam. Bu deyiwen mehz proqram acilan zaman iwledeceyim hemin cache json faylini temsil edir. (+ Hemde etdiyimiz servis deyiwikliklerinden razi olmamagimiz veya deyiwikliklerin sebeb oldugu bir problem movcuddursa bu fayli backup fayli kimide iwletmek olar)
        /// </summary>
        private static string CacheFilePath = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(@"\")) + @"\ServicesList.json";
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
            if (File.Exists(CacheFilePath))
            {
                if (new FileInfo(CacheFilePath).Length > 0)
                {
                    try
                    {
                        List<ServiceEntity> resultEntity = JsonHelper<ServiceEntity>.Deserialize(CacheFilePath);
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
                    FilePath: CacheFilePath
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
                    foreach (ServiceController service in services)
                    {

                        if (service.ServiceName == selectedServiceName)
                        {
                            var servis = new ManagementObject(new ManagementPath(string.Format($"Win32_Service.Name='{service.ServiceName}'")));
                            rtbDescription.Text = servis["Description"] != null ? servis["Description"].ToString() : "No Description Found";

                            rtbDescription.Text = rtbDescription.Text.TrimEnd().EndsWith(".") ? rtbDescription.Text : rtbDescription.Text.Insert(rtbDescription.Text.Length, ".");
                        }
                    }
                }
            }
            catch { }
        }

        private void lvServices_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
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
                catch { }
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
                SFD.Title = "WinServicesMgr";
                SFD.FileName = Guid.NewGuid().ToString().Replace("-", "");
                SFD.RestoreDirectory = true;
                SFD.OverwritePrompt = true;
                SFD.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                SFD.DefaultExt = ".json";
                SFD.Filter = "JSON file|*.json";

                if (SFD.ShowDialog() == DialogResult.OK)
                {
                    string SelectedFilePath = SFD.FileName;

                    List<ServiceStateEntity> listOfServiceEntity = new List<ServiceStateEntity>();

                    foreach (ServiceController service in services)
                    {
                        listOfServiceEntity.Add(new ServiceStateEntity() { ServiceName = service.ServiceName, ServiceStartMode = service.StartType });
                    }

                    JsonHelper<ServiceStateEntity>.Serialize
                    (
                        Entity: listOfServiceEntity,
                        FilePath: SelectedFilePath
                    );
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog())
            {
                OFD.Title = "WinServicesMgr";
                OFD.RestoreDirectory = true;
                OFD.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                OFD.DefaultExt = ".json";
                OFD.Filter = "JSON file|*.json";

                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        List<ServiceStateEntity> resultEntity = JsonHelper<ServiceStateEntity>.Deserialize(CacheFilePath);

                        int result = RegistryHelper.ChangeServiceStartMode(resultEntity);

                        lvServices.Items.Clear();
                        foreach (ServiceController service in services)
                            ControlHelper.AddToListViewAndBeautify(lvServices, service.ServiceName, service.StartType, service.DisplayName);

                        if(result > 0)
                            MessageBox.Show($"x{result} service state changed.");
                        else if(result <= 0)
                            MessageBox.Show("0 service state changed.");
                    }
                    catch { lvServices.Items.Clear(); }
                }
            }
        }
    }
}