namespace ParentApiGenerator.Models
{
    public partial class Method
    {
        public string? Name { get; set; }
        public string? HttpVerb { get; set; }
        public string? Route { get; set; }
        public bool HasBody { get; set; }
        public bool HasForm { get; set; }
        public string? ParameterType { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
