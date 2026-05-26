using System.Text;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static class CSharpRequestWriter
{
    public static string WriteRequest(string requestName, TelegramMethod method)
    {
        var builder = new StringBuilder();
        foreach (var parameter in method.Parameters)
        {
            WriteProperty(builder, requestName, parameter);
        }

        return CSharpTemplateRenderer.Render(
            "Request.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary(method.Description),
                ["parameter_remarks"] = CSharpXmlDocs.WriteParameterRemarks(
                    method.Parameters.Select(parameter => (parameter.Name, parameter.Type, parameter.Required, parameter.Description))),
                ["request_name"] = requestName,
                ["properties"] = builder.ToString()
            });
    }

    private static void WriteProperty(StringBuilder builder, string containingTypeName, TelegramField parameter)
    {
        var csharpType = CSharpTypeMapper.Map(parameter);
        var propertyName = CSharpNaming.ToPropertyName(parameter.Name, containingTypeName);
        var initializer = CSharpTypeMapper.NeedsDefaultInitializer(csharpType, parameter.Required) ? " = default!;" : string.Empty;

        builder.Append(CSharpTemplateRenderer.Render(
            "Property.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary(parameter.Description, 4),
                ["json_name"] = parameter.Name,
                ["property_type"] = csharpType,
                ["property_name"] = propertyName,
                ["initializer"] = initializer
            }));
    }
}
