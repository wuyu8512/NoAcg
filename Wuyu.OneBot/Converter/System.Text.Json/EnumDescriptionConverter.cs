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
    public class JsonDescriptionEnumConverter : JsonConverterFactory
    {
        private readonly JsonNamingPolicy _namingPolicy;
        private readonly bool _allowIntegerValues;
        private readonly JsonStringEnumConverter _baseConverter;

        public JsonDescriptionEnumConverter() : this(null, true)
        {
        }

        public JsonDescriptionEnumConverter(JsonNamingPolicy namingPolicy = null, bool allowIntegerValues = true)
        {
            this._namingPolicy = namingPolicy;
            this._allowIntegerValues = allowIntegerValues;
            this._baseConverter = new JsonStringEnumConverter(namingPolicy, allowIntegerValues);
        }

        public override bool CanConvert(Type typeToConvert) => _baseConverter.CanConvert(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var query = from field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static)
                let attr = field.GetCustomAttribute<DescriptionAttribute>()
                where attr != null
                select (field.Name, attr.Description);
            var dictionary = query.ToDictionary(p => p.Item1, p => p.Item2);
            if (dictionary.Count > 0)
            {
                return new JsonStringEnumConverter(new DictionaryLookupNamingPolicy(dictionary, _namingPolicy),
                    _allowIntegerValues).CreateConverter(typeToConvert, options);
            }
            else
            {
                return _baseConverter.CreateConverter(typeToConvert, options);
            }
        }
    }

    public class JsonNamingPolicyDecorator : JsonNamingPolicy
    {
        private readonly JsonNamingPolicy _underlyingNamingPolicy;

        protected JsonNamingPolicyDecorator(JsonNamingPolicy underlyingNamingPolicy) =>
            this._underlyingNamingPolicy = underlyingNamingPolicy;

        public override string ConvertName(string name) =>
            _underlyingNamingPolicy == null ? name : _underlyingNamingPolicy.ConvertName(name);
    }

    internal class DictionaryLookupNamingPolicy : JsonNamingPolicyDecorator
    {
        private readonly Dictionary<string, string> _dictionary;

        public DictionaryLookupNamingPolicy(Dictionary<string, string> dictionary,
            JsonNamingPolicy underlyingNamingPolicy) : base(underlyingNamingPolicy) =>
            this._dictionary = dictionary ?? throw new ArgumentNullException();

        public override string ConvertName(string name) =>
            _dictionary.TryGetValue(name, out var value) ? value : base.ConvertName(name);
    }
}