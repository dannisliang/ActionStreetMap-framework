using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Infrastructure.Stubs
{
    public class CollectionDependencyClass
    {
        public IEnumerable<IClassA> Classes { get; private set; }

        [Dependency]
        public CollectionDependencyClass(IEnumerable<IClassA> classes)
        {
            Classes = classes;
        }
    }
}
