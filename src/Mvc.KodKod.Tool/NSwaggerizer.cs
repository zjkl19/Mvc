using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using NSwag.SwaggerGeneration;

namespace Mvc.KodKod.Tool
{
    public static class NSwaggerizer
    {
        public static SwaggerDocument GetSwaggerDocument(
            ApiDescriptionGroupCollection apiDescriptionGroupCollection,
            KodKodOptions options)
        {
            var document = new SwaggerDocument()
            {
                Info =
                {
                    Title = options.ApplicationName,
                    Version = "1.2",
                },
                BasePath = options.BaseUrl,
            };

            var settings = new JsonSchemaGeneratorSettings();
            var schemaResolver = new SwaggerSchemaResolver(document, settings);
            var schemaGenerator = new SwaggerJsonSchemaGenerator(settings);
            var attributes = Enumerable.Empty<Attribute>();


            var operations = new List<SwaggerOperationDescription>();
            foreach (var group in apiDescriptionGroupCollection.Items)
            {
                foreach (var apiDescription in group.Items)
                {
                    var operation = new SwaggerOperation
                    {
                        Consumes = apiDescription.SupportedRequestFormats.Select(f => f.MediaType).ToList(),
                        Produces = apiDescription.SupportedResponseTypes.SelectMany(r => r.ApiResponseFormats.Select(t => t.MediaType)).ToList(),
                        Description = apiDescription.ActionDescriptor.DisplayName,
                        OperationId = CreateOperationId(apiDescription),
                    };

                    if (!string.IsNullOrEmpty(group.GroupName))
                    {
                        operation.Tags.Add(group.GroupName);
                    }

                    foreach (var response in apiDescription.SupportedResponseTypes)
                    {
                        string statusCode;
                        if (response.StatusCode == 0)
                        {
                            statusCode = "default";
                        }
                        else
                        {
                            statusCode = response.StatusCode.ToString(CultureInfo.InvariantCulture);
                        }

                        var swaggerResponse = new SwaggerResponse();
                        operation.Responses.Add(statusCode, swaggerResponse);

                        var typeDescription = settings.ReflectionService.GetDescription(response.Type, attributes, settings);
                        if (typeDescription.RequiresSchemaReference(settings.TypeMappers))
                        {
                            swaggerResponse.Schema = schemaGenerator.GenerateAsync(response.Type, attributes, schemaResolver).Result;
                        }

                        if (response.StatusCode >= 400)
                        {
                            swaggerResponse.Description = "Error";
                        }
                        else if (response.StatusCode >= 300)
                        {
                            swaggerResponse.Description = "Redirect";
                        }
                        else if (response.StatusCode >= 200)
                        {
                            swaggerResponse.Description = "Success";
                        }
                        else if (response.StatusCode == 0)
                        {
                            swaggerResponse.Description = "Error";
                        }
                    }

                    foreach (var parameter in apiDescription.ParameterDescriptions)
                    {
                        var parameterType = parameter.ParameterDescriptor.ParameterType;
                        var typeDescription = settings.ReflectionService.GetDescription(parameterType, attributes, settings);

                        if (!Enum.TryParse<SwaggerParameterKind>(parameter.Source.Id, out var kind))
                        {
                            kind = SwaggerParameterKind.Undefined;
                        }

                        var swaggerParameter = new SwaggerParameter
                        {
                            Type = typeDescription.Type,
                            Kind = kind,
                            Name = parameter.Name,
                            IsRequired = parameter.ModelMetadata.IsRequired,
                        };

                        operation.Parameters.Add(swaggerParameter);

                        if (typeDescription.RequiresSchemaReference(settings.TypeMappers))
                        {
                            var schema = schemaGenerator.GenerateAsync(parameterType, attributes, schemaResolver).Result;

                            swaggerParameter.CustomSchema = new JsonSchema4 { SchemaReference = schema.ActualSchema };
                        }
                    }

                    if (!Enum.TryParse<SwaggerOperationMethod>(apiDescription.HttpMethod, ignoreCase: true, result: out var method))
                    {
                        method = SwaggerOperationMethod.Undefined;
                    }

                    document.Paths[apiDescription.RelativePath] = new SwaggerOperations
                    {
                        { method, operation }
                    };
                }
            }

            return document;
        }

        private static string CreateOperationId(ApiDescription apiDescription)
        {
            var actionDescriptor = apiDescription.ActionDescriptor;
            if (!actionDescriptor.RouteValues.TryGetValue("Controller", out var controllerName))
            {
                controllerName = null;
            }

            if (!actionDescriptor.RouteValues.TryGetValue("Action", out var actionName))
            {
                actionName = null;
            }

            Debug.Assert(!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName));

            if (string.Equals(actionName, apiDescription.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                // CatController.Get() -> GetCat
                return actionName + controllerName;
            }
            else
            {
                // CatController.FindById() -> CatFindById()
                return controllerName + actionName;
            }
        }
    }
}
