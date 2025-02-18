using System.Text.Json.Nodes;

namespace ParentApiGenerator.Config
{
    public static class AppSettingsHandler
    {
        public static async Task EnsureAppSettingsExists(string outputFolder)
        {
            string appSettingsPath = Path.Combine(outputFolder, "appsettings.json");

            if (!File.Exists(appSettingsPath))
            {
                string appSettingsContent =
                    "{\n  \"API\": {\n    \"BaseURL\": \"https://localhost:3200\"\n  }\n}";
                await File.WriteAllTextAsync(appSettingsPath, appSettingsContent);
            }
        }

        public static async Task UpdateAppSettingsJson(string outputFolder)
        {
            string appSettingsPath = Path.Combine(outputFolder, "appsettings.json");

            if (File.Exists(appSettingsPath))
            {
                string content = await File.ReadAllTextAsync(appSettingsPath);
                var json = JsonObject.Parse(content);

                if (json is JsonObject jsonObject && !jsonObject.ContainsKey("API"))
                {
                    json["API"] = new JsonObject { ["BaseURL"] = "http://localhost:3200" };
                }
                else
                {
                    var apiSection = json["API"] as JsonObject;
                    if (apiSection != null)
                    {
                        apiSection["BaseURL"] = "http://localhost:3200";
                    }
                }

                await File.WriteAllTextAsync(appSettingsPath, json.ToString());
                Console.WriteLine("appsettings.json Updated with correct BaseURL!");
            }
            else
            {
                string defaultAppSettings =
                    "{\n  \"API\": {\n    \"BaseURL\": \"http://localhost:3200\"\n  }\n}";
                await File.WriteAllTextAsync(appSettingsPath, defaultAppSettings);
                Console.WriteLine("appsettings.json Created with API BaseURL!");
            }
        }
    }
}
