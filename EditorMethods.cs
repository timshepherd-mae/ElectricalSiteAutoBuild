using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;



namespace ElectricalSiteAutoBuild
{
    public class EditorMethods
    {
        // autocad editor methods to reduce commandmethod code
        //
        public double GetDbl(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptDoubleOptions pdo = new PromptDoubleOptions($"\n{prompt}: ");
            PromptDoubleResult pdr = acEd.GetDouble(pdo);
            acEd.WriteMessage($"\n{pdr.Value}\n");
            return pdr.Value;
        }

        public int GetInt(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptIntegerOptions pio = new PromptIntegerOptions($"\n{prompt}: ");
            //PromptDoubleOptions pdo = new PromptDoubleOptions($"\n{prompt}: ");
            PromptIntegerResult pir = acEd.GetInteger(pio);
            acEd.WriteMessage($"\n{pir.Value}\n");
            return pir.Value;
        }

        public int GetEnumFromKeywords(Type e, string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions($"\n{prompt}: ");
            foreach (string s in Enum.GetNames(e) )
            {
                pko.Keywords.Add(s);
            }
            
            PromptResult kwd = acEd.GetKeywords(pko);
            
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (int)Enum.Parse(e, kwd.StringResult);
        }


        public ViewTableRecord ZoomEntity(Editor acEd, Extents3d ext, double zoomfactor)
        {
            ext.TransformBy(acEd.CurrentUserCoordinateSystem.Inverse());

            Point2d min2d = new Point2d(ext.MinPoint.X, ext.MinPoint.Y);
            Point2d max2d = new Point2d(ext.MaxPoint.X, ext.MaxPoint.Y);

            ViewTableRecord view = new ViewTableRecord
            {
                CenterPoint = min2d + ((max2d - min2d) / 2.0),
                Height = (max2d.Y - min2d.Y) * zoomfactor,
                Width = (max2d.X - min2d.X) * zoomfactor
            };

            return view;

        }

        public void RedDiamond(Point2d pnt, double sz)
        {
            using (ResultBuffer rb = new ResultBuffer())
            {
                rb.Add(new TypedValue(5003, 1));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X , pnt.Y + sz)));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X + sz, pnt.Y )));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X + sz, pnt.Y )));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X , pnt.Y - sz)));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X , pnt.Y - sz)));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X - sz, pnt.Y )));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X - sz, pnt.Y )));
                rb.Add(new TypedValue(5002, new Point2d(pnt.X , pnt.Y + sz)));

                Application.DocumentManager.MdiActiveDocument.Editor.DrawVectors(rb, Matrix3d.Identity);
            }

        }


    }
}
