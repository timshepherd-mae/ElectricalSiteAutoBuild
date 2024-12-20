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
            AttNamePos[] PIB = {
                new AttNamePos("NODE_0_L", 0, 0, LengthEquipB),
                new AttNamePos("NODE_0_M", 0, 0, LengthEquipB),
                new AttNamePos("NODE_0_R", 0, 0, LengthEquipB),
                new AttNamePos("NODE_1_L", 0, 0, LengthEquipB),
                new AttNamePos("NODE_1_M", 0, 0, LengthEquipB),
                new AttNamePos("NODE_1_R", 0, 0, LengthEquipB),
            };


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
                appliedTo.Add("AcDb3dSolid");

                codeset4d.SetAppliesToFilter(appliedTo, isStyle);

                PropertyDefinition c4d_region = new PropertyDefinition();
                c4d_region.SetToStandard(acDb);
                c4d_region.SubSetDatabaseDefaults(acDb);
                c4d_region.Name = "4D (1) Region";
                c4d_region.Description = "Region code for 4d coding";
                c4d_region.DataType = Autodesk.Aec.PropertyData.DataType.Text;
                c4d_region.DefaultData = "NON";

                codeset4d.Definitions.Add(c4d_region);

                PropertyDefinition c4d_area = new PropertyDefinition();
                c4d_area.SetToStandard(acDb);
                c4d_area.SubSetDatabaseDefaults(acDb);
                c4d_area.Name = "4D (2) Area";
                c4d_area.Description = "Area code for 4d coding";
                c4d_area.DataType = Autodesk.Aec.PropertyData.DataType.Text;
                c4d_area.DefaultData = "NON";

                codeset4d.Definitions.Add(c4d_area);

                PropertyDefinition c4d_zone = new PropertyDefinition();
                c4d_zone.SetToStandard(acDb);
                c4d_zone.SubSetDatabaseDefaults(acDb);
                c4d_zone.Name = "4D (3) Zone";
                c4d_zone.Description = "Zone code for 4d coding";
                c4d_zone.DataType = Autodesk.Aec.PropertyData.DataType.Text;
                c4d_zone.DefaultData = "NON";

                codeset4d.Definitions.Add(c4d_zone);

                PropertyDefinition c4d_package = new PropertyDefinition();
                c4d_package.SetToStandard(acDb);
                c4d_package.SubSetDatabaseDefaults(acDb);
                c4d_package.Name = "4D (4) Package";
                c4d_package.Description = "Package code for 4d coding";
                c4d_package.DataType = Autodesk.Aec.PropertyData.DataType.Text;
                c4d_package.DefaultData = "NON";

                codeset4d.Definitions.Add(c4d_package);

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

        public Point3d[] InsertModelFeatureSet(string[] PartsList, bool IsThreePhaseItem, Point3d placement, Vector3d pathDirection, EsabRoute route, string zone4d)
        {
            // chain a string of blockrefs from lastAttPnt to ThisInsPnt
            // add all to a new group, return the objId ofthe group
            //
            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;
            Point3d[] EndPointSet = { Point3d.Origin, Point3d.Origin, Point3d.Origin, Point3d.Origin, Point3d.Origin, Point3d.Origin };
            DBObject obj;
            string assignPack4d = "NON";
            string assignZone4d; 

            double orientation = AngleFromX(pathDirection);

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelspace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                ObjectIdCollection blockRefIds = new ObjectIdCollection();
                ObjectId blockRefId = new ObjectId();

                
                Point3d currentAttPnt = placement;
                
// entity build
                // cycle through the vertical parts list for the feature
                //
                for (int i = 0; i < PartsList.Length; i++)
                {
                    if (bt.Has(PartsList[i]))
                    {
                        BlockTableRecord blockDef = (BlockTableRecord)bt[PartsList[i]].GetObject(OpenMode.ForRead);

                        using (BlockReference blockRef = new BlockReference(currentAttPnt, blockDef.ObjectId))
                        {

                            switch (i) // assign layer
                            {
                                case 0:
                                    blockRef.Layer = "_Esab_Model_Foundations";
                                    assignPack4d = "FND";
                                    assignZone4d = zone4d;
                                    break;
                                case 1:
                                    blockRef.Layer = "_Esab_Model_Supports";
                                    assignPack4d = "SUP";
                                    assignZone4d = (Constants.SupportsByArea) ? "SUP" : zone4d;
                                    break;
                                case 2:
                                    blockRef.Layer = "_Esab_Model_Equipment";
                                    assignPack4d = "EQU";
                                    assignZone4d = zone4d;
                                    break;
                                default:
                                    blockRef.Layer = "_Esab_Model_General";
                                    assignPack4d = "NON";
                                    assignZone4d = zone4d;
                                    break;
                            }

                            blockRef.Rotation = orientation;
                            blockRefId = modelspace.AppendEntity(blockRef);
                            tr.AddNewlyCreatedDBObject(blockRef, true);

                            // transfer attribute definitions into new block
                            //
                            AttDefTransfer(tr, blockDef, blockRef);

                            obj = blockRef as DBObject;

                            ApplyCODESET4D(acDb, tr, obj);
                            UpdateCODESET4D(acDb, tr, obj, route.codelist4D_region, route.codelist4D_area, assignZone4d, assignPack4d);

                            // get the new atribute references
                            // and get 3d location of ATTPNT
                            //
                            AttributeCollection attcol = blockRef.AttributeCollection;
                            foreach (ObjectId attId in attcol)
                            {
                                AttributeReference attref = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                                if (attref.Tag == "ATTPNT") currentAttPnt = attref.Position;
                                if (attref.Tag == "NODE_0_L") EndPointSet[0] = attref.Position;
                                if (attref.Tag == "NODE_0_M") EndPointSet[1] = attref.Position;
                                if (attref.Tag == "NODE_0_R") EndPointSet[2] = attref.Position;
                                if (attref.Tag == "NODE_1_L") EndPointSet[3] = attref.Position;
                                if (attref.Tag == "NODE_1_M") EndPointSet[4] = attref.Position;
                                if (attref.Tag == "NODE_1_R") EndPointSet[5] = attref.Position;
                            }

                            blockRefIds.Add(blockRefId);

                        }
                    }
                }
// entity build
                tr.Commit();

            }


            return EndPointSet;

        }

        public void InsertConductor(Point3d start,  Point3d end, Color color, EsabRoute route)
        {
            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;
            DBObject obj;

            // create conductor and add to db
            //
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelspace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                Solid3d Busbar = CylinderTargetted(RadiusBusSTD, start, end, false);
                Busbar.Color = color;
                obj = Busbar as DBObject;
                modelspace.AppendEntity(Busbar);
                tr.AddNewlyCreatedDBObject(Busbar, true);

                tr.Commit();
            }

            ApplyCODESET4D(obj);
            UpdateCODESET4D(obj, route.codelist4D_region, route.codelist4D_area, "CND", "CON");

        }

        public ObjectId BuildSinglePhaseRoute(EsabRoute route, Mline mline)
        {

            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;

            Vector3d pathDirection;
            Point3d currentPoint;
            ObjectId currentFeatureId;
            EsabFeature currentFeature = new EsabFeature();
            Entity currentFeatureEntity;
            ResultBuffer rb;

            Dictionary<string, Color> phasecolours = new Dictionary<string, Color>()
            {
                { "R", Color.FromRgb(255, 190, 190) },
                { "Y", Color.FromRgb(255, 255, 190) },
                { "B", Color.FromRgb(190, 190, 255) }
            };


            //phasecolours.Add("R", Color.FromRgb(255, 190, 190));


            List<Point3d[]> EndPointSetCollection = new List<Point3d[]>();
            
            // cycle through featureIds / vertices
            //
            for (int i = 0; i < route.featureIds.Count; i++)
            {

                // get path vector from vertices i-1 >> i
                // or from 0 >> 1 if at start
                //
                if (i == 0)
                {
                    pathDirection = mline.VertexAt(1) - mline.VertexAt(0);
                }
                else
                {
                    pathDirection = mline.VertexAt(i) - mline.VertexAt(i-1);
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
                            EndPointSetCollection.Add(InsertModelFeatureSet(new string[] { "FND", "SUP1B", "PIB" }, false, currentPoint, pathDirection, route, "PI"));
                        }
                    }

                    tr.Commit();

                }
            }
            
            for (int i = 1; i < EndPointSetCollection.Count; i++)
            {
                InsertConductor(EndPointSetCollection[i - 1][4], EndPointSetCollection[i][1], phasecolours[Enum.GetName(typeof(EsabPhaseColour), route.phasecol)], route);
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

        public void AttDefTransfer(Transaction tr, BlockTableRecord blockDef, BlockReference blockRef)
        {
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
        }

        public void ApplyCODESET4D(DBObject obj)
        {
            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                DictionaryPropertySetDefinitions dictPsd = new DictionaryPropertySetDefinitions(acDb);
                ObjectId psetDefId = dictPsd.GetAt("CODESET4D");
                try
                {
                    tr.GetObject(obj.ObjectId, OpenMode.ForWrite);
                    PropertyDataServices.AddPropertySet(obj, psetDefId);

                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
                }
                //PropertyDataServices.AddPropertySet(obj, psetDefId);
                tr.Commit();

            }
        }

        public void ApplyCODESET4D(Database acDb, Transaction tr, DBObject obj)
        {
            DictionaryPropertySetDefinitions dictPsd = new DictionaryPropertySetDefinitions(acDb);
            ObjectId psetDefId = dictPsd.GetAt("CODESET4D");
            try
            {
                tr.GetObject(obj.ObjectId, OpenMode.ForWrite);
                PropertyDataServices.AddPropertySet(obj, psetDefId);

            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
            }

        }

        public void UpdateCODESET4D(DBObject obj, string rg, string ar, string zn, string pk)
        {
            Database acDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                DictionaryPropertySetDefinitions dictPsd = new DictionaryPropertySetDefinitions(acDb);
                ObjectId psetDefId = dictPsd.GetAt("CODESET4D");

                try
                {
                    //tr.GetObject(obj.ObjectId, OpenMode.ForWrite);
                    ObjectId objPropSetId = PropertyDataServices.GetPropertySet(obj, psetDefId);
                    PropertySet propset = (PropertySet)objPropSetId.GetObject(OpenMode.ForWrite);
                    propset.SetAt(0, rg);
                    propset.SetAt(1, ar);
                    propset.SetAt(2, zn);
                    propset.SetAt(3, pk);


                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
                }

                tr.Commit();

            }

        }

        public void UpdateCODESET4D(Database acDb, Transaction tr, DBObject obj, string rg, string ar, string zn, string pk)
        {
            DictionaryPropertySetDefinitions dictPsd = new DictionaryPropertySetDefinitions(acDb);
            ObjectId psetDefId = dictPsd.GetAt("CODESET4D");

            try
            {
                //tr.GetObject(obj.ObjectId, OpenMode.ForWrite);
                ObjectId objPropSetId = PropertyDataServices.GetPropertySet(obj, psetDefId);
                PropertySet propset = (PropertySet)objPropSetId.GetObject(OpenMode.ForWrite);
                propset.SetAt(0, rg);
                propset.SetAt(1, ar);
                propset.SetAt(2, zn);
                propset.SetAt(3, pk);

            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
            }

        }


        public double AngleFromX(Vector3d v)
        {
            double mag = v.Length;
            double baseAng = (v.Y < 0) ? (2 * Math.PI) - Math.Acos(v.X / mag) : Math.Acos(v.X / mag);
            return baseAng;
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

    }
}
