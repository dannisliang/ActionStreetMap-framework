using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class Detail
    {
        public enum DetailPlace
        {
            Facade,
            Roof
        }

        public enum Orientation
        {
            Up,
            Down,
            Left,
            Right,
            Forward,
            Backward
        }

        private readonly Quaternion[] rotations =
        {
            Quaternion.identity,
            Quaternion.Euler(new Vector3(-180, 0, 0)),
            Quaternion.Euler(new Vector3(-90, 0, 0)),
            Quaternion.Euler(new Vector3(90, 0, 0)),
            Quaternion.Euler(new Vector3(0, 0, 90)),
            Quaternion.Euler(new Vector3(0, 0, -90))
        };

        public Detail(string name)
        {
            Name = name;
            Material = new Material(Shader.Find("Diffuse"));
        }

        public string Name;
        public Mesh Mesh;

        public float FaceHeight = 0;
        public Vector3 Scale = Vector3.one;
        public Orientation DetailOrientation = Orientation.Up;
        public Vector3 UserRotation = Vector3.zero;
        public int Face = 0;
        public DetailPlace Place = DetailPlace.Facade;
        public Material Material;
        
        public Vector3 WorldPosition = Vector3.zero;
        public Quaternion WorldRotation = Quaternion.identity;

        private Vector2 _faceUv = new Vector2(0, 0);   
        public Vector2 FaceUv
        {
            get { return _faceUv; }
            set
            {
                Vector2 newValue = value;
                newValue.x = Mathf.Clamp01(newValue.x);
                newValue.y = Mathf.Clamp01(newValue.y);
                _faceUv = newValue;
            }
        }
    }
}
