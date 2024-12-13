using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;


namespace ElectricalSiteAutoBuild
{
    public static class Constants
    {
        // define app name for extended dictionary content
        //
        public const string XappName = "ESAB";


    }

    public class EsabFeature
    {
        public ObjectId id;
        public EsabXdType type = EsabXdType.Feature;
        public ObjectId parentId;
        public int parentVertex;
        public EsabFeatureType featureType;

        // constructors
        //

        #region transformers
        // xdict to class transformers

        public void ToXdictionary(DBObject dbo)
        {
            TypedValue[] xdata = new TypedValue[5];

            xdata[0] = new TypedValue((int)DxfCode.SoftPointerId, id);
            xdata[1] = new TypedValue((int)DxfCode.Int32, type);
            xdata[2] = new TypedValue((int)DxfCode.SoftPointerId, parentId);
            xdata[3] = new TypedValue((int)DxfCode.Int32, parentVertex);
            xdata[4] = new TypedValue((int)DxfCode.Int32, featureType);

            dbo.SetXDictionaryXrecordData(Constants.XappName, xdata);
        }

        public void FromXdictionary(DBObject dbo)
        {
            ResultBuffer rb = dbo.GetXDictionaryXrecordData(Constants.XappName);

            if (rb != null)
            {
                var data = rb.AsArray();
                id = (ObjectId)data[0].Value;
                type = (EsabXdType)Enum.ToObject(typeof(EsabXdType), data[1].Value);
                parentId = (ObjectId)data[2].Value;
                parentVertex = (int)data[3].Value;
                featureType = (EsabFeatureType)Enum.ToObject(typeof(EsabFeatureType), data[4].Value);

            }
        }
        #endregion transformers

    }

    public class EsabRoute
    {
        public ObjectId id;
        public EsabXdType type = EsabXdType.Route;
        public EsabRating rating;
        public EsabPhaseType phase;
        public int phasesep;
        public EsabPhaseColour phasecol;
        public EsabConductorType defaultConductorType;
        public ObjectIdCollection featureIds = new ObjectIdCollection();

        // constructors
        //

        #region transformers
        // xdict to class transformers

        public void ToXdictionary(DBObject dbo)
        {
            int tvCount = featureIds.Count + 7;
            TypedValue[] xdata = new TypedValue[tvCount];

            xdata[0] = new TypedValue((int)DxfCode.SoftPointerId, id);  // route ent id
            xdata[1] = new TypedValue((int)DxfCode.Int32, type);        // esab type
            xdata[2] = new TypedValue((int)DxfCode.Int32, rating);      // kv value
            xdata[3] = new TypedValue((int)DxfCode.Int32, phase);       // single/three phase
            xdata[4] = new TypedValue((int)DxfCode.Int32, phasecol);    // phace color
            
            for (int i = 0; i < featureIds.Count; i++)
            {
                xdata[i + 7] = new TypedValue((int)DxfCode.SoftPointerId, id);
            }

            dbo.SetXDictionaryXrecordData(Constants.XappName, xdata);
        }

        public void FromXdictionary(DBObject dbo)
        {
            featureIds.Clear();
            ResultBuffer rb = dbo.GetXDictionaryXrecordData(Constants.XappName);

            if (rb != null)
            {
                var data = rb.AsArray();
                id = (ObjectId)data[0].Value;
                type = (EsabXdType)Enum.ToObject(typeof(EsabXdType), data[1].Value);
                rating = (EsabRating)Enum.ToObject(typeof(EsabRating), data[2].Value);
                phase = (EsabPhaseType)Enum.ToObject(typeof(EsabPhaseType), data[3].Value);
                phasecol = (EsabPhaseColour)Enum.ToObject(typeof(EsabPhaseColour), data[4].Value);
                for (int i = 7; i < data.Length; i++)
                {
                        featureIds.Add((ObjectId)data[i].Value);
                }

            }

        }

        #endregion transformers

    }

    public class EsabConductor
    {
        
    }

    public class EsabTerminator
    {
        public ObjectId id;
        public EsabXdType type = EsabXdType.Terminator;
        public ObjectId routeA;
        public ObjectId routeB;
        public EsabTerminatorType terminatortype;
    }

    public class EsabJunction
    {
        public ObjectId id;
        public EsabXdType type = EsabXdType.Junction;
        public ObjectId routemain;
        public ObjectId routebranch;
        public EsabJunctionType junctionType;
    }



    #region Enumerators

    public enum EsabRating
    {
        kv400, kv275
    }

    public enum EsabFeatureType
    {
        PI, RI, ESW, CVT, SA, NUL, Terminator, Junction
    }

    public enum EsabTerminatorType
    {
        SGT, CSE, GIS, OHC, JNC, NUL
    }

    public enum EsabJunctionType
    {
        POST, FORK
    }

    public enum EsabConductorType
    {
        GIB, Busbar, Cable
    }
 
    public enum EsabPhaseType
    {
        Single, ThreePhase
    }

    public enum EsabPhaseColour
    {
        R, Y, B, RYB, BYR, RBY, BRY, YRB, YBR
    }

    public enum EsabXdType
    {
        Route, Feature, Conductor, Terminator, Junction
    } 

    #endregion Enumerators

}
