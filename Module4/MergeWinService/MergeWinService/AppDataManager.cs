using System;
using System.IO;
using Newtonsoft.Json;

namespace MergeWinService
{
    public class AppDataManager
    {
        private readonly string _fileName;

        public AppDataManager(string fileName)
        {
            _fileName = fileName;
        }

        public void WrireToFile(AppData appData)
        {
            string json = JsonConvert.SerializeObject(appData);
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllText(filePath, json);
        }

        public AppData ReadFromFile()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AppData>(json) ?? new AppData();
        }
    }
}
