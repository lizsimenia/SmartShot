using Opulos.Core.UI;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AppCurs
{
    public partial class MainForm : Form
    {
        private Plugin PluginsHolder;
        public Accordion acc { get; set; }
        public List<string> openBoxes { get; set; }

        private Bitmap inBitmap = null;
        private Bitmap outBitmap = null;

        // Переменные для хранения изображений
        private System.Drawing.Image pictureBox1;
        private System.Drawing.Image pictureBox2;

        private string _saveImagePath;
        private string _initImagePath;

        private Dictionary<string, string> _config;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox textInfoProgress;
        private System.Windows.Forms.Panel packetPanel;
        private System.Windows.Forms.ListBox listBoxEditFiles;
        private PictureBox previewPictureBox;

        public List<string> basePluginsName = new List<string> { "Контраст", "Экспозиция", "Умное освещение", "Цветовой тон/насыщенность" };
        public List<string> showPluginsName = new List<string> { };
        public List<string> onFormPluginsName = new List<string> { };

        private DatabaseManager dbMan;
        private License license;

        //LicenseData currentLicense = new LicenseData();

        // Константы для обработки системных сообщений Windows об изменении состояния устройств
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        System.Windows.Forms.Label lbForPacket;
        public System.Windows.Forms.Timer usbCheckTimer;

        // Обработчик системных сообщений
        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_DEVICECHANGE)
            {
                switch ((int)msg.WParam)
                {
                    case DBT_DEVICEARRIVAL:
                        {
                            usbCheckTimer.Stop();
                            usbCheckTimer.Start();
                            break;
                        }

                    case DBT_DEVICEREMOVECOMPLETE:
                        labelTariff.Text = "Базовый";
                        toolTipLicense.Hide(labelTariff);
                        toolTipLicense.SetToolTip(labelTariff, null);
                        foreach (var name in showPluginsName)
                        {
                            PluginsHolder.RemovePluginInterface(name);
                            basePluginsName.Remove(name);
                        }
                        ;
                        showPluginsName.Clear();
                        dbMan.LogActivity(5, 1);
                        break;

                    default:
                        break;
                }
            }
            base.WndProc(ref msg);

        }

        // Таймер на чтение usb
        private void UsbCheckTimer_Tick(object sender, EventArgs e)
        {
            usbCheckTimer.Stop();
            license.CheckUsbForLicense();
        }

        public MainForm()
        {
            openBoxes = new List<string>();
            // Подключение БД
            string connString = "Server=localhost;Port=3306;Database=databasesmartshot;Uid=root;Pwd=;";
            dbMan = new DatabaseManager(connString);
            InitializeComponent();

            try
            {
                PluginsHolder = new Plugin();
                PluginsHolder.Form = this;
                PluginsHolder.SettingsField = SettingsField;
                PluginsHolder.listBoxTypeEdit = listBoxTypeEdit;
                PluginsHolder.basePluginsName = basePluginsName;
                PluginsHolder.showPluginsName = showPluginsName;
                PluginsHolder.onFormPluginsName = onFormPluginsName;
                // Определяем путь к папке с плагинами
                string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

                // Загружаем плагины
                PluginsHolder.LoadPlugins(pluginPath, "*.dll");
                PluginsHolder.BuildPluginsInterface();

                // Проверка лицензии
                license = new License(dbMan, labelTariff, toolTipLicense, showPluginsName, PluginsHolder);
                usbCheckTimer = new System.Windows.Forms.Timer();
                usbCheckTimer.Interval = 1000;
                usbCheckTimer.Tick += UsbCheckTimer_Tick;
                license.CheckUsbForLicense();

                labelDragAndDrop.DragDrop += PictureBox_DragDrop;
                labelDragAndDrop.DragEnter += PictureBox_DragEnter;

                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.UserPaint |
                  ControlStyles.DoubleBuffer |
                  ControlStyles.ResizeRedraw, true);

                SetDoubleBuffered(splitContainer2, true);
                SetDoubleBuffered(splitContainer2.Panel1, true);
                SetDoubleBuffered(splitContainer2.Panel2, true);

                splitContainer2.Panel1.BackColor = Color.Transparent;
                splitContainer2.Panel2.BackColor = Color.Transparent;

                this.UpdateStyles();
                this.DoubleBuffered = true;

                dbMan.LogActivity(1, 1);
            }
            catch
            {
                dbMan.LogActivity(1, 0);
            }
        }
        private void SetDoubleBuffered(System.Windows.Forms.Control control, bool enabled)
        {
            typeof(System.Windows.Forms.Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, control, new object[] { enabled });
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        #region Пути сохранения

        // Подсказка пути при сохранении
        private void UpdatePathInTooltip()
        {
            toolTipPath.SetToolTip(buttonSaveAs, _saveImagePath);
        }

        // Название изменного файла по умолчанию
        private string InitSaveNameFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string filename = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            return Path.Combine(directory, $"{filename}_{DateTime.Now:yyyy_MMdd_HHmmss}{extension}");
        }

        // Папка сохранения по умолчанию
        private string InitSaveDir(string dirPath)
        {
            string parent = Directory.GetParent(dirPath).FullName;
            string lastDirectory = new DirectoryInfo(dirPath).Name;
            return Path.Combine(parent, $"{lastDirectory}_{DateTime.Now:yyyy_MMdd_HHmmss}");
        }

        // Использование API для поиска загрузок 
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid id, int flags, IntPtr token);

        // Поиск папки загрузок на компьютере
        private string SearchDownloadFolder()
        {
            Guid downloadsGuid = new Guid("374DE290-123F-4565-9164-39C4925E467B");
            string downloadsPath = SHGetKnownFolderPath(downloadsGuid, 0, IntPtr.Zero);
            return downloadsPath;
        }

        #endregion

        #region Механизм Drag And Drop
        // Проверка при перетаскивании
        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            // Содержат ли перетаскиваемые данные список файлов и изменение курсора
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        // Алгоритм при перетаскивании для одной фотографии
        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0) return;

            // Только первый файл
            string firstFilePath = files[0];

            // Проверяем расширение
            string ext = Path.GetExtension(firstFilePath).ToLower();
            if (ext == ".jpg" || ext == ".png")
            {
                inBitmap?.Dispose();
                inBitmap = new Bitmap(System.Drawing.Image.FromFile(firstFilePath));
                splitContainer2_Resize(splitContainer2, EventArgs.Empty);

                // Изменение интерфейса
                labelDragAndDrop.Visible = false;
                buttonStartEdit.Enabled = true;
                buttonSaveAs.Enabled = true;

                // Сохранение путей
                _initImagePath = firstFilePath;
                _saveImagePath = InitSaveNameFile(firstFilePath);
                UpdatePathInTooltip();
            }

        }
        // Проверка при перетаскивании пакета фотографий
        private void PacketPictureBox_DragEnter(object sender, DragEventArgs e)
        {
            // Содержат ли перетаскиваемые данные папку и изменение курсора
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (items != null && items.Length > 0)
                {
                    if (Directory.Exists(items[0]))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }
        // Алгоритм при перетаскивании пакета фотографий
        private void PacketPictureBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0) return;

            // Только первая папка
            string firstDirPath = files[0];

            // Изменение интерфейса
            buttonStartEdit.Enabled = true;
            buttonSaveAs.Enabled = true;

            // Сохранение путей
            _initImagePath = firstDirPath;
            _saveImagePath = InitSaveDir(firstDirPath);
            UpdatePathInTooltip();

            // Информация
            string[] extensions = { ".jpg", ".jpeg", ".png" };
            int photoCount = Directory.GetFiles(firstDirPath, "*.*").Where(f => extensions.Contains(Path.GetExtension(f).ToLower())).Count();
            lbForPacket.Text = $"Папка обработки: {firstDirPath}\nКоличество фотографий: {photoCount}";
            if (photoCount == 0)
            {
                buttonStartEdit.Enabled = false;
                buttonSaveAs.Enabled = false;
            }

        }

        #endregion

        // Ctrl + V
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsImage())
                {
                    inBitmap?.Dispose();
                    inBitmap = new Bitmap(Clipboard.GetImage());
                    splitContainer2_Resize(splitContainer2, EventArgs.Empty);

                    // Изменение интерфейса
                    labelDragAndDrop.Visible = false;
                    buttonStartEdit.Enabled = true;
                    buttonSaveAs.Enabled = true;

                    _initImagePath = Path.Combine(SearchDownloadFolder(), "temp");
                    _saveImagePath = InitSaveNameFile(_initImagePath);
                    UpdatePathInTooltip();
                }
            }
        }

        #region Выбор из проводника
        // Выбор из проводника одной фотографии
        private void buttonSelectPhoto_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png|JPEG|*.jpg;*.jpeg|PNG|*.png|Все файлы|*.*";
                dialog.FilterIndex = 1;
                dialog.Title = "Добавить снимок";
                dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;
                    string ext = Path.GetExtension(filePath).ToUpper();

                    if (ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                    {
                        inBitmap?.Dispose();
                        inBitmap = new Bitmap(filePath);

                        // вызвать ресайз
                        splitContainer2_Resize(splitContainer2, EventArgs.Empty);
                        splitContainer2.Panel1.Invalidate();

                        labelDragAndDrop.Visible = false;
                        buttonStartEdit.Enabled = true;
                        buttonSaveAs.Enabled = true;

                        _initImagePath = filePath;
                        _saveImagePath = InitSaveNameFile(filePath);
                        UpdatePathInTooltip();
                    }
                    else
                    {
                        MessageBox.Show("Поддерживаются только JPG, PNG!", "Неверный формат",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void labelDragAndDrop_Click(object sender, EventArgs e)
        {
            buttonSelectPhoto_Click(sender, e);
        }
        // Сохранить как
        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            if (!checkBoxPacketEdit.Checked)
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png|JPEG|*.jpg;*.jpeg|PNG|*.png|Все файлы|*.*";
                    saveDialog.FilterIndex = 1;
                    saveDialog.Title = "Сохранить фотографию";
                    saveDialog.FileName = $"edit_{DateTime.Now:yyyyMMdd}{Path.GetExtension(_initImagePath)}";
                    saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            _saveImagePath = saveDialog.FileName;
                            if (Path.GetExtension(_saveImagePath) != Path.GetExtension(_initImagePath))
                            {
                                MessageBox.Show($"Предупреждение!\nРасширение начального и конечного файла не совпадают. Плагины освещение и улучшение фотографии не будут работать!");
                            }
                            UpdatePathInTooltip();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                                           "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Сохранить папку фотографий";
                    folderDialog.InitialDirectory = _saveImagePath;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string dirPath = folderDialog.SelectedPath;
                        _saveImagePath = dirPath;
                        UpdatePathInTooltip();
                    }
                }

            }


        }

        // Выбор из проводника пакета фотографий
        private void buttonLoadPacket_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку фотографий";
                folderDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string dirPath = folderDialog.SelectedPath;

                    buttonStartEdit.Enabled = true;
                    buttonSaveAs.Enabled = true;

                    _initImagePath = dirPath;
                    _saveImagePath = InitSaveDir(dirPath);
                    UpdatePathInTooltip();

                    if (lbForPacket == null)
                    {
                        lbForPacket = new System.Windows.Forms.Label();
                        lbForPacket.Parent = splitContainer1.Panel2;
                        lbForPacket.Dock = DockStyle.Fill;
                        lbForPacket.TextAlign = ContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        lbForPacket.Visible = true;
                        string[] extensions = { ".jpg", ".jpeg", ".png" };
                        int photoCount = Directory.GetFiles(dirPath, "*.*").Where(f => extensions.Contains(Path.GetExtension(f).ToLower())).Count();
                        lbForPacket.Text = $"Папка обработки: {dirPath}\nКоличество фотографий: {photoCount}";
                        if (photoCount == 0)
                        {
                            buttonStartEdit.Enabled = false;
                            buttonSaveAs.Enabled = false;
                        }
                    }


                }
            }



        }
        #endregion

        #region Сброс настроек
        private void labelCancel_Click(object sender, EventArgs e)
        {
            _initImagePath = "";
            _saveImagePath = "";

            //PluginsHolder?.SetSelectedColor(Color.FromArgb(255, 255, 255, 255));
            //PluginsHolder?.SetSelectedColorEdit(Color.FromArgb(255, 255, 255, 255));

            buttonSaveAs.Enabled = false;
            buttonStartEdit.Enabled = false;

            inBitmap?.Dispose();
            outBitmap?.Dispose();
            inBitmap = null;
            outBitmap = null;

            if (!checkBoxPacketEdit.Checked)
            {
                labelDragAndDrop.Visible = true;
                splitContainer2_Resize(splitContainer2, EventArgs.Empty);
            }
            else
            {
                lbForPacket.Text = $"Перетащите папку";
                if (progressBar != null)
                {
                    progressBar.Visible = false;
                    textInfoProgress.Visible = false;
                    lbForPacket.Visible = true;
                }
            }

        }
        private void labelCancelConfig_Click(object sender, EventArgs e)
        {
            // почистить значения
            foreach (System.Windows.Forms.CheckBox cb in PluginsHolder.pluginCheckBoxes.Values)
            {
                cb.Checked = false;
            }
        }
        #endregion

        #region Адаптация
        // Центрирование настроек обработки
        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            checkBoxPacketEdit.Left = (panelLoading.Width - checkBoxPacketEdit.Width) / 2;
            buttonLoadPacket.Left = (panelLoading.Width - buttonLoadPacket.Width) / 2;

            buttonSelectPhoto.Left = (panelLoading.Width - buttonSelectPhoto.Width) / 2;
            labelCancel.Left = (panelLoading.Width - labelCancel.Width) / 2;

            buttonLoadConf.Left = (panelLoading.Width - buttonLoadConf.Width) / 2;
            labelCancelConfig.Left = (panelLoading.Width - labelCancelConfig.Width) / 2;
            buttonsaveConf.Left = (panelLoading.Width - buttonsaveConf.Width) / 2;

            buttonStartEdit.Left = (panelLoading.Width - buttonStartEdit.Width) / 2;
            buttonSaveAs.Left = (panelLoading.Width - buttonsaveConf.Width) / 2;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            splitContainer1_Panel1_Resize(sender, e);
            splitContainer1_Panel2_Resize(sender, e);
            splitContainer2_Resize(splitContainer2, EventArgs.Empty);
        }
        // Resize фото
        private void splitContainer2_Resize(object sender, EventArgs e)
        {
            // Ограниченная ширина для картинки
            this.splitContainer2.Resize -= splitContainer2_Resize;
            splitContainer2.Dock = DockStyle.Fill;
            if (inBitmap == null)
            {
                splitContainer2.SplitterDistance = splitContainer2.Width - 10;
                return;
            }

            float scaleW = (float)splitContainer2.Width / inBitmap.Width;
            float scaleH = (float)splitContainer2.Height / inBitmap.Height;
            int oldH = splitContainer2.Height;
            float scale = Math.Min(scaleW, scaleH);
            int newWidth = (int)(inBitmap.Width * scale);
            int newHeight = (int)(inBitmap.Height * scale);

            splitContainer2.Dock = DockStyle.None;
            splitContainer2.Size = new Size(newWidth, newHeight);

            Rectangle parentRect = splitContainer2.Parent.ClientRectangle;

            // Горизонтальное и вертикальное центрирование
            splitContainer2.Left = (parentRect.Width - splitContainer2.Width) / 2;
            splitContainer2.Top = (parentRect.Height - splitContainer2.Height) / 2;

            splitContainer2.SplitterDistance = splitContainer2.Width - 10;
            this.splitContainer2.Resize += splitContainer2_Resize;
        }
        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            if (progressBar != null && checkBoxPacketEdit.Checked)
            {
                progressBar.Size = new Size(splitContainer1.Panel2.Width - 300, 30);
                progressBar.Left = (splitContainer1.Panel2.Width - progressBar.Width) / 2;
                progressBar.Top = (splitContainer1.Panel2.Height - progressBar.Height) / 2 - 200;
                textInfoProgress.Left = progressBar.Left;
                textInfoProgress.Top = progressBar.Top + 30 + 10;
                textInfoProgress.Width = progressBar.Width;
            }
            if (packetPanel != null)
            {
                packetPanel.Left = progressBar.Left;
                packetPanel.Top = textInfoProgress.Top + textInfoProgress.Height + 30;
                packetPanel.Width = splitContainer1.Panel2.Width - 300;
            }

        }


        #endregion

        #region Интерфейс для пакетной обработки
        // Пути ко всем фотографиям в папке
        private List<string> GetImageFilesFromFolder(string folderPath)
        {
            string[] extensions = { ".jpg", ".jpeg", ".png" };
            return Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();
        }
        private void AddProgressBar()
        {
            progressBar = new System.Windows.Forms.ProgressBar();
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Size = new Size(splitContainer1.Panel2.Width - 300, 30);
            // Центрирование + panel
            progressBar.Left = (splitContainer1.Panel2.Width - progressBar.Width) / 2;
            progressBar.Top = (splitContainer1.Panel2.Height - progressBar.Height) / 2 - 200;
            splitContainer1.Panel2.Controls.Add(progressBar);
        }

        private void AddTextInfoProgress()
        {
            textInfoProgress = new System.Windows.Forms.TextBox();
            textInfoProgress.Parent = splitContainer1.Panel2;
            textInfoProgress.BorderStyle = BorderStyle.None;
            textInfoProgress.Multiline = true;
            textInfoProgress.ReadOnly = true;
            textInfoProgress.Width = progressBar.Width;
            textInfoProgress.Height = 80;
            textInfoProgress.Text = "Подготовка к обработке...";
            textInfoProgress.Left = progressBar.Left;
            textInfoProgress.Top = progressBar.Bottom + 5;
        }

        private void AddPacketPanel()
        {
            packetPanel = new System.Windows.Forms.Panel();
            packetPanel.Left = progressBar.Left;
            packetPanel.Top = textInfoProgress.Top + textInfoProgress.Height + 30;
            packetPanel.Width = splitContainer1.Panel2.Width - 300;
            packetPanel.Height = 300;
            packetPanel.MinimumSize = new Size(100, 50);
            splitContainer1.Panel2.Controls.Add(packetPanel);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 2;
            table.RowCount = 1;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            packetPanel.Controls.Add(table);

            // в левой колонке
            listBoxEditFiles = new System.Windows.Forms.ListBox();
            listBoxEditFiles.Dock = DockStyle.Fill;
            listBoxEditFiles.DataSource = GetImageFilesFromFolder(_saveImagePath);
            listBoxEditFiles.SelectedIndexChanged += ListBoxEditFiles_SelectedIndexChanged;
            listBoxEditFiles.IntegralHeight = false;
            table.Controls.Add(listBoxEditFiles, 0, 0);

            // в правой колонке
            previewPictureBox = new System.Windows.Forms.PictureBox();
            previewPictureBox.Dock = DockStyle.Fill;
            previewPictureBox.BorderStyle = BorderStyle.FixedSingle;
            previewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            table.Controls.Add(previewPictureBox, 1, 0);
        }

        private void ListBoxEditFiles_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string selectedFile = listBoxEditFiles.SelectedItem?.ToString();
            if (selectedFile != null)
            {
                if (previewPictureBox.Image != null) previewPictureBox.Image.Dispose();
                previewPictureBox.Image = System.Drawing.Image.FromFile(selectedFile);
            }
        }
        #endregion

        #region Обработка 
        // Выбор многошаговой обработки
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            acc.OpenOneOnly = !checkBoxSteps.Checked;
            if (acc.OpenOneOnly == true)
            {
                foreach (System.Windows.Forms.CheckBox cb in PluginsHolder.pluginCheckBoxes.Values)
                {
                    cb.Checked = false;
                }
            }
        }
        // Один шаг обработки фотографии
        private void ProcessImageInOneStep(string selectedPlugin, string local_initImagePath, ref string local_saveImagePath, Bitmap local_inBitmap, ref Bitmap local_outBitmap)
        {
            local_inBitmap?.Dispose();
            local_outBitmap?.Dispose();

            local_inBitmap = new Bitmap(local_initImagePath);

            string pluginType = PluginsHolder.GetPluginType(selectedPlugin);
            if (pluginType == "SmartIMG2IMG")
            {
                // Улучшение фото работает только в png
                if (selectedPlugin == "Улучшение фото")
                {
                    if (!checkBoxPacketEdit.Checked) _saveImagePath = new string(Path.ChangeExtension(_saveImagePath, "png"));
                    local_saveImagePath = Path.ChangeExtension(local_saveImagePath, "png");
                }

                PluginsHolder.ApplyPluginSmartIMG2IMG(selectedPlugin, local_initImagePath, local_saveImagePath, local_inBitmap, ref local_outBitmap, _config[selectedPlugin]);
                using (var tempBmp = new Bitmap(local_saveImagePath))
                {
                    local_outBitmap = new Bitmap(tempBmp);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
            else
            {
                PluginsHolder.ApplyPluginIMG2IMG(selectedPlugin, local_inBitmap, ref local_outBitmap, _config[selectedPlugin]);
                local_outBitmap.Save(local_saveImagePath);
            }
        }

        // Обработка одной фотографии
        private async Task OnePicEdit(string local_initImagePath, string local_saveImagePath)
        {
            List<string> selectedPlugins = new List<string>(_config.Keys);
            int steps = selectedPlugins.Count;

            using (Bitmap local_inBitmap = new Bitmap(local_initImagePath))
            {
                Bitmap local_outBitmap = null;
                string baseTempPath = Path.Combine(Path.GetDirectoryName(local_saveImagePath), Path.GetFileNameWithoutExtension(local_saveImagePath));

                for (int i = 0; i < steps; i++)
                {
                    string tempPath = $"{baseTempPath}#{i}{Path.GetExtension(local_saveImagePath)}"; // временный файл для текущего шага
                    await Task.Run(() => ProcessImageInOneStep(selectedPlugins[i], local_initImagePath, ref tempPath, local_inBitmap, ref local_outBitmap)); // шаг обработки

                    // Если это не последний шаг, подготавливаем следующий
                    if (i < steps - 1)
                    {
                        // освобождение предыдущего Bitmap
                        local_outBitmap?.Dispose();
                        local_outBitmap = null;

                        // сборка мусора для освобождения файла
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        // удаление предыдущего временного файла
                        if (i > 0)
                        {
                            string prevTempPath = $"{baseTempPath}#{i - 1}{Path.GetExtension(local_saveImagePath)}";
                            File.Delete(prevTempPath);
                        }

                        local_initImagePath = tempPath; // для следующего шага выходной это входной
                    }
                    else // последний шаг
                    {
                        string outPath = baseTempPath + Path.GetExtension(local_saveImagePath);
                        local_outBitmap.Save(outPath); // результат в финальный файл
                        local_outBitmap.Dispose();
                        local_outBitmap = null;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        File.Delete(tempPath); // удаление последнего временного файла
                        if (steps > 1) // удаление предыдущего
                        {
                            string prevTempPath = $"{baseTempPath}#{steps - 2}{Path.GetExtension(tempPath)}";
                            if (File.Exists(prevTempPath))
                                File.Delete(prevTempPath);
                            else
                            {
                                prevTempPath = $"{baseTempPath}#{steps - 2}{Path.GetExtension(local_initImagePath)}";
                                if (File.Exists(prevTempPath))
                                    File.Delete(prevTempPath);
                            }
                        }
                    }
                }
            }
        }

        // Функция вызова обработки фотографии/ий
        private async void buttonStartEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxTypeEdit.Items.Count == 0)
                {
                    MessageBox.Show("Выберите вид обработки");
                    return;
                }
                //if (labelTypeEdit.Text == "")
                //{
                //    MessageBox.Show("Выберите вид обработки");
                //    return;
                //}

                if (_initImagePath == "")
                {
                    MessageBox.Show("Выберите фотографию или папку фотографий");
                    return;
                }

                buttonStartEdit.Enabled = false;
                buttonStartEdit.Text = "Обработка...";
                labelCancel.Enabled = false;

                buildConfig();

                // очистка выхода
                outBitmap?.Dispose();
                outBitmap = null;

                // таймеры
                DateTime start;
                DateTime end;

                int count = 0;

                // Одна фотография
                if (!checkBoxPacketEdit.Checked)
                {
                    count = 1;
                    splitContainer2.SplitterDistance = splitContainer2.Width - 10;
                    splitContainer2.IsSplitterFixed = true;

                    start = DateTime.Now;
                    await OnePicEdit(_initImagePath, _saveImagePath);
                    end = DateTime.Now;

                    outBitmap = new Bitmap(_saveImagePath);

                    if (outBitmap != null)
                    {
                        splitContainer2.IsSplitterFixed = false;
                        splitContainer2.SplitterDistance = splitContainer2.Width / 2;
                    }
                }
                else // Пакетная обработка
                {
                    if (!Directory.Exists(_saveImagePath)) { Directory.CreateDirectory(_saveImagePath); }
                    lbForPacket.Visible = false;// скрыть прошлую надпись

                    if (progressBar == null) AddProgressBar(); // Прогресс бар

                    if (textInfoProgress == null) AddTextInfoProgress(); // Информация об обработке
                    if (packetPanel != null) packetPanel.Dispose();
                    if (previewPictureBox != null) previewPictureBox.Image.Dispose();
                    if (listBoxEditFiles != null) listBoxEditFiles.Dispose();

                    progressBar.Value = 0;
                    textInfoProgress.Text = "Подготовка к обработке...";

                    progressBar.Visible = true;
                    textInfoProgress.Visible = true;

                    var files = GetImageFilesFromFolder(_initImagePath);
                    progressBar.Maximum = files.Count;
                    count = files.Count;

                    string local_inFile;
                    string local_outFile;
                    int value = 0;

                    start = DateTime.Now;
                    foreach (string file in files)
                    {
                        textInfoProgress.Text = $"{value}/{files.Count}: Обработка {file}";
                        value += 1;
                        local_outFile = Path.Combine(_saveImagePath, Path.GetFileName(file));
                        local_inFile = file;
                        await OnePicEdit(local_inFile, local_outFile);
                        progressBar.Value = value; // Обновление прогресса
                    }
                    end = DateTime.Now;

                    textInfoProgress.Text = $"Обработка завершена";
                    AddPacketPanel();


                }
                labelCancel.Enabled = true;
                MessageBox.Show($"Обработка завершена!\nПуть сохранения: {_saveImagePath}");

                buttonStartEdit.Enabled = true;
                buttonStartEdit.Text = "Обработать фотографию";
                int IDact = dbMan.LogActivity(4, 1);
                string types = string.Join(";", listBoxTypeEdit.Items.Cast<object>().Select(item => item.ToString()));
                dbMan.LogProcessing(IDact, start, end, types, count);
                //dbMan.LogProcessing(IDact, start, end, labelTypeEdit.Text, count);
            }
            catch
            {
                dbMan.LogActivity(4, 0);
            }
        }
        #endregion

        #region Механизм просмотра фото до/после 
        // До
        private void pictureBefore_Paint(object sender, PaintEventArgs e)
        {
            if (inBitmap == null) return;

            Graphics g = e.Graphics;
            Rectangle panelRect = splitContainer2.Panel1.ClientRectangle;

            // Масштаб по высоте 
            float scale = (float)panelRect.Height / inBitmap.Height;
            int drawWidth = (int)(inBitmap.Width * scale);
            int drawHeight = panelRect.Height;
            int drawX = 0;
            int drawY = 0;

            // Координата разделителя (ширина левой панели в пикселях)
            int splitX = splitContainer2.SplitterDistance;

            // Обрезаем по разделителю
            Region oldClip = g.Clip;
            g.SetClip(new Rectangle(0, 0, splitX, panelRect.Height));

            // Рисуем изображение
            g.DrawImage(inBitmap, drawX, drawY, drawWidth, drawHeight);

            g.Clip = oldClip;
        }

        // После
        private void pictureAfter_Paint(object sender, PaintEventArgs e)
        {
            if (outBitmap == null) return;

            Graphics g = e.Graphics;
            Rectangle panelRect = splitContainer2.Panel2.ClientRectangle;

            float scale = (float)panelRect.Height / outBitmap.Height;
            int splitX = splitContainer2.SplitterDistance;
            int srcSplitX = (int)(splitX / scale);
            srcSplitX = Math.Max(0, Math.Min(srcSplitX, outBitmap.Width));

            Rectangle srcRect = new Rectangle(srcSplitX, 0, outBitmap.Width - srcSplitX, outBitmap.Height);
            Rectangle destRect = new Rectangle(0, 0, (int)(srcRect.Width * scale), panelRect.Height);
            g.DrawImage(outBitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        // Обновление фотографий
        private bool isUpdatePending = false;
        private void splitContainer2_SplitterMoved_1(object sender, SplitterEventArgs e)
        {
            if (!isUpdatePending)
            {
                isUpdatePending = true;
                this.BeginInvoke(new Action(() =>
                {
                    splitContainer2.Panel1.Invalidate();
                    splitContainer2.Panel2.Invalidate();
                    isUpdatePending = false;
                }));
            }
        }
        #endregion

        #region Пипетка / Выбор оттенка с фото "до" 
        private void splitContainer2_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (inBitmap == null) return;
            Point panelClick = new Point(e.X, e.Y);
            Color? pixelColor = GetPixelFromBitmap(inBitmap, panelClick, splitContainer2.Panel1.ClientSize);
            if (pixelColor.HasValue)
            {
                PluginsHolder?.SetSelectedColor(pixelColor.Value);
            }

        }

        // Преобраование координат в реальные координаты фотографии
        private Color? GetPixelFromBitmap(Bitmap bitmap, Point panelClick, Size panelSize)
        {
            if (bitmap == null) return null;

            float scale = (float)panelSize.Height / bitmap.Height;
            int imageWidthScaled = (int)(bitmap.Width * scale);
            int imageHeightScaled = panelSize.Height;

            if (panelClick.X < 0 || panelClick.X >= imageWidthScaled ||
                panelClick.Y < 0 || panelClick.Y >= imageHeightScaled)
                return null;

            int srcX = (int)(panelClick.X / scale);
            int srcY = (int)(panelClick.Y / scale);

            if (srcX < 0 || srcX >= bitmap.Width || srcY < 0 || srcY >= bitmap.Height)
                return null;

            return bitmap.GetPixel(srcX, srcY);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Point coord = new Point();
            var color = inBitmap.GetPixel(coord.X, coord.Y);
            PluginsHolder?.SetSelectedColor(color);
        }
        #endregion

        #region Работа с конфигурациями
        // Сбор конфигурации
        private void buildConfig()
        {
            var selectedPlugins = listBoxTypeEdit.Items;
            //var selectedPlugins = new List<string>();
            //selectedPlugins = labelTypeEdit.Text.Split(';').ToList();
            _config = new Dictionary<string, string>();

            foreach (string pluginName in selectedPlugins)
            {
                _config[pluginName] = PluginsHolder?.ParseSettings(pluginName, SettingsField);

            }
        }
        // Сохранение конфигурации плагинов
        private void buttonsaveConf_Click(object sender, EventArgs e)
        {
            //if (labelTypeEdit.Text == "")
            //{
            //    MessageBox.Show("Вид обработки не выбран!");
            //    return;
            //}

            if (listBoxTypeEdit.Items.Count == 0)
            {
                MessageBox.Show("Выберите вид обработки");
                return;
            }
            buildConfig(); // плагин: его параметры
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Конфигурация|*.ini";
                saveDialog.FilterIndex = 1;
                saveDialog.Title = "Сохранить конфигурацию";
                saveDialog.FileName = $"config_{DateTime.Now:yyyyMMdd}.ini";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveDialog.FileName))
                    {
                        sw.WriteLine("[Plugins]");
                        foreach (var plugin in _config.Keys)
                        {
                            var guid = PluginsHolder.FPluginsList[plugin].PluginGUID;
                            sw.WriteLine($"{guid}={_config[plugin]}");
                        }
                    }
                    MessageBox.Show("Конфигурация сохранена");

                }
            }
        }
        // Чтение ini файла и запись в _config
        private Dictionary<string, string> readIni(string filePath, string section)
        {
            string currentSection = null;
            Dictionary<string, string> iniConfig = new Dictionary<string, string>();
            foreach (string line in File.ReadLines(filePath))
            {
                string trimmed = line.Trim();
                // пустые строки и комментарии
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#"))
                    continue;

                // найти секцию
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);
                    continue;
                }

                // Если мы в нужной секции, ищем ключ
                if (currentSection == section)
                {
                    int eqIndex = trimmed.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        string guid = trimmed.Substring(0, eqIndex).Trim();
                        string paramsPlugin = trimmed.Substring(eqIndex + 1).Trim();
                        iniConfig.Add(guid, paramsPlugin);
                    }
                }
            }
            return iniConfig;
        }
        // Загрузка конфигурации
        private void buttonLoadConf_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Конфигурация|*.ini";
                dialog.FilterIndex = 1;
                dialog.Title = "Загрузить конфигурацию";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;
                    string ext = Path.GetExtension(filePath).ToUpper();
                    if (ext == ".INI")
                    {
                        // всё закрыть перед назначением нового
                        foreach (System.Windows.Forms.CheckBox cb in PluginsHolder.pluginCheckBoxes.Values)
                        {
                            cb.Checked = false;
                        }

                        Dictionary<string, string> iniConfig = readIni(filePath, "Plugins");
                        if (iniConfig.Count > 1) checkBoxSteps.Checked = true;
                        PluginsHolder.UpdateForConfig(iniConfig);
                    }
                    else
                    {
                        MessageBox.Show("Поддерживается только .ini файл!", "Неверный формат",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        #endregion

        #region Верхнее меню приложения
        private void openPhotoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            checkBoxPacketEdit.Checked = false;
            buttonSelectPhoto_Click(sender, e);
        }

        private void openFoldertoolStripMenuItem2_Click(object sender, EventArgs e)
        {
            checkBoxPacketEdit.Checked = true;
            buttonLoadPacket_Click(sender, e);
        }

        private void загрузитьФайлКонфигурацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonLoadConf_Click(sender, e);
        }

        private void сохранитьФайлКонфигурацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonsaveConf_Click(sender, e);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void startEditToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            buttonStartEdit_Click(sender, e);
        }

        // Добавить и удалить плагин с интерфейса
        private void AddRemovePluginsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            AddRemovePluginsToolStripMenuItem.DropDownItems.Clear();

            foreach (var plugin in basePluginsName)
            {
                var item = new ToolStripMenuItem(plugin);
                item.CheckOnClick = true;
                // если он есть в интерфейсе
                item.Checked = onFormPluginsName.Contains(plugin);
                item.Click += (s, ev) =>
                {
                    var mi = (ToolStripMenuItem)s;
                    if (mi.Checked)
                    {
                        PluginsHolder.AddPluginInterface(plugin);
                        onFormPluginsName.Add(plugin);
                    }
                    else
                    {
                        onFormPluginsName.Remove(plugin);
                        PluginsHolder.RemovePluginInterface(plugin);
                    }
                };
                AddRemovePluginsToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        // Выбор вида обработки из верхнего меню
        private void chooseEditToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            chooseEditToolStripMenuItem.DropDownItems.Clear();
            foreach (var plugin in onFormPluginsName)
            {
                var item = new ToolStripMenuItem(plugin);
                item.CheckOnClick = true;
                item.Checked = (PluginsHolder.pluginCheckBoxes[plugin].Checked == true); // если он выбран
                bool flag;
                item.Click += (s, ev) =>
                {
                    var mi = (ToolStripMenuItem)s;
                    // если Checked больше одного, то флаг пошаговой обработки
                    if (mi.Checked)
                    {
                        flag = PluginsHolder.pluginCheckBoxes.Values.Count(cb => cb.Checked) >= 1;
                        checkBoxSteps.Checked = flag;
                        PluginsHolder.pluginCheckBoxes[plugin].Checked = true;
                        if (plugin != "Удаление фона") OpenPluginSettings(item.Text);
                    }
                    else
                    {
                        // закрыть
                        PluginsHolder.pluginCheckBoxes[plugin].Checked = false;
                        flag = PluginsHolder.pluginCheckBoxes.Values.Count(cb => cb.Checked) == 1;
                    }
                };

                chooseEditToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void OpenPluginSettings(string pluginName)
        {
            PluginSettingsForm settingsForm = new PluginSettingsForm(PluginsHolder, pluginName);
            settingsForm.ShowDialog();
        }

        #endregion

        // Включение / Отключение пакетной обработки
        private void checkBoxPacketEdit_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer2.Visible = !checkBoxPacketEdit.Checked;
            buttonLoadPacket.Enabled = checkBoxPacketEdit.Checked;
            buttonSelectPhoto.Enabled = !checkBoxPacketEdit.Checked;
            if (lbForPacket != null) { lbForPacket.Visible = checkBoxPacketEdit.Checked; }

            if (checkBoxPacketEdit.Checked)
            {
                if (lbForPacket == null)
                {
                    lbForPacket = new System.Windows.Forms.Label();
                    lbForPacket.Parent = splitContainer1.Panel2;
                    lbForPacket.Dock = DockStyle.Fill;
                    lbForPacket.TextAlign = ContentAlignment.MiddleCenter;
                    lbForPacket.Click += buttonLoadPacket_Click;
                    lbForPacket.AllowDrop = true;
                    lbForPacket.DragDrop += PacketPictureBox_DragDrop;
                    lbForPacket.DragEnter += PacketPictureBox_DragEnter;
                }
                lbForPacket.Text = $"Перетащите папку";
            }
        }

        // Вызов окна описания плагинов
        private void InfoPluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoPlugins helpForm = new InfoPlugins(PluginsHolder);
            helpForm.Show();
        }

        // Вызов окна описания приложения
        private void aboutPrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.Show();
        }

        // Закрытие приложение и завершение процесса
        private void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                try
                {
                    if (!process.CloseMainWindow())
                        process.Kill();
                    if (!process.WaitForExit(5000))
                        process.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось завершить процесс {process.ProcessName}: {ex.Message}");
                }
                finally
                {
                    process?.Dispose();
                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                KillProcess("upscayl-bin");
                dbMan.LogActivity(2, 1);
                KillProcess("mysqld");   
            }
            catch
            {
                dbMan.LogActivity(2, 0);
            }
        }
    }
    }
