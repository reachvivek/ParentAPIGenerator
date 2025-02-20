namespace ParentApiGenerator.Config
{
    public class ProgramFileHandler
    {
        private readonly string _template;

        public ProgramFileHandler()
        {
            string templatesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            string templateFilePath = Path.Combine(
                templatesFolderPath,
                "ProgramFileTemplate.liquid"
            );

            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException("Template file not found: " + templateFilePath);
            }

            _template = File.ReadAllText(templateFilePath);
        }

        public async Task EnsureProgramCsExists(string outputFolder, string parentNamespace)
        {
            string programCsFilePath = Path.Combine(outputFolder, "Program.cs");
            string programContent = _template.Replace("{{namespace}}", parentNamespace);

            await File.WriteAllTextAsync(programCsFilePath, programContent);
            Console.WriteLine("Program.cs has been successfully created/updated.");
        }
    }
}
