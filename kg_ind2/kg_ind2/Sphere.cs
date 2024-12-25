using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kg_ind2
{
    public class Sphere : Matrix
    {
        float radius; 

        public Pen pen = new Pen(Color.Black);

        public Sphere(_3Dpoint _point, float _radius)
        {
            points.Add(_point);
            radius = _radius;
        }
         public override void set_pen(Pen _pen)
        {
            pen = _pen;

        }
        public static bool intersection_sphere(Ray ray, _3Dpoint center, float radius, out float koef)
        {
            float p1 = _3Dpoint.scalar(ray.start - center, ray.cource);
            float p2 = _3Dpoint.scalar(ray.start - center, ray.start - center) - radius * radius;
            float p3 = _3Dpoint.scalar(ray.start - center, ray.cource) * _3Dpoint.scalar(ray.start - center, ray.cource) - p2;
            koef = 0;
            if (p3 >= 0)
            {
                if (Math.Min(-p1 + (float)Math.Sqrt(p3), -p1 - (float)Math.Sqrt(p3)) > er)
                {
                    koef = Math.Min(-p1 + (float)Math.Sqrt(p3), -p1 - (float)Math.Sqrt(p3));
                }
                else
                {
                    koef = Math.Max(-p1 + (float)Math.Sqrt(p3), -p1 - (float)Math.Sqrt(p3));
                };
                return koef > er;
            }
            return false;
        }

        public override bool intersection_fig(Ray ray, out float koeff, out _3Dpoint normaly)
        {
            koeff = 0;
            normaly = null;

            if (intersection_sphere(ray, points[0], radius, out koeff) && (koeff > er))
            {
                normaly = _3Dpoint.normal(ray.start + ray.cource * koeff - points[0]);
                FigureMaterial.color = new _3Dpoint(pen.Color.R / 255f, pen.Color.G / 255f, pen.Color.B / 255f);
                return true;
            }
            return false;
        }

       
       
    }
}
