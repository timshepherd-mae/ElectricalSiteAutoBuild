
using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;


namespace ElectricalSiteAutoBuild
{

    public class AutoCadCommands
    {

        #region TestCommands

        [CommandMethod("kwdtest")]
        public void Kwdtest()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = acDoc.Editor;

            PromptPointOptions ppo = new PromptPointOptions("Pick a point: ");
            ppo.Keywords.Add("teSt");
            ppo.Keywords.Add("Tree");
            ppo.Keywords.Add("fEnce");

            ppo.AppendKeywordsToMessage = true;

            PromptPointResult res = ed.GetPoint(ppo);
            ed.WriteMessage(res.Status.ToString() + "\n");
            ed.WriteMessage(res.StringResult + "\n");

        }

        [CommandMethod("route2xd")]
        public void Route2xd()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                EsabRoute route = new EsabRoute()
                {
                    id = pline.ObjectId,
                    rating = EsabRating.kv400,
                    phase = EsabPhase.ThreePhase,
                    endType1 = EsabConnectorType.CSE,
                    endType2 = EsabConnectorType.SGT,
                    featureIds = new ObjectIdCollection()
                };
                route.featureIds.Add(pline.ObjectId);
                route.featureIds.Add(pline.ObjectId);

                route.ToXdictionary(pline);

                tr.Commit();               
            }
        }

        [CommandMethod("xd2route")]
        public void Xd2route()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                EsabRoute route = new EsabRoute();
                route.FromXdictionary(pline);

                acEd.WriteMessage("\n" + route.id.ToString());
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabRating), route.rating));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabPhase), route.phase));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabConnectorType), route.endType1));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabConnectorType), route.endType2));
                foreach (ObjectId objid in route.featureIds)
                {
                    acEd.WriteMessage("\n" + objid.ToString());
                }

                tr.Commit();
            }

        }

        [CommandMethod("feature2xd")]
        public void Feature2xd()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select object for feature attachment: ");
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                EsabFeature ef = new EsabFeature()
                {
                    id = ent.ObjectId,
                    parentId = ent.ObjectId,
                    parentVertex = 0,
                    featureType = EsabFeatureType.PI
                };

                ef.ToXdictionary(ent);

                tr.Commit();

            }

        }

        [CommandMethod("xd2feature")]
        public void Xd2feature()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select object for feature inquiry: ");
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                EsabFeature ef = new EsabFeature();
                ef.FromXdictionary(ent);

                acEd.WriteMessage("\n" + ef.id.ToString());
                acEd.WriteMessage("\n" + ef.parentId.ToString());
                acEd.WriteMessage("\n" + ef.parentVertex.ToString());
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabFeatureType), ef.featureType));

                tr.Commit();
            }
        }

        #endregion TestCommands


        [CommandMethod("EsabAssignRouteProps")]
        public void AssignRouteProperties()
        {
            // select a LW Polyline as esab route and 
            // assign with route properties
            //

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                pline.Highlight();
                acEd.UpdateScreen();

                // get route instance
                //
                EsabRoute route = new EsabRoute();

                // editor prompts for property definition
                //
                route.id = pline.ObjectId;
                route.rating = ed.GetRatingFromKeywords();
                route.phase = ed.GetPhaseFromKeywords();
                route.endType1 = ed.GetEndConnectorFromKeywords("End 1");
                route.endType2 = ed.GetEndConnectorFromKeywords("End 2");

                route.ToXdictionary(pline);

                pline.Unhighlight();
                acEd.UpdateScreen();

                tr.Commit();

            }

        }


        [CommandMethod("EsabPopulateRoute")]
        public void PopulateRoute()
        {
            // select an assigned Route Polyline and populate
            // the vertices with features
            //
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            Polyline pline;
            ResultBuffer rb;
            Extents3d ext;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                rb = pline.GetXDictionaryXrecordData(Constants.XappName);
                ext = pline.GeometricExtents;
                tr.Commit();
            }

            acEd.SetCurrentView(ed.ZoomEntity(acEd, ext, 1.4));

            if (rb != null)
            {
                int vCount = pline.NumberOfVertices;

                // cycle through polyline vertices
                //
                for (int i = 0; i < vCount; i++)
                {
                    // focus on current vertex
                    //
                    Point2d vPnt = pline.GetPoint2dAt(i);
                    ed.RedDiamond(vPnt, 0.2);

                    Application.ShowAlertDialog("next");
                    acEd.Regen();

                }

            }
            else
            {
                acEd.WriteMessage("\nPolyline does not contain any ElectricalSiteAutoBuild data");
                acEd.WriteMessage("\nExiting Command\n");
                return;
            }

        }



    }


}
