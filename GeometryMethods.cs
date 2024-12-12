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

            // LYR
            using (Transaction tr = acDb.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)tr.GetObject(acDb.LayerTableId, OpenMode.ForWrite);
                LinetypeTable ltt = (LinetypeTable)tr.GetObject(acDb.LinetypeTableId, OpenMode.ForRead);

                ObjectId RouteLT = (ltt.Has("Dashed")) ? ltt["Dashed"] : ltt["Continuous"];

                if (!lt.Has("_Esab_Markers"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Markers";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 2);
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
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 6);
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
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight025;
                        lyr.LinetypeObjectId = RouteLT;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);

                    }
                }

                tr.Commit();
            }

            List<string> FeatureBlockNames = new List<string>() { "PI", "CVT", "ESW", "SA", "NUL" };

            foreach (string featureBlockName in FeatureBlockNames)
            {
                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                    if (!bt.Has("ESAB" + featureBlockName))
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
                                AlignmentPoint = new Point3d(0.65, 0.65, 0.0),
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
        }

 /*           // PI
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("ESABPI"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "ESABPI";
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
                            TextString = "PI",
                            Height = 0.5,
                            VerticalMode = TextVerticalMode.TextBottom,
                            HorizontalMode = TextHorizontalMode.TextLeft,
                            AlignmentPoint = new Point3d(0.65, 0.65, 0.0),
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

            // CVT
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("ESABCVT"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "ESABCVT";
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
                            StartPoint = new Point3d(-0.5, 0.5, 0.0),
                            EndPoint = new Point3d(0.5, -0.5, 0.0),
                            ColorIndex = 0,
                            LineWeight = LineWeight.ByBlock
                        };
                        btr.AppendEntity(l1);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);
                        tr.Commit();
                    }
                }
            }

            // ESW
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("ESABESW"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "ESABESW";
                        btr.Origin = Point3d.Origin;

                        Polyline pl = new Polyline();
                        pl.AddVertexAt(0, new Point2d(0.0, 0.5), 0, 0, 0);
                        pl.AddVertexAt(0, new Point2d(0.5, 0.0), 0, 0, 0);
                        pl.AddVertexAt(0, new Point2d(0.0, -0.5), 0, 0, 0);
                        pl.AddVertexAt(0, new Point2d(-0.5, 0.0), 0, 0, 0);
                        pl.Closed = true;
                        pl.ColorIndex = 0;
                        pl.LineWeight = LineWeight.ByBlock;
                        btr.AppendEntity(pl);

                        Line l1 = new Line()
                        {
                            StartPoint = new Point3d(-0.5, 0.5, 0.0),
                            EndPoint = new Point3d(0.5, -0.5, 0.0),
                            ColorIndex = 0,
                            LineWeight = LineWeight.ByBlock
                        };
                        btr.AppendEntity(l1);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);
                        tr.Commit();
                    }
                }
            }

            // SA
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("ESABSA"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "ESABSA";
                        btr.Origin = Point3d.Origin;

                        Polyline pl = new Polyline();
                        pl.AddVertexAt(0, new Point2d(0.0, 0.5), 0, 0, 0);
                        pl.AddVertexAt(0, new Point2d(0.5, 0.0), 0, 0, 0);
                        pl.AddVertexAt(0, new Point2d(0.0, -0.5), 0, 0, 0);
                        pl.AddVertexAt(0, new Point2d(-0.5, 0.0), 0, 0, 0);
                        pl.Closed = true;
                        pl.ColorIndex = 0;
                        pl.LineWeight = LineWeight.ByBlock;
                        btr.AppendEntity(pl);

                        Line l1 = new Line()
                        {
                            StartPoint = new Point3d(-0.5, 0.5, 0.0),
                            EndPoint = new Point3d(0.5, -0.5, 0.0),
                            ColorIndex = 0,
                            LineWeight = LineWeight.ByBlock
                        };
                        btr.AppendEntity(l1);

                        Line l2 = new Line()
                        {
                            StartPoint = new Point3d(0.5, 0.5, 0.0),
                            EndPoint = new Point3d(-0.5, -0.5, 0.0),
                            ColorIndex = 0,
                            LineWeight = LineWeight.ByBlock
                        };
                        btr.AppendEntity(l2);

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);
                        tr.Commit();
                    }
                }
            }

            // NUL
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(acDb.BlockTableId, OpenMode.ForRead);

                if (!bt.Has("ESABNUL"))
                {
                    using (BlockTableRecord btr = new BlockTableRecord())
                    {
                        btr.Name = "ESABNUL";
                        btr.Origin = Point3d.Origin;

                        Line l0 = new Line()
                        {
                            StartPoint = new Point3d(0.5, 0.0, 0.0),
                            EndPoint = new Point3d(-0.5, 0.0, 0.0),
                            ColorIndex = 0,
                            LineWeight = LineWeight.ByBlock
                        };
                        btr.AppendEntity(l0);

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

                        tr.GetObject(acDb.BlockTableId, OpenMode.ForWrite);
                        bt.Add(btr);
                        tr.AddNewlyCreatedDBObject(btr, true);
                        tr.Commit();
                    }
                }
            }

        }
 */

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
                        br.Layer = "_Esab_Markers";

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
