
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
                    phase = PhaseType.ThreePhase,
                    endType1 = EsabTerminatorType.CSE,
                    endType2 = EsabTerminatorType.SGT,
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
                acEd.WriteMessage("\n" + Enum.GetName(typeof(xdType), route.type));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabRating), route.rating));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(PhaseType), route.phase));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(PhaseColour), route.phasecol));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabTerminatorType), route.endType1));
                acEd.WriteMessage("\n" + Enum.GetName(typeof(EsabTerminatorType), route.endType2));
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


        [CommandMethod("ESABINIT")]
        public void EsabInitialise()
        {
            GeometryMethods ge = new GeometryMethods();
            ge.InitialiseGeometry();
        }


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
                route.phasecol = ed.GetPhaseColourFromKeywords();
                route.endType1 = ed.GetEndConnectorFromKeywords("End 1");
                route.endType2 = ed.GetEndConnectorFromKeywords("End 2");

                // one feature id per vertex
                //
                for (int i = 0; i < pline.NumberOfVertices; i++)
                {
                    route.featureIds.Add(pline.ObjectId);
                }

                route.ToXdictionary(pline);

                pline.UpgradeOpen();
                pline.Layer = "_Esab_Routes";
                pline.Linetype = "ByLayer";

                pline.Unhighlight();
                acEd.Regen();

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
            GeometryMethods gm = new GeometryMethods();

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            Polyline pline;
            ResultBuffer rb;
            Extents3d ext;
            bool hasFeature;
            DBObject mkr;
            ObjectId mkrid;
            EsabRoute route = new EsabRoute();
            EsabFeature feature;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                rb = pline.GetXDictionaryXrecordData(Constants.XappName);
                var data = rb.AsArray();
                xdType type = (xdType)Enum.ToObject(typeof(xdType), data[1].Value);

                if (type != xdType.Route)
                {
                    acEd.WriteMessage("\nSelection is not an ESAB Route entity\nExiting command\n");
                    return;
                }

                ext = pline.GeometricExtents;

                route.FromXdictionary(pline);

                acEd.SetCurrentView(ed.ZoomEntity(acEd, ext, 1.4));

                tr.Commit();
            }

            if (rb != null)
            {
                int vCount = pline.NumberOfVertices;

                // cycle through polyline vertices
                //
                for (int i = 0; i < vCount; i++)
                {
                    // focus on current vertex
                    //
                    Point2d vPnt2 = pline.GetPoint2dAt(i);
                    Point3d vPnt3 = pline.GetPoint3dAt(i);
                    ed.RedDiamond(vPnt2, 0.2);

                    // notify feature at current vertex
                    //
                    hasFeature = route.featureIds[i] != route.id;
                    if (hasFeature)
                    {
                        // vertex already has feature
                        //
                        acEd.WriteMessage($"\nVertex {i} has feature. \n");
                    }
                    else
                    {
                        // vertex is empty
                        //
                        acEd.WriteMessage($"\nSelect feature for vertex {i}: ");
                        EsabFeatureType ft = ed.GetFeatureFromKeywords("");

                        mkrid = gm.CreateFeatureMarker(ft, 0.2, vPnt3);
                        feature = new EsabFeature()
                        {
                            id = mkrid,
                            type = xdType.Feature,
                            parentId = pline.ObjectId,
                            parentVertex = i,
                            featureType = ft
                        };

                        using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                        {
                            mkr = (DBObject)tr.GetObject(mkrid, OpenMode.ForWrite);
                            feature.ToXdictionary(mkr);
                            tr.Commit();
                        }
                            


                        acEd.Regen();
                    }
                        
                }

            }
            else
            {
                acEd.WriteMessage("\nPolyline does not contain any ElectricalSiteAutoBuild data");
                acEd.WriteMessage("\nExiting Command\n");
                return;
            }


        }

        [CommandMethod("EsabInspect")] 

        public void EsabInspect()
        {
            var iWindow = new InspectionTool();
            iWindow.Show();
        }



    }


}
