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
            // attdef constants
            //
            bool ADVis = false;
            bool ADVer = false;
            bool ADLoc = true;
            
            // create necessary block definitions for modelling
            //
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();
            GeometryMethods gm = new GeometryMethods();

            // create FND TEST
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("FNDTEST"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "FNDTEST";
                        btr.Origin = Point3d.Origin;

                        // create solid profile
                        //
                        Polyline profile = new Polyline();
                        profile.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(1, -1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-1, -1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-1, 1), 0, 0, 0);
                        profile.Closed = true;
                        profile.Elevation = 0.050;

                        Vector3d path = new Vector3d(0, 0, -0.5);

                        Solid3d extrusion = new Solid3d();
                        extrusion.SetDatabaseDefaults();
                        extrusion.CreateExtrudedSolid(profile, path, new SweepOptions());
                        extrusion.ColorIndex = 0;

                        btr.AppendEntity(extrusion);

                        using (AttributeDefinition attdef = new AttributeDefinition())
                        {
                            attdef.Verifiable = true;
                            //attdef.Constant = true;
                            attdef.Tag = "RightFrontTop";
                            attdef.TextString = "RFT";
                            attdef.Height = 0.2;
                            attdef.Position = new Point3d(1, 1, 0.050);

                            btr.AppendEntity(attdef);

                            tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                            bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);
                        }

                        tr.Commit();

                    }
                }
            }

            // create FND
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("FND"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "FND";
                        btr.Origin = Point3d.Origin;

                        // create solid profile
                        //
                        Polyline profile = new Polyline();
                        profile.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(1, -1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-1, -1), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-1, 1), 0, 0, 0);
                        profile.Closed = true;
                        profile.Elevation = 0.050;

                        Vector3d path = new Vector3d(0, 0, -0.5);

                        Solid3d extrusion = new Solid3d();
                        extrusion.SetDatabaseDefaults();
                        extrusion.CreateExtrudedSolid(profile, path, new SweepOptions());
                        extrusion.ColorIndex = 0;

                        btr.AppendEntity(extrusion);

                        using (AttributeDefinition attdef = new AttributeDefinition())
                        {
                            attdef.Verifiable = ADVer;
                            attdef.Visible = ADVis;
                            attdef.LockPositionInBlock = ADLoc;
                            //attdef.Constant = true;
                            attdef.Tag = "ATTPNT";
                            attdef.TextString = "ATTPNT";
                            attdef.Height = 0.2;
                            attdef.Position = new Point3d(0, 0, 0.050);

                            btr.AppendEntity(attdef);

                            Add4dCodeAttributes(btr);

                            tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                            bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);
                        }

                        
                        tr.Commit();

                    }
                }
            }

            // create SUP
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("SUP"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "SUP";
                        btr.Origin = Point3d.Origin;

                        // create solid profile for low support end
                        //
                        Polyline profile = new Polyline();
                        profile.AddVertexAt(0, new Point2d(0.5, 0.5), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(0.5, -0.5), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-0.5, -0.5), 0, 0, 0);
                        profile.AddVertexAt(0, new Point2d(-0.5, 0.5), 0, 0, 0);
                        profile.Closed = true;
                        profile.Elevation = 0.0;

                        Vector3d path = new Vector3d(0, 0, 0.1);

                        Solid3d baseplate = new Solid3d();
                        baseplate.SetDatabaseDefaults();
                        baseplate.CreateExtrudedSolid(profile, path, new SweepOptions());
                        baseplate.ColorIndex = 0;

                        btr.AppendEntity(baseplate);

                        // recycle profile for high support end
                        //
                        profile.Elevation = 2.0;
                        path = new Vector3d(0, 0, -0.1);

                        Solid3d topplate = new Solid3d();
                        topplate.SetDatabaseDefaults();
                        topplate.CreateExtrudedSolid(profile, path, new SweepOptions());
                        topplate.ColorIndex = 0;

                        btr.AppendEntity(topplate);

                        // create main support structure
                        //
                        Circle c = new Circle();
                        c.Center = new Point3d(0, 0, 0.1);
                        c.Radius = 0.25;
                        path = new Vector3d(0, 0, 1.8);
                        
                        Solid3d body = new Solid3d();
                        body.SetDatabaseDefaults();
                        body.CreateExtrudedSolid(c, path, new SweepOptions());
                        body.ColorIndex = 0;

                        btr.AppendEntity(body);


                        using (AttributeDefinition attdef = new AttributeDefinition())
                        {
                            attdef.Verifiable = ADVer;
                            attdef.Visible = ADVis;
                            attdef.LockPositionInBlock = ADLoc;
                            //attdef.Constant = true;
                            attdef.Tag = "ATTPNT";
                            attdef.TextString = "ATTPNT";
                            attdef.Height = 0.2;
                            attdef.Position = new Point3d(0, 0, 2);

                            btr.AppendEntity(attdef);

                            Add4dCodeAttributes(btr);

                            tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                            bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);

                        }

                        tr.Commit();
                    }
                }
                
            }

        }

        public void Add4dCodeAttributes(BlockTableRecord btr)
        {
            AttributeDefinition attdef;

            attdef = new AttributeDefinition();
            attdef.Verifiable = false;
            attdef.Visible = false;
            attdef.LockPositionInBlock = true;
            attdef.Tag = "4D_Region";
            attdef.TextString = "NON";
            attdef.Height = 0.2;
            attdef.Position = new Point3d(0, 0, 0);
            btr.AppendEntity(attdef);

            attdef = new AttributeDefinition();
            attdef.Verifiable = false;
            attdef.Visible = false;
            attdef.LockPositionInBlock = true;
            attdef.Tag = "4D_Area";
            attdef.TextString = "NON";
            attdef.Height = 0.2;
            attdef.Position = new Point3d(0, 0, 0);
            btr.AppendEntity(attdef);

            attdef = new AttributeDefinition();
            attdef.Verifiable = false;
            attdef.Visible = false;
            attdef.LockPositionInBlock = true;
            attdef.Tag = "4D_Zone";
            attdef.TextString = "NON";
            attdef.Height = 0.2;
            attdef.Position = new Point3d(0, 0, 0);
            btr.AppendEntity(attdef);

            attdef = new AttributeDefinition();
            attdef.Verifiable = false;
            attdef.Visible = false;
            attdef.LockPositionInBlock = true;
            attdef.Tag = "4D_Package";
            attdef.TextString = "NON";
            attdef.Height = 0.2;
            attdef.Position = new Point3d(0, 0, 0);
            btr.AppendEntity(attdef);

        }

    }
}
