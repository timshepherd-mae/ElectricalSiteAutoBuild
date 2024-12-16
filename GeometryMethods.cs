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
    public class GeometryMethods
    {
        public void InitialiseGeometry()
        {
            // create necessary block definitions for feature markers
            //
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            EditorMethods ed = new EditorMethods();
            GeometryMethods gm = new GeometryMethods();

            #region Layers

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tr.GetObject(acDb.LayerTableId, OpenMode.ForWrite);
                LinetypeTable ltt = (LinetypeTable)tr.GetObject(acDb.LinetypeTableId, OpenMode.ForRead);

                ObjectId RouteLT = (ltt.Has("Dashed")) ? ltt["Dashed"] : ltt["Continuous"];

                if (!lt.Has("_Esab_Features"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Features";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 3);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight025;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);

                    }
                }

                if (!lt.Has("_Esab_Junctions"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Junctions";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight025;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);

                    }
                }

                if (!lt.Has("_Esab_Terminators"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Terminators";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight025;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);

                    }
                }

                if (!lt.Has("_Esab_Routes"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Routes";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 7);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight025;
                        lyr.LinetypeObjectId = RouteLT;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);

                    }
                }

                if (!lt.Has("_Esab_Model"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Model";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 7);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight025;
                        lyr.LinetypeObjectId = RouteLT;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);

                    }
                }

                tr.Commit();
            }

            #endregion Layers

            #region Marker Blocks

            string[] FeatureBlockNames = Enum.GetNames(typeof(EsabFeatureType));
            foreach (string featureBlockName in FeatureBlockNames)
            {
                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                    if (!bt.Has("ESAB" + featureBlockName) && !featureBlockName.Contains("_"))
                    {
                        using (BlockTableRecord btr = new BlockTableRecord())
                        {
                            btr.Name = "ESAB" + featureBlockName;
                            btr.Origin = Point3d.Origin;

                            Circle c = new Circle()
                            {
                                Radius = 0.5,
                                Center = btr.Origin,
                                ColorIndex = 0,
                                LineWeight = LineWeight.ByBlock
                            };
                            btr.AppendEntity(c);

                            Line l1 = new Line()
                            {
                                StartPoint = new Point3d(0.5, 0.5, 0.0),
                                EndPoint = new Point3d(-0.5, -0.5, 0.0),
                                ColorIndex = 0,
                                LineWeight = LineWeight.ByBlock
                            };
                            btr.AppendEntity(l1);

                            Line l2 = new Line()
                            {
                                StartPoint = new Point3d(-0.5, 0.5, 0.0),
                                EndPoint = new Point3d(0.5, -0.5, 0.0),
                                ColorIndex = 0,
                                LineWeight = LineWeight.ByBlock
                            };
                            btr.AppendEntity(l2);

                            DBText t = new DBText()
                            {
                                TextString = featureBlockName,
                                Height = 0.5,
                                VerticalMode = TextVerticalMode.TextBottom,
                                HorizontalMode = TextHorizontalMode.TextLeft,
                                AlignmentPoint = new Point3d(0.5, 0.5, 0.0),
                                ColorIndex = 0
                            };
                            btr.AppendEntity(t);

                            tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                            bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);
                            tr.Commit();
                        }
                    }
                }
            }

            string[] TerminatorBlockNames = Enum.GetNames(typeof(EsabTerminatorType));
            foreach (string terminatorBlockName in TerminatorBlockNames)
            {
                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                    if (!bt.Has("ESAB" + terminatorBlockName) && !terminatorBlockName.Contains("_"))
                    {
                        using (BlockTableRecord btr = new BlockTableRecord())
                        {
                            btr.Name = "ESAB" + terminatorBlockName;
                            btr.Origin = Point3d.Origin;

                            Polyline pl = new Polyline();
                            pl.AddVertexAt(0, new Point2d(0.0, 0.6), 0, 0, 0);
                            pl.AddVertexAt(0, new Point2d(0.6, 0.0), 0, 0, 0);
                            pl.AddVertexAt(0, new Point2d(0.0, -0.6), 0, 0, 0);
                            pl.AddVertexAt(0, new Point2d(-0.6, 0.0), 0, 0, 0);
                            pl.Closed = true;
                            pl.ColorIndex = 0;
                            pl.LineWeight = LineWeight.ByBlock;
                            btr.AppendEntity(pl);

                            Line l1 = new Line()
                            {
                                StartPoint = new Point3d(0.5, 0.5, 0.0),
                                EndPoint = new Point3d(-0.5, -0.5, 0.0),
                                ColorIndex = 0,
                                LineWeight = LineWeight.ByBlock
                            };
                            btr.AppendEntity(l1);

                            Line l2 = new Line()
                            {
                                StartPoint = new Point3d(-0.5, 0.5, 0.0),
                                EndPoint = new Point3d(0.5, -0.5, 0.0),
                                ColorIndex = 0,
                                LineWeight = LineWeight.ByBlock
                            };
                            btr.AppendEntity(l2);

                            DBText t = new DBText()
                            {
                                TextString = terminatorBlockName,
                                Height = 0.5,
                                VerticalMode = TextVerticalMode.TextBottom,
                                HorizontalMode = TextHorizontalMode.TextLeft,
                                AlignmentPoint = new Point3d(0.5, 0.5, 0.0),
                                ColorIndex = 0
                            };
                            btr.AppendEntity(t);

                            tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                            bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);
                            tr.Commit();
                        }
                    }
                }
            }

            string[] JunctionBlockNames = Enum.GetNames(typeof(EsabJunctionType));
            foreach (string junctionBlockName in JunctionBlockNames)
            {
                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                    if (!bt.Has("ESAB" + junctionBlockName) && !junctionBlockName.Contains("_"))
                    {
                        using (BlockTableRecord btr = new BlockTableRecord())
                        {
                            btr.Name = "ESAB" + junctionBlockName;
                            btr.Origin = Point3d.Origin;

                            Polyline pl = new Polyline();
                            pl.AddVertexAt(0, new Point2d(0.0, 0.5), 0, 0, 0);
                            pl.AddVertexAt(0, new Point2d(0.6, -0.5), 0, 0, 0);
                            pl.AddVertexAt(0, new Point2d(-0.6, -0.5), 0, 0, 0);
                            pl.Closed = true;
                            pl.ColorIndex = 0;
                            pl.LineWeight = LineWeight.ByBlock;
                            btr.AppendEntity(pl);

                            DBText t = new DBText()
                            {
                                TextString = junctionBlockName,
                                Height = 0.5,
                                VerticalMode = TextVerticalMode.TextBottom,
                                HorizontalMode = TextHorizontalMode.TextLeft,
                                AlignmentPoint = new Point3d(0.5, 0.5, 0.0),
                                ColorIndex = 0
                            };
                            btr.AppendEntity(t);

                            tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                            bt.Add(btr);
                            tr.AddNewlyCreatedDBObject(btr, true);
                            tr.Commit();
                        }
                    }
                }

                MlineStyle mls = new MlineStyle()
                {
                    
                };

            }

            #endregion Marker Blocks

            #region MLine Styles

            MlineStyle mlStyle;
            MlineStyleElement element;

            Color R = Color.FromColorIndex(ColorMethod.ByAci, 1);
            Color Y = Color.FromColorIndex(ColorMethod.ByAci, 2);
            Color B = Color.FromColorIndex(ColorMethod.ByAci, 5);

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                DBDictionary mlDict = (DBDictionary)tr.GetObject(acDb.MLStyleDictionaryId, OpenMode.ForRead);
                if (!mlDict.Contains("esabRYB"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabRYB", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabRYB";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.15, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(+0.00, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(-0.15, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabBYR"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabBYR", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabBYR";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.15, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(+0.00, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(-0.15, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabRBY"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabRBY", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabRBY";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.15, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(+0.00, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(-0.15, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabBRY"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabBRY", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabBRY";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.15, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(+0.00, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(-0.15, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabYRB"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabYRB", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabYRB";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.15, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(+0.00, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(-0.15, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabYBR"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabYBR", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabYBR";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.15, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(+0.00, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);
                    element = new MlineStyleElement(-0.15, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabR"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabR", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabR";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.00, R, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabY"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabY", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabY";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.00, Y, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }
                if (!mlDict.Contains("esabB"))
                {
                    mlDict.UpgradeOpen();

                    mlStyle = new MlineStyle();
                    mlDict.SetAt("esabB", mlStyle);
                    tr.AddNewlyCreatedDBObject(mlStyle, true);

                    mlStyle.Name = "esabB";
                    mlStyle.StartAngle = 3.14159 * 0.5;
                    mlStyle.EndAngle = 3.14159 * 0.5;

                    element = new MlineStyleElement(+0.00, B, acDb.ByLayerLinetype);
                    mlStyle.Elements.Add(element, true);

                }


                tr.Commit();
            }

            #endregion MLine Styles

        }

        public ObjectId CreateFeatureMarker(EsabFeatureType ft, double size, Point3d placement)
        {

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            ObjectId mkrid = ObjectId.Null;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                string FeatureBlockName = "ESAB" + Enum.GetName(typeof(EsabFeatureType), ft);
                if (bt.Has(FeatureBlockName))
                {

                    ObjectId FbnId = bt[FeatureBlockName];
                    using (BlockReference br = new BlockReference(placement, FbnId))
                    {

                        br.TransformBy(Matrix3d.Scaling(size, placement));
                        br.Layer = "_Esab_Features";

                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
                        btr.AppendEntity(br);
                        tr.AddNewlyCreatedDBObject(br, true);

                        mkrid = br.ObjectId;

                   }

                }

                tr.Commit();
            }

            return mkrid;
        }

        public ObjectId CreateJunctionMarker(EsabJunctionType ft, double size, Point3d placement)
        {

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            ObjectId mkrid = ObjectId.Null;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                string FeatureBlockName = "ESAB" + Enum.GetName(typeof(EsabJunctionType), ft);
                if (bt.Has(FeatureBlockName))
                {

                    ObjectId FbnId = bt[FeatureBlockName];
                    using (BlockReference br = new BlockReference(placement, FbnId))
                    {

                        br.TransformBy(Matrix3d.Scaling(size, placement));
                        br.Layer = "_Esab_Junctions";
                        br.ColorIndex = 8;

                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
                        btr.AppendEntity(br);
                        tr.AddNewlyCreatedDBObject(br, true);

                        mkrid = br.ObjectId;

                    }

                }

                tr.Commit();
            }

            return mkrid;
        }

        public ObjectId CreateTerminatorMarker(EsabTerminatorType ft, double size, Point3d placement)
        {

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            ObjectId mkrid = ObjectId.Null;

            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {

                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                string FeatureBlockName = "ESAB" + Enum.GetName(typeof(EsabTerminatorType), ft);
                if (bt.Has(FeatureBlockName))
                {

                    ObjectId FbnId = bt[FeatureBlockName];
                    using (BlockReference br = new BlockReference(placement, FbnId))
                    {

                        br.TransformBy(Matrix3d.Scaling(size, placement));
                        br.Layer = "_Esab_Terminators";
                        br.ColorIndex = 8;

                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
                        btr.AppendEntity(br);
                        tr.AddNewlyCreatedDBObject(br, true);

                        mkrid = br.ObjectId;

                    }

                }

                tr.Commit();
            }

            return mkrid;
        }


    }
}
