using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class TypeInfoExtractor
    {
        private static readonly BindingFlags _Flags = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, MethodInfo[]> _methodsCache =
            new ConcurrentDictionary<Type, MethodInfo[]>();

        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        public IReadOnlyList<MethodInfo> GetMethods(Type type)
        {
            if (_methodsCache.ContainsKey(type))
                return _methodsCache[type];

            var methods = type.GetMethods(_Flags);
            _methodsCache[type] = methods;
            return methods;
        }

        public IReadOnlyList<(MethodInfo method, TAttribute attribute)> GetMethodsWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            var methods = GetMethods(type);
            return methods
                .Select(m => (method: m, attribute: m.GetCustomAttribute<TAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();
        }

        public IReadOnlyList<PropertyInfo> GetProperties(Type type)
        {
            if (_propertiesCache.ContainsKey(type))
                return _propertiesCache[type];

            var properties = type.GetProperties(_Flags);
            _propertiesCache[type] = properties;
            return properties;
        }

        public IReadOnlyList<(PropertyInfo property, TAttribute attribute)> GetPropertiesWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            var properties = GetProperties(type);
            return properties
                .Select(p => (property: p, attribute: p.GetCustomAttribute<TAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();
        }
    }
}
