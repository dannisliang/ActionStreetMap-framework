using System.Collections.Generic;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public class MapScene : IScene
    {
        #region Areas

        private List<Area> _areas;
        public IEnumerable<Area> Areas
        {
            get
            {
                return _areas;
            }
        }
        public void AddArea(Area area)
        {
            _areas.Add(area);
        }

        #endregion

        #region Ways

        private List<Way> _ways;
        public IEnumerable<Way> Ways
        {
            get
            {
                return _ways;
            }
        }
        public void AddWay(Way way)
        {
            _ways.Add(way);
        }

        #endregion

        public MapScene()
        {
            _areas = new List<Area>();
            _ways = new List<Way>();
        }
   }
}
