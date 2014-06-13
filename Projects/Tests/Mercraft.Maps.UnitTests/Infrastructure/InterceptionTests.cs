using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Dependencies.Interception.Behaviors;
using Mercraft.Maps.UnitTests.Infrastructure.Stubs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Infrastructure
{
    [TestFixture]
    public class InterceptionTests
    {
        [Test]
        public void CanInterceptWithSingleton()
        {
            // ARRANGE
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>()
                                            .Use<ClassA1>()
                                            .Proxy<ClassAProxy>()
                                            .AddBehavior(new ExecuteBehavior())
                                            .Singleton());

                // ACT
                var classA = container.Resolve<IClassA>();
                var classA2 = container.Resolve<IClassA>();

                // ASSERT
                Assert.AreSame(classA, classA2);
            }
        }

        [Test]
        public void CanUseProxy()
        {
            // ARRANGE
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>()
                                            .Use<ClassA1>()
                                            .Proxy<ClassAProxy>()
                                            .AddBehavior(new ExecuteBehavior())
                                            .Singleton());

                // ACT
                var classA = container.Resolve<IClassA>();
                var result = classA.SayHello("Ilya");

                // ASSERT
                Assert.IsInstanceOf(typeof(ClassAProxy), classA);
                Assert.AreEqual("Hello from A1, Ilya", result);
            }
        }
    }
}