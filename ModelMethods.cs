using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.Aec.PropertyData.DatabaseServices;
using System.Collections.Specialized;

namespace ElectricalSiteAutoBuild
{
    public class ModelMethods
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
        

        #endregion Model Method Constants

        public void InitialiseModels()
        {


            // create AttNamePos groups
            //
            AttNamePos[] FND = { new AttNamePos("ATTPNT", 0, 0, ElevationTOC) };
            AttNamePos[] SUP1A = { new AttNamePos("ATTPNT", 0, 0, ElevationTier1 - LengthEquipA - ElevationTOC) };
            AttNamePos[] SUP2A = { new AttNamePos("ATTPNT", 0, 0, ElevationTier2 - LengthEquipA - ElevationTOC) };
            AttNamePos[] SUP1B = { new AttNamePos("ATTPNT", 0, 0, ElevationTier1 - LengthEquipB - ElevationTOC) };
            AttNamePos[] SUP2B = { new AttNamePos("ATTPNT", 0, 0, ElevationTier2 - LengthEquipB - ElevationTOC) };
            AttNamePos[] PIB = { new AttNamePos("ENDPNT", 0, 0, LengthEquipB) };


            // create necessary block definitions for modelling
            //
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();
            GeometryMethods gm = new GeometryMethods();


            #region PropertySet Definitions

            // create propset definition for 4d codesets
            //
            try
            {
                PropertySetDefinition codeset4d = new PropertySetDefinition();
                codeset4d.SetToStandard(acDb);
                codeset4d.SubSetDatabaseDefaults(acDb);
                codeset4d.Description = "CodeSet to handle 4d code values";

                bool isStyle = false;
                StringCollection appliedTo = new StringCollection();
                appliedTo.Add("AcDbBlockReference");

                codeset4d.SetAppliesToFilter(appliedTo, isStyle);

                PropertyDefinition c4d_region = new PropertyDefinition();
                c4d_region.SetToStandard(acDb);
                c4d_region.SubSetDatabaseDefaults(acDb);
                c4d_region.Name = "4D_Region";
                c4d_region.Description = "Region code for 4d coding";
                c4d_region.DataType = Autodesk.Aec.PropertyData.DataType.Text;
                c4d_region.DefaultData = "test";

                codeset4d.Definitions.Add(c4d_region);

                using (Transaction tr = acDb.TransactionManager.StartTransaction())
                {
                    DictionaryPropertySetDefinitions dictPsetDef = new DictionaryPropertySetDefinitions(acDb);
                    if (dictPsetDef.Has("CODESET4D", tr))
                        return;

                    dictPsetDef.AddNewRecord("CODESET4D", codeset4d);
                    tr.AddNewlyCreatedDBObject(codeset4d, true);
                    tr.Commit();
                }

            }
            catch
            {
                return;
            }


            #endregion PropertySet Definitions


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

            // create PI(B)
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

        public Point3d InsertModelFeatureSet(string[] FeatureList, Point3d placement, double orientation, EsabRoute route, string zone4d)
        {
            // chain a string of blockrefs from lastAttPnt to ThisInsPnt
            // add all to a new group, return the objId ofthe group
            //
            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;
            Point3d EndPoint = new Point3d();

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

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

                                        if (attref.Tag == "4D_Region") attref.TextString = route.codelist4D_region;
                                        if (attref.Tag == "4D_Area") attref.TextString = route.codelist4D_area;
                                        if (attref.Tag == "4D_Zone") attref.TextString = (i == 1) ? "SUP" : zone4d;
                                        if (attref.Tag == "4D_Package") attref.TextString = (i == 0) ? "FND" : (i == 1) ? "SUP" : (i == 2) ? "EQU" : "NON";

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
                                if (attref.Tag == "ENDPNT")
                                    EndPoint = attref.Position;
                            }

                            blockRefIds.Add(blockRefId);

                        }
                    }
                }

                tr.Commit();

            }

            return EndPoint;

        }

        public void CreateInsertBusbar(string NameBase, Point3d start, Point3d end, Color color, EsabRoute route)
        {

            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;
            ObjectId blockDefId = new ObjectId();
            ObjectId blockRefId = new ObjectId();
            string BlockName;


            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                BlockName = NextFreeBlockName(bt, NameBase);

                // create unique busbar block definition
                //
                using (BlockTableRecord blockDef = new BlockTableRecord())
                {
                    blockDef.Name = BlockName;
                    blockDef.Origin = Point3d.Origin;

                    Solid3d Busbar = CylinderTargetted(RadiusBusSTD, start, end, true);
                    Busbar.ColorIndex = 0;
                    blockDef.AppendEntity(Busbar);

                    Add4dCodeAttributes(blockDef);

                    tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                    blockDefId = bt.Add(blockDef);
                    tr.AddNewlyCreatedDBObject(blockDef, true);
                }

                tr.Commit();
            }

            // insert block reference
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelspace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                BlockTableRecord blockDef = (BlockTableRecord)bt[BlockName].GetObject(OpenMode.ForRead);

                using (BlockReference blockRef = new BlockReference(start, blockDef.ObjectId))
                {
                    blockRef.Layer = "_Esab_Model_Conductors";
                    blockRef.Color = color;
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

                                if (attref.Tag == "4D_Region") attref.TextString = route.codelist4D_region;
                                if (attref.Tag == "4D_Area") attref.TextString = route.codelist4D_area;
                                if (attref.Tag == "4D_Zone") attref.TextString = "CON";
                                if (attref.Tag == "4D_Package") attref.TextString = Enum.GetName(typeof(EsabConductorType), route.defaultConductorType);

                                blockRef.AttributeCollection.AppendAttribute(attref);
                                tr.AddNewlyCreatedDBObject(attref, true);
                            }
                        }

                    }

                }

                tr.Commit();

            }

        }


        public ObjectId BuildSinglePhaseRoute( string GroupNameBase, EsabRoute route, Mline mline)
        {

            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;

            Vector3d pathDirection;
            Double pathOrientation;
            Point3d currentPoint;
            ObjectId currentFeatureId;
            EsabFeature currentFeature = new EsabFeature();
            Entity currentFeatureEntity;
            ResultBuffer rb;

            List<Point3d> EndPoints = new List<Point3d>();
            
            // cycle through featureIds / vertices
            //
            for (int i = 0; i < route.featureIds.Count; i++)
            {

                // get path vector from vertices i-1 >> i
                // or from 0 >> 1 if at start
                //
                if (i == 0)
                {
                    pathDirection = mline.VertexAt(0) - mline.VertexAt(1);
                    pathOrientation = pathDirection.GetAngleTo(Vector3d.XAxis);
                }
                else
                {
                    pathDirection = mline.VertexAt(i-1) - mline.VertexAt(i);
                    pathOrientation = pathDirection.GetAngleTo(Vector3d.XAxis);
                }

                currentPoint = mline.VertexAt(i);
                currentFeatureId = route.featureIds[i];

                // get the feature and populate an EsabFeature class
                //
                using (Transaction tr = acDb.TransactionManager.StartTransaction())
                {
                    currentFeatureEntity = (Entity)tr.GetObject(currentFeatureId, OpenMode.ForRead);
                    rb = currentFeatureEntity.GetXDictionaryXrecordData(Constants.XappName);
                    var data = rb.AsArray();
                    EsabXdType type = (EsabXdType)Enum.ToObject(typeof(EsabXdType), data[1].Value);

                    // check that feature xdType is 'feature'
                    // TODO encapsulate all xdTypes in switch
                    //
                    if (type == EsabXdType.Feature)
                    {
                        currentFeature.FromXdictionary(currentFeatureEntity);

                        if (currentFeature.featureType == EsabFeatureType.PI)
                        {
                            EndPoints.Add(InsertModelFeatureSet(new string[] { "FND", "SUP1B", "PIB" }, currentPoint, pathOrientation, route, "PI"));
                        }
                    }

                    tr.Commit();

                }
            }
            
            for (int i = 1; i < EndPoints.Count; i++)
            {
                CreateInsertBusbar("BUS", EndPoints[i - 1], EndPoints[i], Color.FromRgb(255, 190, 190), route);
            }
            
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
            attdef.TextString = "0";
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

        public string NextFreeBlockName(BlockTable bt, string nameBase, int startFrom = 1)
        {
            // find next numbers name that does not already exist in the blocktable
            //
            int current = startFrom;
            while (bt.Has(nameBase + current.ToString())) { current++; }
            return nameBase + current.ToString();
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

        public Solid3d CylinderTargetted(double radius, Point3d start, Point3d end, bool Normalised)
        {
            Solid3d cylindertargetted = new Solid3d();
            Vector3d path = end - start;

            Circle circ = new Circle();
            circ.Center = (Normalised) ? Point3d.Origin : start;
            circ.Normal = path;
            circ.Radius = radius;

            cylindertargetted.CreateExtrudedSolid(circ, path, new SweepOptions());

            cylindertargetted.ColorIndex = 0;

            return cylindertargetted;
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

        public struct CodeList4d
        {
            public string region;
            public string area;
            public string zone;
            public string package;
        }
        

    }
}
