﻿using System;
using System.Windows.Forms;
using Octgn.Client;
using System.Diagnostics;

namespace Octgn.WindowsClient
{
    public partial class Form1 : Form
    {
        public Server Server { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Server = new Server();
            Server.Start(@"/../../../Octgn.WebApp");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://localhost:9000/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Server.PingClients();
        }
    }
}
