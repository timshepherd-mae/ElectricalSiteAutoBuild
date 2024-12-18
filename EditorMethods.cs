using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Documents;



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
            PromptIntegerResult pir = acEd.GetInteger(pio);
            acEd.WriteMessage($"\n{pir.Value}\n");
            return pir.Value;
        }

        public string GetStr(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptStringOptions pso = new PromptStringOptions($"\n{prompt}: ");
            PromptResult psr = acEd.GetString(pso);

            acEd.WriteMessage($"\n{psr.StringResult}\n");
            return psr.StringResult;

        }

        public string GetStr(string prompt, string defaultChoice)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptStringOptions pso = new PromptStringOptions($"\n{prompt} <{defaultChoice}>: ");
            PromptResult psr = acEd.GetString(pso);

            string result = (psr.StringResult.Length == 0) ? defaultChoice : psr.StringResult;
            acEd.WriteMessage($"\n{result}\n");
            return result;
        }


        public int GetEnumFromKeywords(Type e, string prompt, string ignore = "")
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions($"\n{prompt}: ");
            foreach (string s in Enum.GetNames(e) )
            {
                if (!ignore.Contains(s))
                    pko.Keywords.Add(s);
            }
            
            PromptResult kwd = acEd.GetKeywords(pko);
            
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (int)Enum.Parse(e, kwd.StringResult);
        }

        public ObjectId SelectExistingMarker(EsabXdType xdt)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select Existing Marker: ");
            peo.SetRejectMessage("\nOnly BlockRef entities allowed: ");
            peo.AddAllowedClass(typeof(BlockReference), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return ObjectId.Null;

            ObjectId mkrid = ObjectId.Null;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                DBObject dbo = (DBObject)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                ResultBuffer rb = dbo.GetXDictionaryXrecordData(Constants.XappName);

                // check for esab xdict data
                //
                if (rb == null)
                    return ObjectId.Null;

                var data = rb.AsArray();
                
                // check for xdtype == junction
                //
                if ((int)data[1].Value != (int)xdt)
                    return ObjectId.Null;

                mkrid = (ObjectId)data[0].Value;
                tr.Commit();
            }

            return mkrid;

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
