using ParentApiGenerator.Services;

namespace ParentApiGenerator.Controllers
{
    public static class ControllerGenerator
    {
        public static async Task GenerateControllers(
            string inputFolder,
            string outputFolder,
            string parentNamespace
        )
        {
            string controllersFolder = Path.Combine(inputFolder, "Controllers");
            if (!Directory.Exists(controllersFolder))
            {
                controllersFolder =
                    Directory
                        .GetDirectories(inputFolder, "Controllers", SearchOption.AllDirectories)
                        .FirstOrDefault() ?? string.Empty;

                if (string.IsNullOrEmpty(controllersFolder))
                {
                    Console.WriteLine("No 'Controllers' folder found in the input directory.");
                    return;
                }
            }

            Console.WriteLine($"Searching for controllers in folder: {controllersFolder}");

            var files = Directory.GetFiles(controllersFolder, "*.cs", SearchOption.AllDirectories);

            // Ensure the output folder ends with "Controllers" folder path
            string outputControllersFolder;
            if (outputFolder.EndsWith("Controllers", StringComparison.OrdinalIgnoreCase))
            {
                outputControllersFolder = outputFolder;  // If it's already the Controllers folder
            }
            else
            {
                outputControllersFolder = Path.Combine(outputFolder, "Controllers");  // Otherwise, append Controllers
            }

            // Ensure the "Controllers" subfolder exists in the output directory
            if (!Directory.Exists(outputControllersFolder))
            {
                Directory.CreateDirectory(outputControllersFolder);
            }

            // Process each file and generate controllers in the appropriate folder
            foreach (var file in files)
            {
                await GenerateControllerFromFile(file, outputControllersFolder, parentNamespace);
            }
        }

        private static async Task GenerateControllerFromFile(
            string file,
            string outputFolder,
            string parentNamespace
        )
        {
            // Read the file content
            string fileContent = await File.ReadAllTextAsync(file);

            // Parse the controllers from the file content
            var controllers = ControllerParser.ParseControllers(fileContent);

            // Now use CodeGenerator to generate the controllers in the "Controllers" folder
            var codeGenerator = new CodeGenerator();
            await codeGenerator.GenerateCodeAsync(controllers, outputFolder, parentNamespace);

            Console.WriteLine($"Controller generation for {file} completed.");
        }
    }
}
