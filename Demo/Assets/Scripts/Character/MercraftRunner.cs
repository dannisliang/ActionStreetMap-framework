using System;
using System.Collections.Generic;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Commands;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Explorer.Bootstrappers;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class MercraftRunner : MonoBehaviour
    {
        public float delta = 50;
        private GameRunner component;
        private Vector2 position2D;
        
        private DebugConsole _console;

        // NOTE store listeners here to prevent GC
        private List<object> _listeners = new List<object>();

        // Use this for initialization
        private void Start()
        {
            // create and register DebugConsole inside Container
            var container = new Container();
            var messageBus = new MessageBus();
            var pathResolver = new DemoPathResolver();
            var trace = new DebugConsoleTrace();
            container.RegisterInstance(typeof(IPathResolver), pathResolver);
            container.RegisterInstance<IConfigSection>(new ConfigSettings(@"Config/app.config", pathResolver).GetRoot());
            container.RegisterInstance<ITrace>(trace);

            // actual boot service
            container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperService>().Use<BootstrapperService>());

            // boot plugins
            container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<InfrastructureBootstrapper>().Named("infrastructure"));
            container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<OsmBootstrapper>().Named("osm"));
            container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<ZoneBootstrapper>().Named("zone"));
            container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<SceneBootstrapper>().Named("scene"));
            container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<DemoBootstrapper>().Named("demo"));

            InitializeConsole(container);
            InitializeMessageBusListeners(messageBus, trace);
            try
            {
                component = new GameRunner(container, messageBus);
                component.RunGame();
            }
            catch (Exception ex)
            {
                _console.LogMessage(new ConsoleMessage("Error running game:" + ex.ToString(), RecordType.Error, Color.red));
                throw;
            }
        }

        // Update is called once per frame
        void Update () {
            if (Math.Abs(transform.position.x - position2D.x) > delta
                || Math.Abs(transform.position.z - position2D.y) > delta)
            {
                //_trace.Normal("position change:" + transform.position);
                position2D = new Vector2(transform.position.x, transform.position.z);
                component.OnMapPositionChanged(new MapPoint(transform.position.x, transform.position.z));
            }
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            _console = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(_console);
            _console.CommandManager.Register("scene", new SceneCommand(container));
        }

        private void InitializeMessageBusListeners(IMessageBus messageBus, ITrace trace)
        {
            // NOTE not sure that these classes won't be collected during GC
            new DemoTileListener(messageBus, trace);
            new DemoZoneListener(messageBus, trace);
        }
    }
}
