using System;
using System.Collections.Generic;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public class MapScene : IScene
    {
        private bool _disposed = false;

        #region Canvas

        public Canvas Canvas { get; set; }

        #endregion

        #region Areas

        private List<Area> _areas;
        public IEnumerable<Area> Areas
        {
            get
            {
                ThrowIfDisposed();
                return _areas;
            }
        }
        public void AddArea(Area area)
        {
            ThrowIfDisposed();
            _areas.Add(area);
        }

        #endregion

        #region Ways

        private List<Way> _ways;
        public IEnumerable<Way> Ways
        {
            get
            {
                ThrowIfDisposed();
                return _ways;
            }
        }
        public void AddWay(Way way)
        {
            ThrowIfDisposed();
            _ways.Add(way);
        }

        #endregion

        public MapScene()
        {
            _areas = new List<Area>(64);
            _ways = new List<Way>(64);
        }

        private void ThrowIfDisposed()
        {
            if(_disposed)
                throw new ObjectDisposedException("MapScene is already disposed!");
        }

        public void Dispose()
        {
            Canvas = null;
            _areas.Clear();
            _ways.Clear();
            _disposed = true;
        }
    }
}
