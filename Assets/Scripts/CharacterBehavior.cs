using System;
using System.Linq;
using Assets.Scripts.Console;
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterBehavior : MonoBehaviour
    {
        public float delta = 1;
        private GameRunner component;
        private Vector2 position2D;
        private ITrace _trace;

        // Use this for initialization
        void Start ()
        {
            // create and register DebugConsole inside Container
            var consoleGameObject = new GameObject("_DebugConsole_");
            var debugConsole = consoleGameObject.AddComponent<DebugConsole>();
            var container = new Container();
            container.RegisterInstance(debugConsole);

	        component = new GameRunner(container, @"Config\app.config");
            _trace = container.Resolve<ITrace>();
            //component.RunGame(new GeoCoordinate(52.531036, 13.384866)); // Invlidenstr. 117
            //52.529814, 13.388015));
        }
	
        // Update is called once per frame
        void Update () {
            if (Math.Abs(transform.position.x - position2D.x) > delta
                || Math.Abs(transform.position.z - position2D.y) > delta)
            {
                _trace.Normal("position change detect:" + transform.position);
                position2D = new Vector2(transform.position.x, transform.position.z);
               //component.OnMapPositionChanged(position2D);
            }
        }
    }
}
