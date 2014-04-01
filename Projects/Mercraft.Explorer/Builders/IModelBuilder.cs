using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public interface IModelBuilder
    {
        string Name { get; }
        GameObject BuildArea(GeoCoordinate center, Rule rule, Area area);
        GameObject BuildWay(GeoCoordinate center,  Rule rule, Way way);
    }

    public class ModelBuilder: IModelBuilder, IConfigurable
    {
        private const string NameKey = "@name";
        public string Name { get; private set; }

        [Dependency]
        protected ITrace Trace { get; set; }

        public virtual GameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            return null;
        }

        public virtual GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            return null;
        }

        public virtual void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(NameKey);
        }
    }
}
