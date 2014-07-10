namespace Mercraft.Models.Roads
{
    public class Cubic
    {
        private float _a, _b, _c, _d;

        public Cubic(float a, float b, float c, float d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        public float Eval(float u)
        {
            return (((_d * u) + _c) * u + _b) * u + _a;
        }
    }
}