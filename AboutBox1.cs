using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCurs
{

    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.labelProductName.Text = "SmartShot";
            this.labelVersion.Text = "Версия 1.0";
            this.labelCopyright.Text = "Copyright © 2026";
            this.labelCompanyName.Text = "СибГУ";
            this.textBoxDescription.Text = "Приложение предназначено для обработки фотографий, исходное качество которых не соответствует коммерческим требованиям.";
        }

        private void AboutBox1_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
