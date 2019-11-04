using System;
using System.Linq;
using System.Reflection;

using AutoFixture;
using AutoFixture.Kernel;

namespace FGS.Tests.Support
{
    /// <summary>
    /// Extends <see cref="Fixture"/> with functionality to improve its ergonomics.
    /// </summary>
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Creates an instance of the given <paramref name="type"/>, using the given <paramref name="fixture"/>.
        /// </summary>
        /// <param name="fixture">The <see cref="Fixture"/> to use to create the result.</param>
        /// <param name="type">The <see cref="Type"/> of result to create.</param>
        /// <returns>An instance of the type represented by <paramref name="type"/>.</returns>
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

        /// <summary>
        /// Creates a random small positive <see cref="ushort"/>, no larger than <value>15</value>.
        /// </summary>
        /// <param name="fixture">The <see cref="Fixture"/> to use to create the result.</param>
        /// <returns>A random small positive number.</returns>
        public static ushort CreateSmallPositiveRandomNumber(this Fixture fixture)
        {
            return (ushort)(fixture.Create<ushort>() & 0xF | 0x1);
        }

        /// <summary>
        /// Creates a random tiny positive <see cref="ushort"/>, no larger than <value>7</value>.
        /// </summary>
        /// <param name="fixture">The <see cref="Fixture"/> to use to create the result.</param>
        /// <returns>A random tiny positive number.</returns>
        public static ushort CreateTinyPositiveRandomNumber(this Fixture fixture)
        {
            return (ushort)(fixture.Create<ushort>() & 0x7 | 0x1);
        }

        /// <summary>
        /// Creates a random <see cref="int"/> that is clamped to be no less than <paramref name="minValue"/>.
        /// </summary>
        /// <param name="fixture">The <see cref="Fixture"/> to use to create the result.</param>
        /// <param name="minValue">The lower-bound of possible result numbers.</param>
        /// <returns>A random <see cref="int"/>.</returns>
        public static int CreateIntOfMinValue(this Fixture fixture, int minValue)
        {
            var value = fixture.Create<int>();
            return value < minValue ? minValue : value;
        }

        /// <summary>
        /// Creates a random <see cref="int"/>, that is clamped to be no less than <paramref name="minValue"/> and no greater than <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="fixture">The <see cref="Fixture"/> to use to create the result.</param>
        /// <param name="minValue">The lower-bound of possible result numbers.</param>
        /// <param name="maxValue">The upper-bound of possible result numbers.</param>
        /// <returns>A random <see cref="int"/>.</returns>
        public static int CreateIntBetween(this Fixture fixture, int minValue, int maxValue)
        {
            var value = fixture.Create<int>();
            var range = maxValue - minValue + 1;

            var moddedValue = value % range;
            return moddedValue + minValue;
        }

        /// <summary>
        /// Creates a random value purporting to be a value of the enum <typeparamref name="TEnum"/>, but in fact falls outside the range of the enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum to return the result cast to.</typeparam>
        /// <param name="fixture">The <see cref="Fixture"/> to use to create the result.</param>
        /// <returns>A value purporting to be a value of the enum <typeparamref name="TEnum"/>.</returns>
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
