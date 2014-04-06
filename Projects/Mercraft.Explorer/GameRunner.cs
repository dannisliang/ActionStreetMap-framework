using System;
using Mercraft.Core;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using UnityEngine;
using Component = Mercraft.Infrastructure.Dependencies.Component;

namespace Mercraft.Explorer
{
    /// <summary>
    /// Represents application component root
    /// </summary>
    public class GameRunner : IGameRunner, IPositionListener
    {
        private const string SystemBootKey = "system/bootstrapping";

        /// <summary>
        /// DI container
        /// </summary>
        private readonly IContainer _container;

        /// <summary>
        /// Holds config reference
        /// NOTE Do not remove!
        /// </summary>
        private readonly ConfigSettings _config;

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

        /// <summary>
        /// Actual zone loader
        /// </summary>
        private IPositionListener _positionListener;

        public GameRunner(string configPath)
            : this(new Container(), new ConfigSettings(configPath))
        {
        }

        public GameRunner(IContainer container, string configPath)
            : this(container, new ConfigSettings(configPath))
        {
        }

        public GameRunner(IContainer container, ConfigSettings configSettings)
        {
            _container = container;
            _config = configSettings;
            Initialize();
        }

        private void Initialize()
        {
            // config root
            _container.RegisterInstance(_config);

            // component configurator
            _container.Register(Component
                .For<ComponentConfigurator>()
                .Use<ComponentConfigurator>()
                .Singleton());

            // register bootstrapping service which will register all dependencies
            var bootSection = _config.GetSection(SystemBootKey);
            var bootServiceType = bootSection.GetType(ConfigKeys.Type);

            _container.Register(Component
                .For<IBootstrapperService>()
                .Use(bootServiceType, new Type[0])
                .SetConfig(bootSection)
                .Singleton());
            
            // run bootstrappers
            _container.Resolve<IBootstrapperService>().Run();
        }

        public void RunGame()
        {
            _positionListener = _container.Resolve<IPositionListener>();
            OnMapPositionChanged(new Vector2(0, 0));
        }

        public void RunGame(GeoCoordinate coordinate)
        {
            _positionListener = _container.Resolve<IPositionListener>();

            OnGeoPositionChanged(coordinate);
            OnMapPositionChanged(new Vector2(0, 0));
        }

        public void OnMapPositionChanged(Vector2 position)
        {
            _positionListener.OnMapPositionChanged(position);
        }

        public void OnGeoPositionChanged(GeoCoordinate position)
        {
            _positionListener.OnGeoPositionChanged(position);
        }
    }
}
