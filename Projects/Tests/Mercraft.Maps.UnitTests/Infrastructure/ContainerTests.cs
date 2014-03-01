
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Dependencies.Lifetime;
using Mercraft.Maps.UnitTests.Infrastructure.Stubs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Infrastructure
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void CanUseTypeMapping()
        {
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>().Use<ClassA1>().Singleton());

                var classA = container.Resolve<IClassA>();
                Assert.IsNotNull(classA);
                Assert.AreEqual("Hello from A1, Ilya", classA.SayHello("Ilya"));
                Assert.AreEqual(5, classA.Add(2, 3));
            }
        }

        [Test]
        public void CanUseRegisterInstance()
        {
            //arrange
            using (IContainer container = new Container())
            {
                IClassA a = new ClassA1();
                //act
                container.RegisterInstance<IClassA>(a);
                IClassA aFromContainer = container.Resolve<IClassA>();

                //assert
                Assert.AreSame(a, aFromContainer);
            }
        }

        [Test]
        public void CanRegisterSingleton()
        {
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>().Use<ClassA1>().Singleton());
                Assert.AreSame(container.Resolve<IClassA>(), container.Resolve<IClassA>());
            }
        }

        [Test]
        public void CanRegisterTransient()
        {
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>().Use<ClassA1>().Transient());
                Assert.AreNotSame(container.Resolve<IClassA>(), container.Resolve<IClassA>());
            }
        }

        [Test]
        public void CanUseRegisterTypeWithName()
        {
            const string instance1 = "instance1", instance2 = "instance2";
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassA>().Use<ClassA1>().Named(instance1).Transient());
                ClassA1 instance = new ClassA1();
                container.RegisterInstance<ClassA1>(instance, instance2);

                Assert.IsNotNull(container.Resolve(instance1));
                Assert.AreNotSame(container.Resolve(instance1), container.Resolve(instance2));
                Assert.AreSame(instance, container.Resolve(instance2));
            }
        }

        [Test]
        public void CanAutoRegisterEnumerable()
        {
            using (var container = new Container())
            {
                // arrange
                container.Register(Component.For<IClassA>().Use<ClassA1>().Named("A1"));
                container.Register(Component.For<IClassA>().Use<ClassA2>().Named("A2"));
                container.Register(Component.For<IClassA>().Use<ClassA2>().Named("A3"));
                container.Register(Component.For<CollectionDependencyClass>().Use<CollectionDependencyClass>());

                // act
                var collectionDependencyClassInstance = container.Resolve<CollectionDependencyClass>();
                var collection1 = container.Resolve<IEnumerable<IClassA>>();
                var collection2 = container.ResolveAll<IClassA>();

                // assert
                Assert.AreEqual(3, collectionDependencyClassInstance.Classes.Count());
                Assert.AreEqual(3, collection1.Count());
                Assert.AreEqual(3, collection2.Count());
            }
        }

        [Test]
        public void CanUseConfigurable()
        {
            using (var container = new Container())
            {
                var configSection = new ConfigSection(new ConfigElement(null));
                container.Register(Component.For<ConfigurableClass>()
                    .Use<ConfigurableClass>()
                    .SetConfig(configSection)
                    .Singleton());

                var instance = container.Resolve<ConfigurableClass>();

                Assert.AreSame(configSection, instance.ConfigSection);

            }
        }
    }
}
