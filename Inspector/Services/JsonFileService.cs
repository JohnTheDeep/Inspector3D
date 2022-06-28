using Inspector.Models;
using Inspector.Services.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace Inspector.Services
{
    internal class JsonFileService : IFileService
    {
        private string _filePath { get; set; }
        public bool Open(string _fileNameWithNoEx)
        {
            _filePath = Path.Combine(@"3dmodels", (_fileNameWithNoEx + @"\" + $@"{_fileNameWithNoEx}.json"));
            if (File.Exists(_filePath))
                return true;
            else return false;
        }

        public void Save(string filepath, object file) =>
            throw new NotImplementedException();
        public void LoadJsonFile(string _fileNameWithNoEx, ref ObservableCollection<ModelElement> _modelElements)
        {
            if (Open(_fileNameWithNoEx))
            {
                var jsonFile = File.ReadAllText(_filePath);
                _modelElements = JsonConvert.DeserializeObject<ObservableCollection<ModelElement>>(jsonFile);
            }   
        }

        public void MakeJsonFiles(string _fileNameWithNoEx, ObservableCollection<ModelElement> _modelElements)
        {
            if(!Open(_fileNameWithNoEx))
            {
                string jsonText = JsonConvert.SerializeObject(_modelElements?.ToArray(), Formatting.Indented);
                File.WriteAllText(_filePath, jsonText);
            }
        }
    }
}
