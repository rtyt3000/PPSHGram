using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static class CSharpGenerator
{
    public static void Generate(TelegramSchema schema, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        var typesDirectory = Path.Combine(outputDirectory, "Types");
        var requestsDirectory = Path.Combine(outputDirectory, "Requests");
        var methodsDirectory = Path.Combine(outputDirectory, "Methods");
        var unionImplementations = FindUnionImplementations(schema);
        var unionTypeNames = unionImplementations.Values
            .SelectMany(typeNames => typeNames)
            .ToHashSet(StringComparer.Ordinal);

        RecreateDirectory(typesDirectory);
        RecreateDirectory(requestsDirectory);
        RecreateDirectory(methodsDirectory);

        foreach (var type in schema.Types)
        {
            File.WriteAllText(
                Path.Combine(typesDirectory, $"{type.Name}.g.cs"),
                CSharpTypeWriter.WriteType(
                    type,
                    unionImplementations.GetValueOrDefault(type.Name, Array.Empty<string>())));
        }

        foreach (var placeholderType in FindMissingReferencedTypes(schema))
        {
            File.WriteAllText(
                Path.Combine(typesDirectory, $"{placeholderType}.g.cs"),
                CSharpTypeWriter.WritePlaceholderType(placeholderType, unionTypeNames.Contains(placeholderType)));
        }

        foreach (var method in schema.Methods)
        {
            var requestName = $"{CSharpNaming.ToPascalCase(method.Name)}Request";
            File.WriteAllText(
                Path.Combine(requestsDirectory, $"{requestName}.g.cs"),
                CSharpRequestWriter.WriteRequest(requestName, method));
        }

        File.WriteAllText(
            Path.Combine(methodsDirectory, "TelegramMethods.g.cs"),
            CSharpMethodsWriter.WriteMethods(schema.Methods));

        File.WriteAllText(
            Path.Combine(outputDirectory, "Api.g.cs"),
            CSharpApiWriter.WriteApi(schema.Methods));
    }

    private static IReadOnlyList<string> FindMissingReferencedTypes(TelegramSchema schema)
    {
        var knownTypes = schema.Types.Select(type => type.Name).ToHashSet(StringComparer.Ordinal);
        var referencedTypes = schema.Types
            .SelectMany(type => type.Fields)
            .Concat(schema.Methods.SelectMany(method => method.Parameters))
            .SelectMany(CSharpTypeMapper.GetReferencedTypeNames);

        return referencedTypes
            .Where(typeName => !knownTypes.Contains(typeName))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(typeName => typeName, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> FindUnionImplementations(TelegramSchema schema)
    {
        return schema.Types
            .SelectMany(type => type.Fields)
            .Concat(schema.Methods.SelectMany(method => method.Parameters))
            .Select(CSharpTypeMapper.GetUnionMapping)
            .Where(mapping => mapping is not null)
            .Select(mapping => mapping!)
            .SelectMany(mapping => mapping.PartTypeNames.Select(partTypeName => (partTypeName, mapping.TypeName)))
            .Where(mapping => mapping.partTypeName != mapping.TypeName)
            .GroupBy(mapping => mapping.partTypeName, mapping => mapping.TypeName, StringComparer.Ordinal)
            .ToDictionary(
                grouping => grouping.Key,
                grouping => (IReadOnlyCollection<string>)grouping.Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
    }

    private static void RecreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return;
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.g.cs", SearchOption.TopDirectoryOnly))
        {
            File.Delete(file);
        }
    }
}
