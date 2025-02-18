
# Parent API Generator 🚀

Welcome to the **Parent API Generator**! This tool helps you quickly generate a Parent API that can forward requests to child APIs using configurable routes, methods, and endpoints.

With this generator, you can dynamically create controllers and related configurations, making it easier to manage and maintain APIs that depend on child APIs. 🎉

## 🛠️ Features

- Automatically generate controllers based on existing ones.
- Customize the Base URL of the child APIs with ease.
- Integrate with Dependency Injection for streamlined HTTP client usage.
- Easy configuration via `appsettings.json` for flexible environments.
- Supports GET, POST, PUT, and DELETE methods with route mapping.

## ⚙️ How to Use

### 1. **Setup Project Structure**

The tool will automatically generate the following folders and files in your output directory:

- `Controllers`: Contains generated controllers for the Parent API.
- `Program.cs`: Contains the setup code to forward requests to the child API.
- `appsettings.json`: For easy configuration of the **BaseURL** for child APIs.

### 2. **Install Dependencies**

Make sure you have all necessary dependencies for your API project. For example, you will need `DotLiquid` for template rendering:

```bash
dotnet add package DotLiquid
```

### 3. **Running the Parent API Generator**

- Clone this repository or include the **Parent API Generator** in your project.
- Run the tool to generate the Parent API with controllers based on your child API:

```csharp
await ControllerGenerator.GenerateControllers(inputFolder, outputFolder, parentNamespace);
```

### 4. **Configuring the Base URL**

You can easily configure the **Base URL** in the `appsettings.json` file. This allows you to forward requests to the correct child API.

#### Example `appsettings.json`:

```json
{
  "API": {
    "BaseURL": "https://yourchildapi.com/api/"
  },
  // Other settings...
}
```

- The **Base URL** is used by the `ApiRequest` utility to forward all requests to the child API.

### 5. **Program.cs Configuration**

The **Program.cs** file contains the necessary code to register the `ApiRequest` service and ensure that the **Base URL** from the `appsettings.json` file is used.

Here’s how `Program.cs` should look:

```csharp
using {parentNamespace}.Utility;

var builder = WebApplication.CreateBuilder(args);

// Register ApiRequest with HttpClient and Base URL from appsettings
builder.Services.AddHttpClient<ApiRequest>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["API:BaseURL"]);
});

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
await app.RunAsync();
```

This configuration ensures the **Base URL** is dynamically picked up and used for all forwarded requests.

### 6. **Using ApiRequest to Forward Requests**

The `ApiRequest` utility is registered via Dependency Injection and can be used throughout your controllers to forward requests to the child API.

Example:

```csharp
public class MyController : ControllerBase
{
    private readonly ApiRequest _apiRequest;

    public MyController(ApiRequest apiRequest)
    {
        _apiRequest = apiRequest;
    }

    [HttpGet("fetch-data")]
    public async Task<IActionResult> FetchData()
    {
        var result = await _apiRequest.GetAsync("endpoint/child-resource");
        return Ok(result);
    }
}
```

- In this example, the **Base URL** from `appsettings.json` is automatically used, and requests are forwarded to `https://yourchildapi.com/api/endpoint/child-resource`.

### 7. **Switching Between Different Environments**

To switch between different environments (development, staging, production), you can maintain separate configuration files such as `appsettings.Development.json`, `appsettings.Production.json`, etc.

Example for **staging**:

```json
{
  "API": {
    "BaseURL": "https://staging-api.example.com/api/"
  }
}
```

The tool will automatically pick the correct configuration for your environment.

---

## ✨ Benefits of Using the Parent API Generator

- **Time Saver**: Automatically generate the code you need for the Parent API, saving you from repetitive coding tasks.
- **Customization**: Easily customize and extend generated code to fit your needs.
- **Flexible**: Configure different base URLs for different environments and manage routing easily.

---

## 📝 Contributing

Contributions are welcome! If you have suggestions or improvements, feel free to open an issue or submit a pull request.

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Enjoy building your Parent APIs! 🚀
