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
        public static Dictionary<string, List<(int, int)>> FeatureMarkers = new Dictionary<string, List<(int, int)>>()
        {
            { "PI",  new List<(int, int)> { (+0, +0), (+0, +1), (+1, +0), (+0, -1), (-1, +0), (+0, +1) } },
            { "ESW", new List<(int, int)> { (+0, +0), (-1, +1), (+1, +1), (-1, -1), (+1, -1), (+0, +0) } },
            { "CVT", new List<(int, int)> { (+0, +0), (+0, +1), (-1, +1), (-1, -1), (+1, -1), (+1, +1), (+0, +1) } },
            { "SA",  new List<(int, int)> { (+0, +0), (+0, +1), (-1, +1), (-1, -1), (+1, -1), (+1, +1), (+0, +1) } },
            { "NUL", new List<(int, int)> { (+0, +0), (+1, +1), (-1, -1), (+0, +0), (-1, +1), (+1, -1) } }
        };
        
        public void InitialiseFeatureGeometry()
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

                if (!lt.Has("_Esab_Markers"))
                {
                    using (LayerTableRecord lyr = new LayerTableRecord())
                    {
                        lyr.Name = "_Esab_Markers";
                        Color lcol = new Color();
                        lcol = Color.FromColorIndex(ColorMethod.ByAci, 2);
                        lyr.Color = lcol;
                        lyr.LineWeight = LineWeight.LineWeight040;

                        lt.Add(lyr);
                        tr.AddNewlyCreatedDBObject(lyr, true);
                        tr.Commit();
                    }
                }
            }

            // PI
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




        public Polyline FeatureMarker(EsabFeatureType ft, double size, Point3d placement)
        {
            Polyline pline = new Polyline();
            string MarkerName = Enum.GetName(typeof(EsabFeatureType), ft);
            var MarkerGeometry = FeatureMarkers[MarkerName];
            for (int i = 0; i < MarkerGeometry.Count; i++)
            {
                pline.AddVertexAt(i, new Point2d(MarkerGeometry[i].Item1 * size * 0.5, MarkerGeometry[i].Item2 * size * 0.5), 0, 0, 0);
            }
            var mat = Matrix3d.Displacement(placement - Point3d.Origin);
            pline.TransformBy(mat);
            pline.ColorIndex = 2;
            pline.LineWeight = LineWeight.LineWeight040;

            return pline;
        }

        public void CreateFeatureMarker(EsabFeatureType ft, double size, Point3d placement)
        {

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

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

                    }

                }

                tr.Commit();
            }
            
       }

        public void EntityTransactionAdd(Transaction tr, Entity ent)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(acDb.CurrentSpaceId, OpenMode.ForWrite);
            btr.AppendEntity(ent);
            tr.AddNewlyCreatedDBObject(ent, true);

            //tr.Commit();
            acEd.Regen();

        }
    }
}
