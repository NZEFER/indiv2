using kg_ind2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kg_ind2
{
    public partial class Form1 : Form
    {
        public List<Matrix> scene = new List<Matrix>();
        public List<Light> lights = new List<Light>();   
        public Color[,] color;                    
        public _3Dpoint[,] pixels;
        public _3Dpoint camera;
        public _3Dpoint left_up, right_up, left_down, right_down; 
        public int h, w;

        public Form1()
        {
            InitializeComponent();
            h = pictureBox1.Height;
            w = pictureBox1.Width;
            pictureBox1.Image = new Bitmap(w, h);

            camera = new _3Dpoint();
            left_up = new _3Dpoint();
            right_up = new _3Dpoint();
            left_down = new _3Dpoint();
            right_down = new _3Dpoint();

            get_scene();
            start_trace();
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    (pictureBox1.Image as Bitmap).SetPixel(i, j, color[i, j]);
                }
            }
            pictureBox1.Invalidate();
        }
        public bool visit_check(_3Dpoint start, _3Dpoint end)
        {
            foreach (Matrix figure in scene)
                if (figure.intersection_fig(new Ray(end, start), out float t, out _3Dpoint n)
                    && (t < (start - end).length() && t > Matrix.er))
                    return false;
            return true;
        }

        public _3Dpoint ray_trace(Ray ray, int iter, float env)
        {
            if (iter <= 0)
                return new _3Dpoint(0, 0, 0);

            float t = 0;       
            _3Dpoint normaly = null;
            Material m = new Material();
            _3Dpoint result = new _3Dpoint(0, 0, 0);
            bool IsRef = false; 

            foreach (Matrix fig in scene)
            {
                if (fig.intersection_fig(ray, out float intersect, out _3Dpoint n))

                    if (intersect < t || t == 0)     
                    {
                        t = intersect;
                        normaly = n;
                        m = new Material(fig.FigureMaterial);
                    }
            }

            if (t == 0)
                return new _3Dpoint(0, 0, 0);

            if (_3Dpoint.scalar(ray.cource, normaly) > 0)
            {
                normaly *= -1;
                IsRef = true;
            }
            foreach (Light l in lights)
            {
                _3Dpoint amb = l.light_сolor * m.ambient;
                amb.x *= m.color.x;
                amb.y *= m.color.y;
                amb.z *= m.color.z;
                result += amb;

                 if (visit_check(l.light, ray.start + ray.cource * t))
                   result += l.ShadingInterpolation(ray.start + ray.cource * t, normaly, m.color, m.diffuse); 
            }
                return result;
        }

        

        public void start_trace()
        {
            get_pixels();
            for (int i = 0; i < w; ++i)
                for (int j = 0; j < h; ++j)
                {
                    Ray r = new Ray(camera, pixels[i, j]);
                    r.start = new _3Dpoint(pixels[i, j]);
                    _3Dpoint color = ray_trace(r, 10, 1);
                    if (color.x > 1.0f || color.y > 1.0f || color.z > 1.0f)
                        color = _3Dpoint.normal(color); 
                    this.color[i, j] = Color.FromArgb((int)(255 * color.x), (int)(255 * color.y), (int)(255 * color.z));
                }
        }

        public void get_pixels()
        {
            pixels = new _3Dpoint[w, h];
            color = new Color[w, h];
            _3Dpoint u = new _3Dpoint(left_up);
            _3Dpoint d = new _3Dpoint(left_down);
            for (int i = 0; i < w; ++i)
            {
                _3Dpoint p = new _3Dpoint(d);
                for (int j = 0; j < h; ++j)
                {
                    pixels[i, j] = p;
                    p += (u - d) / (h - 1);
                }
                u += (right_up - left_up) / (w - 1);
                d += (right_down - left_down) / (w - 1);
            }
        }

       

        public void get_scene()
        {
           
            Matrix room = Matrix.Hexahedron(10);
            left_up = room.faces[0].point_index(0);
            right_up = room.faces[0].point_index(1);
            right_down = room.faces[0].point_index(2);
            left_down = room.faces[0].point_index(3);


            _3Dpoint normal = Face.normal(room.faces[0]);                          
            _3Dpoint center = (left_up + right_up + left_down + right_down) / 4;   
            camera = center + normal * 10;

            room.set_pen(new Pen(Color.LightGray));

            room.check_room = true;
            room = get_fig(room);

        }

        public Matrix get_fig(Matrix room)
        {
            float Ambient = 0.1f, Diffusion = 0.8f;
            room.UpWallMaterial = new Material( Ambient, Diffusion);
            room.DownWallMaterial = new Material( Ambient, Diffusion);
            room.faces[0].pen = new Pen(Color.Gray);
            room.BackWallMaterial = new Material( Ambient, Diffusion);
            room.faces[1].pen = new Pen(Color.AliceBlue);
            room.FrontWallMaterial = new Material( Ambient, Diffusion);
            room.faces[2].pen = new Pen(Color.Aqua);
            room.RightWallMaterial = new Material( Ambient, Diffusion);
            room.faces[3].pen = new Pen(Color.Salmon);
            room.LeftWallMaterial = new Material( Ambient, Diffusion);
            
            lights.Add(new Light(new _3Dpoint(1f, 1f, 4 - 0.1f), new _3Dpoint(1f, 1f, 1f)));
            //lights.Add(new Light(new _3Dpoint(1f, 1f, 4 - 0.1f), new _3Dpoint(0.5f, 0.2f, 0.2f)));


            Sphere GlassSphere = new Sphere(new _3Dpoint(0.5f, -1, -1.5f), 2f);
            GlassSphere.set_pen(new Pen(Color.PaleGreen));
            GlassSphere.FigureMaterial = new Material(Ambient, Diffusion);
            
            Matrix GlassCube = Matrix.Hexahedron(2.6f);
            GlassCube.bias(-2.4f, 2, -3.8f);
            GlassCube.rotate(70, "CX");
            GlassCube.rotate(70, "CY");
            GlassCube.set_pen(new Pen(Color.Plum));
            GlassCube.FigureMaterial = new Material( Ambient, Diffusion);
            
            Sphere RefractionSphere = new Sphere(new _3Dpoint(2.5f, 2f, -3.4f), 1.7f);
            RefractionSphere.set_pen(new Pen(Color.Yellow));
            RefractionSphere.FigureMaterial = new Material(Ambient, Diffusion);
            
            scene.Add(room);
            scene.Add(GlassCube);
            scene.Add(GlassSphere);
            scene.Add(RefractionSphere);
            return room;

        }

    }
}
