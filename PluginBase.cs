using AppCurs;
using Opulos.Core.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace AppCurs
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr TIMG_info_plugin();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double TIMG_proc_img_plugin(IntPtr Inmas, IntPtr Outmas, int width, int height, int stride, IntPtr cfg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double TSmartIMG_proc_img_plugin(IntPtr Infile, IntPtr Outfile, IntPtr cfg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void info_plugin();


    public class PluginFDetails
    {
        public IntPtr PluginLibHandle; // номер загруженной DLL в таблице Windows
        public string PluginGUID;
        public string PluginName;
        public string PluginDescription;
        public string PluginType;
        public string PluginAuthor;
        public string PluginVersion;
        public string PluginDataRealese;
        public string PluginGUIconfig;
        public IntPtr PluginDoWorkFunction;
    }
    public class Plugin
    {
        public MainForm Form { get; set; }
        // Параметры для вещественных значений в трекбарах
        private Dictionary<string, int> Offcets;
        private Dictionary<string, int> Divizors;

        // Перечень функций из плагинов
        private Dictionary<string, string> FLoaded_GUIDPluginsList; // Ключ – GIUD плагина, значение – файл
        private Dictionary<string, string> GUIDPluginsList;// Ключ – GIUD плагина, значение – название
        public Dictionary<string, PluginFDetails> FPluginsList;
        private Dictionary<string, PluginFDetails> BasePluginsList;
        private Dictionary<string, Panel> pluginPanels = new Dictionary<string, Panel>();

        public Dictionary<string, CheckBox> pluginCheckBoxes = new Dictionary<string, CheckBox>();
        public List<string> basePluginsName;
        public List<string> showPluginsName;
        public List<string> onFormPluginsName;

        private Dictionary<string, int> pluginParamCounts;

        public Panel SettingsField;
        public ListBox FaListBox { get; set; }

        public ListBox listBoxTypeEdit;
        public double Time;

        // Windows API для динамической загрузки плагинов
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        public Plugin()
        {
            Offcets = new Dictionary<string, int>(); // смещение
            Divizors = new Dictionary<string, int>(); // делитель
            FPluginsList = new Dictionary<string, PluginFDetails>();
            FLoaded_GUIDPluginsList = new Dictionary<string, string>();
            GUIDPluginsList = new Dictionary<string, string>();
        }
        public void Dispose()
        {
            foreach (var key in FPluginsList.Keys)
            {
                FreeLibrary(FPluginsList[key].PluginLibHandle);
            }
            FPluginsList.Clear();
            GC.SuppressFinalize(this);
        }
        ~Plugin()
        {
            Dispose();
        }

        /////////////////////////
        // Загрузка расширений //
        /////////////////////////

        public bool VerifyPluginFile(string filename)
        {
            IntPtr libHandle = IntPtr.Zero;
            bool result = false;

            try
            {
                libHandle = LoadLibrary(filename);
                if (libHandle != IntPtr.Zero)
                {
                    // Проверка наличия нужных функций в библиотеке
                    if (GetProcAddress(libHandle, "PluginGetName") != IntPtr.Zero &&
                        GetProcAddress(libHandle, "PluginGetDescription") != IntPtr.Zero &&
                        GetProcAddress(libHandle, "PluginGetPluginType") != IntPtr.Zero &&
                        GetProcAddress(libHandle, "PluginGetGUIDString") != IntPtr.Zero &&
                        GetProcAddress(libHandle, "PluginGetGetGUIinfo") != IntPtr.Zero &&
                        GetProcAddress(libHandle, "PluginDoWork") != IntPtr.Zero)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при загрузке плагина: " + ex.Message);
            }
            finally
            {
                if (libHandle != IntPtr.Zero)
                {
                    FreeLibrary(libHandle);
                }
            }

            return result;
        }

        public bool LoadPluginFunction(string filename)
        {
            IntPtr thLib = IntPtr.Zero;
            bool isAdded = false;
            bool result = false;

            if (thLib == IntPtr.Zero)
                thLib = LoadLibrary(filename);


            if (thLib != IntPtr.Zero)
            {
                TIMG_info_plugin GetPluginData = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(GetProcAddress(thLib, "PluginGetGUIDString"), typeof(TIMG_info_plugin));

                string guidStr = string.Empty;
                if (GetPluginData != null)
                {
                    guidStr = Marshal.PtrToStringAnsi(GetPluginData());
                    isAdded = FLoaded_GUIDPluginsList.TryGetValue(guidStr, out string searchFileName);
                }

                if (!isAdded)
                {
                    FLoaded_GUIDPluginsList.Add(guidStr, filename);
                    result = true;

                    PluginFDetails pfItem = new PluginFDetails();
                    pfItem.PluginLibHandle = thLib;
                    pfItem.PluginGUID = guidStr;

                    TIMG_info_plugin GFPN;
                    // Получаем имя функции плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetName"), typeof(TIMG_info_plugin));
                    pfItem.PluginName = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;
                    // Получаем описание функции плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetDescription"), typeof(TIMG_info_plugin));
                    pfItem.PluginDescription = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;
                    // Получаем тип плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetPluginType"), typeof(TIMG_info_plugin));
                    pfItem.PluginType = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;
                    // Получаем информацию о GUI плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetGetGUIinfo"), typeof(TIMG_info_plugin));
                    pfItem.PluginGUIconfig = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;
                    // Получаем информацию об авторе плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetAuthor"), typeof(TIMG_info_plugin));
                    pfItem.PluginAuthor = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;
                    // Получаем информацию о версии плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetVersion"), typeof(TIMG_info_plugin));
                    pfItem.PluginVersion = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;
                    // Получаем информацию о дате релиза плагина
                    GFPN = (TIMG_info_plugin)Marshal.GetDelegateForFunctionPointer(
                        GetProcAddress(thLib, "PluginGetDataRealese"), typeof(TIMG_info_plugin));
                    pfItem.PluginDataRealese = GFPN != null ? Marshal.PtrToStringAnsi(GFPN()) : string.Empty;


                    // Получаем указатель на рабочую функцию плагина
                    pfItem.PluginDoWorkFunction = GetProcAddress(thLib, "PluginDoWork");
                    // Добавляем плагин в список
                    FPluginsList.Add(pfItem.PluginName, pfItem);
                    GUIDPluginsList.Add(guidStr, pfItem.PluginName);
                    //if (pfItem.PluginType == "IMG2IMG" || pfItem.PluginType == "SmartIMG2IMG")
                    //{
                    //    FaListBox.Items.Add(pfItem.PluginName);
                    //}

                }
            }
            return result;
        }

        public void LoadPlugins(string path, string mask)
        {
            string[] files = Directory.GetFiles(path, mask);
            // Перебираем все плагины
            foreach (string file in files)
            {
                // делаем проверку с одновременной загрузкой
                if (VerifyPluginFile(file))
                {
                    LoadPluginFunction(file);
                }
            }
        }


        #region Создание интерфейса для пипетки и панелей цвета
        // Панель исходного цвета
        public void SetSelectedColor(Color color)
        {
            string colorPanelName = "PCOLOR_SRC";
            // Панель, которая отображает выбранные цвет
            Control? colorPanel = SettingsField.Controls.Find(colorPanelName + "_Цветовой тон/насыщенность", true).FirstOrDefault();

            if (colorPanel is Panel panel)
            {
                panel.BackColor = color;

                // RGB в HSV
                ColorToHSV(color, out double hue, out double saturation, out double value);

                // Имена трекбаров
                string hueTB = "INPUT_1";
                string satTB = "INPUT_2";
                string valTB = "INPUT_3";

                string parentName = colorPanel.Parent?.Name ?? "";

                string hueControlName = hueTB + "_" + parentName;
                string satControlName = satTB + "_" + parentName;
                string valControlName = valTB + "_" + parentName;

                // Находим трекбары
                TrackBar? hueTrackBar = SettingsField.Controls.Find(hueControlName, true).FirstOrDefault() as TrackBar;
                TrackBar? satTrackBar = SettingsField.Controls.Find(satControlName, true).FirstOrDefault() as TrackBar;
                TrackBar? valTrackBar = SettingsField.Controls.Find(valControlName, true).FirstOrDefault() as TrackBar;

                string EhueControlName = "E" + hueTB + "_" + parentName;
                string EsatControlName = "E" + satTB + "_" + parentName;
                string EvalControlName = "E" + valTB + "_" + parentName;


                TextBox? hueTextBox = SettingsField.Controls.Find(EhueControlName, true).FirstOrDefault() as TextBox;
                TextBox? satTextBox = SettingsField.Controls.Find(EsatControlName, true).FirstOrDefault() as TextBox;
                TextBox? valTextBox = SettingsField.Controls.Find(EvalControlName, true).FirstOrDefault() as TextBox;


                if (hueTrackBar != null)
                {
                    hueTrackBar.Value = (int)Math.Round(hue);
                    hueTextBox.Text = GetTrackBarValue(hueTrackBar).ToString();
                }
                if (satTrackBar != null)
                {
                    satTrackBar.Value = (int)Math.Round(saturation);
                    satTextBox.Text = GetTrackBarValue(satTrackBar).ToString();
                }
                if (valTrackBar != null)
                {
                    valTrackBar.Value = (int)Math.Round(value);
                    valTextBox.Text = GetTrackBarValue(valTrackBar).ToString();
                }
            }
        }

        // Панель измененного цвета
        public void SetSelectedColorEdit(Color color)
        {
            string colorPanelName = "PCOLOR_TGT";
            // Панель, которая отображает выбранные цвет
            Control? colorPanel = SettingsField.Controls.Find(colorPanelName + "_Цветовой тон/насыщенность", true).FirstOrDefault();

            if (colorPanel is Panel panel)
            {
                panel.BackColor = color;

                // RGB в HSV
                ColorToHSV(color, out double hue, out double saturation, out double value);

                // Имена трекбаров
                string hueTB = "INPUT_4";
                string satTB = "INPUT_5";
                string valTB = "INPUT_6";

                string parentName = colorPanel.Parent?.Name ?? "";

                string hueControlName = hueTB + "_" + parentName;
                string satControlName = satTB + "_" + parentName;
                string valControlName = valTB + "_" + parentName;

                // Находим трекбары
                TrackBar? hueTrackBar = SettingsField.Controls.Find(hueControlName, true).FirstOrDefault() as TrackBar;
                TrackBar? satTrackBar = SettingsField.Controls.Find(satControlName, true).FirstOrDefault() as TrackBar;
                TrackBar? valTrackBar = SettingsField.Controls.Find(valControlName, true).FirstOrDefault() as TrackBar;

                string EhueControlName = "E" + hueTB + "_" + parentName;
                string EsatControlName = "E" + satTB + "_" + parentName;
                string EvalControlName = "E" + valTB + "_" + parentName;


                TextBox? hueTextBox = SettingsField.Controls.Find(EhueControlName, true).FirstOrDefault() as TextBox;
                TextBox? satTextBox = SettingsField.Controls.Find(EsatControlName, true).FirstOrDefault() as TextBox;
                TextBox? valTextBox = SettingsField.Controls.Find(EvalControlName, true).FirstOrDefault() as TextBox;


                if (hueTrackBar != null)
                {
                    hueTrackBar.Value = (int)Math.Round(hue);
                    hueTextBox.Text = GetTrackBarValue(hueTrackBar).ToString();
                }
                if (satTrackBar != null)
                {
                    satTrackBar.Value = (int)Math.Round(saturation);
                    satTextBox.Text = GetTrackBarValue(satTrackBar).ToString();
                }
                if (valTrackBar != null)
                {
                    valTrackBar.Value = (int)Math.Round(value);
                    valTextBox.Text = GetTrackBarValue(valTrackBar).ToString();
                }
            }
        }

        // Метод преобразования RGB в HSV
        private void ColorToHSV(Color color, out double h, out double s, out double v)
        {

            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            // h, s, v = hue, saturation, value
            double cmax = Math.Max(r, Math.Max(g, b));
            double cmin = Math.Min(r, Math.Min(g, b));
            double diff = cmax - cmin;
            h = -1;
            s = -1;

            if (cmax == cmin)
                h = 0;
            else if (cmax == r)
                h = (60 * ((g - b) / diff) + 360) % 360;
            else if (cmax == g)
                h = (60 * ((b - r) / diff) + 120) % 360;
            else if (cmax == b)
                h = (60 * ((r - g) / diff) + 240) % 360;
            if (cmax == 0)
                s = 0;
            else
                s = (diff / cmax) * 100;
            v = cmax * 100;

        }

        private void childResized(object sender, EventArgs e)
        {
            Panel pluginPanel = sender as Panel;
            if (pluginPanel == null) return;

            foreach (Control child in pluginPanel.Controls)
            {
                if (child is Panel pan && pan.Name.Contains("PCOLOR"))
                {
                    pan.Left = pan.Parent.Width - 70;
                }
                else if (child is TrackBar tb)
                {
                    tb.Width = tb.Parent.Width - 30;
                    //tb.Left = 10;
                }
                else if (child is TextBox ed && ed.Name.Contains("EINPUT"))
                {
                    ed.Left = ed.Parent.Width - 70;
                }
                else if (child is Label lb && lb.Name.Contains("ED"))
                {
                    lb.Left = lb.Parent.Width - 30;
                }
                else if (child is Label maxLbl && maxLbl.Name.Contains("LBL_MAX"))
                {
                    maxLbl.Left = maxLbl.Parent.Width - 30 - 20;
                }
                else if (child is Label srLbl && srLbl.Name.Contains("LBL_SR"))
                {
                    srLbl.Left = (srLbl.Parent.Width - 30) / 2;
                }
            }
        }

        private void TrackbarPos(object sender, EventArgs e)
        {
            if (sender is TrackBar trackBar)
            {
                Control parent = trackBar.Parent;
                string cnam = "E" + trackBar.Name;
                Control comp = parent.Controls.Find(cnam, true).FirstOrDefault();
                //Control comp = SettingsField.Controls.Find(cnam, true).FirstOrDefault();

                if (comp is TextBox ed)
                {
                    // Обновление textbox
                    float k = GetTrackBarValue(trackBar);
                    ed.Text = k.ToString("0.##", CultureInfo.InvariantCulture);
                }
                UpdateColorPanels(parent);
            }
        }

        // Обновление панелей цвета
        private void UpdateColorPanels(Control parentContainer)
        {

            string[] colorPanels = { "PCOLOR_SRC", "PCOLOR_TGT" };

            foreach (string panelName in colorPanels)
            {
                Control? colorPanel = parentContainer.Controls.Find(panelName + "_Цветовой тон/насыщенность", true).FirstOrDefault();

                //MessageBox.Show(colorPanel.Parent.Name);

                if (colorPanel is Panel panel)
                {
                    string panelType = colorPanel.Tag?.ToString() ?? "";  // PCOLOR_SRC

                    // Соответствующие TrackBar
                    string hueTB = panelType == "PCOLOR_SRC" ? "INPUT_1" : "INPUT_4";
                    string satTB = panelType == "PCOLOR_SRC" ? "INPUT_2" : "INPUT_5";
                    string valTB = panelType == "PCOLOR_SRC" ? "INPUT_3" : "INPUT_6";

                    TrackBar hueTrackBar = parentContainer.Controls.Find(hueTB + "_" + colorPanel.Parent.Name, true).FirstOrDefault() as TrackBar;
                    TrackBar satTrackBar = parentContainer.Controls.Find(satTB + "_" + colorPanel.Parent.Name, true).FirstOrDefault() as TrackBar;
                    TrackBar valTrackBar = parentContainer.Controls.Find(valTB + "_" + colorPanel.Parent.Name, true).FirstOrDefault() as TrackBar;

                    //float h = GetTrackBarValue(hueTB + "_" + colorPanel.Parent.Name);
                    //float s = GetTrackBarValue(satTB + "_" + colorPanel.Parent.Name);
                    //float v = GetTrackBarValue(valTB + "_" + colorPanel.Parent.Name);

                    float h = GetTrackBarValue(hueTrackBar);
                    float s = GetTrackBarValue(satTrackBar);
                    float v = GetTrackBarValue(valTrackBar);

                    colorPanel.BackColor = HsvToRgb(h, s, v);
                }
            }
        }

        private Color HsvToRgb(double h, double s, double v)
        {
            // перевод в доли
            h = (h + 360) % 360;
            s = s / 100;
            v = v / 100;

            // сектор hsv
            int i = (int)(h / 60);

            double f = (h / 60) - i; // смещение оттенка внутри сектора (доля поворота внутри сектора)
            double p = v * (1 - s); // минимальная яркость
            double q = v * (1 - s * f); // яркость для канала, который убывает по мере приближения к концу сектора
            double t = v * (1 - s * (1 - f)); // яркость для канала, который возрастает по мере приближения к концу сектора

            return i switch
            {
                0 => Color.FromArgb((int)(v * 255), (int)(t * 255), (int)(p * 255)),
                1 => Color.FromArgb((int)(q * 255), (int)(v * 255), (int)(p * 255)),
                2 => Color.FromArgb((int)(p * 255), (int)(v * 255), (int)(t * 255)),
                3 => Color.FromArgb((int)(p * 255), (int)(q * 255), (int)(v * 255)),
                4 => Color.FromArgb((int)(t * 255), (int)(p * 255), (int)(v * 255)),
                _ => Color.FromArgb((int)(v * 255), (int)(p * 255), (int)(q * 255))
            };
        }

        #endregion

        #region Создание интерфейса для плагинов
        #region Вспомогательные методы и события
        // Обновление TrackBar в зависимости от единиц измерения
        private float GetTrackBarValue(TrackBar tb)
        {
            if (tb != null)
            {
                float offset = Offcets[tb.Name];
                float divisor = Divizors[tb.Name];
                return tb.Value / divisor - offset;
            }
            return 0;
        }

        // Обновление TrackBar в зависимости от ползунков
        private void edChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb && tb.Name.StartsWith("E"))
            {
                string trackName = tb.Name.Replace("E", "");
                //Control comp = SettingsField.Controls.Find(trackName, true).FirstOrDefault();
                Control comp = tb.Parent.Controls.Find(trackName, true).FirstOrDefault();

                if (comp is TrackBar trackBar)
                {
                    if (!float.TryParse(tb.Text, out float value))
                    {
                        trackBar.Value = 0;
                    }
                    else
                    {
                        trackBar.Value = (int)Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, value));
                    }

                    float k = GetTrackBarValue(trackBar);
                    tb.Text = k.ToString("0.##", CultureInfo.InvariantCulture);

                    UpdateColorPanels(tb.Parent);
                }
            }
        }

        // Сохранение TextBox
        private void setEdit(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    edChanged(textBox, EventArgs.Empty);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Потухание ошибок из введеных TextBox
        private async void HighlightError(TextBox tb)
        {
            tb.BackColor = Color.LightPink;
            await Task.Delay(3000);
            if (tb.IsDisposed) return;
            tb.BackColor = SystemColors.Window;
        }

        // Ввод в TextBox и проверки
        private void Ed_KeyPress(object? sender, KeyPressEventArgs e)
        {

            if (char.IsControl(e.KeyChar))
                return;

            TextBox tb = sender as TextBox;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
                tb.BackColor = Color.LightPink;
                HighlightError(tb);
                return;
            }

            if (e.KeyChar == '-')
            {
                if (tb.SelectionStart != 0 || tb.Text.Contains("-"))
                {
                    // не в начале и уже есть
                    e.Handled = true;
                    tb.BackColor = Color.LightPink;
                    HighlightError(tb);
                    return;
                }
            }

            if (e.KeyChar == '.')
            {
                if (tb.Text.Contains("."))
                {
                    // вторая точка
                    e.Handled = true;
                    tb.BackColor = Color.LightPink;
                    HighlightError(tb);
                    return;
                }
                if (tb.Text.Length == 0 || (tb.Text.Length == 1 && tb.Text[0] == '-'))
                {
                    // точка в начале строки или нет цифры перед ней
                    e.Handled = true;
                    tb.BackColor = Color.LightPink;
                    HighlightError(tb);
                    return;
                }
            }
            tb.BackColor = SystemColors.Window;
        }

        #endregion
        // Генерация урезанного интефрейса в отдельном окне
        public void CreateShortSettingField(string pluginName, Control parentContainer)
        {
            string str = FPluginsList[pluginName].PluginGUIconfig;
            // Создание списков для загрузки конфигурации интерфейса
            var list1 = new List<string>();
            var list2 = new List<string>();

            list1 = str.Split('!').ToList();
            list2 = new List<string>();
            // создание GUI на основе описательной схемы из DLL
            foreach (var item in list1)
            {
                // заполнение информации о параметрах компонента согласно разделителя ;
                list2 = item.Split(';').ToList();
                // Если параметров больше 2 то это описание компонента
                if (list2.Count > 2)
                {
                    if (list2[0] == "Panel")
                    {
                        var pan = new Panel();
                        pan.Name = list2[1] + "_" + pluginName;
                        pan.Tag = list2[1];
                        pan.Parent = parentContainer;
                        pan.Size = new Size(40, 20);
                        pan.Left = pan.Parent.Width - 90;
                        pan.Top = int.Parse(list2[2]);
                        pan.BorderStyle = BorderStyle.FixedSingle;
                    }

                    if (list2[0] == "Label")
                    {
                        // Создание Label'ов
                        var lb = new Label();
                        lb.Name = list2[1] + "_" + pluginName;
                        lb.Parent = parentContainer;
                        lb.Left = (int.Parse(list2[2]) <= 0) ? (lb.Parent.Width - 50 + int.Parse(list2[2])) : int.Parse(list2[2]);
                        lb.Top = int.Parse(list2[3]);
                        lb.Text = list2[4];
                        lb.AutoSize = true;
                    }
                    else if (list2[0] == "Edit")
                    {
                        var ed = new TextBox();
                        ed.Name = list2[1] + "_" + pluginName;
                        ed.Parent = parentContainer;
                        ed.Left = ed.Parent.Width - 90;
                        ed.Top = int.Parse(list2[3]);
                        ed.Width = 40;

                        ed.Text = list2[4]; // из основного интерфейса
                        ed.Font = new Font("Segoe UI", 7);
                        ed.TextAlign = HorizontalAlignment.Center;

                        ed.Leave += edChanged;
                        ed.KeyDown += setEdit;
                        ed.KeyPress += Ed_KeyPress;
                    }

                    else if (list2[0] == "checkBox")
                    {
                        // Создание checkBox'ов
                        var cb = new CheckBox();
                        cb.Name = list2[1] + "_" + pluginName;
                        cb.Parent = parentContainer;
                        cb.Text = list2[2];
                        cb.Checked = true; //из основного интерфейса
                        cb.Left = int.Parse(list2[3]);
                        cb.Top = int.Parse(list2[4]);
                        cb.AutoSize = true;
                    }

                    else if (list2[0] == "TrackBar")
                    {
                        // Создание TrackBar'ов
                        var tb = new TrackBar();
                        tb.Name = list2[1] + '_' + pluginName;
                        tb.Parent = parentContainer;
                        tb.Left = int.Parse(list2[2]);
                        tb.Top = int.Parse(list2[3]);
                        tb.Width = tb.Parent.Width - 30;
                        tb.Minimum = int.Parse(list2[4]);
                        tb.Maximum = int.Parse(list2[5]);
                        tb.Value = int.Parse(list2[6]);
                        tb.TickStyle = TickStyle.BottomRight;
                        tb.TickFrequency = (tb.Maximum - tb.Minimum) / 2;
                        tb.SmallChange = 2;
                        tb.LargeChange = 2;
                        tb.AutoSize = false;
                        tb.Height = 20;
                        tb.Scroll += TrackbarPos;
                        tb.Tag = list2[1];
                    }
                }
            }
        }

        // Генерация элементов интерфейса по PluginGUIconfig мз DLL
        public void CreateSettingsField(string cfg, Panel pluginPanel)
        {
            string str = cfg;
            // Создание списков для загрузки конфигурации интерфейса
            var list1 = new List<string>();
            var list2 = new List<string>();

            /// заполнение информации о создаваемых компонентах в качестве разделителя !
            list1 = str.Split('!').ToList();
            list2 = new List<string>();


            // создание GUI на основе описательной схемы из DLL
            foreach (var item in list1)
            {
                // заполнение информации о параметрах компонента согласно разделителя ;
                list2 = item.Split(';').ToList();
                // Если параметров больше 2 то это описание компонента
                if (list2.Count > 2)
                {
                    if (list2[0] == "Panel")
                    {
                        var pan = new Panel();
                        pan.Name = list2[1] + '_' + pluginPanel.Name;
                        pan.Tag = list2[1];
                        pan.Parent = pluginPanel;
                        pan.Size = new Size(40, 20);
                        pan.Left = pan.Parent.Width - 30 - 30;
                        pan.Top = int.Parse(list2[2]);
                        pan.BorderStyle = BorderStyle.FixedSingle;
                    }
                    if (list2[0] == "Label")
                    {
                        // Создание Label'ов
                        var lb = new Label();
                        lb.Name = list2[1] + '_' + pluginPanel.Name;
                        lb.Parent = pluginPanel;
                        //lb.Left = int.Parse(list2[2]);
                        lb.Left = (int.Parse(list2[2]) <= 0) ? (lb.Parent.Width - 30 + int.Parse(list2[2])) : int.Parse(list2[2]);

                        lb.Top = int.Parse(list2[3]);
                        lb.Text = list2[4];
                        lb.AutoSize = true;
                    }
                    else if (list2[0] == "Edit")
                    {
                        // Создание Edit'ов т.н. TextBox элементов C#
                        var ed = new TextBox();
                        ed.Name = list2[1] + '_' + pluginPanel.Name;
                        ed.Parent = pluginPanel;
                        ed.Left = (ed.Parent.Width - 30) - 30;
                        ed.Top = int.Parse(list2[3]);
                        ed.Width = 40;
                        ed.Text = list2[4];
                        ed.Font = new Font("Segoe UI", 7);
                        ed.TextAlign = HorizontalAlignment.Center;
                        ed.Leave += edChanged;
                        ed.KeyDown += setEdit;
                        ed.KeyPress += Ed_KeyPress;
                    }
                    else if (list2[0] == "TrackBar")
                    {
                        // Создание TrackBar'ов
                        var tb = new TrackBar();
                        tb.Name = list2[1] + '_' + pluginPanel.Name;
                        tb.Parent = pluginPanel;
                        tb.Left = int.Parse(list2[2]);
                        tb.Top = int.Parse(list2[3]);
                        tb.Width = tb.Parent.Width - 30;
                        tb.Minimum = int.Parse(list2[4]);
                        tb.Maximum = int.Parse(list2[5]);
                        tb.Value = int.Parse(list2[6]);
                        tb.TickStyle = TickStyle.BottomRight;
                        tb.TickFrequency = (tb.Maximum - tb.Minimum) / 2;
                        tb.SmallChange = 2;
                        tb.LargeChange = 2;
                        tb.AutoSize = false;
                        tb.Height = 20;
                        if (!Offcets.ContainsKey(tb.Name)) Offcets.Add(tb.Name, int.Parse(list2[7]));
                        if (!Divizors.ContainsKey(tb.Name)) Divizors.Add(tb.Name, int.Parse(list2[8]));
                        tb.Scroll += TrackbarPos;
                        tb.Tag = list2[1];

                        // Минимум
                        Label lblMin = new Label();
                        lblMin.Name = "LBL_MIN_" + tb.Name;
                        lblMin.Text = (tb.Minimum / Divizors[tb.Name] - Offcets[tb.Name]).ToString();
                        lblMin.AutoSize = true;
                        lblMin.Parent = pluginPanel;
                        lblMin.Left = tb.Left + 1;
                        lblMin.Top = tb.Bottom;
                        lblMin.Font = new Font("Segoe UI", 7);

                        if (pluginPanel.Name != "Улучшение фото") {
                            // Среднее
                            Label lblSr = new Label();
                            lblSr.Name = "LBL_SR_" + tb.Name;
                            lblSr.Text = ((tb.Minimum + tb.Maximum) / 2 / Divizors[tb.Name] - Offcets[tb.Name]).ToString();
                            lblSr.AutoSize = true;
                            lblSr.Parent = pluginPanel;
                            lblSr.Left = tb.Width / 2;
                            lblSr.Top = tb.Bottom;
                            lblSr.Font = new Font("Segoe UI", 7);
                        }

                        // Максимум
                        Label lblMax = new Label();
                        lblMax.Name = "LBL_MAX_" + tb.Name;
                        lblMax.Text = (tb.Maximum / Divizors[tb.Name] - Offcets[tb.Name]).ToString();
                        lblMax.AutoSize = true;
                        lblMax.Parent = pluginPanel;
                        lblMax.Left = tb.Width - 1;
                        lblMax.Top = tb.Bottom;
                        lblMax.Font = new Font("Segoe UI", 7);

                    }
                    else if (list2[0] == "checkBox")
                    {
                        // Создание checkBox'ов
                        var cb = new CheckBox();
                        cb.Name = list2[1] + '_' + pluginPanel.Name;
                        cb.Parent = pluginPanel;
                        cb.Text = list2[2];
                        cb.Left = int.Parse(list2[3]);
                        cb.Top = int.Parse(list2[4]);
                        cb.AutoSize = true;
                    }
                }
                else
                {
                    if (!pluginParamCounts.ContainsKey(pluginPanel.Name))
                        pluginParamCounts[pluginPanel.Name] = int.Parse(list2[0]);
                }
            }
        }

        #endregion

        public string ParseSettings(string pluginName, Control panelParent)
        {
            string res = "";
            int fPFParamCNT = pluginParamCounts[pluginName];
            for (int i = 1; i <= fPFParamCNT; i++)
            {
                Control comp = panelParent.Controls.Find("INPUT_" + i + '_' + pluginName, true).FirstOrDefault();

                if (comp != null)
                {
                    if (i > 1)
                        res += " ";
                    if (comp is TextBox textBox)
                        res += textBox.Text;
                    else if (comp is CheckBox checkBox)
                        res += checkBox.Checked ? '1' : '0';
                    else if (comp is TrackBar trackBar)
                        res += ((float)trackBar.Value / Divizors[comp.Name] - Offcets[comp.Name]).ToString("G", CultureInfo.InvariantCulture);
                }
            }

            return res;
        }

        #region Механизм работы с аккордеоном
        // Для работы с аккордеоном
        internal abstract class AccordionPanel : Panel
        {
            public Accordion acc = new Accordion();
            public AccordionPanel()
            {
                Dock = DockStyle.Fill;
                Controls.Add(acc);
            }

        }
        // Открытие аакордеона = будущая обработка плагином
        private void OnPluginCheckChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is string pluginName)
            {
                if (cb.Checked)
                {
                    Form.openBoxes.Add(pluginName);
                    listBoxTypeEdit.Items.Add(pluginName);
                }
                else
                {
                    Form.openBoxes.Remove(pluginName);
                    listBoxTypeEdit.Items.Remove(pluginName);
                }
            }
        }
        // Удаление плагина с интрефейса
        public void RemovePluginInterface(string pluginName)
        {

            if (!pluginPanels.TryGetValue(pluginName, out Panel? pluginPanel) || pluginPanel == null)
                return;

            if (!pluginCheckBoxes.TryGetValue(pluginName, out CheckBox? cb) || cb == null)
                return;

            foreach (Control control in pluginPanel.Controls)
            {
                if (control is Panel panel)
                    panel.Resize -= childResized;
                else if (control is TrackBar trackBar)
                    trackBar.Scroll -= TrackbarPos;
                else if (control is TextBox textBox)
                {
                    textBox.Leave -= edChanged;
                    textBox.KeyDown -= setEdit;
                }
            }
            pluginPanel.Resize -= childResized;

            cb.Checked = false;
            cb.CheckedChanged -= OnPluginCheckChanged;
            Form.acc.Controls.Remove(cb);
            cb.Dispose();

            if (pluginPanel.Parent != null)
                pluginPanel.Parent.Controls.Remove(pluginPanel);
            pluginPanel.Dispose();


            pluginPanels.Remove(pluginName);
            pluginCheckBoxes.Remove(pluginName);
            onFormPluginsName.Remove(pluginName);


            var keysToRemove = Offcets.Keys.Where(k => k.EndsWith("_" + pluginName)).ToList();
            foreach (var key in keysToRemove)
            {
                Offcets.Remove(key);
                Divizors.Remove(key);
            }
            pluginParamCounts.Remove(pluginName);

        }
        // Добавление плагина в интерфейс
        public void AddPluginInterface(string pluginName)
        {
            PluginFDetails details = FPluginsList[pluginName];
            Panel pluginPanel = new Panel { Dock = DockStyle.Fill, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom };

            pluginPanel.Name = pluginName;
            // в случае наличия конфигурации
            if (details.PluginGUIconfig.Length > 10)
            {
                CreateSettingsField(details.PluginGUIconfig, pluginPanel);
                pluginPanel.Resize += childResized;
                CheckBox cb = Form.acc.Add(pluginPanel, details.PluginName);
                cb.Name = pluginName;
                onFormPluginsName.Add(pluginName);
                cb.Tag = pluginName; // сохраняем имя плагина
                cb.CheckedChanged += OnPluginCheckChanged;

                pluginPanels[pluginName] = pluginPanel;
                pluginCheckBoxes[pluginName] = cb;
            }
        }
        // Построение интерфейса аккордеона для плагина с нуля
        public void BuildPluginsInterface()
        {
            //ClearPluginInterface();
            if (Form.acc == null)
            {
                // если не сущ
                var newAcc = new Accordion();
                newAcc.OpenOneOnly = true;
                newAcc.FillLastOpened = true;        // сохраняет размер при переоткрытии
                newAcc.OpenOneOnly = true;           // только один открыт
                newAcc.FillWidth = true;
                newAcc.FillHeight = true;
                newAcc.Insets = new Padding(0, 10, 0, 10);
                newAcc.ControlMinimumWidthIsItsPreferredWidth = false;
                newAcc.HorizontalScroll.Enabled = false;
                newAcc.HorizontalScroll.Visible = false;

                Form.acc = newAcc;
                pluginParamCounts = new Dictionary<string, int>();
                SettingsField.Controls.Add(Form.acc);
            }

            foreach (var plugin in FPluginsList)
            {
                string pluginName = plugin.Key;
                if ((!onFormPluginsName.Contains(pluginName)) && (basePluginsName.Contains(pluginName) || showPluginsName.Contains(pluginName)))
                {
                    PluginFDetails details = plugin.Value;
                    Panel pluginPanel = new Panel { Dock = DockStyle.Fill, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom };

                    pluginPanel.Name = pluginName;
                    // в случае наличия конфигурации (длина более 10 символов)
                    if (details.PluginGUIconfig.Length > 10)
                    {
                        CreateSettingsField(details.PluginGUIconfig, pluginPanel);
                        pluginPanel.Resize += childResized;
                        CheckBox cb = Form.acc.Add(pluginPanel, details.PluginName);
                        cb.Name = pluginName;
                        onFormPluginsName.Add(pluginName);
                        if (!basePluginsName.Contains(pluginName)) basePluginsName.Add(pluginName);
                        cb.Tag = pluginName; // сохраняем имя плагина
                        cb.CheckedChanged += OnPluginCheckChanged;

                        pluginPanels[pluginName] = pluginPanel;
                        pluginCheckBoxes[pluginName] = cb;
                    }

                }
            }
            SettingsField.Invalidate();
        }
        #endregion

        // Информация о плагинах 
        public Dictionary<string, string> InfoAboutPlugins()
        {
            Dictionary<string, string> infos = new Dictionary<string, string>();
            foreach (var plugin in FPluginsList.Keys)
            {
                infos[plugin] = FPluginsList[plugin].PluginDescription;
            }
            return infos;
        }
        // Создание строки конфигурации
        public void UpdateForConfig(Dictionary<string, string> iniConfig)
        {
            if (iniConfig == null) return;
            string pluginName;
            List<string> setting;

            foreach (var guid in iniConfig.Keys)
            {
                pluginName = GUIDPluginsList[guid];
                if (!onFormPluginsName.Contains(pluginName)) return;
                pluginCheckBoxes[pluginName].Checked = true;
                Control comp1;
                Control comp2;

                setting = new List<string>(iniConfig[guid].Split(' '));
                for (int i = 1; i <= pluginParamCounts[pluginName]; i++)
                {
                    string valueStr = setting[i - 1];
                    comp1 = SettingsField.Controls.Find("INPUT_" + i + '_' + pluginName, true).FirstOrDefault(); // компонент для обновления интерфейса
                    if (comp1 == null) continue;

                    if (comp1 is TrackBar trackBar)
                    {
                        float realValue = float.Parse(valueStr, CultureInfo.InvariantCulture);
                        int trackValue = (int)((realValue + Offcets[comp1.Name]) * Divizors[comp1.Name]);
                        trackBar.Value = Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, trackValue));
                    }
                    else if (comp1 is CheckBox checkBox)
                    {
                        checkBox.Checked = (valueStr == "1");
                    }

                    comp2 = SettingsField.Controls.Find("EINPUT_" + i + '_' + pluginName, true).FirstOrDefault(); // компонент для обновления интерфейса

                    if (comp2 is TextBox textBox)
                    {
                        float realValue = float.Parse(valueStr, CultureInfo.InvariantCulture);
                        textBox.Text = ((int)((realValue + Offcets[comp1.Name]) * Divizors[comp1.Name])).ToString();
                    }
                }

            }
        }

        // Определение типа для запуска правильной обработки
        public string GetPluginType(string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName)) return "UNKNOWN";
            return FPluginsList.TryGetValue(pluginName, out PluginFDetails details)
                ? details.PluginType
                : "UNKNOWN";
        }

        // Применение плагинов
        public void ApplyPluginSmartIMG2IMG(string pluginName, string inFile, string outFile, Bitmap inBitmap, ref Bitmap outBitmap, string setting)
        {
            if (FPluginsList.TryGetValue(pluginName, out PluginFDetails pfItem))
            {
                //string setting = ParseSettings(pluginName);
                if (string.IsNullOrEmpty(setting)) setting = "1";
                string[] parts = setting.Trim().Split(' ');
                if (parts.Length == 1) outBitmap = new Bitmap(inBitmap.Width * int.Parse(setting), inBitmap.Height * int.Parse(setting), PixelFormat.Format32bppArgb);
                else outBitmap = new Bitmap(inBitmap.Width, inBitmap.Height, PixelFormat.Format32bppArgb);

                // LockBits выход  
                BitmapData outData = outBitmap.LockBits(
                        new Rectangle(0, 0, outBitmap.Width, outBitmap.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                // Проверяем наличие обработчика в плагине
                if (pfItem.PluginDoWorkFunction != IntPtr.Zero)
                {
                    TSmartIMG_proc_img_plugin pluginF = (TSmartIMG_proc_img_plugin)Marshal.GetDelegateForFunctionPointer(pfItem.PluginDoWorkFunction, typeof(TSmartIMG_proc_img_plugin));
                    IntPtr settingsPtr = string.IsNullOrEmpty(setting) ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(setting ?? "");
                    Time = pluginF(Marshal.StringToHGlobalAnsi(inFile), Marshal.StringToHGlobalAnsi(outFile), settingsPtr);
                    outBitmap.UnlockBits(outData);
                }
            }
        }

        public void ApplyPluginIMG2IMG(string pluginName, Bitmap inBitmap, ref Bitmap outBitmap, string setting)
        {
            if (FPluginsList.TryGetValue(pluginName, out PluginFDetails pfItem))
            {
                outBitmap = new Bitmap(inBitmap.Width, inBitmap.Height, PixelFormat.Format32bppArgb);

                // LockBits вход
                BitmapData inData = inBitmap.LockBits(
                    new Rectangle(0, 0, inBitmap.Width, inBitmap.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                // LockBits выход  
                BitmapData outData = outBitmap.LockBits(
                    new Rectangle(0, 0, outBitmap.Width, outBitmap.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


                // Проверяем наличие обработчика в плагине
                if (pfItem.PluginDoWorkFunction != IntPtr.Zero)
                {
                    IntPtr settingsPtr = string.IsNullOrEmpty(setting) ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(setting ?? "");
                    TIMG_proc_img_plugin pluginF = (TIMG_proc_img_plugin)Marshal.GetDelegateForFunctionPointer(pfItem.PluginDoWorkFunction, typeof(TIMG_proc_img_plugin));
                    Time = pluginF(inData.Scan0, outData.Scan0, inBitmap.Width, inBitmap.Height, inData.Stride, settingsPtr);
                }

                // Разблокируем изображения
                inBitmap.UnlockBits(inData);
                outBitmap.UnlockBits(outData);
            }
        }
    }
}