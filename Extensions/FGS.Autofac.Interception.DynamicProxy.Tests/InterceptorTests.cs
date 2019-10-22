using System.Threading.Tasks;

using Autofac;

using AutoFixture;

using FGS.Interception.Abstractions;

using Moq;

using NUnit.Framework;

using IInterceptor = FGS.Interception.Abstractions.IInterceptor;
using IInvocation = FGS.Interception.Abstractions.IInvocation;

namespace FGS.Autofac.Interception.DynamicProxy.Tests
{
    [Category("Unit")]
    [TestFixture]
    public class InterceptorTests
    {
        private Fixture _fixture;
        private Moq.Mock<IInterceptionTestSubject> _mockInnermostImplementation;
        private int _arbitraryInt;
        private string _arbitraryString;

        [SetUp]
        public void SetUp()
        {
            _fixture = new AutoFixture.Fixture();
            _mockInnermostImplementation = new Moq.Mock<IInterceptionTestSubject>();
            _arbitraryInt = _fixture.Create<int>();
            _arbitraryString = _fixture.Create<string>();
        }

        [Test]
        public void Given_InterceptingWithPassthru_ExecuteVoid_Returns()
        {
            var subject = Given_InterceptedSubject<PassthruInterceptor>();

            subject.ExecuteVoid(_arbitraryInt, _arbitraryString);

            _mockInnermostImplementation.Verify(i => i.ExecuteVoid(_arbitraryInt, _arbitraryString));
        }

        [Test]
        public void Given_InterceptingWithPassthru_ExecuteReturnValue_ReturnsExpected()
        {
            var expected = _fixture.Create<bool>();
            _mockInnermostImplementation.Setup(i => i.ExecuteReturnValue(_arbitraryInt, _arbitraryString)).Returns(expected);
            var subject = Given_InterceptedSubject<PassthruInterceptor>();

            var actual = subject.ExecuteReturnValue(_arbitraryInt, _arbitraryString);

            _mockInnermostImplementation.Verify(i => i.ExecuteReturnValue(_arbitraryInt, _arbitraryString));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Given_InterceptingWithPassthru_ExecuteReturnReference_ReturnsExpected()
        {
            var expected = _fixture.CreateMany<bool>();
            _mockInnermostImplementation.Setup(i => i.ExecuteReturnReference(_arbitraryInt, _arbitraryString)).Returns(expected);
            var subject = Given_InterceptedSubject<PassthruInterceptor>();

            var actual = subject.ExecuteReturnReference(_arbitraryInt, _arbitraryString);

            _mockInnermostImplementation.Verify(i => i.ExecuteReturnReference(_arbitraryInt, _arbitraryString));
            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        public async Task Given_InterceptingWithPassthru_ExecuteReturnAsync_Returns()
        {
            _mockInnermostImplementation.Setup(i => i.ExecuteReturnAsync(_arbitraryInt, _arbitraryString)).Returns(Task.CompletedTask);
            var subject = Given_InterceptedSubject<PassthruInterceptor>();

            await subject.ExecuteReturnAsync(_arbitraryInt, _arbitraryString);

            _mockInnermostImplementation.Verify(i => i.ExecuteReturnAsync(_arbitraryInt, _arbitraryString));
        }

        [Test]
        public async Task Given_InterceptingWithPassthru_ExecuteReturnAsyncValue_ReturnsExpected()
        {
            var expected = _fixture.Create<bool>();
            _mockInnermostImplementation.Setup(i => i.ExecuteReturnAsyncValue(_arbitraryInt, _arbitraryString)).ReturnsAsync(expected);
            var subject = Given_InterceptedSubject<PassthruInterceptor>();

            var actual = await subject.ExecuteReturnAsyncValue(_arbitraryInt, _arbitraryString);

            _mockInnermostImplementation.Verify(i => i.ExecuteReturnAsyncValue(_arbitraryInt, _arbitraryString));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task Given_InterceptingWithPassthru_ExecuteReturnAsyncReference_ReturnsExpected()
        {
            var expected = _fixture.CreateMany<bool>();
            _mockInnermostImplementation.Setup(i => i.ExecuteReturnAsyncReference(_arbitraryInt, _arbitraryString)).ReturnsAsync(expected);
            var subject = Given_InterceptedSubject<PassthruInterceptor>();

            var actual = await subject.ExecuteReturnAsyncReference(_arbitraryInt, _arbitraryString);

            _mockInnermostImplementation.Verify(i => i.ExecuteReturnAsyncReference(_arbitraryInt, _arbitraryString));
            Assert.That(actual, Is.SameAs(expected));
        }

        private IInterceptionTestSubject Given_InterceptedSubject<TInterceptor>()
            where TInterceptor : IInterceptor
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AttributeBasedInterceptionModule<InterceptForTestAttribute, TInterceptor>>();
            containerBuilder.Register(ctx => new InterceptionSubjectInnerImplementationDecorator(_mockInnermostImplementation.Object)).AsSelf().InstancePerDependency();
            var container = containerBuilder.Build();
            return container.Resolve<InterceptionSubjectInnerImplementationDecorator>();
        }

        private class PassthruInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
            }

            public async Task InterceptAsync(IAsyncInvocation invocation)
            {
                await invocation.ProceedAsync();
            }
        }
    }
}
