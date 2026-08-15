using System;
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
    public partial class InfoPlugins : Form
    {
        private Plugin _pluginsHolder;
        public InfoPlugins(Plugin pluginsHolder)
        {
            InitializeComponent();
            _pluginsHolder = pluginsHolder;
            this.Width = 960;
        }

        private void Info_Load(object sender, EventArgs e)
        {
            // Таблица информации о плагинах
            dataGridViewPlugins.AutoGenerateColumns = false;
            dataGridViewPlugins.RowHeadersVisible = false;

            dataGridViewPlugins.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewPlugins.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewPlugins.EnableHeadersVisualStyles = false;

            dataGridViewPlugins.DefaultCellStyle.SelectionBackColor = dataGridViewPlugins.DefaultCellStyle.BackColor;
            dataGridViewPlugins.DefaultCellStyle.SelectionForeColor = dataGridViewPlugins.DefaultCellStyle.ForeColor;
            dataGridViewPlugins.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridViewPlugins.Columns.Clear();

            dataGridViewPlugins.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Название",
                DataPropertyName = "PluginName",
                Width = 150
            });
            dataGridViewPlugins.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colType",
                HeaderText = "Тип",
                DataPropertyName = "PluginType",
                Width = 120
            });
            dataGridViewPlugins.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAuthor",
                HeaderText = "Автор",
                DataPropertyName = "PluginAuthor",
                Width = 200
            });
            dataGridViewPlugins.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVersion",
                HeaderText = "Версия",
                DataPropertyName = "PluginVersion",
                Width = 80
            });
            dataGridViewPlugins.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDataRealese",
                HeaderText = "Дата релиза",
                DataPropertyName = "PluginDataRealese",
                Width = 130
            });
            dataGridViewPlugins.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDesc",
                HeaderText = "Описание",
                DataPropertyName = "PluginDescription",
                Width = 250,
            });

            //dataGridViewPlugins.Columns["colDesc"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //dataGridViewPlugins.Columns["colName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
          
            var list = _pluginsHolder.basePluginsName
                .Select(name => _pluginsHolder.FPluginsList[name])
                .Select(p => new
                {
                    p.PluginName,
                    p.PluginType,
                    p.PluginAuthor,
                    p.PluginVersion,
                    p.PluginDataRealese,
                    p.PluginDescription
                })
                .ToList();

            dataGridViewPlugins.DataSource = list;
        }
    }
}
