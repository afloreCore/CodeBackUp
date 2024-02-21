using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using GrwPinPadNet.Properties;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace GrwPinPadNet
{
    [ComVisible(false)]
    static public class Utilities
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        private static string _secretkey = "S0fl @nd2O22!";
        static string path = "";
        static string _app = "";
        private static string _publickey = "12345678";
        static CultureInfo _culture = FormatDate.CultureFormat(TypeDate.fr_FR);
        static System.Configuration.Configuration config;
        static Dictionary<string, string> _dicDataFile;
        static public bool ConsoleWrite { set; get; }
        static public string PublicKey { get => _publickey;}
        static public string ReadSetting(string key)
        {
            _app = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                _app = appSettings[key] ?? "";

            }
            catch (ConfigurationErrorsException ex)
            {
                if (ConsoleWrite) Console.WriteLine("Error reading app settings {0}", ex);
            }
            return _app;
        }

        static public string ReadPropertiesSettings(string key)
        {
            _app = "";
            try
            {
                _app = Properties.Settings.Default[key].ToString() ?? "";
            }
            catch (ConfigurationErrorsException ex)
            {
                if (ConsoleWrite) Console.WriteLine("Error reading app settings {0}", ex);
            }
            return _app;

        }
        static public void WritePropertiesSettings(string key, string value)
        {
            try
            {
                Properties.Settings.Default[key] = value;
                //save the setting
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationErrorsException ex)
            {
                if (ConsoleWrite) Console.WriteLine("Error reading app settings {0}", ex);
            }
        }
        static public string ReadSection(string section, string key)
        {
            _app = "";
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection appSettingSection = (AppSettingsSection)config.GetSection(section);
                _app = appSettingSection.Settings[key].Value ?? "";
            }
            catch (ConfigurationErrorsException ex)
            {
                if (ConsoleWrite) Console.WriteLine("Error reading app settings {0}", ex);
            }
            return _app;
        }
        static public void AddUpdateAppSettings(string key, string value, string section = "")
        {
            KeyValueConfigurationCollection settings;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (section.Length > 0)
                {
                    AppSettingsSection appSettingSection = (AppSettingsSection)config.GetSection(section);
                    var settingsSection = appSettingSection.Settings;
                    settings = settingsSection;
                }
                else
                    settings = config.AppSettings.Settings;

                if (settings.Count == 0 | settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                RefleshSection(section);
            }
            catch (ConfigurationErrorsException ex)
            {
                if (ConsoleWrite) Console.WriteLine("Error reading app settings {0}", ex);
            }
        }

        static public void ReadPathConfigFile()
        {
            //Machine Configuration Path
            string path1 = ConfigurationManager.OpenMachineConfiguration().FilePath;
            //Application Configuration Path
            string path2 = ConfigurationManager.OpenExeConfiguration(
                  ConfigurationUserLevel.None).FilePath;
            //User Configuration Path 
            string path3 = ConfigurationManager.OpenExeConfiguration(
                  ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
        }
        static public bool DateValidate(string date, TypeDate outtd)
        {
            bool _true = false;
            DateTime _fchaux;
            try
            {
                _true = DateTime.TryParseExact(date, FormatDate.StringFormat(outtd), FormatDate.CultureFormat(outtd), DateTimeStyles.None, out _fchaux);
            }
            catch (Exception ex)
            {
                _true = false;
                if (ConsoleWrite) Console.WriteLine(ex.Message);
            }
            return _true;
        }
        static public bool DateString(string date, TypeDate current, TypeDate outtd, out string datetime)
        {
            bool _true = false;
            string _dateaux = "";
            DateTime _fchmov;
            try
            {
                if (DateTime.TryParse(date, FormatDate.CultureFormat(current), DateTimeStyles.None, out _fchmov))
                {
                    _dateaux = _fchmov.ToString(FormatDate.StringFormat(outtd));
                    _true = DateTime.TryParseExact(_dateaux, FormatDate.StringFormat(outtd), FormatDate.CultureFormat(outtd), DateTimeStyles.None, out _fchmov);

                }
            }
            catch (Exception ex)
            {
                _true = false;
                if (ConsoleWrite) Console.WriteLine(ex.Message);
            }
            datetime = _dateaux;
            return _true;
        }
        static public bool DateString(string date, CultureInfo culture, TypeDate outtd, out string datetime)
        {
            bool _true = false;
            string _dateaux = "";
            DateTime _fchmov;
            if (!DateTime.TryParse(date, culture, DateTimeStyles.None, out _fchmov))
            {
                _fchmov = DateTime.Now.Date;
            }
            try
            {
                _dateaux = _fchmov.ToString(FormatDate.StringFormat(outtd));
                _true = true; //DateTime.TryParseExact(_dateaux, FormatDate.StringFormat(outtd), FormatDate.CultureFormat(outtd), DateTimeStyles.None, out _fchmov);
            }
            catch (Exception ex)
            {
                _true = false;
                if (ConsoleWrite) Console.WriteLine(ex.Message);
            }
            datetime = _dateaux;
            return _true;
        }
        static public bool DateString(string date, TipoFuncion TF, TipoOperacion TP, TipoPinPad tPinPad, out string datetime)
        {
            //Parte->CL-40497->
            //bool _true = false;
            bool _true = true;
            //
            string _dateaux = "";
            DateTime _fchmov;

            //Parte->CL-40497->
            if (!DateTime.TryParse(date, out _fchmov))
            {
                _fchmov = DateTime.Now.Date;
            }
            else
                //Now > _fchmov->1, Now = _fchmov->0, Now < _fchmov-> -1, 
                if (DateTime.Now.Date.CompareTo(_fchmov) < 0)
                     _fchmov = DateTime.Now.Date;

            if (TP == TipoOperacion.Devolucion && tPinPad == TipoPinPad.PinPad)
            {
                try
                {
                    _dateaux = _fchmov.ToString("ddMMyyyy", CultureInfo.CreateSpecificCulture("ja-JP"));
                }
                catch
                {
                    _true = false;
                }
            }
            else
            {
                try
                {
                    _dateaux = _fchmov.ToString("yyyyMMdd", CultureInfo.CreateSpecificCulture("fr-FR"));
                }
                catch
                {
                    _true = false;
                }
            }
            datetime = _dateaux;
            return _true;
        }

        static public bool HourString(string date, out string hours)
        {
            bool _true = false;
            DateTime _fchmov;
            string _out = "";
            try
            {
                if (DateTime.TryParse(date, _culture, DateTimeStyles.None, out _))
                {
                    _true = DateTime.TryParseExact(date, "HHmmss", _culture, DateTimeStyles.None, out _fchmov);
                    _out = _fchmov.ToString();
                }
            }
            catch (Exception ex)
            {
                _true = false;
                if (ConsoleWrite) Console.WriteLine(ex.Message);
            }
            hours = _out;
            return _true;
        }

        static public void IniWriteValue(string FileName, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, GetPathFile(FileName));
        }

        static public string IniReadValue(string FileName, string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, GetPathFile(FileName));
            return temp.ToString();
        }
        public static string AssemblyDirectory
        {
            get
            {

                if (path.Length == 0)
                {
                    Assembly a = Assembly.GetExecutingAssembly();
                    path = Path.GetDirectoryName(a.Location);
                }
                return path;
            }
        }
        public static string GetPathFile(string name)
        {
            return Path.Combine(AssemblyDirectory, name);
        }
        public static string ConvertNumber<T>(T number, string separtor)
        {
            string result = "";
            var str = number.ToString();
            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";
            //ci.NumberFormat.NumberGroupSeparator = ".";
            ci.NumberFormat.NumberGroupSizes = new[] { 0 };
            try
            {
                switch (Type.GetTypeCode(number.GetType()))
                {
                    case TypeCode.Decimal:
                        result = Decimal.Parse(str).ToString("N", ci);
                        break;

                    case TypeCode.Int32:
                        result = Int32.Parse(str).ToString("N", ci);
                        break;
                    case TypeCode.Double:
                        result = Double.Parse(str).ToString("N", ci);
                        break;
                }
            }
            catch { result = "0"; }
            return result;
        }
        public static string GetNumberFromStr(string Text, string StringFind, int Long, string SubString = "")
        {
            string _str = "";
            //string str = "123456 cupon: 000012345678";
            int index = Text.ToUpper().LastIndexOf(StringFind.ToUpper());
            string _aux = Text.Substring(index,Text.IndexOf(SubString, index)- index);
            _aux = _aux.Substring(_aux.Length - Long);
            Int64 nrocupon;
            if (Int64.TryParse(_aux, out nrocupon))
                _str = _aux;

            return _str;
        }

        public static void Resize<T>(this List<T> list, int sz, T c)
        {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                    list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }
        public static void Resize<T>(this List<T> list, int sz) where T : new()
        {
            Resize(list, sz, new T());
        }
        //concatena elementos a través de la Func<T> y separa los registros con el char delimiter
        //Ejempl de la Func: Func<TransPending, string> splistr = str => string.Concat(str.Date, separador)
        public static string ConcatStrList(this List<TransPending> listData, string listdelimiter, string fieldsdelimiter)
        {
            string value="";
            foreach (TransPending item in listData)
            {
                value += String.Concat("Date=", item.Date, fieldsdelimiter, "Comprobante=", item.Comprobante ,fieldsdelimiter, "Import=", item.Import, listdelimiter) ;
            }
            return value;
        }
        //Ultima Transacción Pendiente
        public static TransPending GetLastPending(List<TransPending> listData)
        {
            TransPending tr = new TransPending();
            string fecha = "";
            string hora = "", horaux = "";
            //DateTime fchmov, fchaux = DateTime.Now.AddYears(-1);
            string fchaux = "";
            if (listData != null)
            {
                foreach (var item in listData)
                {
                    fecha = item.Date;
                    hora = item.Hour;
                    //if (DateTime.TryParse(fecha, _culture, DateTimeStyles.None, out fchmov))
                    if ((fchaux.CompareTo(fecha) < 0) || (fchaux.CompareTo(fecha) == 0 && horaux.CompareTo(hora) < 0))
                    {
                        fchaux = fecha;
                        horaux = hora;
                        tr.Date = fecha;
                        tr.Hour = item.Hour;
                        tr.Comprobante = item.Comprobante;
                        tr.Import = item.Import;
                    }
                }
            }
            return tr;
        }
       
        public static List<TransPending> StringToListClass(string text, string listdeliliter, string fieldsdelimiter)
        {
            List<TransPending> lista = new List<TransPending>();
            //!string.IsNullOrWhiteSpace(x[0]))
            var l = text.Split(listdeliliter[0])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            /*
            Dictionary<string, string> d = values.Split('|')
                                           .Select(x => x.Split('=')).Where(y => !string.IsNullOrWhiteSpace(y[0]))
                                           .ToDictionary(x => x[0], x => x[1]);
            */
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (var t in l)
            {
                var dic = t.Split(fieldsdelimiter[0])
                    .Select(x => x.Split('='))
                    .Where(y => !string.IsNullOrWhiteSpace(y[0]))
                    .ToDictionary(x => x[0], x => x[1]);
                
                if (dic.Count>=2)
                {
                    TransPending tr = new TransPending { Date = dic["Date"], Comprobante = dic["Comprobante"] };
                    lista.Add(tr);
                }
               
            }
         
            d = null;
            return lista;
            
        }
        public static void WriteDataFile(string filename, string listDelimiter, string nexoDelimiter, Dictionary<string, string> dicdatafile)
        {
            List<string> list = new List<string>();
            
            //using (StreamWriter w = new StreamWriter(filename, false))
            //{
            foreach (KeyValuePair<string, string> kv in dicdatafile)
            {
                    var encode = (string.Concat(kv.Key.ToUpper(), nexoDelimiter[0], kv.Value, listDelimiter[0]));
                    //w.Write(Encrypt(encode));
                    //encode = Encrypt(encode);
                    byte[] myEncodeData_byte = new byte[encode.Length - 1];
                    myEncodeData_byte =  Encoding.UTF8.GetBytes(encode);
                    string myEncodedData = Convert.ToBase64String(myEncodeData_byte);
                    
                //data = string.Concat(data, myEncodedData);
                //File.WriteAllText(filename, myEncodedData);}
                list.Add(myEncodedData);


            }
           
            String[] str = list.ToArray();
            using (var file = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var wr = new StreamWriter(file))
            {
                foreach (String s in str)
                    wr.WriteLine(s);
            }
        }
        public static bool WriteDataFile(string filename, string listDelimiter, string fieldDelimiter, string data)
        {
            bool b = false;
            try 
            {
                using (var file = new FileStream(filename, FileMode.Create))
                {
                    b = true;
                }
            }
            catch {b = false; }

            return b;

            //using (FileStream fs = new FileStream(filename, mode))
            //{
            //    using (BinaryWriter w = new BinaryWriter(fs))
            //    {
            //        var dic = data.Split(fieldDelimiter[0]);
            //        w.Write(string.Concat(dic[0].ToUpper() , fieldDelimiter[0], dic[1], listDelimiter[0]));
            //    }
            //}
        }

        public static string GetValueFromDataFile(string listDelimiter, string nexoDelimiter, string keyValue)
        {
            string value = "";
            if (_dicDataFile != null)
                if (_dicDataFile.Count > 0)
                {
                    //if (File.Exists(filename))
                    //{
                    //    byte[] b = new byte[0];
                    //    using (StreamReader reader = new StreamReader(filename))
                    //    {
                    //        var line = reader.ReadLine();
                    //        while (line != null)
                    //        {
                    //            b = Convert.FromBase64String(line);
                    //            data = string.Concat(data, Encoding.Default.GetString(b));
                    //            line = reader.ReadLine();
                    //            //byte[] b = File.ReadAllBytes(filename);
                    //            //byte [] b = File.ReadAllBytes(filename);
                    //            //string[] str = File.ReadAllLines(filename);
                    //        }

                    //        //foreach (var d in str)
                    //        //{
                    //        //    b = Convert.FromBase64String(d);
                    //        //    data = string.Concat(data, Encoding.Default.GetString(b));
                    //        //}

                    //        var dic = data.Split(listDelimiter[0]).Select(x => x.Split(nexoDelimiter[0]))
                    //                .Where(y => !string.IsNullOrWhiteSpace(y[0]))
                    //                .ToDictionary(x => x[0], x => x[1]);
                    //        //keyValue = Encoder( keyValue.ToUpper() );
                    //        _dicDataFile = dic;
                    //    }
                    //}

                    if (_dicDataFile.ContainsKey(keyValue.ToUpper()))
                        value = _dicDataFile[keyValue.ToUpper()];
                }
            return value;
        }
        public static string GetKeysValuesFromDataFile(string filename, string listDelimiter, string nexoDelimiter)
        {
            string data = "";
            if (_dicDataFile == null)
                _dicDataFile = new Dictionary<string, string>();
            else
                _dicDataFile.Clear();
       
            if (File.Exists(filename))
            {
                byte[] b = new byte[0];
                using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        var line = reader.ReadLine();

                        while (line != null)
                        {
                            b = Convert.FromBase64String(line);
                            data = string.Concat(data, Encoding.Default.GetString(b));
                            line = reader.ReadLine();
                        }
                    }
                }
                //byte[] b = File.ReadAllBytes(filename);
                //byte [] b = File.ReadAllBytes(filename);
                //string[] str = File.ReadAllLines(filename);

                ////byte[] b = new byte[0];

                //foreach (var d in str)
                //{
                //    b = Convert.FromBase64String(d);
                //    data = string.Concat(data, Encoding.Default.GetString(b));
                //}

                var dic = data.Split(listDelimiter[0]).Select(x => x.Split(nexoDelimiter[0]))
                        .Where(y => !string.IsNullOrWhiteSpace(y[0]))
                        .ToDictionary(x => x[0], x => x[1]);

                _dicDataFile = dic;
            }
            foreach (var kv in _dicDataFile)
            {
                data = string.Concat(data, kv.Key, nexoDelimiter, kv.Value, listDelimiter);
            }
                   
            return data;
        }


        public static string Encrypt(string textToEncrypt)
        {
            string ToReturn = "";
            try
            {
                //string textToEncrypt = "WaterWorld";
                
                string publickey = _publickey;
                string secretkey = _secretkey;
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch{}
            return ToReturn;
        }
        public static string Decrypt(string textToDecrypt)
        {
            string ToReturn = "";
            try
            {
                //string textToDecrypt = "6+PXxVWlBqcUnIdqsMyUHA==";
                
                string publickey = _publickey;
                string secretkey = _secretkey;
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                    //ToReturn = Convert.ToBase64String(ms.ToArray());
                }
               
            }
            catch
            {
                ;// throw new Exception(ae.Message, ae.InnerException);
            }
            return ToReturn;
        }


        #region Private
        static void RefleshSection(string section = "")
        {
            if (section.Length == 0)
                section = "appSettings";
            ConfigurationManager.RefreshSection(section);
        }

        
        static bool IsNumeric<T>(T value)
        {
            string s = value.ToString();
            return s.All(char.IsNumber);
        }

    }
}

        #endregion
