namespace Inspector.Models
{
    public class ModelElement
    {
        private Model _parentModel;
        private string _modelName;
        private string _modelHeader;
        private string _modelDescription;

        public Model ParentModel { get { return _parentModel; } set { _parentModel = value; } }
        public string ModelName { get { return _modelName; } set { _modelName = value; } }
        public string ModelHeader { get { return _modelHeader; } set { _modelHeader = value; } }
        public string ModelDescription { get { return _modelDescription; } set { _modelDescription = value; } }
        public ModelElement(Model parentModel, string mName = "", string mHeader = "", string mDescription = "")
        {
            this.ParentModel = parentModel;
            this.ModelName = mName;
            this.ModelHeader = mHeader;
            this.ModelDescription = mDescription;
        }
    }
}
