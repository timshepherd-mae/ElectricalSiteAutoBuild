
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

        [CommandMethod("MLTEST01")]
        public void mltest01()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                DBDictionary mlDict = (DBDictionary)tr.GetObject(acDb.MLStyleDictionaryId, OpenMode.ForRead);



                Mline mline = new Mline();
                mline.Normal = pline.Normal;
                mline.Style = mlDict.GetAt("esabRYB");
                mline.Scale = 1;
                mline.Justification = MlineJustification.Zero;
                mline.Layer = "_Esab_Routes";

                for (int i = 0; i < pline.NumberOfVertices; i++)
                {
                    mline.AppendSegment(pline.GetPoint3dAt(i));
                }

                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
                btr.AppendEntity(mline);
                tr.AddNewlyCreatedDBObject(mline, true);

                //DBObject dbo = (DBObject)tr.GetObject(pline.ObjectId, OpenMode.ForWrite);
                //dbo.Erase();

                pline.UpgradeOpen();
                pline.Erase();
                


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
                route.rating = (EsabRating)ed.GetEnumFromKeywords(typeof(EsabRating), "Rating");
                route.phase = (EsabPhaseType)ed.GetEnumFromKeywords(typeof(EsabPhaseType), "Phase");
                if (route.phase == EsabPhaseType.ThreePhase) route.phasesep = ed.GetInt("Phase Separation Distance");
                route.phasecol = (EsabPhaseColour)ed.GetEnumFromKeywords(typeof(EsabPhaseColour), "Phase Colour");
                route.defaultConductorType = (EsabConductorType)ed.GetEnumFromKeywords(typeof(EsabConductorType), "Default Conductor");

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
            Database acDb = acDoc.Database;
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
            Entity mkr;
            ObjectId mkrid;
            EsabRoute route = new EsabRoute();
            EsabFeature feature;
            EsabFeatureType ft = EsabFeatureType.NUL;
            EsabTerminator terminator;
            EsabJunction junction;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                rb = pline.GetXDictionaryXrecordData(Constants.XappName);
                var data = rb.AsArray();
                EsabXdType type = (EsabXdType)Enum.ToObject(typeof(EsabXdType), data[1].Value);

                if (type != EsabXdType.Route)
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
                        // vertex is empty, create feature
                        //
                        ed.RedDiamond(vPnt2, 0.2);
                        
                        
                        // exclude terminator feature type unless start or end vertex
                        //
                        if (i == 0 || i == vCount - 1)
                        {
                            ft = (EsabFeatureType)ed.GetEnumFromKeywords(typeof(EsabFeatureType), $"\nSelect feature for vertex {i}: ");
                        }
                        else
                        {
                            ft = (EsabFeatureType)ed.GetEnumFromKeywords(typeof(EsabFeatureType), $"\nSelect feature for vertex {i}: ", "Terminator");
                        }

                        switch (ft)
                        {
                            case EsabFeatureType.Terminator:

                                EsabTerminatorType tm = (EsabTerminatorType)ed.GetEnumFromKeywords(typeof(EsabTerminatorType), "\nSelect termination type: ");

                                // is terminator selection a LinkTo
                                //
                                if (tm == EsabTerminatorType.LinkTo)
                                {
                                    // select an existing terminator feature
                                    //
                                    mkrid = ObjectId.Null;
                                    {
                                        mkrid = ed.SelectExistingMarker(EsabXdType.Terminator);
                                    }

                                    using (Transaction tr = acDb.TransactionManager.StartTransaction())
                                    {
                                        mkr = (Entity)tr.GetObject(mkrid, OpenMode.ForRead);
                                        terminator = new EsabTerminator();
                                        terminator.FromXdictionary(mkr);

                                        // if i is first, then this terminator node is already on RouteB, so assign RouteA
                                        // if i is last, then this terminator node is already on RouteA, so assign RouteB
                                        //
                                        if (i == 0)
                                        {
                                            terminator.routeA = pline.ObjectId;
                                        }
                                        else
                                        {
                                            terminator.routeB = pline.ObjectId;
                                        }

                                        mkr.UpgradeOpen();
                                        mkr.ColorIndex = 256;
                                        terminator.ToXdictionary(mkr);
                                        tr.Commit();
                                    }

                                }
                                else
                                {
                                    mkrid = gm.CreateTerminatorMarker(tm, 0.5, vPnt3);
                                    terminator = new EsabTerminator()
                                    {
                                        id = mkrid,
                                        type = EsabXdType.Terminator,
                                        routeA = pline.ObjectId,
                                        routeB = pline.ObjectId,
                                        terminatortype = tm
                                    };

                                    using (Transaction tr = acDb.TransactionManager.StartTransaction())
                                    {
                                        mkr = (Entity)tr.GetObject(mkrid, OpenMode.ForWrite);
                                        terminator.ToXdictionary(mkr);
                                        tr.Commit();
                                    }

                                    acEd.Regen();

                                }

                                break;

                            case EsabFeatureType.Junction:

                                EsabJunctionType jn = (EsabJunctionType)ed.GetEnumFromKeywords(typeof(EsabJunctionType), "\nSelect junction type: ");

                                // is junction selection a LinkTo
                                //
                                if (jn == EsabJunctionType.LinkTo)
                                {
                                    // select an existing junction feature
                                    //
                                    mkrid = ObjectId.Null;
                                    while (mkrid == ObjectId.Null)
                                    {
                                        mkrid = ed.SelectExistingMarker(EsabXdType.Junction);
                                    }

                                    using (Transaction tr = acDb.TransactionManager.StartTransaction())
                                    {
                                        mkr = (Entity)tr.GetObject(mkrid, OpenMode.ForRead);
                                        junction = new EsabJunction();
                                        junction.FromXdictionary(mkr);

                                        // if i is 0 or last, then this junction node is on branch
                                        // if i is mid, then this junction node is on main
                                        //
                                        if (i == 0 || i == vCount - 1)
                                        {
                                            junction.routebranch = pline.ObjectId;
                                        }
                                        else
                                        {
                                            junction.routemain = pline.ObjectId;
                                        }

                                        mkr.UpgradeOpen();
                                        mkr.ColorIndex = 256;
                                        junction.ToXdictionary(mkr);
                                        tr.Commit();   

                                    }
                                }
                                else
                                {
                                    mkrid = gm.CreateJunctionMarker(jn, 0.5, vPnt3);
                                    junction = new EsabJunction()
                                    {
                                        id = mkrid,
                                        type = EsabXdType.Junction,
                                        routemain = pline.ObjectId,
                                        routebranch = pline.ObjectId,
                                        junctiontype = jn
                                    };

                                    using (Transaction tr = acDb.TransactionManager.StartTransaction())
                                    {
                                        mkr = (Entity)tr.GetObject(mkrid, OpenMode.ForWrite);
                                        junction.ToXdictionary(mkr);
                                        tr.Commit();
                                    }

                                    acEd.Regen();

                                }

                                break;  

                            default:

                                mkrid = gm.CreateFeatureMarker(ft, 0.5, vPnt3);
                                feature = new EsabFeature()
                                {
                                    id = mkrid,
                                    type = EsabXdType.Feature,
                                    parentId = pline.ObjectId,
                                    parentVertex = i,
                                    featureType = ft
                                };

                                using (Transaction tr = acDb.TransactionManager.StartTransaction())
                                {
                                    mkr = (Entity)tr.GetObject(mkrid, OpenMode.ForWrite);
                                    feature.ToXdictionary(mkr);
                                    tr.Commit();
                                }

                                acEd.Regen();
                                
                            break;
                        }

                        route.featureIds[i] = mkrid;
                        acEd.WriteMessage(mkrid.ToString());
                        
                    }
                    
                        
                }

                using (Transaction tr = acDb.TransactionManager.StartTransaction())
                {
                    
                    DBObject dbo = (DBObject)tr.GetObject(route.id, OpenMode.ForWrite);
                    route.ToXdictionary(dbo);
                    tr.Commit();
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
