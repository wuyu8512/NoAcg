#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Converter.System.Text.Json
{
    public class EnumDescriptionConverter<T> : JsonConverter<T> where T : Enum
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Enum);
        }
        
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var fields = typeToConvert.GetFields();
            var readValue = reader.GetString() ?? string.Empty;
            foreach (var field in fields)
            {
                var objects = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objects.Any(item => (item as DescriptionAttribute)?.Description ==
                                        readValue))
                {
                    return (T?)Convert.ChangeType(field.GetValue(-1), typeToConvert);
                }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                writer.WriteStringValue("");
                return;
            }

            var fieldInfo = value.GetType().GetField(value.ToString()!);
            if (fieldInfo == null)
            {
                writer.WriteStringValue("");
                return;
            }

            var attributes =
                (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            writer.WriteStringValue(attributes.Length > 0 ? attributes[0].Description : "");
        }
    }
}