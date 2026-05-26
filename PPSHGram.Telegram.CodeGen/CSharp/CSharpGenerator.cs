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
        var baseTypeImplementations = MergeImplementations(
            FindUnionImplementations(schema),
            FindImplicitFamilyImplementations(schema));
        var baseTypeNames = baseTypeImplementations.Values
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
                    baseTypeImplementations.GetValueOrDefault(type.Name, Array.Empty<string>())));
        }

        foreach (var placeholderType in FindMissingReferencedTypes(schema))
        {
            var derivedTypeNames = FindDerivedTypeNames(baseTypeImplementations, placeholderType);
            File.WriteAllText(
                Path.Combine(typesDirectory, $"{placeholderType}.g.cs"),
                CSharpTypeWriter.WritePlaceholderType(
                    placeholderType,
                    baseTypeNames.Contains(placeholderType),
                    derivedTypeNames));
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

    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> FindImplicitFamilyImplementations(
        TelegramSchema schema)
    {
        var referencedMissingTypes = FindMissingReferencedTypes(schema).ToHashSet(StringComparer.Ordinal);
        var result = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        AddFamily(
            "InlineQueryResult",
            typeName => typeName.StartsWith("InlineQueryResult", StringComparison.Ordinal)
                && !typeName.StartsWith("InlineQueryResults", StringComparison.Ordinal));
        AddFamily(
            "InputMessageContent",
            typeName => typeName.StartsWith("Input", StringComparison.Ordinal)
                && typeName.EndsWith("MessageContent", StringComparison.Ordinal));
        AddFamily(
            "InputMedia",
            typeName => typeName.StartsWith("InputMedia", StringComparison.Ordinal));

        return result.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyCollection<string>)pair.Value.Distinct(StringComparer.Ordinal).ToArray(),
            StringComparer.Ordinal);

        void AddFamily(string interfaceName, Func<string, bool> predicate)
        {
            if (!referencedMissingTypes.Contains(interfaceName))
            {
                return;
            }

            foreach (var typeName in schema.Types.Select(type => type.Name).Where(predicate))
            {
                if (typeName.Equals(interfaceName, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!result.TryGetValue(typeName, out var interfaces))
                {
                    interfaces = [];
                    result[typeName] = interfaces;
                }

                interfaces.Add(interfaceName);
            }
        }
    }

    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> MergeImplementations(
        params IReadOnlyDictionary<string, IReadOnlyCollection<string>>[] implementations)
    {
        var result = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var implementation in implementations)
        {
            foreach (var (typeName, interfaceNames) in implementation)
            {
                if (!result.TryGetValue(typeName, out var currentInterfaceNames))
                {
                    currentInterfaceNames = [];
                    result[typeName] = currentInterfaceNames;
                }

                currentInterfaceNames.AddRange(interfaceNames);
            }
        }

        return result.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyCollection<string>)pair.Value.Distinct(StringComparer.Ordinal).ToArray(),
            StringComparer.Ordinal);
    }

    private static IReadOnlyCollection<string> FindDerivedTypeNames(
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> implementations,
        string interfaceName)
    {
        return implementations
            .Where(pair => pair.Value.Contains(interfaceName, StringComparer.Ordinal))
            .Select(pair => pair.Key)
            .Order(StringComparer.Ordinal)
            .ToArray();
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
