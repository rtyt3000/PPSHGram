using System.Text;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static class CSharpMethodsWriter
{
    public static string WriteMethods(IEnumerable<TelegramMethod> methods)
    {
        var builder = new StringBuilder();
        foreach (var method in methods.OrderBy(method => method.Name, StringComparer.Ordinal))
        {
            builder.Append(CSharpTemplateRenderer.Render(
                "MethodConstant.cs.template",
                new Dictionary<string, string>
                {
                    ["summary"] = CSharpXmlDocs.WriteSummary(method.Description, 4),
                    ["parameter_remarks"] = CSharpXmlDocs.WriteParameterRemarks(
                        method.Parameters.Select(parameter => (parameter.Name, parameter.Type, parameter.Required, parameter.Description)),
                        4),
                    ["method_name"] = CSharpNaming.ToPascalCase(method.Name),
                    ["telegram_method_name"] = method.Name
                }));
        }

        return CSharpTemplateRenderer.Render(
            "Methods.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary("Telegram Bot API method names."),
                ["constants"] = builder.ToString()
            });
    }
}
