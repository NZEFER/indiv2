using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kg_ind2
{
    public class Ray
    {
        public _3Dpoint start;
        public _3Dpoint cource;
        public Ray() { }
        public Ray(_3Dpoint _start, _3Dpoint _cource)
        {
            start = new _3Dpoint(_start);
            this.cource = _3Dpoint.normal(_cource - _start);
        }

        public Ray(Ray _ray)
        {
            start = _ray.start;
            cource = _ray.cource;
        }

        // вычисляет отраженный луч 
        public Ray reflect(_3Dpoint impact_point, _3Dpoint normal)
        {
            return new Ray(impact_point, impact_point + cource - 2 * normal * _3Dpoint.scalar(cource, normal));
        }

        //Вычисляет преломленный луч 
        public Ray refract(_3Dpoint impact_point, _3Dpoint normal, float koef)
        {
            Ray r = new Ray();
            if (1 - koef * koef * (1 - _3Dpoint.scalar(normal, cource) * _3Dpoint.scalar(normal, cource)) >= 0)
            {
                r.start = new _3Dpoint(impact_point);
                r.cource = _3Dpoint.normal(koef * cource - ((float)Math.Sqrt(1 - koef * koef *
                    (1 - _3Dpoint.scalar(normal, cource) * _3Dpoint.scalar(normal, cource))) +
                    koef * _3Dpoint.scalar(normal, cource)) * normal);
                return r;
            }
            else
                return null;
        }
    }
}
