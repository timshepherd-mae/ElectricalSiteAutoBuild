using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

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
