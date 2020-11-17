using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkMapperRR_FIT
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            LoadSettings();
        }

        string franzITPath = @"\\W2K16DMS\Transferroad\";
        string roadrunnerPath = @"\\w2k16-PRT\Transferroad\";

        private void LoadSettings()
        {
            FillComboBox();
            cbProvider.SelectedIndex = BaseSettings.Default.DataProvider;
            btnFITSettings.Visible = true;


#if DEBUG
            btnStartPDS.Visible = true;
#endif
        }

        private void FillComboBox()
        {
            Dictionary<int, string> providers = new Dictionary<int, string>();

            providers.Add(0, "Roadrunner");
            providers.Add(1, "Franz IT");

            cbProvider.DataSource = providers.ToArray();

            cbProvider.DisplayMember = "Value";
            cbProvider.ValueMember = "Key";
        }

        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (IsProcessOpen("notepad++") == true)
            {
                MessageBox.Show("Bitte zuerst notepad++ beenden!");
                cbProvider.SelectedIndex = BaseSettings.Default.DataProvider;
                BaseSettings.Default.Save();
            }
#endif            

            if (IsProcessOpen("Preisdaten-Server") == false)
            {
                BaseSettings.Default.DataProvider = cbProvider.SelectedIndex;
                BaseSettings.Default.Save();
                lblProgress.Visible = true;
                lblProgress.Text = "Entferne altes Netzlaufwerk...";

                pbMain.Visible = true;
                pbMain.Value = 30;

                Process.Start("net.exe", "use x: /delete");                              
                pbMain.Value = 60;                
               
                
                //Roadrunner
                if (BaseSettings.Default.DataProvider == 0)
                {
                    //StopDownloadService("JHDownloader");
                    var confirmResult = MessageBox.Show("Wollen Sie den Inhalt des Roadrunner Ordners löschen?", "Achtung!!", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        DeleteFolderContent(roadrunnerPath);
                    }
                    
                    Process.Start("net.exe", "use x: \\\\w2k16-PRT\\Transferroad");                    
                    pbMain.Value = 100;                    
                    MessageBox.Show("Roadrunner wurde erfolgreich eingebunden.");
                    lblProgress.Visible = false;
                    pbMain.Visible = false;
                    btnStartPDS.Visible = true;

                }
                //Franz IT
                else if (BaseSettings.Default.DataProvider == 1)
                {                   

                    var confirmResult = MessageBox.Show("Wollen Sie den Inhalt des Franz IT Ordners löschen?", "Achtung!!", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        DeleteFolderContent(franzITPath);
#if DEBUG
                        DeleteFolderContent(@"C:\JHDownloader\Logs\");
#endif
                    }

                    //StartDownloadService("JHDownloader");
                    Process.Start("net.exe", "use x: \\\\W2K16DMS\\Transferroad");
                    pbMain.Value = 100;                    
                    MessageBox.Show("Franz IT wurde erfolgreich eingebunden.");
                    lblProgress.Visible = false;
                    pbMain.Visible = false;
                    btnStartPDS.Visible = true;
                }                                
            }
            else
            {
                MessageBox.Show("Bitte zuerst den Preisdaten-Server beenden!");
                cbProvider.SelectedIndex = BaseSettings.Default.DataProvider;
            }

            
            

        }

        private void cbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (IsProcessOpen("notepad++") == true)
            {
                MessageBox.Show("Bitte zuerst notepad++ beenden!");
                cbProvider.SelectedIndex = BaseSettings.Default.DataProvider;
            }
#endif

            if (IsProcessOpen("Preisdaten-Server") == true)
            {
                MessageBox.Show("Bitte zuerst den Preisdaten-Server beenden!");
                cbProvider.SelectedIndex = BaseSettings.Default.DataProvider;
            }    
            
        }


        private void btnStartPDS_Click(object sender, EventArgs e)
        {
#if DEBUG
            ProcessStartInfo testserv = new ProcessStartInfo(@"C:\Program Files (x86)\Everything\Everything.exe");
            testserv.UseShellExecute = false;
            testserv.Verb = "runas";
            Process.Start(testserv);

#endif
            ProcessStartInfo pdserv = new ProcessStartInfo(@"C:\PCSServer\Preisdaten - Server.exe");
            pdserv.UseShellExecute = false;
            pdserv.Verb = "runas";
            Process.Start(pdserv);
            
        }

        private void cbProvider_SelectionChangeCommitted(object sender, EventArgs e)
        {
            
        }

        public void DeleteFolderContent(string path)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(path);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
                pbMain.Value = 80;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Fehler");
            }

        } 

        private void btnFITSettings_Click(object sender, EventArgs e)
        {
            FITSettings fITSettings = new FITSettings();
            fITSettings.Show();
            
        }

        private void StartDownloadService(string name)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = "net";
                process.StartInfo.Arguments = "start " + name;
                process.StartInfo.Verb = "runas";//run as administrator
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }

        }

        private void StopDownloadService(string name)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = "net";
                process.StartInfo.Arguments = "stop " + name;
                process.StartInfo.Verb = "runas";//run as administrator
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }

        }

    }
}
