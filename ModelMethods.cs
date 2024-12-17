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

            #region Model Method Constants

            // attdef constants
            //
            bool ADVis = false;
            bool ADVer = false;
            bool ADLoc = true;

            // model constants
            //
            double ElevationTier1 = 6.85;
            double ElevationTier2 = 11.25;
            double RadiusBusSTD = 0.1;
            double RadiusBusGIB = 0.2;
            double RadiusSupportA = 0.15;
            double RadiusSupportB = 0.20;
            double LengthEquipA = 3.0;
            double LengthEquipB = 4.0;
            double SizePlateEquipA = 0.6;
            double SizePlateEquipB = 0.8;
            double DepthPlate = 0.05;
            double ElevationTOC = 0.05;

            // create AttNamePos groups
            //
            AttNamePos[] FND = { new AttNamePos("ATTPNT", 0, 0, ElevationTOC) };
            AttNamePos[] SUP1A = { new AttNamePos("ATTPNT", 0, 0, ElevationTier1 - LengthEquipA - ElevationTOC) };
            AttNamePos[] SUP2A = { new AttNamePos("ATTPNT", 0, 0, ElevationTier2 - LengthEquipA - ElevationTOC) };
            AttNamePos[] SUP1B = { new AttNamePos("ATTPNT", 0, 0, ElevationTier1 - LengthEquipB - ElevationTOC) };
            AttNamePos[] SUP2B = { new AttNamePos("ATTPNT", 0, 0, ElevationTier2 - LengthEquipB - ElevationTOC) };
            AttNamePos[] PIB = { new AttNamePos("ATTPNT", 0, 0, LengthEquipB) };

            #endregion Model Method Constants


            // create necessary block definitions for modelling
            //
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();
            GeometryMethods gm = new GeometryMethods();


            #region Foundations

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

                        btr.AppendEntity(Foundation(2, 0.8, ElevationTOC));

                        AddNodeAttributes(btr, FND, true);
                        Add4dCodeAttributes(btr);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);
                        
                        tr.Commit();

                    }
                }
            }

            #endregion Foundations

            #region Supports

            // create SUP1A
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("SUP1A"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "SUP1A";
                        btr.Origin = Point3d.Origin;

                        double height = ElevationTier1 - LengthEquipA - ElevationTOC;
                        btr.AppendEntity(Support(SizePlateEquipA, DepthPlate, height, RadiusSupportA, Point2d.Origin));

                        AddNodeAttributes(btr, SUP1A, true);
                        Add4dCodeAttributes(btr);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);

                        tr.Commit();
                    }
                }
            }

            // create SUP1B
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("SUP1B"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "SUP1B";
                        btr.Origin = Point3d.Origin;

                        double height = ElevationTier1 - LengthEquipB - ElevationTOC;
                        btr.AppendEntity(Support(SizePlateEquipB, DepthPlate, height, RadiusSupportB, Point2d.Origin));

                        AddNodeAttributes(btr, SUP1B, true);
                        Add4dCodeAttributes(btr);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);

                        tr.Commit();
                    }
                }
            }

            // create SUP2A
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("SUP2A"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "SUP2A";
                        btr.Origin = Point3d.Origin;

                        double height = ElevationTier2 - LengthEquipA - ElevationTOC;
                        btr.AppendEntity(Support(SizePlateEquipA, DepthPlate, height, RadiusSupportA, Point2d.Origin));

                        AddNodeAttributes(btr, SUP2A, true);
                        Add4dCodeAttributes(btr);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);

                        tr.Commit();
                    }
                }
            }

            // create SUP2B
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("SUP2B"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "SUP2B";
                        btr.Origin = Point3d.Origin;

                        double height = ElevationTier2 - LengthEquipB - ElevationTOC;
                        btr.AppendEntity(Support(SizePlateEquipB, DepthPlate, height, RadiusSupportB, Point2d.Origin));

                        AddNodeAttributes(btr, SUP2B, true);
                        Add4dCodeAttributes(btr);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);

                        tr.Commit();
                    }
                }
            }


            #endregion Supports

            #region Equipment

            // create PI(A)
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("PIB"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "PIB";
                        btr.Origin = Point3d.Origin;

                        btr.AppendEntity(Insulator(3, 3.35, Point2d.Origin));
                        btr.AppendEntity(CylinderBasic(0.1, LengthEquipB - 0.2, Point2d.Origin, 0.1));
                        btr.AppendEntity(CylinderBasic(0.2, 0.1, Point2d.Origin, 0.0));
                        
                        Solid3d frustum = new Solid3d();
                        frustum.CreateFrustum(0.3, 0.35, 0.35, 0.1);
                        Matrix3d mat = new Matrix3d();
                        mat = Matrix3d.Displacement(new Vector3d(0, 0, LengthEquipB - 0.25));
                        frustum.TransformBy(mat);
                        frustum.ColorIndex = 0;

                        btr.AppendEntity(frustum);

                        AddNodeAttributes(btr, PIB, true);
                        Add4dCodeAttributes(btr);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);

                        tr.Commit();
                    }
                }
            }






            #endregion Equipment




        }

        #region Model Build Methods

        public ObjectId InsertModelFeatureGroup(string GroupNameBase, string[] FeatureList, Point3d placement, double orientation)
        {
            // chain a string of blockrefs from lastAttPnt to ThisInsPnt
            // add all to a new group, return the objId ofthe group
            //
            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;
            ObjectId returnId = new ObjectId();

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                DBDictionary gd = (DBDictionary)tr.GetObject(acDb.GroupDictionaryId, OpenMode.ForRead);

                int q = 1;
                while (gd.Contains(GroupNameBase + q.ToString()))
                {
                    q++;
                }
                string gname = GroupNameBase + q.ToString();

                Group grp = new Group(gname, true);

                gd.UpgradeOpen();
                ObjectId grpId = gd.SetAt(gname, grp);
                tr.AddNewlyCreatedDBObject(grp, true);

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelspace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                ObjectIdCollection blockRefIds = new ObjectIdCollection();
                ObjectId blockRefId = new ObjectId();

                Point3d currentAttPnt = placement;

                for (int i = 0; i < FeatureList.Length; i++)
                {
                    if (bt.Has(FeatureList[i]))
                    {
                        BlockTableRecord blockDef = (BlockTableRecord)bt[FeatureList[i]].GetObject(OpenMode.ForRead);
                        
                        using (BlockReference blockRef = new BlockReference(currentAttPnt, blockDef.ObjectId))
                        {

                            switch (i) // assign layer
                            {
                                case 0:
                                    blockRef.Layer = "_Esab_Model_Foundations";
                                    break;
                                case 1:
                                    blockRef.Layer = "_Esab_Model_Supports";
                                    break;
                                case 2:
                                    blockRef.Layer = "_Esab_Model_Equipment";
                                    break;
                                default:
                                    blockRef.Layer = "_Esab_Model_General";
                                    break;
                            }

                            blockRef.Rotation = orientation;
                            blockRefId = modelspace.AppendEntity(blockRef);
                            tr.AddNewlyCreatedDBObject(blockRef, true);

                            // transfer attribute definitions into new block
                            //
                            foreach (ObjectId id in blockDef)
                            {
                                DBObject obj = id.GetObject(OpenMode.ForRead);
                                AttributeDefinition attdef = obj as AttributeDefinition;

                                if ((attdef != null) && (!attdef.Constant))
                                {
                                    using (AttributeReference attref = new AttributeReference())
                                    {
                                        attref.SetAttributeFromBlock(attdef, blockRef.BlockTransform);
                                        blockRef.AttributeCollection.AppendAttribute(attref);
                                        tr.AddNewlyCreatedDBObject(attref, true);
                                    }
                                }
                            }

                            // get the new atribute references
                            // and get 3d location of ATTPNT
                            //
                            AttributeCollection attcol = blockRef.AttributeCollection;
                            foreach (ObjectId attId in attcol)
                            {
                                AttributeReference attref = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                                if (attref.Tag == "ATTPNT")
                                    currentAttPnt = attref.Position;
                            }

                            blockRefIds.Add(blockRefId);

                        }
                    }
                }
            
                grp.InsertAt(0, blockRefIds);
                returnId = grp.ObjectId;
                tr.Commit();

            }

            return returnId;
        }

        public ObjectId BuildSinglePhaseRoute( string GroupNameBase, EsabRoute route, Mline mline)
        {
            return ObjectId.Null;
        }



        #endregion Model Build Methods



        #region Helper Functions

        public void AddNodeAttributes(BlockTableRecord btr, AttNamePos[] anps, bool IncludePointGeometry)
        {
            AttributeDefinition attdef;
            DBPoint pnt;

            foreach (AttNamePos anp in anps)
            {
            attdef = new AttributeDefinition();
            attdef.Verifiable = false;
            attdef.Visible = false;
            attdef.LockPositionInBlock = true;
            attdef.Tag = anp.AttName;
            attdef.TextString = anp.AttName;
            attdef.Height = 0.2;
            attdef.Position = anp.AttPos;
            btr.AppendEntity(attdef);

            if (IncludePointGeometry)
                {
                    pnt = new DBPoint(anp.AttPos);
                    btr.AppendEntity(pnt);
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

        public Solid3d Insulator(double length, double topElevation, Point2d columnPosition)
        {
            double partlength = 0.25;
            double radiusOut = 0.2;
            double radiusIn = 0.16;

            bool IsBig = true; // flip between big and small cylider components per cycle

            Solid3d insulator = new Solid3d();
            Solid3d temp;
            Vector3d path = new Vector3d(0, 0, -partlength);
            Circle c = new Circle();

            for (double h = topElevation; h > topElevation - length; h-= partlength)
            {
                c.Radius = (IsBig) ? radiusOut : radiusIn;
                c.Center = new Point3d(columnPosition.X, columnPosition.Y, h);

                temp = new Solid3d();
                temp.CreateExtrudedSolid(c, path, new SweepOptions());

                insulator.BooleanOperation(BooleanOperationType.BoolUnite, temp);

                IsBig = !IsBig;
            }

            insulator.ColorIndex = 14;

            return insulator;
        }

        public Solid3d CylinderBasic(double radius, double height, Point2d columnPosition, double baseElevation)
        {
            Solid3d cylinderbasic = new Solid3d();
            Vector3d path;
            
            Circle circ = new Circle();
            circ.Center = new Point3d(columnPosition.X, columnPosition.Y, baseElevation);
            circ.Normal = Vector3d.ZAxis;
            circ.Radius = radius;

            path = new Vector3d(0, 0, height);

            cylinderbasic.CreateExtrudedSolid(circ, path, new SweepOptions());

            cylinderbasic.ColorIndex = 0;

            return cylinderbasic;
        }

        public Solid3d Foundation(double size, double depth, double toc)
        {
            double sz = size / 2;

            // create solid profile
            //
            Polyline profile = new Polyline();
            profile.AddVertexAt(0, new Point2d(sz, sz), 0, 0, 0);
            profile.AddVertexAt(0, new Point2d(sz, -sz), 0, 0, 0);
            profile.AddVertexAt(0, new Point2d(-sz, -sz), 0, 0, 0);
            profile.AddVertexAt(0, new Point2d(-sz, sz), 0, 0, 0);
            profile.Closed = true;
            profile.Elevation = toc;

            // create solid path
            //
            Vector3d path = new Vector3d(0, 0, -depth);

            Solid3d extrusion = new Solid3d();
            extrusion.SetDatabaseDefaults();
            extrusion.CreateExtrudedSolid(profile, path, new SweepOptions());
            extrusion.ColorIndex = 0;

            return extrusion;
        }

        public Solid3d Support(double plateSize, double plateDepth, double columnHeight, double columnRadius, Point2d columnPosition)
        {
            double sz = plateSize / 2;
            double cpx = columnPosition.X;
            double cpy = columnPosition.Y;

            Solid3d support = new Solid3d();
            Solid3d temp;

            // create plate low
            //
            Polyline profile = new Polyline();
            profile.AddVertexAt(0, new Point2d(cpx + sz, cpy + sz), 0, 0, 0);
            profile.AddVertexAt(0, new Point2d(cpx + sz, cpy - sz), 0, 0, 0);
            profile.AddVertexAt(0, new Point2d(cpx - sz, cpy - sz), 0, 0, 0);
            profile.AddVertexAt(0, new Point2d(cpx - sz, cpy + sz), 0, 0, 0);
            profile.Closed = true;
            profile.Elevation = 0;

            Vector3d path = new Vector3d(0, 0, plateDepth);

            temp = new Solid3d();
            temp.CreateExtrudedSolid(profile, path, new SweepOptions());

            support.BooleanOperation(BooleanOperationType.BoolUnite, temp);

            // create plate high
            //
            profile.Elevation = columnHeight;
            path = new Vector3d(0, 0, -plateDepth);

            temp = new Solid3d();
            temp.CreateExtrudedSolid(profile, path, new SweepOptions());

            support.BooleanOperation(BooleanOperationType.BoolUnite, temp);

            // create column
            //
            Circle circ = new Circle();
            circ.Center = new Point3d(cpx, cpy, plateDepth);
            circ.Normal = Vector3d.ZAxis;
            circ.Radius = columnRadius;

            path = new Vector3d(0, 0, columnHeight - (2 * plateDepth));

            temp = new Solid3d();
            temp.CreateExtrudedSolid(circ, path, new SweepOptions());

            support.BooleanOperation(BooleanOperationType.BoolUnite, temp);

            support.ColorIndex = 0;

            return support;

        }

 
        #endregion Helper Functions

 
        public struct AttNamePos
        {
            // container for attribute name and 2d location
            // used in AddNodeAttributes helper function
            //
            public string AttName;
            public Point3d AttPos;

            public AttNamePos(string name, double x, double y, double z)
            {
                this.AttName = name;
                this.AttPos = new Point3d(x, y, z);
            }


        }

    }
}
