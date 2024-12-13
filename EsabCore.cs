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
        public xdType type = xdType.Feature;
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
                type = (xdType)Enum.ToObject(typeof(xdType), data[1].Value);
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
        public xdType type = xdType.Route;
        public EsabRating rating;
        public EsabTerminatorType endType1;
        public EsabTerminatorType endType2;
        public PhaseType phase;
        public int phasesep;
        public PhaseColour phasecol;
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
            xdata[5] = new TypedValue((int)DxfCode.Int32, endType1);
            xdata[6] = new TypedValue((int)DxfCode.Int32, endType2);
            
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
                type = (xdType)Enum.ToObject(typeof(xdType), data[1].Value);
                rating = (EsabRating)Enum.ToObject(typeof(EsabRating), data[2].Value);
                phase = (PhaseType)Enum.ToObject(typeof(PhaseType), data[3].Value);
                phasecol = (PhaseColour)Enum.ToObject(typeof(PhaseColour), data[4].Value);
                endType1 = (EsabTerminatorType)Enum.ToObject(typeof(EsabTerminatorType), data[5].Value);
                endType2 = (EsabTerminatorType)Enum.ToObject(typeof(EsabTerminatorType), data[6].Value);
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
        EsabTerminatorType connectionType;
    }



    #region Enumerators

    public enum EsabRating
    {
        kv400, kv275
    }

    public enum EsabFeatureType
    {
        PI, ESW, CVT, SA, NUL
    }

    public enum EsabTerminatorType
    {
        SGT, CSE, GIS, OHC, JNC, NUL
    }
    public enum ConductorType
    {
        GIB, Busbar, Cable
    }
 
    public enum PhaseType
    {
        Single, ThreePhase
    }

    public enum PhaseColour
    {
        Red, Yellow, Blue, RYB, BYR
    }

    public enum xdType
    {
        Route, Feature, Conductor, Connector
    } 

    #endregion Enumerators

}
