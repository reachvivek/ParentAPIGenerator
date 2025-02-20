using DotLiquid;
using ParentApiGenerator.Models;

namespace ParentApiGenerator.Services
{
    public class CodeGenerator
    {
        private readonly string _template;

        public CodeGenerator()
        {
            // Ensure this path points to your templates
            string templatesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            // Reading the template file from the templates folder
            _template = File.ReadAllText(
                Path.Combine(templatesFolderPath, "ControllerTemplate.liquid")
            );
        }

        public async Task GenerateCodeAsync(
            List<Controller> controllers,
            string outputFolder,
            string parentNamespace
        )
        {
            // Parse the DotLiquid template
            Template template = Template.Parse(_template);

            foreach (var controller in controllers)
            {
                Console.WriteLine($"Controller Name: {controller.ControllerName}");
                Console.WriteLine($"GetMethods: {controller.GetMethods.Count}");
                Console.WriteLine($"PutMethods: {controller.PutMethods.Count}");
                Console.WriteLine($"PostMethods: {controller.PostMethods.Count}");
                Console.WriteLine($"DeleteMethods: {controller.DeleteMethods.Count}");

                // Prepare the model for the template rendering
                var model = new Dictionary<string, object>
                {
                    ["namespace"] = parentNamespace,
                    ["controllername"] = controller.ControllerName,
                    ["getmethods"] = controller
                        .GetMethods.Select(gm => new Dictionary<string, object>
                        {
                            { "route", gm.Route ?? string.Empty },
                            { "name", gm.Name ?? string.Empty },
                            { "hasbody", gm.HasBody },
                            { "parametertype", gm.ParameterType ?? string.Empty },
                            {
                                "parameters",
                                gm
                                    .Parameters.Select(p => new Dictionary<string, object>
                                    {
                                        { "type", p.Type ?? string.Empty },
                                        { "name", p.Name ?? string.Empty },
                                    })
                                    .ToList()
                            },
                        })
                        .ToList(),
                    ["putmethods"] = controller
                        .PutMethods.Select(pm => new Dictionary<string, object>
                        {
                            { "route", pm.Route ?? string.Empty },
                            { "name", pm.Name ?? string.Empty },
                            { "hasbody", pm.HasBody },
                            { "parametertype", pm.ParameterType ?? string.Empty },
                            {
                                "parameters",
                                pm
                                    .Parameters.Select(p => new Dictionary<string, object>
                                    {
                                        { "type", p.Type ?? string.Empty },
                                        { "name", p.Name ?? string.Empty },
                                    })
                                    .ToList()
                            },
                        })
                        .ToList(),
                    ["postmethods"] = controller
                        .PostMethods.Select(pm => new Dictionary<string, object>
                        {
                            { "route", pm.Route ?? string.Empty },
                            { "name", pm.Name ?? string.Empty },
                            { "hasbody", pm.HasBody },
                            { "parametertype", pm.ParameterType ?? string.Empty },
                            {
                                "parameters",
                                pm
                                    .Parameters.Select(p => new Dictionary<string, object>
                                    {
                                        { "type", p.Type ?? string.Empty },
                                        { "name", p.Name ?? string.Empty },
                                    })
                                    .ToList()
                            },
                        })
                        .ToList(),
                    ["deletemethods"] = controller
                        .DeleteMethods.Select(dm => new Dictionary<string, object>
                        {
                            { "route", dm.Route ?? string.Empty },
                            { "name", dm.Name ?? string.Empty },
                            {
                                "parameters",
                                dm
                                    .Parameters.Select(p => new Dictionary<string, object>
                                    {
                                        { "type", p.Type ?? string.Empty },
                                        { "name", p.Name ?? string.Empty },
                                    })
                                    .ToList()
                            },
                        })
                        .ToList(),
                };

                // Define the output file path
                var outputFile = Path.Combine(
                    outputFolder,
                    $"{controller.ControllerName}Controller.cs"
                );

                // Render the template using the model
                var rendered = template.Render(Hash.FromDictionary(model));

                // Write the rendered content to the output file
                await File.WriteAllTextAsync(outputFile, rendered);
                Console.WriteLine($"Generated: {outputFile}");
            }
        }
    }
}
