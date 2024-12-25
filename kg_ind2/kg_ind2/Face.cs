using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kg_ind2
{
    public class Face
    {
        public Matrix face = null; 
        public List<int> points_face = new List<int>();
        public _3Dpoint Normal; 
        public Pen pen = new Pen(Color.Black);
       
        public Face(Face _face)
        {
            points_face = new List<int>(_face.points_face);
            face = _face.face;
            Normal = new _3Dpoint(_face.Normal);
            pen = _face.pen.Clone() as Pen;
           
        }
        public Face(Matrix _face = null)
        {
            face = _face;
        }
        public _3Dpoint point_index(int ind)
        {
            if (face != null)
                return face.points[points_face[ind]];
            return null;
        }
       
        public static _3Dpoint normal(Face _face)
        {
            if (_face.points_face.Count() < 3)
                return new _3Dpoint(0, 0, 0);
            return _3Dpoint.normal((_face.point_index(1) - _face.point_index(0)) * (_face.point_index(_face.points_face.Count - 1) - _face.point_index(0)));
        }
        

    }
}
