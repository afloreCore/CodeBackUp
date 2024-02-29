using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Web;

namespace WrapperMercadoPagoAPI.General;
public static class Utilities
{
    private const int KeySize = 256; // in bits
    private const string formatISO8601 = "yyyy-MM-ddTHH:mm:ss.000zzzz";
    public static void SimpleWrite(object obj, string fileName)
    {
        var jsonString = JsonConvert.SerializeObject(obj);
        try
        {
            File.WriteAllText(fileName, jsonString);
            // serialize JSON directly to a file
            //using (StreamWriter file = File.CreateText(@"c:\Temp\Sucursales.json"))
            //{
            //    JsonConvert.SerializeObject(_options);
            //}
            //using FileStream createStream = File.Create(fileName);
            //JsonSerializer.SerializeAsync(createStream, jsonString);
        }
        catch { }

    }
    public static string MakeUriParameters<T>(T obj, bool nullValue = true)
    {
        var settings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        if (nullValue)
            settings.NullValueHandling = NullValueHandling.Ignore;

        var ser = JsonConvert.SerializeObject(obj, settings);
        var jObj = (JObject?)JsonConvert.DeserializeObject(ser);

        var query = String.Join("&",
                        jObj!.Children().Cast<JProperty>()
                        .Select(jp => jp.Name + "=" + HttpUtility.UrlEncode(jp.Value.ToString())));

        return query;
    }
    public static void DisposeItems<T>(this IEnumerable<T> source) where T : IDisposable
    {
        foreach (var item in source)
        {
            item.Dispose();
        }
    }
    public static string GetStrDateISO8601(DateTime date)
    {
        DateTimeOffset localTimeAndOffset = new DateTimeOffset(date, TimeZoneInfo.Local.GetUtcOffset(date));
        return localTimeAndOffset.ToString(formatISO8601);

    }
    public static DateTime GetDateTimeFromISO8601(string date)
    {
        DateTime.TryParseExact(date, formatISO8601, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime datetime);
        return datetime;
    }
    public static T CastEnum<T>(string name)
    {
        return (T)System.Enum.Parse(typeof(T), name);
    }
    /*******************************************************************************************************/
    public static string EncryptStringToBase64String(string plainText, byte[] Key)
    {
        // Check arguments. 
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        byte[] returnValue;
        using var aes = Aes.Create();

        aes.KeySize = KeySize;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        var iv = aes.IV;
        if (string.IsNullOrEmpty(plainText))
            return Convert.ToBase64String(iv);
        var encryptor = aes.CreateEncryptor(Key, iv);

        // Create the streams used for encryption. 
        using MemoryStream msEncrypt = new MemoryStream();
        using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using StreamWriter swEncrypt = new StreamWriter(csEncrypt);

        //Write all data to the stream.
        swEncrypt.Write(plainText);
        // this is just our encrypted data
        var encrypted = msEncrypt.ToArray();
        returnValue = new byte[encrypted.Length + iv.Length];
        // append our IV so our decrypt can get it
        Array.Copy(iv, returnValue, iv.Length);
        // append our encrypted data
        Array.Copy(encrypted, 0, returnValue, iv.Length, encrypted.Length);

        // return encrypted bytes converted to Base64String
        return Convert.ToBase64String(returnValue);
    }

    public static string DecryptStringFromBase64String(string cipherText, byte[] Key)
    {
        // Check arguments. 
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");

        string plaintext = null!;
        // this is all of the bytes
        var allBytes = Convert.FromBase64String(cipherText);

        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.CBC;

            // get our IV that we pre-pended to the data
            byte[] iv = new byte[aes.BlockSize / 8];
            if (allBytes.Length < iv.Length)
                throw new ArgumentException("Message was less than IV size.");
            Array.Copy(allBytes, iv, iv.Length);
            // get the data we need to decrypt
            byte[] cipherBytes = new byte[allBytes.Length - iv.Length];
            Array.Copy(allBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

            // Create a decrytor to perform the stream transform.
            var decryptor = aes.CreateDecryptor(Key, iv);

            // Create the streams used for decryption. 
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream 
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
    public static string DecryptStringFromIVBase64(string cipherText, string strKey, string strIV)
    {
        // Check arguments. 
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;
        byte[] Key = Convert.FromBase64String(strKey);
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");

        string plaintext = null!;
        // this is all of the bytes
        var allBytes = Convert.FromBase64String(cipherText);

        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.CBC;

            // get our IV that we pre-pended to the data
            byte[] iv = Convert.FromBase64String(strIV);
            if (allBytes.Length < iv.Length)
                throw new ArgumentException("Message was less than IV size.");
            Array.Copy(allBytes, iv, iv.Length);
            // get the data we need to decrypt
            byte[] cipherBytes = new byte[allBytes.Length - iv.Length];
            Array.Copy(allBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

            // Create a decrytor to perform the stream transform.
            var decryptor = aes.CreateDecryptor(Key, iv);

            // Create the streams used for decryption. 
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream 
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}

