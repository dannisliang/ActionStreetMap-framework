using System;
using Mercraft.Core;
using Mercraft.Explorer;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    public class CharacterBehavior : MonoBehaviour
    {
        private float FloatEpsilon = 1;
        private ComponentRoot component;
        private Vector3 position;

        // Use this for initialization
        void Start ()
        {
            Debug.Log("onStart");
	        component = new ComponentRoot(@"Config\app.config");

            Debug.Log("Run Game");
            component.RunGame(new GeoCoordinate(52.529814, 13.388015));
        }
	
        // Update is called once per frame
        void Update () {
            if (Math.Abs(transform.position.x - position.x) > FloatEpsilon
                && Math.Abs(transform.position.z - position.y) > FloatEpsilon)
            {
                Debug.Log("position change detect:" + transform.position);
                position = new Vector2(transform.position.x, transform.position.y);
                component.OnMapPositionChanged(position);
            }
        }
    }
}
