using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace kg_ind2
{
    public class Matrix
    {
        public Material FrontWallMaterial;
        public Material BackWallMaterial;
        public Material LeftWallMaterial;
        public Material RightWallMaterial;
        public Material UpWallMaterial;
        public Material DownWallMaterial;
        public Material FigureMaterial;
        public List<_3Dpoint> points = new List<_3Dpoint>();
        public List<Face> faces = new List<Face>();
        public static float er = 0.0001f;
        public bool check_room = false;


        public Matrix() { }
        public float[,] matrix_coord()
        {
            var res = new float[points.Count, 4];
            for (int i = 0; i < points.Count; i++)
            {
                res[i, 0] = points[i].x;
                res[i, 1] = points[i].y;
                res[i, 2] = points[i].z;
                res[i, 3] = 1;
            }
            return res;
        }

        private static float[,] matrix_mult(float[,] a1, float[,] a2)
        {
            float[,] res = new float[a1.GetLength(0), a2.GetLength(1)];
            for (int i = 0; i < a1.GetLength(0); i++)
            {
                for (int j = 0; j < a2.GetLength(1); j++)
                {
                    for (int k = 0; k < a2.GetLength(0); k++)
                    {
                        res[i, j] += a1[i, k] * a2[k, j];
                    }
                }
            }
            return res;
        }
       
        private _3Dpoint center_matrix()
        {
            _3Dpoint res = new _3Dpoint(0, 0, 0);
            foreach (_3Dpoint p in points)
            {
                res.x += p.x;
                res.y += p.y;
                res.z += p.z;

            }
            res.x /= points.Count();
            res.y /= points.Count();
            res.z /= points.Count();
            return res;
        }
        public bool intersection_ray(Ray ray, _3Dpoint p1, _3Dpoint p2, _3Dpoint p3, out float chek_inter)
        {
            chek_inter = -1;
            if (_3Dpoint.scalar(p2 - p1, ray.cource * (p3 - p1)) > -er && _3Dpoint.scalar(p2 - p1, ray.cource * (p3 - p1)) < er)
                return false;

            float f = 1.0f / _3Dpoint.scalar(p2 - p1, ray.cource * (p3 - p1));
            if (f * _3Dpoint.scalar((ray.start - p1), ray.cource * (p3 - p1)) < 0 || f * _3Dpoint.scalar((ray.start - p1), ray.cource * (p3 - p1)) > 1)
                return false;

            if (f * _3Dpoint.scalar(ray.cource, (ray.start - p1) * (p2 - p1)) < 0 || f * _3Dpoint.scalar((ray.start - p1),
                ray.cource * (p3 - p1)) + f * _3Dpoint.scalar(ray.cource, (ray.start - p1) * (p2 - p1)) > 1)
                return false;

            if (f * _3Dpoint.scalar(p3 - p1, (ray.start - p1) * (p2 - p1)) > er)
            {
                chek_inter = f * _3Dpoint.scalar(p3 - p1, (ray.start - p1) * (p2 - p1));
                return true;
            }
            return false;
        }
       
        public void apply_matrix(float[,] matrix)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].x = matrix[i, 0] / matrix[i, 3];
                points[i].y = matrix[i, 1] / matrix[i, 3];
                points[i].z = matrix[i, 2] / matrix[i, 3];

            }
        }
        

        private static float[,] rotate_y(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), 0, -(float)Math.Sin(angle), 0 }, { 0, 1, 0, 0 },
                { (float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return matrix_mult(transform_matrix, rotationMatrix);
        }

        private static float[,] rotate_x(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, (float)Math.Cos(angle), (float)Math.Sin(angle), 0 },
                { 0, -(float)Math.Sin(angle), (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return matrix_mult(transform_matrix, rotationMatrix);
        }


        private static float[,] rotate_z(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), (float)Math.Sin(angle), 0, 0 }, { -(float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0 },
                { 0, 0, 1, 0 }, { 0, 0, 0, 1} };
            return matrix_mult(transform_matrix, rotationMatrix);
        }
        public void bias(float xb, float yb, float zb)
        {
            apply_matrix(get_bias(matrix_coord(), xb, yb, zb));
        }

        
        private static float[,] get_bias(float[,] matrix, float x, float y, float z)
        {
            float[,] translationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { x, y, z, 1 } };
            return matrix_mult(matrix, translationMatrix);
        }
        
        public virtual void set_pen(Pen pen)
        {
            foreach (Face face in faces)
                face.pen = pen;

        }

        
        public void rotate_point(float rangle, string type)
        {
            float[,] m = matrix_coord();
            _3Dpoint center = center_matrix();
            switch (type)
            {
                case "CX":
                    m = get_bias(m, -center.x, -center.y, -center.z);
                    m = rotate_x(m, rangle);
                    m = get_bias(m, center.x, center.y, center.z);
                    break;
                case "CY":
                    m = get_bias(m, -center.x, -center.y, -center.z);
                    m = rotate_y(m, rangle);
                    m = get_bias(m, center.x, center.y, center.z);
                    break;
                case "CZ":
                    m = get_bias(m, -center.x, -center.y, -center.z);
                    m = rotate_z(m, rangle);
                    m = get_bias(m, center.x, center.y, center.z);
                    break;
               
            }
            apply_matrix(m);
        }

        public void rotate(float angle, string type)
        {
            rotate_point(angle * (float)Math.PI / 180, type);
        }

        public virtual bool intersection_fig(Ray ray, out float chec_inter, out _3Dpoint normaly)
        {
            chec_inter = 0;
            normaly = null;
            Face Side = null;
            int n = -1;

            for (int i = 0; i < faces.Count; ++i)
            {

                if (intersection_ray(ray, faces[i].point_index(0), faces[i].point_index(1), faces[i].point_index(3), out float t) && (chec_inter == 0 || t < chec_inter))
                {
                    n = i;
                    chec_inter = t;
                    Side = faces[i];
                }
                else if (intersection_ray(ray, faces[i].point_index(1), faces[i].point_index(2), faces[i].point_index(3), out t) && (chec_inter == 0 || t < chec_inter))
                {
                    n = i;
                    chec_inter = t;
                    Side = faces[i];
                }
                
            }

            if (chec_inter != 0)
            {
                normaly = Face.normal(Side);
                if (check_room)
                    switch (n)
                    {
                        case 0:
                            FigureMaterial = new Material(BackWallMaterial);
                            break;
                        case 1:
                            FigureMaterial = new Material(FrontWallMaterial);
                            break;
                        case 2:
                            FigureMaterial = new Material(RightWallMaterial);
                            break;
                        case 3:
                            FigureMaterial = new Material(LeftWallMaterial);
                            break;
                        case 4:
                            FigureMaterial = new Material(UpWallMaterial);
                            break;
                        case 5:
                            FigureMaterial = new Material(DownWallMaterial);
                            break;
                        default:
                            break;
                    }
                FigureMaterial.color = new _3Dpoint(Side.pen.Color.R / 255f, Side.pen.Color.G / 255f, Side.pen.Color.B / 255f);
                return true;
            }

            return false;
        }


       
        // Куб          
        static public Matrix Hexahedron(float sz)
        {
            Matrix res = new Matrix();
            res.points.Add(new _3Dpoint(sz / 2, sz / 2, sz / 2));
            res.points.Add(new _3Dpoint(-sz / 2, sz / 2, sz / 2));
            res.points.Add(new _3Dpoint(-sz / 2, sz / 2, -sz / 2));
            res.points.Add(new _3Dpoint(sz / 2, sz / 2, -sz / 2));

            res.points.Add(new _3Dpoint(sz / 2, -sz / 2, sz / 2));
            res.points.Add(new _3Dpoint(-sz / 2, -sz / 2, sz / 2));
            res.points.Add(new _3Dpoint(-sz / 2, -sz / 2, -sz / 2));
            res.points.Add(new _3Dpoint(sz / 2, -sz / 2, -sz / 2));

            Face face = new Face(res);
            face.points_face.AddRange(new int[] { 3, 2, 1, 0 });
            res.faces.Add(face);

            face = new Face(res);
            face.points_face.AddRange(new int[] { 4, 5, 6, 7 });
            res.faces.Add(face);

            face = new Face(res);
            face.points_face.AddRange(new int[] { 2, 6, 5, 1 });
            res.faces.Add(face);

            face = new Face(res);
            face.points_face.AddRange(new int[] { 0, 4, 7, 3 });
            res.faces.Add(face);

            face = new Face(res);
            face.points_face.AddRange(new int[] { 1, 5, 4, 0 });
            res.faces.Add(face);

            face = new Face(res);
            face.points_face.AddRange(new int[] { 2, 3, 7, 6 });
            res.faces.Add(face);

            return res;
        }
    }
}
