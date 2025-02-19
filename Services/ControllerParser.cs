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
                    ? fullControllerName.Substring(0, fullControllerName.Length - "Controller".Length)
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
            // Retrieve methods with HTTP verb attributes (GET, POST, PUT, DELETE)
            var methods = controller
                .Members.OfType<MethodDeclarationSyntax>()
                .Where(m =>
                    m.AttributeLists.SelectMany(a => a.Attributes)
                        .Any(attr => attr.Name.ToString().StartsWith("Http"))
                );

            foreach (var method in methods)
            {
                // Get the HTTP verb and route from the method's attributes
                var httpAttr = method
                    .AttributeLists.SelectMany(a => a.Attributes)
                    .FirstOrDefault(attr => attr.Name.ToString().StartsWith("Http"));

                string methodName = method.Identifier.Text;
                string httpVerb =
                    httpAttr?.Name.ToString().Replace("Attribute", "").Replace("Http", "") ?? "";
                string route =
                    httpAttr?.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"') ?? "";

                // Check if the method has a body parameter (using [FromBody])
                bool hasBody = method.ParameterList.Parameters.Any(p =>
                    p.AttributeLists.Any(attr => attr.ToString().Contains("FromBody"))
                );

                // Extract the parameter type for the body, if applicable
                string parameterType = "";
                if (hasBody)
                {
                    var bodyParam = method.ParameterList.Parameters.FirstOrDefault(p =>
                        p.AttributeLists.Any(attr => attr.ToString().Contains("FromBody"))
                    );
                    parameterType = bodyParam?.Type?.ToString() ?? string.Empty;
                }

                // Extract non-body parameters
                var nonBodyParameters = method.ParameterList.Parameters
                    .Where(p => !p.AttributeLists.Any(attr => attr.ToString().Contains("FromBody")))
                    .Select(p => new Parameter
                    {
                        Type = p.Type?.ToString() ?? string.Empty,
                        Name = p.Identifier.ToString()
                    })
                    .ToList();

                // Create a method object and add it to the corresponding HTTP verb list in the controller
                var methodObj = new Method
                {
                    Name = methodName,
                    HttpVerb = httpVerb,
                    Route = route,
                    HasBody = hasBody,
                    ParameterType = parameterType,
                    Parameters = nonBodyParameters
                };

                // Group methods by HTTP verb (Get, Put, Post, Delete)
                if (httpVerb == "Get")
                {
                    controllerModel.GetMethods.Add(methodObj);
                }
                else if (httpVerb == "Put")
                {
                    controllerModel.PutMethods.Add(methodObj);
                }
                else if (httpVerb == "Post")
                {
                    controllerModel.PostMethods.Add(methodObj);
                }
                else if (httpVerb == "Delete")
                {
                    controllerModel.DeleteMethods.Add(methodObj);
                }
            }
        }
    }
}
