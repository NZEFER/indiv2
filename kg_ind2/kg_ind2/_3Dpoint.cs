using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kg_ind2
{
    public class _3Dpoint
    {
        public float x, y, z;

        public _3Dpoint()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public _3Dpoint(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public _3Dpoint(_3Dpoint p)
        {
            if (p == null)
                return;
            x = p.x;
            y = p.y;
            z = p.z;
        }



        public static _3Dpoint operator -(_3Dpoint p1, _3Dpoint p2)
        {
            return new _3Dpoint(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);

        }

        public static float scalar(_3Dpoint p1, _3Dpoint p2)
        {
            return p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
        }


        public static _3Dpoint operator +(_3Dpoint p1, _3Dpoint p2)
        {
            return new _3Dpoint(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);

        }

        public static _3Dpoint operator *(_3Dpoint p1, _3Dpoint p2)
        {
            return new _3Dpoint(p1.y * p2.z - p1.z * p2.y, p1.z * p2.x - p1.x * p2.z, p1.x * p2.y - p1.y * p2.x);
        }

        public static _3Dpoint operator *(float t, _3Dpoint p1)
        {
            return new _3Dpoint(p1.x * t, p1.y * t, p1.z * t);
        }
    
        

        public float length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
       
        public static _3Dpoint operator /(_3Dpoint p1, float t)
        {
            return new _3Dpoint(p1.x / t, p1.y / t, p1.z / t);
        }


        public static _3Dpoint operator *(_3Dpoint p1, float t)
        {
            return new _3Dpoint(p1.x * t, p1.y * t, p1.z * t);
        }


        public static _3Dpoint normal(_3Dpoint p)
        {
            float z = (float)Math.Sqrt((float)(p.x * p.x + p.y * p.y + p.z * p.z));
            if (z == 0)
                return new _3Dpoint(p);
            return new _3Dpoint(p.x / z, p.y / z, p.z / z);
        }

    }
}
