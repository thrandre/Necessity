using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AgileObjects.NetStandardPolyfills;

namespace Necessity
{
    public static class EnumExtensions
    {
        public static string Description(this Enum @enum)
        {
            return @enum
                .GetType()
                .GetPublicStaticFields()
                .First(f => f.Name == @enum.ToString())
                .Pipe(f => f.GetCustomAttribute<DescriptionAttribute>()
                               ?.Description
                           ?? @enum.ToString());
        }
    }
}
