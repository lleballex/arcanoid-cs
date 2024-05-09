using System;

namespace arcanoid_cs
{
    public class Speed
    {
        public float value;
        public float angle;

        public float x
        {
            get { return value * (float)Math.Cos(angle * Math.PI / 180); }
        }
        public float y
        {
            get { return value * (float)Math.Sin(angle * Math.PI / 180);  }
        }

        public Speed(float value_, float angle_ = 0)
        {
            value = value_;
            angle = angle_;
        }
    }
}
