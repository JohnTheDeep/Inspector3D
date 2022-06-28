using Inspector.Models;
using System.Collections.ObjectModel;

namespace Inspector.Services.Interfaces
{
    public interface IFileService
    {
        bool Open(string _fileNameWithNoEx);
        void Save(string filepath, object file);
        void MakeJsonFiles(string _fileNameWithNoEx, ObservableCollection<ModelElement> _modelElements);
    }
}
