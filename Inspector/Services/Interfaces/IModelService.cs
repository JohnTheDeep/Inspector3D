using Inspector.Models;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace Inspector.Services.Interfaces
{
    public interface IModelService
    {
        Model3DGroup open(); 
        ObservableCollection<ModelElement> FillModelElementsCollection(Model3DGroup _model);
    }
}
