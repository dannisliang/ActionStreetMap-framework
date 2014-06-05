using System;
using System.IO;
using System.Linq;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Commands;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterBehavior : MonoBehaviour
    {
        public float delta = 10;
        private GameRunner component;
        private Vector2 position2D;
        private ITrace _trace;
        private DebugConsole _console;

        // Use this for initialization
        private void Start()
        {
            // create and register DebugConsole inside Container
            var container = new Container();
            var pathResolver = new DemoPathResolver();
            container.RegisterInstance(typeof(IPathResolver), pathResolver);
            InitializeConsole(container);
            try
            {
                component = new GameRunner(container, @"Config/app.config", pathResolver);
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
    }
}
