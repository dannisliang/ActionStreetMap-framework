using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class Detail
    {
        public enum Types
        {
            Facade,
            Roof
        }

        public enum Orientations
        {
            Up,
            Down,
            Left,
            Right,
            Forward,
            Backward
        }

        private Quaternion[] rotations = new[]
        {
            Quaternion.identity,
            Quaternion.Euler(new Vector3(-180, 0, 0)),
            Quaternion.Euler(new Vector3(-90, 0, 0)),
            Quaternion.Euler(new Vector3(90, 0, 0)),
            Quaternion.Euler(new Vector3(0, 0, 90)),
            Quaternion.Euler(new Vector3(0, 0, -90))
        };

        public string name = "";
        public Mesh mesh;
        [SerializeField] private Vector2 _faceUv = new Vector2(0, 0);
        public float faceHeight = 0;
        public Vector3 scale = Vector3.one;
        public Orientations orientation = Orientations.Up;
        public Vector3 userRotation = Vector3.zero;
        public int face = 0;
        public Types type = Types.Facade;
        public Material material;
        public Transform transform; //reference stored for display purposes
        public Vector3 worldPosition = Vector3.zero;
        public Quaternion worldRotation = Quaternion.identity;


        public Detail(string newName)
        {
            name = newName;
            material = new Material(Shader.Find("Diffuse"));
        }

        public Detail Duplicate()
        {
            return Duplicate(name + " copy");
        }

        public Detail Duplicate(string newName)
        {
            Detail newDetail = new Detail(newName);
            newDetail.mesh = mesh;
            newDetail.faceUv = _faceUv;
            newDetail.faceHeight = faceHeight;
            newDetail.scale = scale;
            newDetail.face = face;
            newDetail.type = type;
            newDetail.userRotation = userRotation;
            newDetail.orientation = orientation;
            newDetail.material = new Material(material);

            return newDetail;
        }

        public Quaternion rotation
        {
            get { return rotations[(int) orientation]; }
        }

        public Vector2 faceUv
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
