namespace AppCurs
{
    partial class InfoPlugins
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridViewPlugins = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPlugins).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewPlugins
            // 
            dataGridViewPlugins.AllowUserToAddRows = false;
            dataGridViewPlugins.AllowUserToDeleteRows = false;
            dataGridViewPlugins.AllowUserToResizeColumns = false;
            dataGridViewPlugins.AllowUserToResizeRows = false;
            dataGridViewPlugins.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewPlugins.BackgroundColor = SystemColors.Control;
            dataGridViewPlugins.BorderStyle = BorderStyle.None;
            dataGridViewPlugins.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridViewPlugins.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPlugins.Dock = DockStyle.Fill;
            dataGridViewPlugins.Location = new Point(0, 0);
            dataGridViewPlugins.Name = "dataGridViewPlugins";
            dataGridViewPlugins.ReadOnly = true;
            dataGridViewPlugins.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewPlugins.RowHeadersWidth = 51;
            dataGridViewPlugins.ScrollBars = ScrollBars.Vertical;
            dataGridViewPlugins.ShowCellToolTips = false;
            dataGridViewPlugins.Size = new Size(825, 450);
            dataGridViewPlugins.TabIndex = 0;
            // 
            // InfoPlugins
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(825, 450);
            Controls.Add(dataGridViewPlugins);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(400, 200);
            Name = "InfoPlugins";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Информация о плагинах";
            Load += Info_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewPlugins).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewPlugins;
    }
}