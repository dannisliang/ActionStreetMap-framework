using System;
using System.Linq;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Commands;
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterBehavior : MonoBehaviour
    {
        public float delta = 10;
        private const string configPath = @"Config\app.config";
        private GameRunner component;
        private Vector2 position2D;
        //private ITrace _trace;

        // Use this for initialization
        private void Start()
        {
            // create and register DebugConsole inside Container
            var container = new Container();
            InitializeConsole(container);

            component = new GameRunner(container, configPath);
            //_trace = container.Resolve<ITrace>();
            component.RunGame();
            
        }

        // Update is called once per frame
        void Update () {
            if (Math.Abs(transform.position.x - position2D.x) > delta
                || Math.Abs(transform.position.z - position2D.y) > delta)
            {
                //_trace.Normal("position change:" + transform.position);
                position2D = new Vector2(transform.position.x, transform.position.z);
                component.OnMapPositionChanged(position2D);
            }
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            var debugConsole = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(debugConsole);
            debugConsole.CommandManager.Register("scene", new SceneCommand(container));
        }
    }
}
