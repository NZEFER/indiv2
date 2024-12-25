using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kg_ind2
{
    public class Light : Matrix
    {
        public _3Dpoint light;       // точка, где находится источник света
        public _3Dpoint light_сolor;       // цвет источника света

        public Light(_3Dpoint _light, _3Dpoint _color)
        {
            light = new _3Dpoint(_light);
            light_сolor = new _3Dpoint(_color);
        }
        // Затенение
        public _3Dpoint ShadingInterpolation(_3Dpoint point, _3Dpoint normal, _3Dpoint color, float koef_diffusion)
        {
            //Вычисляем диффузную составляющую цвета освещения
            _3Dpoint diffusion = koef_diffusion * light_сolor * Math.Max(_3Dpoint.scalar(normal, _3Dpoint.normal(light - point)), 0);
            return new _3Dpoint(diffusion.x * color.x, diffusion.y * color.y, diffusion.z * color.z);
        }
    }
}
