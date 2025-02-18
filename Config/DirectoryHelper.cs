namespace ParentApiGenerator.Config
{
    public static class DirectoryHelper
    {
        public static void CreateDirectoryStructure(string outputFolder)
        {
            Directory.CreateDirectory(Path.Combine(outputFolder, "Controllers"));
            Directory.CreateDirectory(Path.Combine(outputFolder, "Utility"));
        }
    }
}
