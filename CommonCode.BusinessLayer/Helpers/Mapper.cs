using System.Collections.Generic;
using System.Linq;

namespace CommonCode.BusinessLayer.Helpers
{
    public static class Mapper
    {
        public static T2 Map<T1, T2>(this T1 source)
            where T2 : new()
        {
            if (source == null)
            {
                return default(T2);
            }

            var target = new T2();
            var targetProperties = target?.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                var targetProperty = targetProperties?.SingleOrDefault(x => x.Name == sourceProperty.Name);

                if (targetProperty == null ||
                    targetProperty.PropertyType != sourceProperty.PropertyType ||
                    !targetProperty.CanWrite)
                {
                    continue;
                }

                var sourceGetter = sourceProperty.GetGetMethod();
                var targetSetter = targetProperty.GetSetMethod();
                var valueToSet = sourceGetter.Invoke(source, null);
                targetSetter.Invoke(target, new[] { valueToSet });
            }

            return target;
        }

        public static IEnumerable<T2> MapAll<T1, T2>(this IEnumerable<T1> sources)
            where T2 : new()
        {
            return sources?.Select(Map<T1, T2>);
        }

        public static DataResult<T2> Convert<T1, T2>(this DataResult<T1> dataResult)
            where T2 : new()
        {
            var newValue = dataResult.Value == null
                ? default(T2)
                : dataResult.Value.Map<T1, T2>();

            return new DataResult<T2>(newValue, dataResult);
        }

        public static DataResult<IEnumerable<T1>> ConvertSingleToEnumerable<T1>(this DataResult<T1> dataResult)
        {
            var newValues = dataResult.Value == null
                ? new[] { default(T1) }
                : new[] { dataResult.Value };

            return new DataResult<IEnumerable<T1>>(newValues, dataResult);
        }

        public static DataResult<IEnumerable<T2>> ConvertSingleToEnumerable<T1, T2>(this DataResult<T1> dataResult)
            where T2 : new()
        {
            var newValues = dataResult.Value == null
                ? new[] { default(T2) }
                : new[] { dataResult.Value.Map<T1, T2>() };

            return new DataResult<IEnumerable<T2>>(newValues, dataResult);
        }

        public static DataResult<IEnumerable<T2>> ConvertAll<T1, T2>(this DataResult<IEnumerable<T1>> dataResult)
            where T2 : new()
        {
            var newValues = dataResult.Value == null
                ? new[] { default(T2) }
                : dataResult.Value.MapAll<T1, T2>();

            return new DataResult<IEnumerable<T2>>(newValues, dataResult);
        }
    }
}