using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
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

        private void LoadSettings()
        {
            FillComboBox();
            cbProvider.SelectedIndex = BaseSettings.Default.DataProvider;
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

                System.Diagnostics.Process.Start("net.exe", "use x: /delete");                              
                pbMain.Value = 60;                
                pbMain.Value = 80;
                

                if (BaseSettings.Default.DataProvider == 0)
                {                    
                    System.Diagnostics.Process.Start("net.exe", "use x: \\\\w2k16-PRT\\Transferroad");
                    pbMain.Value = 100;                    
                    MessageBox.Show("Roadrunner wurde erfolgreich eingebunden.");
                    lblProgress.Visible = false;
                    pbMain.Visible = false;
                    btnStartPDS.Visible = true;

                }
                else if (BaseSettings.Default.DataProvider == 1)
                {                    
                    System.Diagnostics.Process.Start("net.exe", "use x: \\\\W2K16DMS\\Transferroad");
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
            ProcessStartInfo pdserv = new ProcessStartInfo(@"C:\Program Files (x86)\Everything\Everything.exe"); 
            pdserv.UseShellExecute = false;
            pdserv.Verb = "runas";
            Process.Start(pdserv);
            //"C:\\PCSServer\\Preisdaten - Server.exe"
        }
    }
}
