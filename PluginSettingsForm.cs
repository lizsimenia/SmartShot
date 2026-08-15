using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCurs
{
    public partial class PluginSettingsForm : Form
    {
        Plugin _pluginsHolder;
        string _pluginName;
        Button saveParamBtn;

        public PluginSettingsForm(Plugin PluginsHolder, string pluginName)
        {
            _pluginsHolder = PluginsHolder;
            _pluginName = pluginName;
            // найти textBox взять из него числа
            // посторить интерфейс с label + ввод + единицы измерения + кнопка срхранить
            InitializeComponent();
        }

        private void PluginSettingsForm_Load(object sender, EventArgs e)
        {

            if (_pluginName == "Цветовой тон/насыщенность")
            {
                this.Height = this.Height * 3 + 50;
            }
            if (_pluginName == "Умное освещение")
            {
                this.Height = this.Height + 50;
                this.Width = this.Width + 100;
            }
            _pluginsHolder.CreateShortSettingField(_pluginName, this);
            this.Name = $"{_pluginName}";
            this.Text = $"{_pluginName}";
            saveParamBtn = new Button();
            saveParamBtn.Parent = this;
            saveParamBtn.Width = 100;
            saveParamBtn.Height = 30;
            saveParamBtn.Cursor = Cursors.Hand;
            saveParamBtn.Left = (this.Width - saveParamBtn.Width)/2;
            saveParamBtn.Top = this.Height - 100;
            saveParamBtn.Text = "Добавить";
            saveParamBtn.Click += SaveParamBtn_Click;

        }

        private void SaveParamBtn_Click(object? sender, EventArgs e)
        {
            var ini_config = new Dictionary<string, string>();
            ini_config[_pluginsHolder.FPluginsList[_pluginName].PluginGUID] = _pluginsHolder.ParseSettings(_pluginName, this);
            _pluginsHolder.UpdateForConfig(ini_config);
            Close();
        }
    }
}
