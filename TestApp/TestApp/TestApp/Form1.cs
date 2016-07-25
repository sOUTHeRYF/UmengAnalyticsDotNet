using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Forms;

using UmengSDK;
namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit;
            UmengAnalytics.IsDebug = true;
            UmengAnalytics.Init("5791c0a367e58e3370000aee", "TestGame.Yodo1", "0.1.1.0");
            UmengAnalytics.StartTrack();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            UmengAnalytics.EndTrack();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UmengAnalytics.TrackEvent("TestEvent");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Param is " + UmengAnalytics.GetOnlineParam("TestParam"));
        }
    }
}
