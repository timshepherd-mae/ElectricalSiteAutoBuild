using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;

namespace ElectricalSiteAutoBuild
{
    public class ModelMethods
    {
        public void InitialiseModels()
        {
            // create necessary block definitions for modelling
            //
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();
            GeometryMethods gm = new GeometryMethods();

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("TESTMODEL"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "TESTMODEL";
                        btr.Origin = Point3d.Origin;

                        // create solid profile
                        //
                        Polyline profile = new Polyline();
                        profile.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(1, -1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-1, -1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-1, 1), 0, 0, 0);
                        profile.Closed = true;
                        profile.Elevation = 0.25;

                        Vector3d path = new Vector3d(0, 0, -0.5);

                        Solid3d extrusion = new Solid3d();
                        extrusion.SetDatabaseDefaults();
                        extrusion.CreateExtrudedSolid(profile, path, new SweepOptions());

                        btr.AppendEntity(extrusion);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);
                        tr.Commit();


                    }
                }
            }
        }
    }
}
