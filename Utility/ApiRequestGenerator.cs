using DotLiquid;

namespace ParentApiGenerator.Utility
{
    public static class ApiRequestGenerator
    {
        public static async Task GenerateApiRequestFile(string outputFolder, string parentNamespace)
        {
            string utilityFolder = Path.Combine(outputFolder, "Utility");
            Directory.CreateDirectory(utilityFolder); // Ensure Utility folder exists

            string apiRequestTemplatePath = Path.Combine("Templates", "ApiRequestTemplate.liquid");

            if (!File.Exists(apiRequestTemplatePath))
            {
                Console.WriteLine("Template file 'ApiRequestTemplate.liquid' not found.");
                return;
            }

            string apiRequestTemplate = await File.ReadAllTextAsync(apiRequestTemplatePath);
            Template parsedTemplate = Template.Parse(apiRequestTemplate);

            var model = new { Namespace = parentNamespace };
            string renderedApiRequest = parsedTemplate.Render(Hash.FromAnonymousObject(model));

            string apiRequestOutputFile = Path.Combine(utilityFolder, "ApiRequest.cs");

            await File.WriteAllTextAsync(apiRequestOutputFile, renderedApiRequest);
            Console.WriteLine($"Generated ApiRequest File: {apiRequestOutputFile}");
        }
    }
}
