using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clapton.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Finds all Types derived from the current <see cref="Type"/> within a given <see cref="Assembly"/>
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="assembly">The <see cref="Assembly"/> in which to search for derived types.</param>
        /// <returns><see cref="IEnumerable{Type}"/> containing all found derived types</returns>
        public static IEnumerable<Type> FindDerivedTypes(this Type baseType, Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }
    }
}
