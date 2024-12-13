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
        public double GetRealInput(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptDoubleOptions pdo = new PromptDoubleOptions($"\n{prompt}: ");
            PromptDoubleResult pdr = acEd.GetDouble(pdo);
            acEd.WriteMessage($"\n{pdr.Value.ToString()}\n");
            return pdr.Value;
        }

        public EsabRating GetRatingFromKeywords()
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\nRating: ");
            foreach (string s in Enum.GetNames(typeof(EsabRating)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (EsabRating)Enum.Parse(typeof(EsabRating), kwd.StringResult);
        }

        public PhaseType GetPhaseFromKeywords()
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\nPhase: ");
            foreach (string s in Enum.GetNames(typeof(PhaseType)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (PhaseType)Enum.Parse(typeof(PhaseType), kwd.StringResult);
        }

        public PhaseColour GetPhaseColourFromKeywords()
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\nPhase Colour: ");
            foreach (string s in Enum.GetNames(typeof(PhaseColour)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (PhaseColour)Enum.Parse(typeof(PhaseColour), kwd.StringResult);
        }
        public EsabTerminatorType GetEndConnectorFromKeywords(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\n" + prompt + ": ");
            foreach (string s in Enum.GetNames(typeof(EsabTerminatorType)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (EsabTerminatorType)Enum.Parse(typeof(EsabTerminatorType), kwd.StringResult);
        }

        public EsabFeatureType GetFeatureFromKeywords(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\n" + prompt + ": ");
            foreach (string s in Enum.GetNames(typeof(EsabFeatureType)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (EsabFeatureType)Enum.Parse(typeof(EsabFeatureType), kwd.StringResult);
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
