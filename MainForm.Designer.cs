namespace AppCurs
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            labelTariff = new Label();
            label2 = new Label();
            label1 = new Label();
            labelDragAndDrop = new Label();
            SettingsField = new Panel();
            panel3 = new Panel();
            checkBoxSteps = new CheckBox();
            label4 = new Label();
            panel4 = new Panel();
            labelCancelConfig = new Label();
            buttonSaveAs = new Button();
            listBoxTypeEdit = new ListBox();
            label3 = new Label();
            buttonStartEdit = new Button();
            buttonsaveConf = new Button();
            panelLoading = new Panel();
            checkBoxPacketEdit = new CheckBox();
            buttonLoadPacket = new Button();
            buttonSelectPhoto = new Button();
            labelCancel = new Label();
            buttonLoadConf = new Button();
            splitContainer1 = new SplitContainer();
            panel2 = new Panel();
            panelConfig = new Panel();
            splitContainer2 = new SplitContainer();
            toolTipLicense = new ToolTip(components);
            menuStrip1 = new MenuStrip();
            файлToolStripMenuItem = new ToolStripMenuItem();
            openPhotoToolStripMenuItem1 = new ToolStripMenuItem();
            openFoldertoolStripMenuItem2 = new ToolStripMenuItem();
            configToolStripMenuItem = new ToolStripMenuItem();
            загрузитьФайлКонфигурацииToolStripMenuItem = new ToolStripMenuItem();
            сохранитьФайлКонфигурацииToolStripMenuItem = new ToolStripMenuItem();
            SaveAstoolStripMenuItem1 = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            пакетИзображенийToolStripMenuItem = new ToolStripMenuItem();
            chooseEditToolStripMenuItem = new ToolStripMenuItem();
            startEditToolStripMenuItem1 = new ToolStripMenuItem();
            AddRemovePluginsToolStripMenuItem = new ToolStripMenuItem();
            справкаToolStripMenuItem = new ToolStripMenuItem();
            InfoPluginsToolStripMenuItem = new ToolStripMenuItem();
            aboutPrToolStripMenuItem = new ToolStripMenuItem();
            toolTipPath = new ToolTip(components);
            panel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panelLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel2.SuspendLayout();
            panelConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(flowLayoutPanel1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 28);
            panel1.Name = "panel1";
            panel1.Size = new Size(982, 46);
            panel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(labelTariff);
            flowLayoutPanel1.Controls.Add(label2);
            flowLayoutPanel1.Dock = DockStyle.Right;
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Font = new Font("Segoe UI", 12F);
            flowLayoutPanel1.Location = new Point(624, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(358, 46);
            flowLayoutPanel1.TabIndex = 4;
            // 
            // labelTariff
            // 
            labelTariff.Anchor = AnchorStyles.Top;
            labelTariff.AutoSize = true;
            labelTariff.Location = new Point(264, 0);
            labelTariff.Name = "labelTariff";
            labelTariff.Size = new Size(91, 28);
            labelTariff.TabIndex = 1;
            labelTariff.Text = "Базовый";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.ImageAlign = ContentAlignment.MiddleLeft;
            label2.Location = new Point(184, 0);
            label2.Name = "label2";
            label2.Size = new Size(74, 28);
            label2.TabIndex = 2;
            label2.Text = "Тариф:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label1.Location = new Point(4, 6);
            label1.Name = "label1";
            label1.Size = new Size(133, 32);
            label1.TabIndex = 0;
            label1.Text = "SmartShot";
            // 
            // labelDragAndDrop
            // 
            labelDragAndDrop.AllowDrop = true;
            labelDragAndDrop.Cursor = Cursors.Hand;
            labelDragAndDrop.Dock = DockStyle.Fill;
            labelDragAndDrop.Location = new Point(0, 0);
            labelDragAndDrop.Name = "labelDragAndDrop";
            labelDragAndDrop.Size = new Size(426, 679);
            labelDragAndDrop.TabIndex = 1;
            labelDragAndDrop.Text = "Перетащите изображение";
            labelDragAndDrop.TextAlign = ContentAlignment.MiddleCenter;
            labelDragAndDrop.Click += labelDragAndDrop_Click;
            // 
            // SettingsField
            // 
            SettingsField.AutoSize = true;
            SettingsField.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SettingsField.Dock = DockStyle.Fill;
            SettingsField.Location = new Point(0, 557);
            SettingsField.Name = "SettingsField";
            SettingsField.Size = new Size(400, 122);
            SettingsField.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(checkBoxSteps);
            panel3.Controls.Add(label4);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(0, 470);
            panel3.Name = "panel3";
            panel3.Size = new Size(400, 87);
            panel3.TabIndex = 4;
            // 
            // checkBoxSteps
            // 
            checkBoxSteps.AutoSize = true;
            checkBoxSteps.Cursor = Cursors.Hand;
            checkBoxSteps.Location = new Point(22, 47);
            checkBoxSteps.Name = "checkBoxSteps";
            checkBoxSteps.Size = new Size(231, 24);
            checkBoxSteps.TabIndex = 1;
            checkBoxSteps.Text = "Несколько шагов обработки";
            checkBoxSteps.UseVisualStyleBackColor = true;
            checkBoxSteps.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label4
            // 
            label4.Dock = DockStyle.Top;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(0, 0);
            label4.Name = "label4";
            label4.Size = new Size(398, 32);
            label4.TabIndex = 0;
            label4.Text = "Выберите вид обработки";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(labelCancelConfig);
            panel4.Controls.Add(buttonSaveAs);
            panel4.Controls.Add(listBoxTypeEdit);
            panel4.Controls.Add(label3);
            panel4.Controls.Add(buttonStartEdit);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 245);
            panel4.Name = "panel4";
            panel4.Size = new Size(400, 225);
            panel4.TabIndex = 6;
            // 
            // labelCancelConfig
            // 
            labelCancelConfig.AutoSize = true;
            labelCancelConfig.Cursor = Cursors.Hand;
            labelCancelConfig.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            labelCancelConfig.ForeColor = SystemColors.ControlDarkDark;
            labelCancelConfig.Location = new Point(140, 194);
            labelCancelConfig.Name = "labelCancelConfig";
            labelCancelConfig.Size = new Size(75, 20);
            labelCancelConfig.TabIndex = 5;
            labelCancelConfig.Text = "Сбросить";
            labelCancelConfig.Click += labelCancelConfig_Click;
            // 
            // buttonSaveAs
            // 
            buttonSaveAs.Enabled = false;
            buttonSaveAs.Location = new Point(55, 162);
            buttonSaveAs.Name = "buttonSaveAs";
            buttonSaveAs.Size = new Size(254, 29);
            buttonSaveAs.TabIndex = 7;
            buttonSaveAs.Text = "Сохранить как";
            buttonSaveAs.UseVisualStyleBackColor = true;
            buttonSaveAs.Click += buttonSaveAs_Click;
            // 
            // listBoxTypeEdit
            // 
            listBoxTypeEdit.Dock = DockStyle.Top;
            listBoxTypeEdit.FormattingEnabled = true;
            listBoxTypeEdit.Location = new Point(0, 40);
            listBoxTypeEdit.Name = "listBoxTypeEdit";
            listBoxTypeEdit.Size = new Size(398, 84);
            listBoxTypeEdit.TabIndex = 1;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Top;
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Size = new Size(398, 40);
            label3.TabIndex = 5;
            label3.Text = "Выбранный вид обработки";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonStartEdit
            // 
            buttonStartEdit.Enabled = false;
            buttonStartEdit.Location = new Point(55, 127);
            buttonStartEdit.Name = "buttonStartEdit";
            buttonStartEdit.Size = new Size(254, 29);
            buttonStartEdit.TabIndex = 3;
            buttonStartEdit.Text = "Обработать";
            buttonStartEdit.UseVisualStyleBackColor = true;
            buttonStartEdit.Click += buttonStartEdit_Click;
            // 
            // buttonsaveConf
            // 
            buttonsaveConf.Location = new Point(56, 56);
            buttonsaveConf.Name = "buttonsaveConf";
            buttonsaveConf.Size = new Size(254, 29);
            buttonsaveConf.TabIndex = 4;
            buttonsaveConf.Text = "Сохранить конфигурацию";
            buttonsaveConf.UseVisualStyleBackColor = true;
            buttonsaveConf.Click += buttonsaveConf_Click;
            // 
            // panelLoading
            // 
            panelLoading.BorderStyle = BorderStyle.FixedSingle;
            panelLoading.Controls.Add(checkBoxPacketEdit);
            panelLoading.Controls.Add(buttonLoadPacket);
            panelLoading.Controls.Add(buttonSelectPhoto);
            panelLoading.Controls.Add(labelCancel);
            panelLoading.Dock = DockStyle.Top;
            panelLoading.Location = new Point(0, 0);
            panelLoading.Name = "panelLoading";
            panelLoading.Size = new Size(400, 139);
            panelLoading.TabIndex = 3;
            // 
            // checkBoxPacketEdit
            // 
            checkBoxPacketEdit.AutoSize = true;
            checkBoxPacketEdit.Location = new Point(92, 9);
            checkBoxPacketEdit.Name = "checkBoxPacketEdit";
            checkBoxPacketEdit.Size = new Size(174, 24);
            checkBoxPacketEdit.TabIndex = 4;
            checkBoxPacketEdit.Text = "Пакетная обработка";
            checkBoxPacketEdit.UseVisualStyleBackColor = true;
            checkBoxPacketEdit.CheckedChanged += checkBoxPacketEdit_CheckedChanged;
            // 
            // buttonLoadPacket
            // 
            buttonLoadPacket.Enabled = false;
            buttonLoadPacket.FlatStyle = FlatStyle.System;
            buttonLoadPacket.Location = new Point(55, 72);
            buttonLoadPacket.Name = "buttonLoadPacket";
            buttonLoadPacket.Size = new Size(254, 29);
            buttonLoadPacket.TabIndex = 3;
            buttonLoadPacket.Text = "Загрузить пакет фотографий";
            buttonLoadPacket.UseVisualStyleBackColor = true;
            buttonLoadPacket.Click += buttonLoadPacket_Click;
            // 
            // buttonSelectPhoto
            // 
            buttonSelectPhoto.FlatStyle = FlatStyle.System;
            buttonSelectPhoto.Location = new Point(55, 37);
            buttonSelectPhoto.Name = "buttonSelectPhoto";
            buttonSelectPhoto.Size = new Size(254, 29);
            buttonSelectPhoto.TabIndex = 0;
            buttonSelectPhoto.Text = "Загрузить фотографию";
            buttonSelectPhoto.UseVisualStyleBackColor = true;
            buttonSelectPhoto.Click += buttonSelectPhoto_Click;
            // 
            // labelCancel
            // 
            labelCancel.AutoSize = true;
            labelCancel.Cursor = Cursors.Hand;
            labelCancel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            labelCancel.ForeColor = SystemColors.ControlDarkDark;
            labelCancel.Location = new Point(140, 103);
            labelCancel.Name = "labelCancel";
            labelCancel.Size = new Size(75, 20);
            labelCancel.TabIndex = 2;
            labelCancel.Text = "Сбросить";
            labelCancel.Click += labelCancel_Click;
            // 
            // buttonLoadConf
            // 
            buttonLoadConf.Location = new Point(56, 21);
            buttonLoadConf.Name = "buttonLoadConf";
            buttonLoadConf.Size = new Size(254, 29);
            buttonLoadConf.TabIndex = 1;
            buttonLoadConf.Text = "Загрузить конфигурацию";
            buttonLoadConf.UseVisualStyleBackColor = true;
            buttonLoadConf.Click += buttonLoadConf_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 74);
            splitContainer1.MinimumSize = new Size(300, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = SystemColors.ButtonHighlight;
            splitContainer1.Panel1.Controls.Add(SettingsField);
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Resize += splitContainer1_Panel1_Resize;
            splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Panel2.Resize += splitContainer1_Panel2_Resize;
            splitContainer1.Panel2MinSize = 300;
            splitContainer1.Size = new Size(982, 679);
            splitContainer1.SplitterDistance = 400;
            splitContainer1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.AutoScroll = true;
            panel2.AutoSize = true;
            panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(panelConfig);
            panel2.Controls.Add(panelLoading);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(400, 557);
            panel2.TabIndex = 6;
            // 
            // panelConfig
            // 
            panelConfig.BorderStyle = BorderStyle.FixedSingle;
            panelConfig.Controls.Add(buttonLoadConf);
            panelConfig.Controls.Add(buttonsaveConf);
            panelConfig.Dock = DockStyle.Top;
            panelConfig.Location = new Point(0, 139);
            panelConfig.Name = "panelConfig";
            panelConfig.Size = new Size(400, 106);
            panelConfig.TabIndex = 5;
            // 
            // splitContainer2
            // 
            splitContainer2.BackColor = SystemColors.ControlLightLight;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(labelDragAndDrop);
            splitContainer2.Panel1.RightToLeft = RightToLeft.No;
            splitContainer2.Panel1.Paint += pictureBefore_Paint;
            splitContainer2.Panel1.MouseDown += splitContainer2_Panel1_MouseDown;
            splitContainer2.Panel1MinSize = 0;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Paint += pictureAfter_Paint;
            splitContainer2.Panel2MinSize = 0;
            splitContainer2.Size = new Size(578, 679);
            splitContainer2.SplitterDistance = 426;
            splitContainer2.SplitterWidth = 10;
            splitContainer2.TabIndex = 2;
            splitContainer2.SplitterMoved += splitContainer2_SplitterMoved_1;
            splitContainer2.Resize += splitContainer2_Resize;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem, пакетИзображенийToolStripMenuItem, AddRemovePluginsToolStripMenuItem, справкаToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(982, 28);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openPhotoToolStripMenuItem1, openFoldertoolStripMenuItem2, configToolStripMenuItem, SaveAstoolStripMenuItem1, closeToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(59, 24);
            файлToolStripMenuItem.Text = "Файл";
            // 
            // openPhotoToolStripMenuItem1
            // 
            openPhotoToolStripMenuItem1.Name = "openPhotoToolStripMenuItem1";
            openPhotoToolStripMenuItem1.Size = new Size(292, 26);
            openPhotoToolStripMenuItem1.Text = "Открыть фотографию...";
            openPhotoToolStripMenuItem1.Click += openPhotoToolStripMenuItem1_Click;
            // 
            // openFoldertoolStripMenuItem2
            // 
            openFoldertoolStripMenuItem2.Name = "openFoldertoolStripMenuItem2";
            openFoldertoolStripMenuItem2.Size = new Size(292, 26);
            openFoldertoolStripMenuItem2.Text = "Открыть папку фотографий...";
            openFoldertoolStripMenuItem2.Click += openFoldertoolStripMenuItem2_Click;
            // 
            // configToolStripMenuItem
            // 
            configToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { загрузитьФайлКонфигурацииToolStripMenuItem, сохранитьФайлКонфигурацииToolStripMenuItem });
            configToolStripMenuItem.Name = "configToolStripMenuItem";
            configToolStripMenuItem.Size = new Size(292, 26);
            configToolStripMenuItem.Text = "Конфигурация";
            // 
            // загрузитьФайлКонфигурацииToolStripMenuItem
            // 
            загрузитьФайлКонфигурацииToolStripMenuItem.Name = "загрузитьФайлКонфигурацииToolStripMenuItem";
            загрузитьФайлКонфигурацииToolStripMenuItem.Size = new Size(310, 26);
            загрузитьФайлКонфигурацииToolStripMenuItem.Text = "Загрузить файл конфигурации";
            загрузитьФайлКонфигурацииToolStripMenuItem.Click += загрузитьФайлКонфигурацииToolStripMenuItem_Click;
            // 
            // сохранитьФайлКонфигурацииToolStripMenuItem
            // 
            сохранитьФайлКонфигурацииToolStripMenuItem.Name = "сохранитьФайлКонфигурацииToolStripMenuItem";
            сохранитьФайлКонфигурацииToolStripMenuItem.Size = new Size(310, 26);
            сохранитьФайлКонфигурацииToolStripMenuItem.Text = "Сохранить файл конфигурации";
            сохранитьФайлКонфигурацииToolStripMenuItem.Click += сохранитьФайлКонфигурацииToolStripMenuItem_Click;
            // 
            // SaveAstoolStripMenuItem1
            // 
            SaveAstoolStripMenuItem1.Name = "SaveAstoolStripMenuItem1";
            SaveAstoolStripMenuItem1.Size = new Size(292, 26);
            SaveAstoolStripMenuItem1.Text = "Сохранить как...";
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(292, 26);
            closeToolStripMenuItem.Text = "Выход";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // пакетИзображенийToolStripMenuItem
            // 
            пакетИзображенийToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { chooseEditToolStripMenuItem, startEditToolStripMenuItem1 });
            пакетИзображенийToolStripMenuItem.Name = "пакетИзображенийToolStripMenuItem";
            пакетИзображенийToolStripMenuItem.Size = new Size(99, 24);
            пакетИзображенийToolStripMenuItem.Text = "Обработка";
            // 
            // chooseEditToolStripMenuItem
            // 
            chooseEditToolStripMenuItem.Name = "chooseEditToolStripMenuItem";
            chooseEditToolStripMenuItem.Size = new Size(237, 26);
            chooseEditToolStripMenuItem.Text = "Вид обработки";
            chooseEditToolStripMenuItem.DropDownOpening += chooseEditToolStripMenuItem_DropDownOpening;
            // 
            // startEditToolStripMenuItem1
            // 
            startEditToolStripMenuItem1.Name = "startEditToolStripMenuItem1";
            startEditToolStripMenuItem1.Size = new Size(237, 26);
            startEditToolStripMenuItem1.Text = "Запустить обработку";
            startEditToolStripMenuItem1.Click += startEditToolStripMenuItem1_Click;
            // 
            // AddRemovePluginsToolStripMenuItem
            // 
            AddRemovePluginsToolStripMenuItem.Name = "AddRemovePluginsToolStripMenuItem";
            AddRemovePluginsToolStripMenuItem.Size = new Size(85, 24);
            AddRemovePluginsToolStripMenuItem.Text = "Плагины";
            AddRemovePluginsToolStripMenuItem.DropDownOpening += AddRemovePluginsToolStripMenuItem_DropDownOpening;
            // 
            // справкаToolStripMenuItem
            // 
            справкаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { InfoPluginsToolStripMenuItem, aboutPrToolStripMenuItem });
            справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            справкаToolStripMenuItem.Size = new Size(81, 24);
            справкаToolStripMenuItem.Text = "Справка";
            // 
            // InfoPluginsToolStripMenuItem
            // 
            InfoPluginsToolStripMenuItem.Name = "InfoPluginsToolStripMenuItem";
            InfoPluginsToolStripMenuItem.Size = new Size(265, 26);
            InfoPluginsToolStripMenuItem.Text = "Установленные плагины";
            InfoPluginsToolStripMenuItem.Click += InfoPluginsToolStripMenuItem_Click;
            // 
            // aboutPrToolStripMenuItem
            // 
            aboutPrToolStripMenuItem.Name = "aboutPrToolStripMenuItem";
            aboutPrToolStripMenuItem.Size = new Size(265, 26);
            aboutPrToolStripMenuItem.Text = "О программе";
            aboutPrToolStripMenuItem.Click += aboutPrToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 753);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            KeyPreview = true;
            MinimumSize = new Size(1000, 800);
            Name = "MainForm";
            Text = "SmartShot";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            Resize += Form1_Resize;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panelLoading.ResumeLayout(false);
            panelLoading.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panelConfig.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label labelDragAndDrop;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label2;
        private Label labelTariff;
        private Panel panel4;
        private Button buttonsaveConf;
        private Button buttonStartEdit;
        private Panel panel3;
        private CheckBox checkBoxSteps;
        private Label label4;
        private Panel panelLoading;
        private Button buttonSelectPhoto;
        private Button buttonLoadConf;
        private Label labelCancel;
        private Label label3;
        private Panel SettingsField;
        private SplitContainer splitContainer1;
        private Button buttonSaveAs;
        private ToolTip toolTipLicense;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem пакетИзображенийToolStripMenuItem;
        private ToolStripMenuItem chooseEditToolStripMenuItem;
        private ToolStripMenuItem startEditToolStripMenuItem1;
        private ToolStripMenuItem справкаToolStripMenuItem;
        private ToolStripMenuItem InfoPluginsToolStripMenuItem;
        private ToolStripMenuItem aboutPrToolStripMenuItem;
        private ToolTip toolTipPath;
        private Panel drawingPanelBefore;
        private SplitContainer splitContainer2;
        private Panel drawingPanelAfter;
        private Button buttonLoadPacket;
        private CheckBox checkBoxPacketEdit;
        private Panel panelConfig;
        private Label labelCancelConfig;
        private Panel panel2;
        private ToolStripMenuItem configToolStripMenuItem;
        private ToolStripMenuItem загрузитьФайлКонфигурацииToolStripMenuItem;
        private ToolStripMenuItem сохранитьФайлКонфигурацииToolStripMenuItem;
        private ToolStripMenuItem openPhotoToolStripMenuItem1;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem SaveAstoolStripMenuItem1;
        private ToolStripMenuItem openFoldertoolStripMenuItem2;
        private ToolStripMenuItem AddRemovePluginsToolStripMenuItem;
        private ListBox listBoxTypeEdit;
    }
}
