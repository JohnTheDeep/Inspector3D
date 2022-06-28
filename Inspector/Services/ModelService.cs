using System.IO;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using Inspector.Models;
using Inspector.Services.Interfaces;
using WpfApp4;

namespace Inspector.Services
{
    public class ModelService : IModelService
    {
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;

        private string _fileName;
        private string _fileNameWithNoEx;
        private ObjReader _objReader;
        private ObservableCollection<ModelElement> _modelElements;

        private event EventHandler MakeJsonFilesHandler;
        public ModelService(IDialogService dialogService, IFileService fileService)
        {
            this._dialogService = dialogService;
            this._fileService = fileService;
            _modelElements = new ObservableCollection<ModelElement>();
            MakeJsonFilesHandler += MakeJsonFiles;
        }
        private void MakeJsonFiles(object sender, EventArgs e)
        {
            Repository reposit = new Repository(_fileName);
            reposit.Dispose();
            _fileService.MakeJsonFiles(_fileNameWithNoEx, _modelElements);
        }

        public ObservableCollection<ModelElement> FillModelElementsCollection(Model3DGroup _model)
        {
            _modelElements?.Clear();
            if(_model != null)
            {
                foreach (var item in _model.Children)
                    _modelElements?.Add(new ModelElement
                        (new Model(_fileNameWithNoEx, _fileNameWithNoEx),
                        item.GetName().ToString(),
                        $"{item.GetName()}Header",
                        "Default Description"));
                MakeJsonFilesHandler(this, new EventArgs());
            }
            return _modelElements;
        }
        public Model3DGroup open()
        {
            Model3DGroup _model = new Model3DGroup();
            if (_dialogService.OpenFileDialog())
            {
                _fileName = _dialogService.FileName;
                _fileNameWithNoEx = Path.GetFileNameWithoutExtension(_fileName);
                if (_model != null)
                {
                    _objReader = new ObjReader();
                    _model = _objReader.Read(_fileName);
                }
            }
            return _model;
        }
        public ModelVisual3D LoadVisualModel(Model3DGroup _model)
        {
            ModelVisual3D _visualModel = new ModelVisual3D();
            if(_visualModel != null && _model != null)
            { _visualModel.Content = _model; }
            return _visualModel;
        }
    }
}
