using System.Text;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static class CSharpApiWriter
{
    public static string WriteApi(IEnumerable<TelegramMethod> methods)
    {
        var builder = new StringBuilder();
        foreach (var method in methods.OrderBy(method => method.Name, StringComparer.Ordinal))
        {
            WriteMethod(builder, method);
        }

        return CSharpTemplateRenderer.Render(
            "Api.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary("Typed Telegram Bot API client."),
                ["methods"] = builder.ToString()
            });
    }

    private static void WriteMethod(StringBuilder builder, TelegramMethod method)
    {
        var methodName = CSharpNaming.ToPascalCase(method.Name);
        var requestName = $"{methodName}Request";
        var returnType = QualifyApiType(CSharpTypeMapper.Map(method.ReturnType, required: true));

        builder.Append(CSharpTemplateRenderer.Render(
            "ApiRequestMethod.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary(method.Description, 4),
                ["parameter_remarks"] = CSharpXmlDocs.WriteParameterRemarks(
                    method.Parameters.Select(parameter => (parameter.Name, parameter.Type, parameter.Required, parameter.Description)),
                    4),
                ["return_type"] = returnType,
                ["method_name"] = methodName,
                ["request_name"] = requestName
            }));

        WriteRequiredParametersOverload(builder, method, methodName, requestName, returnType);
    }

    private static void WriteRequiredParametersOverload(
        StringBuilder builder,
        TelegramMethod method,
        string methodName,
        string requestName,
        string returnType)
    {
        var requiredParameters = method.Parameters
            .Where(parameter => parameter.Required)
            .ToArray();

        var parametersBuilder = new StringBuilder();
        foreach (var parameter in requiredParameters)
        {
            var parameterType = QualifyApiType(CSharpTypeMapper.Map(parameter.Type, required: true));
            var parameterName = CSharpNaming.ToParameterName(parameter.Name);
            if (parameterName == "cancellationToken")
            {
                parameterName = "cancellationTokenValue";
            }

            parametersBuilder.AppendLine($"        {parameterType} {parameterName},");
        }

        var bodyBuilder = new StringBuilder();

        if (requiredParameters.Length == 0)
        {
            bodyBuilder.AppendLine($"        return {methodName}(new {requestName}(), cancellationToken);");
        }
        else
        {
            bodyBuilder.AppendLine($"        return {methodName}(new {requestName}");
            bodyBuilder.AppendLine("        {");
            foreach (var parameter in requiredParameters)
            {
                var propertyName = CSharpNaming.ToPropertyName(parameter.Name, requestName);
                var parameterName = CSharpNaming.ToParameterName(parameter.Name);
                if (parameterName == "cancellationToken")
                {
                    parameterName = "cancellationTokenValue";
                }

                bodyBuilder.AppendLine($"            {propertyName} = {parameterName},");
            }

            bodyBuilder.AppendLine("        }, cancellationToken);");
        }

        builder.Append(CSharpTemplateRenderer.Render(
            "ApiRequiredParametersOverload.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary(method.Description, 4),
                ["parameter_remarks"] = CSharpXmlDocs.WriteParameterRemarks(
                    requiredParameters.Select(parameter => (parameter.Name, parameter.Type, parameter.Required, parameter.Description)),
                    4),
                ["return_type"] = returnType,
                ["method_name"] = methodName,
                ["parameters"] = parametersBuilder.ToString(),
                ["body"] = bodyBuilder.ToString()
            }));
    }

    private static string QualifyApiType(string csharpType)
    {
        const string listPrefix = "IReadOnlyList<";

        if (csharpType.StartsWith(listPrefix, StringComparison.Ordinal) && csharpType.EndsWith(">", StringComparison.Ordinal))
        {
            var inner = csharpType[listPrefix.Length..^1];
            return $"{listPrefix}{QualifyApiType(inner)}>";
        }

        return IsGeneratedType(csharpType)
            ? $"PPSHGram.Telegram.Generated.Types.{csharpType}"
            : csharpType;
    }

    private static bool IsGeneratedType(string csharpType)
    {
        return csharpType is not "object"
            and not "string"
            and not "long"
            and not "int"
            and not "double"
            and not "bool"
            && csharpType.Length > 0
            && char.IsUpper(csharpType[0]);
    }
}
