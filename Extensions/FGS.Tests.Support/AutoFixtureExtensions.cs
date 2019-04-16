using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoFixture;
using AutoFixture.Kernel;

using Moq;

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

        public static Mock<T> Mock<T>(this Fixture fixture)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>());
        }

        public static Mock<T> Mock<T>(this Fixture fixture, MockBehavior behavior)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(behavior));
        }

        public static Mock<T> Mock<T>(this Fixture fixture, params object[] args)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(MockBehavior.Default, args));
        }

        public static Mock<T> Mock<T>(this Fixture fixture, MockBehavior behavior, params object[] args)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(behavior, args));
        }

        public static IEnumerable<Mock<T>> MockMany<T>(this Fixture fixture, int count = 3)
            where T : class
        {
            var mocks = Enumerable.Range(0, count).Select(_ => new Mock<T>()).ToList();
            mocks.ForEach(m => fixture.Register(() => m.Object));
            return mocks;
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

        private static Mock<T> Mock<T>(this Fixture fixture, Func<Mock<T>> mockFactory)
            where T : class
        {
            var mock = mockFactory();
            fixture.Inject(mock);
            fixture.Register(() => fixture.Create<Mock<T>>().Object);
            fixture.Freeze<T>();
            return mock;
        }
    }
}
