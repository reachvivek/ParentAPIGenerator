namespace ParentApiGenerator.Config
{
    public static class ProgramFileHandler
    {
        public static async Task EnsureProgramCsExists(string outputFolder, string parentNamespace)
        {
            string programCsFilePath = Path.Combine(outputFolder, "Program.cs");

            if (!File.Exists(programCsFilePath))
            {
                string programContent =
                    $@"
                        using {parentNamespace}.Utility;

                        var builder = WebApplication.CreateBuilder(args);

                        // Add CORS policy configuration
                        builder.Services.AddCors(options =>
                        {{
                            options.AddPolicy(""AllowAllPolicy"", builder =>
                            {{
                                builder
                                    .AllowAnyOrigin()  // Allow all origins
                                    .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
                                    .AllowAnyHeader(); // Allow any headers
                            }});
                        }});

                        builder.Services.AddHttpClient<ApiRequest>(client =>
                        {{
                            client.BaseAddress = new Uri(builder.Configuration[""API:BaseURL""]);
                        }});

                        builder.Services.AddControllers();

                        var app = builder.Build();

                        app.UseCors(""AllowAllPolicy"");

                        app.MapControllers();

                        await app.RunAsync();";

                await File.WriteAllTextAsync(programCsFilePath, programContent);
            }
        }

        public static async Task RegisterApiRequestInProgramCs(string outputFolder)
        {
            string programCsFilePath = Path.Combine(outputFolder, "Program.cs");
            if (!File.Exists(programCsFilePath))
                return;

            string programContent = await File.ReadAllTextAsync(programCsFilePath);

            // Check if the HttpClient registration already exists
            if (!programContent.Contains("AddHttpClient<ApiRequest>"))
            {
                string diCode =
                    @"
                    // Add HTTP client for API requests
                    builder.Services.AddHttpClient<ApiRequest>(client =>
                    {
                        client.BaseAddress = new Uri(builder.Configuration[""API:BaseURL""]);
                    });";

                int insertIndex = programContent.IndexOf("builder.Services.AddControllers();");
                programContent = programContent.Insert(
                    insertIndex + "builder.Services.AddControllers();".Length,
                    diCode
                );

                await File.WriteAllTextAsync(programCsFilePath, programContent);
                Console.WriteLine("Successfully added DI registration for ApiRequest.");
            }

            // Check if the CORS policy is already included
            if (!programContent.Contains("AllowAllPolicy"))
            {
                string corsPolicyCode =
                    @"
                    builder.Services.AddCors(options =>
                    {
                        options.AddPolicy(""AllowAllPolicy"", builder =>
                        {
                            builder
                                .AllowAnyOrigin()  // Allow all origins
                                .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
                                .AllowAnyHeader(); // Allow any headers
                        });
                    });";

                int insertIndexCors = programContent.IndexOf("builder.Services.AddControllers();");
                programContent = programContent.Insert(
                    insertIndexCors + "builder.Services.AddControllers();".Length,
                    corsPolicyCode
                );

                // Add the use of CORS in the pipeline
                string useCorsCode = 
                    @"
                    app.UseCors(""AllowAllPolicy"");";

                int insertIndexUseCors = programContent.IndexOf("app.MapControllers();");
                programContent = programContent.Insert(
                    insertIndexUseCors,
                    useCorsCode
                );

                await File.WriteAllTextAsync(programCsFilePath, programContent);
                Console.WriteLine("Successfully added CORS policy configuration.");
            }
        }
    }
}
