using System.Collections.Generic;
using System.Reflection;

namespace FGS.Pump.Extensions
{
    // Borrowed from http://stackoverflow.com/questions/11345854/implementing-iequalitycomparert-for-comparing-arbitrary-properties-of-any-clas
    public class PropertyComparer<T> : IEqualityComparer<T>
    {
        private readonly PropertyInfo propertyToCompare;

        public PropertyComparer(string propertyName)
        {
            propertyToCompare = typeof(T).GetProperty(propertyName);
        }

        public bool Equals(T x, T y)
        {
            object valueX = propertyToCompare.GetValue(x, null);
            object valueY = propertyToCompare.GetValue(y, null);
            return valueX.Equals(valueY);
        }

        public int GetHashCode(T obj)
        {
            object objValue = propertyToCompare.GetValue(obj, null);
            return objValue.GetHashCode();
        }
    }
}
