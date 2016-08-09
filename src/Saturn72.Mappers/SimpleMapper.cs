#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Saturn72.Mappers
{
    public class SimpleMapper
    {
        public static TDestination Map<TSource, TDestination>(TSource source) where TSource : class
            where TDestination : class
        {
            var propDictionary = ConvertInstanceToPropertyDictionary(source);

            return Map<TDestination>(propDictionary);
        }

        public static TDestination Map<TDestination>(IDictionary<string, Func<object>> srcPropsDictionary)
            where TDestination : class
        {
            var instance = Activator.CreateInstance<TDestination>();
            return Map(srcPropsDictionary, instance);
        }


        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            var propDictionary = ConvertInstanceToPropertyDictionary(source);

            Map(propDictionary, destination);
        }


        public static TDestination Map<TDestination>(IDictionary<string, Func<object>> srcPropsDictionary,
            TDestination instance)
        {
            foreach (var key in srcPropsDictionary.Keys)
            {
                var pInfo = typeof(TDestination).GetProperty(key,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (pInfo == null)
                    continue;

                var mapLogic = MapLogic.N_A;

                var value = srcPropsDictionary[key]();

                if (pInfo != null && value != null
                    && pInfo.CanWrite && srcPropsDictionary.ContainsKey(key)
                    && TypesCanBeMapped(value.GetType(), pInfo.PropertyType, ref mapLogic))
                {
                    switch (mapLogic)
                    {
                        case MapLogic.TypeToType:
                            pInfo.SetValue(instance, value);
                            continue;
                        case MapLogic.IntToEnum:
                            pInfo.SetValue(instance, Enum.ToObject(pInfo.PropertyType, value));
                            continue;
                            ;
                        case MapLogic.EnumToInt:
                            pInfo.SetValue(instance, (int) value);
                            continue;
                            ;
                        case MapLogic.ObjectToString:
                            pInfo.SetValue(instance, value.ToString());
                            continue;
                    }
                }
            }
            return instance;
        }

        private static bool TypesCanBeMapped(Type sourceType, Type destType, ref MapLogic mapLogic)
        {
            if (sourceType == destType
                || IsNullableToObjectOrObjectToNullable(sourceType, destType)
                || IsNullableToObjectOrObjectToNullable(destType, sourceType)
                || destType.IsAssignableFrom(sourceType))
            {
                mapLogic = MapLogic.TypeToType;
                return true;
            }

            if (sourceType == typeof(int) && destType.IsEnum)
            {
                mapLogic = MapLogic.IntToEnum;
                return true;
            }
            if (sourceType.IsEnum && destType == typeof(int))
            {
                mapLogic = MapLogic.EnumToInt;
                return true;
            }

            if (destType == typeof(string))
            {
                mapLogic = MapLogic.ObjectToString;
                return true;
            }

            return false;
        }

        private static IDictionary<string, Func<object>> ConvertInstanceToPropertyDictionary<TSource>(TSource source)
            where TSource : class
        {
            var allProps = source.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead);

            return allProps.ToDictionary(prop=> prop.Name, prop => new Func<object>(()=> prop.GetValue(source)));
        }

        private static bool IsNullableToObjectOrObjectToNullable(Type sourceType, Type destType)
        {
            return destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   Nullable.GetUnderlyingType(destType) == sourceType;
        }

        private enum MapLogic
        {
            N_A,
            TypeToType,
            IntToEnum,
            EnumToInt,
            ObjectToString
        }
    }
}