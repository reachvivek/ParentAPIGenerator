namespace ParentApiGenerator.Config
{
    public static class ProjectInitializer
    {
        public static async Task InitializeProject(string outputFolder, string parentNamespace)
        {
            DirectoryHelper.CreateDirectoryStructure(outputFolder);
            await ProgramFileHandler.EnsureProgramCsExists(outputFolder, parentNamespace);
            await AppSettingsHandler.EnsureAppSettingsExists(outputFolder);
        }
    }
}
