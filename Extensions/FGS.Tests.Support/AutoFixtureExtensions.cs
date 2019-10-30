using System;
using System.Linq;
using System.Reflection;

using AutoFixture;
using AutoFixture.Kernel;

namespace FGS.Tests.Support
{
    public static class AutoFixtureExtensions
    {
        public static object CreateInstanceOfType(this Fixture fixture, Type type)
        {
            MethodInfo methodInfo = typeof(SpecimenFactory).GetMethod(
                "Create",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { typeof(ISpecimenBuilder) },
                null);

            var result = methodInfo.MakeGenericMethod(type).Invoke(null, new object[] { fixture });
            return result;
        }

        public static ushort CreateSmallPositiveRandomNumber(this Fixture fixture)
        {
            return (ushort)(fixture.Create<ushort>() & 0xF | 0x1);
        }

        public static ushort CreateTinyPositiveRandomNumber(this Fixture fixture)
        {
            return (ushort)(fixture.Create<ushort>() & 0x7 | 0x1);
        }

        public static int CreateIntOfMinValue(this Fixture fixture, int minValue)
        {
            var value = fixture.Create<int>();
            return value < minValue ? minValue : value;
        }

        public static int CreateIntBetween(this Fixture fixture, int minValue, int maxValue)
        {
            var value = fixture.Create<int>();
            var range = maxValue - minValue + 1;

            var moddedValue = value % range;
            return moddedValue + minValue;
        }

        public static TEnum CreateUndefinedEnumValue<TEnum>(this Fixture fixture)
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException("Only valid for Enum types");
            if (typeof(TEnum).GetEnumUnderlyingType() != typeof(int)) throw new InvalidOperationException("Enum underlying type must be System.Int32");

            var maxInteger = typeof(TEnum).GetEnumValues().Cast<int>().Max();

            var additive = fixture.Create<int>();

            return (TEnum)(object)(maxInteger + additive);
        }
    }
}
