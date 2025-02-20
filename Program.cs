using ParentApiGenerator.Config;
using ParentApiGenerator.Controllers;
using ParentApiGenerator.Utility;

namespace ParentApiGenerator
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var (inputFolder, outputFolder, parentNamespace) = await UserInputHandler.GetUserInput(args);

            Console.WriteLine($"Input Folder: {inputFolder}");
            Console.WriteLine($"Output Folder: {outputFolder}");
            Console.WriteLine($"Parent Namespace: {parentNamespace}");

            // Initialize Project
            await ProjectInitializer.InitializeProject(outputFolder, parentNamespace);

            // Generate Controllers
            await ControllerGenerator.GenerateControllers(inputFolder, outputFolder, parentNamespace);

            // Generate ApiRequest File
            await ApiRequestGenerator.GenerateApiRequestFile(outputFolder, parentNamespace);

            // Register DI in Program.cs
            var programFileHandler = new ProgramFileHandler();
            await programFileHandler.EnsureProgramCsExists(outputFolder, parentNamespace);

            // Finalize appsettings.json
            await AppSettingsHandler.UpdateAppSettingsJson(outputFolder);

            Console.WriteLine("Parent API Generation Complete!");
        }
    }
}
