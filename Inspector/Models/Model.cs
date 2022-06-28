namespace Inspector.Models
{
    public class Model
    {
        public string ModelName { get; } 
        public string ModelHeader { get; }

        public Model(string modelName, string modelHeader)
        {
            this.ModelName = modelName;
            this.ModelHeader = modelHeader;
        }
    }
}
