namespace ParentApiGenerator.Models
{
    public class Controller
    {
        public required string ControllerName { get; set; }
        public List<Method> Methods { get; set; } = new List<Method>();

        // Add these new properties to group methods by their HTTP verbs
        public List<Method> GetMethods { get; set; } = new List<Method>();
        public List<Method> PutMethods { get; set; } = new List<Method>();
        public List<Method> PostMethods { get; set; } = new List<Method>();
        public List<Method> DeleteMethods { get; set; } = new List<Method>();
    }
}
