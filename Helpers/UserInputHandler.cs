using Spectre.Console;

namespace ParentApiGenerator
{
    public static class UserInputHandler
    {
        public static Task<(
            string inputFolder,
            string outputFolder,
            string parentNamespace
        )> GetUserInput(string[] args)
        {
            string inputFolder = string.Empty;
            string outputFolder = string.Empty;
            string parentNamespace = string.Empty;

            if (args.Length == 0)
            {
                inputFolder = AnsiConsole.Ask<string>("Enter the [green]input folder[/]:");
                outputFolder = AnsiConsole.Ask<string>("Enter the [green]output folder[/]:");
                parentNamespace = AnsiConsole.Ask<string>("Enter the [green]parent namespace[/]:");
            }
            else
            {
                inputFolder = args.Length > 0 ? args[0] : string.Empty;
                outputFolder = args.Length > 1 ? args[1] : string.Empty;
                parentNamespace = args.Length > 2 ? args[2] : string.Empty;

                if (string.IsNullOrEmpty(inputFolder))
                    inputFolder = AnsiConsole.Ask<string>("Enter the [green]input folder[/]:");

                if (string.IsNullOrEmpty(outputFolder))
                    outputFolder = AnsiConsole.Ask<string>("Enter the [green]output folder[/]:");

                if (string.IsNullOrEmpty(parentNamespace))
                    parentNamespace = AnsiConsole.Ask<string>(
                        "Enter the [green]parent namespace[/]:"
                    );
            }

            return Task.FromResult((inputFolder, outputFolder, parentNamespace));
        }
    }
}
