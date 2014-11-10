using System;
using Mercraft.Core;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Explorer
{
    /// <summary>
    ///     Represents application component root.
    /// </summary>
    public class GameRunner : IGameRunner, IPositionListener
    {
        /// <summary>
        ///     DI container.
        /// </summary>
        private readonly IContainer _container;

        /// <summary>
        ///     Message bus.
        /// </summary>
        private readonly IMessageBus _messageBus;

        /// <summary>
        ///     Actual zone loader.
        /// </summary>
        private IPositionListener _positionListener;

        /// <summary>
        ///     Returns relative null geo coordinate point which is used as center for calculation
        /// </summary>
        public GeoCoordinate RelativeNullPoint
        {
            get { return _positionListener.RelativeNullPoint; }
            set { throw new InvalidOperationException(Strings.CannotChangeRelativeNullPoint); }
        }

        /// <summary>
        ///     Creates GameRunner
        /// </summary>
        /// <param name="container">DI container.</param>
        /// <param name="messageBus">Message bus.</param>
        public GameRunner(IContainer container, IMessageBus messageBus)
        {
            _container = container;
            _messageBus = messageBus;
            Initialize();
        } 

        private void Initialize()
        {
            // run bootstrappers
            _container.RegisterInstance(_messageBus);
            _container.Resolve<IBootstrapperService>().Run();
        }

        /// <inheritdoc />
        public void RunGame()
        {
            _positionListener = _container.Resolve<IPositionListener>();
            OnMapPositionChanged(new MapPoint(0, 0));
        }

        /// <inheritdoc />
        public void RunGame(GeoCoordinate coordinate)
        {
            _positionListener = _container.Resolve<IPositionListener>();
            _positionListener.RelativeNullPoint = coordinate;

            OnGeoPositionChanged(coordinate);
        }

        /// <inheritdoc />
        public void OnMapPositionChanged(MapPoint position)
        {
            _positionListener.OnMapPositionChanged(position);
        }

        /// <inheritdoc />
        public void OnGeoPositionChanged(GeoCoordinate position)
        {
            _positionListener.OnGeoPositionChanged(position);
        }
    }
}