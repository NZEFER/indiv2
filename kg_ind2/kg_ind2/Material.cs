using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kg_ind2
{
    public class Material
    {
        public float ambient;       // коэффициент принятия фонового освещения
        public float diffuse;       // коэффициент принятия диффузного освещения
        public _3Dpoint color;   // цвет материала

        public Material() { }
        public Material( float amb, float dif)
        {
            ambient = amb;
            diffuse = dif;
        }

        public Material(Material m)
        {
            ambient = m.ambient;
            diffuse = m.diffuse;
            color = new _3Dpoint(m.color);
        }

    }
}
