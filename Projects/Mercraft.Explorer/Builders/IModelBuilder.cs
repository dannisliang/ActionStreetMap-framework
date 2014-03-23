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
        void BuildArea(GeoCoordinate center, GameObject gameObject,  Rule rule, Area area);
        void BuildWay(GeoCoordinate center, GameObject gameObject, Rule rule, Way way);
    }

    public class ModelBuilder: IModelBuilder, IConfigurable
    {
        private const string NameKey = "@name";
        public string Name { get; private set; }

        [Dependency]
        protected ITrace Trace { get; set; }

        public virtual void BuildArea(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
        {
            
        }

        public virtual void BuildWay(GeoCoordinate center, GameObject gameObject, Rule rule, Way way)
        {

        }

        public virtual void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(NameKey);
        }
    }
}
