using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonyAudioControl.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }

        public static Dictionary<string, object> ToDictionary(this object obj, bool includeEmptyValues = false)
        {
            if (obj is null)
                return null;

            var properties = obj.GetType().GetProperties();

            return properties
                .Select(prop => new { name = prop.Name, value = prop.GetValue(obj) })
                .Where(x => !x.value.IsEmpty())
                .ToDictionary(x => x.name, x => x.value);
        }

        public static bool IsEmpty(this object obj)
        {
            var type = obj?.GetType();

            if (type == null)
                return true;
            else if (obj is string str && string.IsNullOrEmpty(str))
                return true;
            else if (typeof(IEnumerable).IsAssignableFrom(type) && ((IEnumerable)obj).IsNullOrEmpty())
                return true;
            else if (type.IsValueType && obj == Activator.CreateInstance(type))
                return true;

            return false;
        }
    }
}
