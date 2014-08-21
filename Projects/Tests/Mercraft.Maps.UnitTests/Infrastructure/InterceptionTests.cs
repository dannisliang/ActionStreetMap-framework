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
                                            .WithProxy<ClassAProxy>()
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
                                            .WithProxy<ClassAProxy>()
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

        [Test]
        public void CanGenerateProxy()
        {
            // ARRANGE
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>()
                                            .Use<ClassA1>()
                                            .WithProxy()
                                            .AddBehavior(new ExecuteBehavior())
                                            .Singleton());
                // ACT
                var classA = container.Resolve<IClassA>();
                var result = classA.SayHello("Ilya");

                // ASSERT
                Assert.AreEqual("Mercraft.Dynamics.IClassAProxy", classA.GetType().FullName);
                Assert.AreEqual("Hello from A1, Ilya", result);
            }
        }

        [Test]
        public void CanUseGlobalBehavior()
        {
            // ARRANGE
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>()
                                            .Use<ClassA1>()
                                            .WithProxy<ClassAProxy>()
                                            .Singleton());
                container.AddGlobalBehavior(new ExecuteBehavior());

                // ACT
                var classA = container.Resolve<IClassA>();
                var result = classA.SayHello("Ilya");

                // ASSERT
                Assert.AreEqual("Hello from A1, Ilya", result);
            }
        }

        [Test]
        public void CanAutogenerateProxy()
        {
            // ARRANGE
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>()
                                            .Use<ClassA1>()
                                            .Singleton());
                container.AllowProxy = true;
                container.AutoGenerateProxy = true;
                container.AddGlobalBehavior(new ExecuteBehavior());

                // ACT
                var classA = container.Resolve<IClassA>();
                var result = classA.SayHello("Ilya");

                // ASSERT
                Assert.AreEqual("Mercraft.Dynamics.IClassAProxy", classA.GetType().FullName);
                Assert.AreEqual("Hello from A1, Ilya", result);
            }
        }
    }
}