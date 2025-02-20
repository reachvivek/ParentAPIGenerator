using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParentApiGenerator.Models;

namespace ParentApiGenerator.Services
{
    public static class ControllerParser
    {
        public static List<Controller> ParseControllers(string fileContent)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            var root = syntaxTree.GetRoot();

            var controllerClasses = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cls =>
                    cls.AttributeLists.SelectMany(attrList => attrList.Attributes)
                        .Any(attr => attr.Name.ToString().Contains("ApiController"))
                );

            var controllers = new List<Controller>();

            foreach (var controller in controllerClasses)
            {
                string fullControllerName = controller.Identifier.Text;
                string simpleControllerName = fullControllerName.EndsWith("Controller")
                    ? fullControllerName.Substring(
                        0,
                        fullControllerName.Length - "Controller".Length
                    )
                    : fullControllerName;

                var controllerModel = new Controller { ControllerName = simpleControllerName };

                // Extract methods and group them
                ExtractMethods(controller, controllerModel);

                controllers.Add(controllerModel);
            }

            return controllers;
        }

        private static void ExtractMethods(
            ClassDeclarationSyntax controller,
            Controller controllerModel
        )
        {
            var methods = controller
                .Members.OfType<MethodDeclarationSyntax>()
                .Where(m =>
                    m.AttributeLists.SelectMany(a => a.Attributes)
                        .Any(attr => attr.Name.ToString().StartsWith("Http"))
                );

            foreach (var method in methods)
            {
                var httpAttr = method
                    .AttributeLists.SelectMany(a => a.Attributes)
                    .FirstOrDefault(attr => attr.Name.ToString().StartsWith("Http"));

                string methodName = method.Identifier.Text;
                string httpVerb =
                    httpAttr?.Name.ToString().Replace("Attribute", "").Replace("Http", "") ?? "";
                // Get route from Http* attribute, if present.
                string route =
                    httpAttr?.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"') ?? "";

                // If route is still empty, try to get it from a separate [Route] attribute.
                if (string.IsNullOrEmpty(route))
                {
                    var routeAttr = method
                        .AttributeLists.SelectMany(a => a.Attributes)
                        .FirstOrDefault(attr => attr.Name.ToString().Contains("Route"));
                    route =
                        routeAttr?.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"')
                        ?? "";
                }

                // Detect [FromBody] and [FromForm]
                bool hasBody = method.ParameterList.Parameters.Any(p =>
                    p.AttributeLists.Any(attr =>
                        attr.ToString().Contains("FromBody") || attr.ToString().Contains("FromForm")
                    )
                );
                bool hasForm = method.ParameterList.Parameters.Any(p =>
                    p.AttributeLists.Any(attr => attr.ToString().Contains("FromForm"))
                );

                string parameterType = "";
                if (hasBody || hasForm)
                {
                    var bodyParam = method.ParameterList.Parameters.FirstOrDefault(p =>
                        p.AttributeLists.Any(attr =>
                            attr.ToString().Contains("FromBody")
                            || attr.ToString().Contains("FromForm")
                        )
                    );
                    parameterType = bodyParam?.Type?.ToString() ?? string.Empty;
                }

                var nonBodyParameters = method
                    .ParameterList.Parameters.Where(p =>
                        !p.AttributeLists.Any(attr =>
                            attr.ToString().Contains("FromBody")
                            || attr.ToString().Contains("FromForm")
                        )
                    )
                    .Select(p => new Parameter
                    {
                        Type = p.Type?.ToString() ?? string.Empty,
                        Name = p.Identifier.ToString(),
                    })
                    .ToList();

                var methodObj = new Method
                {
                    Name = methodName,
                    HttpVerb = httpVerb,
                    Route = route,
                    HasBody = hasBody,
                    HasForm = hasForm, // Add a new property in the Method model
                    ParameterType = parameterType,
                    Parameters = nonBodyParameters,
                };

                if (httpVerb == "Get")
                    controllerModel.GetMethods.Add(methodObj);
                else if (httpVerb == "Put")
                    controllerModel.PutMethods.Add(methodObj);
                else if (httpVerb == "Post")
                    controllerModel.PostMethods.Add(methodObj);
                else if (httpVerb == "Delete")
                    controllerModel.DeleteMethods.Add(methodObj);
            }
        }
    }
}
