using System;
using Mercraft.Core;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Explorer
{
    /// <summary>
    /// Represents application component root
    /// </summary>
    public class GameRunner : IGameRunner, IPositionListener
    {
        /// <summary>
        /// DI container
        /// </summary>
        private readonly IContainer _container;

        /// <summary>
        /// Actual zone loader
        /// </summary>
        private IPositionListener _positionListener;

        /// <summary>
        /// Returns relative null geo coordinate point which is used as center for calculation
        /// </summary>
        public GeoCoordinate RelativeNullPoint
        {
            get
            {
                return _positionListener.RelativeNullPoint;
            }
        }

        public GameRunner(IContainer container)
        {
            _container = container;
            Initialize();
        }

        private void Initialize()
        {           
            // run bootstrappers
            _container.Resolve<IBootstrapperService>().Run();
        }

        public void RunGame()
        {
            _positionListener = _container.Resolve<IPositionListener>();
            OnMapPositionChanged(new MapPoint(0, 0));
        }

        public void RunGame(GeoCoordinate coordinate)
        {
            _positionListener = _container.Resolve<IPositionListener>();

            OnGeoPositionChanged(coordinate);
            OnMapPositionChanged(new MapPoint(0, 0));
        }

        public void OnMapPositionChanged(MapPoint position)
        {
            _positionListener.OnMapPositionChanged(position);
        }

        public void OnGeoPositionChanged(GeoCoordinate position)
        {
            _positionListener.OnGeoPositionChanged(position);
        }
    }
}
