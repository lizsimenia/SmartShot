//using System;
//using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

//using System.Threading.Tasks;
using static AppCurs.MainForm;

namespace AppCurs
{

    internal class License
    {
        private DatabaseManager dbMan;
        private Label labelTariff;
        private ToolTip toolTipLicense;
        private List<string> showPluginsName;
        private Plugin PluginsHolder;

        LicenseData currentLicense = new LicenseData();
        // Структура лицензии
        public class LicenseData
        {
            public string UserName { get; set; }
            public string Organization { get; set; }
            public DateTime ExpirationDate { get; set; }
            public string USBSerialNumber { get; set; }
            public string EnablePlugins { get; set; }
        }

        public License(DatabaseManager dbMan, Label tariffLabel, ToolTip toolTip, List<string> pluginsList, Plugin pluginsHolder)
        {
            this.dbMan = dbMan;
            this.labelTariff = tariffLabel;
            this.toolTipLicense = toolTip;
            this.showPluginsName = pluginsList;
            this.PluginsHolder = pluginsHolder;
        }

        // Проверка лицензии
        public void CheckUsbForLicense()
        {
            try
            {
                // Получение серийного номера и диска
                Dictionary<string, string> usbSerDisk = new Dictionary<string, string>();
                usbSerDisk = GetUSBSerialNumber();

                // Проверка и расшифровка всех полученных файлов
                string serial;

                foreach (var disk in usbSerDisk.Keys)
                {
                    serial = usbSerDisk[disk];
                    string keyPath = System.IO.Path.Combine(disk, "license.key");
                    if (File.Exists(keyPath))
                    {
                        // Расшифровать и применить лицензию
                        string licenseString = File.ReadAllText(keyPath);
                        string descriptionString = AesEncryption.Decrypt(licenseString, "_pass!!wor78521893");
                        if (descriptionString != null)
                        {
                            List<string> listLicense = descriptionString.Split('!').ToList();
                            // Сравнение серийника
                            if (listLicense[0] != serial)
                            {
                                // Пртоколирование
                                dbMan.LogActivity(3, 0);
                                continue;
                            }

                            // Запись в структуру расшифровки
                            currentLicense.USBSerialNumber = listLicense[0];
                            currentLicense.UserName = listLicense[1];
                            currentLicense.Organization = listLicense[2];
                            currentLicense.ExpirationDate = DateTime.Parse(listLicense[3]);
                            currentLicense.EnablePlugins = listLicense[4];

                            // Информация о лицензии
                            toolTipLicense.SetToolTip(labelTariff,
                                $"Лицензия для: {currentLicense.UserName}\n" +
                                $"Организация: {currentLicense.Organization}\n" +
                                $"Действует до: {currentLicense.ExpirationDate:yyyy-MM-dd}");

                            // Изменение интерфейса
                            List<string> pluginsNames = currentLicense.EnablePlugins.Split(';').ToList();
                            foreach (var pluginName in pluginsNames)
                            {
                                if (!showPluginsName.Contains(pluginName))
                                    showPluginsName.Add(pluginName);
                            }
                            PluginsHolder.BuildPluginsInterface();
                            labelTariff.Text = "Продвинутый";

                            // Пртоколирование
                            dbMan.LogActivity(3, 1);
                            return; // Первая валидная флешка
                        }
                        else
                        {
                            // Пртоколирование
                            dbMan.LogActivity(3, 0);
                        }
                    }
                }
                labelTariff.Text = "Базовый";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка проверки лицензии: " + ex.Message);
                dbMan.LogActivity(3, 0);
            }
        }

        // Буква диска
        private static string GetDriveLetterFromPnpDeviceId(string pnpDeviceId)
        {
            try
            {
                // Экранируем обратные слеши в PNPDeviceID для WMI запроса
                string formattedPnpDeviceId = pnpDeviceId.Replace("\\", "\\\\");

                // 1. Поиск физического диска
                string diskQuery = $"SELECT * FROM Win32_DiskDrive WHERE PNPDeviceID = '{formattedPnpDeviceId}'";
                using (var searcherDisk = new ManagementObjectSearcher(diskQuery))
                {
                    foreach (ManagementObject disk in searcherDisk.Get())
                    {
                        // 2. Связь диска с разделами (ASSOCIATORS OF)
                        string partitionQuery = $"ASSOCIATORS OF {{{disk.Path.Path}}} WHERE AssocClass=Win32_DiskDriveToDiskPartition";
                        using (var searcherPartition = new ManagementObjectSearcher(partitionQuery))
                        {
                            foreach (ManagementObject partition in searcherPartition.Get())
                            {
                                // 3. Связь разделов с логическими дисками
                                string logicalDiskQuery = $"ASSOCIATORS OF {{{partition.Path.Path}}} WHERE AssocClass=Win32_LogicalDiskToPartition";
                                using (var searcherLogicalDisk = new ManagementObjectSearcher(logicalDiskQuery))
                                {
                                    foreach (ManagementObject logicalDisk in searcherLogicalDisk.Get())
                                    {
                                        // Возвращаем букву диска (DeviceID, например, "F:")
                                        // выходим (т.к. для USB Flash обычно нужна одна буква)
                                        return logicalDisk["DeviceID"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("Ошибка WMI: " + e.Message);
            }

            return null; // Диск не найден или не имеет буквы
        }

        // Серийник
        private static string Retrieve_serial(string strSource)
        {
            string strStart = "\\";
            int Start, End;
            Start = strSource.LastIndexOf(strStart) + strStart.Length;
            End = strSource.IndexOf("&0", Start);
            string serial = strSource.Substring(Start, End - Start);
            return serial;
        }

        // Получение информации о usb flash (серия и диск)
        private static Dictionary<string, string> GetUSBSerialNumber()
        {
            string query = "SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            Dictionary<string, string> usbSerDisk = new Dictionary<string, string>();
            foreach (ManagementObject diskDrive in searcher.Get().Cast<ManagementObject>())
            {
                // Получаем строку идентификации устройства 
                string DeviceIDstr = diskDrive["PNPDeviceID"]?.ToString();
                string serial = Retrieve_serial(DeviceIDstr)?.ToString();
                string drive_letter = GetDriveLetterFromPnpDeviceId(DeviceIDstr)?.ToString();

                if (drive_letter != null) usbSerDisk[drive_letter] = serial ?? string.Empty;
            }
            return usbSerDisk;

        }
        // Класс шифрования
        private static class AesEncryption
        {
            // Случайные байты
            private static readonly byte[] Salt = { 0x1F, 0x9B, 0x3C, 0x4D, 0x5E, 0x6F, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2, 0x03 };

            // Деширование
            public static string Decrypt(string cipherBase64, string password)
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherBase64);

                using (var aes = Aes.Create())
                {
                    using (var derive = new Rfc2898DeriveBytes(password, Salt, 1000, HashAlgorithmName.SHA256))
                    {
                        aes.Key = derive.GetBytes(32);
                        aes.IV = derive.GetBytes(16);
                    }
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream(cipherBytes))
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }


    }
}
