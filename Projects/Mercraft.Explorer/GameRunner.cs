﻿using System;
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

        #region Privates state. Try avoid usage of the corresponding properties except unit tests
        /// <summary>
        /// Holds config reference
        /// NOTE Do not remove!
        /// </summary>
        private readonly ConfigSettings _config;

        public ConfigSettings Config
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// DI container
        /// </summary>
        private readonly IContainer _container = new Container();

        public IContainer Container
        {
            get
            {
                return _container;
            }
        }

        #endregion

        /// <summary>
        /// Actual zone loader
        /// </summary>
        private IPositionListener _positionListener;

        public GameRunner(string configPath)
        {
            var configSettings = new ConfigSettings(configPath);
            InitializeFromConfig(configSettings);
        }

        public GameRunner(ConfigSettings configSettings)
        {
            _config = configSettings;
            InitializeFromConfig(configSettings);
        }

        private void InitializeFromConfig(ConfigSettings configSettings)
        {
            // config root
            _container.RegisterInstance(configSettings);

            // component configurator
            _container.Register(Component
                .For<ComponentConfigurator>()
                .Use<ComponentConfigurator>()
                .Singleton());

            // register bootstrapping service which will register all dependencies
            var bootSection = configSettings.GetSection("system/bootstrapping");
            var bootServiceType = bootSection.GetType(ConfigKeys.Type);

            _container.Register(Component
                .For<IBootstrapperService>()
                .Use(bootServiceType, new Type[0])
                .SetConfig(bootSection)
                .Singleton());
            
            // run bootstrappers
            _container.Resolve<IBootstrapperService>().Run();
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