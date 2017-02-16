using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using CasparCG.Osc.net;
using System.Configuration;


namespace OscWebSocketTester
{
    public partial class Form1 : Form
    {

        IDisposable disposeObject;

        public Form1()
        {
            InitializeComponent();
            VersionLabel.Text = "Version: " + Application.ProductVersion;
            trayIcon.Visible = true;
            StartHub();
                          
        }

        private void StartHub()
        {
            //Starting Signal R Self Hosting
            var url = ConfigurationManager.AppSettings["WebSocketUrl"];
            disposeObject = WebApp.Start(url);
            Logger.logInfo(string.Format("WebSocket Server running on {0}", url));

            //Getting App Variable
            Logger.logInfo(string.Format("Watching for {0} in xml config", "OscHost"));
            var ip = ConfigurationManager.AppSettings["OscHost"];
            Logger.logInfo(string.Format("{0} setted to {1}", "OscHost", ip));

            Logger.logInfo(string.Format("Watching for {0} in xml config", "OscPort"));
            var port = Int32.Parse(ConfigurationManager.AppSettings["OscPort"]);
            Logger.logInfo(string.Format("{0} setted to {1}", "OscHost", port));

            Logger.logInfo(string.Format("Watching for {0} in xml config", "OscPrimaryChannel"));
            var channel = int.Parse(ConfigurationManager.AppSettings["OscPrimaryChannel"]);
            Logger.logInfo(string.Format("{0} setted to {1}", "OscHost", channel));

            Logger.logInfo(string.Format("Watching for {0} in xml config", "OscPrimaryLayer"));
            var layer = int.Parse(ConfigurationManager.AppSettings["OscPrimaryLayer"]);
            Logger.logInfo(string.Format("{0} setted to {1}", "OscHost", layer));

            //Starting OSC Relay, this module catch OSC notifications and send them to the WebSocket Hub.
            Logger.logInfo(string.Format("Starting OSC Hub Relay, listenning OSC Message on {0}:{1}", ip, port));
            OscHubRelay t = new OscHubRelay(ip, port);
            Logger.logInfo("Transmitter started");
            Logger.logInfo(string.Format("Primary channel is: {0}. Primary layer is: {1}", t.PrimaryChannel, t.PrimaryLayer));
        }

        //For the app tray
        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                disposeObject.Dispose();
                // Release the icon resource.
                trayIcon.Dispose();
            }

            if (isDisposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(isDisposing);
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowInfosControls();
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }
        
        //Edit Variable with UI
        private void ShowInfosControls()
        {
            urlTextBox.Text = ConfigurationManager.AppSettings["WebSocketUrl"];
            ipTextBox.Text = ConfigurationManager.AppSettings["OscHost"];
            portTextBox.Text = ConfigurationManager.AppSettings["OscPort"];
        }
        private void SaveCongig()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            Uri uriResult;
            if (Uri.TryCreate(urlTextBox.Text, UriKind.Absolute, out uriResult))
            {
                config.AppSettings.Settings["WebSocketUrl"].Value = urlTextBox.Text;

            }
            else
            {
                urlTextBox.ValidValue = false;
                return;

            }

            System.Net.IPAddress Ip;
            if (System.Net.IPAddress.TryParse(ipTextBox.Text, out Ip))
            {
                config.AppSettings.Settings["OscHost"].Value = ipTextBox.Text;

            }
            else
            {
                ipTextBox.ValidValue = false;
                return;
            }

            int port;
            if (int.TryParse(portTextBox.Text, out port))
            {
                config.AppSettings.Settings["OscPort"].Value = portTextBox.Text;
            }
            else
            {

                portTextBox.ValidValue = false;
                return;
            }

            config.Save(ConfigurationSaveMode.Modified);
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;

        }

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {
            var ctl = sender as InfluenceTheme.InfluenceControls.InfluenceTextBoxValidation;
            ctl.ValidValue = true;
        }
        private void ipTextBox_TextChanged(object sender, EventArgs e)
        {
            var ctl = sender as InfluenceTheme.InfluenceControls.InfluenceIpBox;
            ctl.ValidValue = false;
        }
        private void btnValidate_Click(object sender, EventArgs e)
        {
            SaveCongig();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
        }

    }


    //Need for OWIN 
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }



}
