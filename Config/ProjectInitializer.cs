namespace ParentApiGenerator.Config
{
    public static class ProjectInitializer
    {
        public static async Task InitializeProject(string outputFolder, string parentNamespace)
        {
            DirectoryHelper.CreateDirectoryStructure(outputFolder);

            var programFileHandler = new ProgramFileHandler();
            await programFileHandler.EnsureProgramCsExists(outputFolder, parentNamespace);
            await AppSettingsHandler.EnsureAppSettingsExists(outputFolder);
        }
    }
}
