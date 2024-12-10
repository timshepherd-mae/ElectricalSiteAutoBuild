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

        public EsabPhase GetPhaseFromKeywords()
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\nPhase: ");
            foreach (string s in Enum.GetNames(typeof(EsabPhase)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (EsabPhase)Enum.Parse(typeof(EsabPhase), kwd.StringResult);
        }

        public EsabConnectorType GetEndConnectorFromKeywords(string prompt)
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptKeywordOptions pko = new PromptKeywordOptions("\n" + prompt + ": ");
            foreach (string s in Enum.GetNames(typeof(EsabConnectorType)))
            {
                pko.Keywords.Add(s);
            }

            PromptResult kwd = acEd.GetKeywords(pko);
            acEd.WriteMessage("\n" + kwd.StringResult + "\n");
            return (EsabConnectorType)Enum.Parse(typeof(EsabConnectorType), kwd.StringResult);
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

        public void QuickAdd(Entity ent)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                // get block table record
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
                btr.AppendEntity(ent);
                tr.AddNewlyCreatedDBObject(ent, true);
                tr.Commit();
            }
            acDoc.Editor.Regen();
        }

        public void QuickRemove(Entity ent)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                // get block table record
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
                //btr.AppendEntity(ent);
                //tr.AddNewlyCreatedDBObject(ent, true);

                ent.Erase();
                tr.Commit();
            }
            acDoc.Editor.Regen();
        }




    }
}
