using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Preceda.HealthCheck.DataLayer.Entities;

namespace Preceda.HealthCheck.DataLayer
{
    public class ImportConfiguration
    {          
        private readonly string Key = "aD34v2>,pd##jf94mflddd945DF(%244";
        private readonly Guid _ConfigId = new Guid(0xffb40f0a, 0x4624, 0x44f1, 0x89, 0xbc, 0x45, 0x62, 0xb8, 0x30, 0x8f, 0x7a);
        private readonly Repository<Configuration> _Repository;

        public string ISeriesServer { get; set; }
        public string ISeriesUser { get; set; }
        public string ISeriesPassword { get; set; }

        public ImportConfiguration(IDbConnection connection)
        {
            _Repository = new Repository<Configuration>(connection);
        }

        public async Task LoadConfiguration()
        {
            var configuration = await _Repository.Get(_ConfigId);

            if (configuration != null)
            {
                ISeriesServer = configuration.ISeriesServer;
                ISeriesUser = configuration.ISeriesUser;
                if (configuration.ISeriesPassword == "")
                    ISeriesPassword = "";
                else
                    ISeriesPassword = DecryptString(Key, configuration.ISeriesPassword);
            }
            else
            {
                ISeriesServer = "";
                ISeriesUser = "";
                ISeriesPassword = "";
            }
        }

        public async Task SaveConfiguration()
        {
            var configuration = new Configuration()
            {
                Id = _ConfigId,
                ISeriesServer = ISeriesServer,
                ISeriesUser = ISeriesUser
            };
            if (ISeriesPassword == "")
                configuration.ISeriesPassword = "";
            else
                configuration.ISeriesPassword = EncryptString(Key, ISeriesPassword);

            await _Repository.Update(configuration);
        }

        public static string EncryptString(string key, string plainInput)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
