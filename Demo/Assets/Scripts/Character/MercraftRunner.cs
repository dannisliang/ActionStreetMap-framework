using System;
using System.Reactive.Linq;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Explorer.Bootstrappers;
using Mercraft.Infrastructure;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Infrastructure.IO;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class MercraftRunner : MonoBehaviour
    {
        public float Delta = 10;

        private GameRunner _gameRunner;

        private ITrace _trace;

        private Vector2 _position2D;
        
        private DebugConsole _console;

        private event DataEventHandler<MapPoint> CharacterMove;

        // Use this for initialization
        private void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update () {
            if (Math.Abs(transform.position.x - _position2D.x) > Delta
                || Math.Abs(transform.position.z - _position2D.y) > Delta)
            {
                _position2D = new Vector2(transform.position.x, transform.position.z);
                if (CharacterMove != null)
                {
                    CharacterMove(this, new DataEventArgs<MapPoint>(
                        new MapPoint(transform.position.x, transform.position.z)));
                }
            }
        }


        #region Initialization

        private void Initialize()
        {
            // create and register DebugConsole inside Container
            var container = new Container();
            var messageBus = new MessageBus();
            var pathResolver = new DemoPathResolver();
            InitializeConsole(container);
            try
            {
                var fileSystemService = new DemoWebFileSystemService(pathResolver);
                container.RegisterInstance(typeof(IPathResolver), pathResolver);
                container.RegisterInstance(typeof (IFileSystemService), fileSystemService);
                container.RegisterInstance<IConfigSection>(new ConfigSection(@"Config/settings.json", fileSystemService));

                // actual boot service
                container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperService>().Use<BootstrapperService>());

                // boot plugins
                container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<InfrastructureBootstrapper>().Named("infrastructure"));
                container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<OsmBootstrapper>().Named("osm"));
                container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<TileBootstrapper>().Named("tile"));
                container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<SceneBootstrapper>().Named("scene"));
                container.Register(Mercraft.Infrastructure.Dependencies.Component.For<IBootstrapperPlugin>().Use<DemoBootstrapper>().Named("demo"));


                InitializeMessageBusListeners(messageBus, _trace);

                // interception
                //container.AllowProxy = true;
                //container.AutoGenerateProxy = true;
                //container.AddGlobalBehavior(new TraceBehavior(_trace));

                _gameRunner = new GameRunner(container, messageBus);
                _gameRunner.RunGame();
            }
            catch (Exception ex)
            {
                _console.LogMessage(new ConsoleMessage("Error running game:" + ex.ToString(), RecordType.Error, Color.red));
                throw;
            }

            // subscribe on position changes
            Observable.FromEventPattern<DataEventHandler<MapPoint>, DataEventArgs<MapPoint>>(h =>
               CharacterMove += h, h => CharacterMove -= h)
              .Do(e => _gameRunner.OnMapPositionChanged(e.EventArgs.Data))
              .Subscribe();
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            _console = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(_console);
            _trace = new DebugConsoleTrace(_console);
            container.RegisterInstance<ITrace>(_trace);
            //_console.CommandManager.Register("scene", new SceneCommand(container));
        }

        private void InitializeMessageBusListeners(IMessageBus messageBus, ITrace trace)
        {
            // NOTE not sure that these classes won't be collected during GC
            new DemoTileListener(messageBus, trace);
        }

        #endregion
    }
}
