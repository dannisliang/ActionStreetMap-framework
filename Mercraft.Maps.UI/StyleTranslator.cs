
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Complete;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.UI.Scenes;

namespace Mercraft.Maps.UI
{
    /// <summary>
    /// Represents a style interpreter.
    /// </summary>
    public abstract class StyleTranslator
    {
        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="source"></param>
        /// <param name="element"></param>
        public virtual void Translate(IScene scene, IDataSourceReadOnly source, IProjection projection, Element element)
        {
            switch (element.Type)
            {
                case ElementType.Node:
                    this.Translate(scene, projection, CompleteNode.CreateFrom(element as Node));
                    break;
                case ElementType.Way:
                    this.Translate(scene, projection, CompleteWay.CreateFrom(element as Way, source));
                    break;
                case ElementType.Relation:
                    this.Translate(scene, projection, CompleteRelation.CreateFrom(element as Relation, source));
                    break;
            }
        }

        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        /// <param name="projection">The projection to use.</param>
        /// <param name="osmGeo">The osm object.</param>
        /// <param name="scene">The scene to fill with the resulting geometries.</param>
        /// <returns></returns>
        public abstract void Translate(IScene scene, IProjection projection, CompleteOsmGeo osmGeo);

        /// <summary>
        /// Returns true if this style applies to the given object.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract bool AppliesTo(Element element);
    }
}