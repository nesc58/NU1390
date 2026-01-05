using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace NU1390.Abstracts;

public static class MyClassHelpers
{
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.None
    };

    internal static readonly JsonSerializer DefaultJsonSerializer = JsonSerializer.Create(DefaultSettings);

    private static readonly ConcurrentDictionary<Type, GenericTypeInformation> GenericTypeCache = new();

    internal static readonly Dictionary<string, Type> AllMyClassBaseImplementations =
        AppDomain.CurrentDomain.GetAssemblies() // System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies
        .Where(assembly => !assembly.IsDynamic)
        .SelectMany(assembly => assembly.GetExportedTypes())
        .Where(type => type.IsSubclassOf(typeof(MyClassBase)))
        .Where(type => !string.IsNullOrWhiteSpace(type.FullName))
        .ToDictionary(type => type.FullName!, type => type, StringComparer.Ordinal);

    public static Dto<T> DeserializeJsonToDto<T>(string messageJson, string type) where T : MyClassBase, new()
    {
        var payloadType = AllMyClassBaseImplementations[type];
        var genericTypeInformation = GenericTypeCache.GetOrAdd(payloadType, f =>
        {
            var genericType = typeof(Dto<>).MakeGenericType(f);
            return new GenericTypeInformation
            {
                GenericType = genericType,
                IndexProperty = genericType.GetProperty(nameof(Dto<T>.Index)),
                PayloadProperty = genericType.GetProperty(nameof(Dto<T>.Payload))
            };
        });
        var deserializedMessage = Deserialize(messageJson, genericTypeInformation.GenericType);
        var deserializedIndex = genericTypeInformation.IndexProperty?.GetValue(deserializedMessage);
        var deserializedPayload = genericTypeInformation.PayloadProperty?.GetValue(deserializedMessage);
        var messageWithSubscriptionType = new Dto<T> {Payload = (T) deserializedPayload!, Index = (long?) deserializedIndex};
        return messageWithSubscriptionType;
    }

    private static object? Deserialize(string messageJson, Type? type)
    {
        using var stringReader = new StringReader(messageJson);
        using var reader = new JsonTextReader(new StringReader(messageJson));
        return DefaultJsonSerializer.Deserialize(reader, type);
    }

    private sealed class GenericTypeInformation
    {
        public Type? GenericType { get; init; }

        public PropertyInfo? IndexProperty { get; init; }

        public PropertyInfo? PayloadProperty { get; init; }
    }
}