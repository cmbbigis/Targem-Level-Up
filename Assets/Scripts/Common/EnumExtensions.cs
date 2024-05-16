using System;
using System.Collections.Generic;
using System.Linq;
using Units.Models.Unit;

namespace Common
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}