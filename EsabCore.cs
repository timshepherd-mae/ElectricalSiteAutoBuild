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
        public ObjectId parentId;
        public int parentVertex;
        public EsabFeatureType featureType;

        // constructors
        //

        #region transformers
        // xdict to class transformers

        public void ToXdictionary(DBObject dbo)
        {
            TypedValue[] xdata = new TypedValue[4];

            xdata[0] = new TypedValue((int)DxfCode.SoftPointerId, id);
            xdata[1] = new TypedValue((int)DxfCode.SoftPointerId, parentId);
            xdata[2] = new TypedValue((int)DxfCode.Int32, parentVertex);
            xdata[3] = new TypedValue((int)DxfCode.Int32, featureType);

            dbo.SetXDictionaryXrecordData(Constants.XappName, xdata);
        }

        public void FromXdictionary(DBObject dbo)
        {
            ResultBuffer rb = dbo.GetXDictionaryXrecordData(Constants.XappName);

            if (rb != null)
            {
                var data = rb.AsArray();
                id = (ObjectId)data[0].Value;
                parentId = (ObjectId)data[1].Value;
                parentVertex = (int)data[2].Value;
                featureType = (EsabFeatureType)Enum.ToObject(typeof(EsabFeatureType), data[3].Value);

            }
        }
        #endregion transformers

    }

    public class EsabRoute
    {
        public ObjectId id;
        public EsabRating rating;
        public EsabConnectorType endType1;
        public EsabConnectorType endType2;
        public PhaseType phase;
        public PhaseColour phasecol;
        public ObjectIdCollection featureIds = new ObjectIdCollection();

        // constructors
        //

        #region transformers
        // xdict to class transformers

        public void ToXdictionary(DBObject dbo)
        {
            int tvCount = featureIds.Count + 6;
            TypedValue[] xdata = new TypedValue[tvCount];

            xdata[0] = new TypedValue((int)DxfCode.SoftPointerId, id);
            xdata[1] = new TypedValue((int)DxfCode.Int32, rating);
            xdata[2] = new TypedValue((int)DxfCode.Int32, phase);
            xdata[3] = new TypedValue((int)DxfCode.Int32, phasecol);
            xdata[4] = new TypedValue((int)DxfCode.Int32, endType1);
            xdata[5] = new TypedValue((int)DxfCode.Int32, endType2);
            
            for (int i = 0; i < featureIds.Count; i++)
            {
                xdata[i + 6] = new TypedValue((int)DxfCode.SoftPointerId, id);
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
                rating = (EsabRating)Enum.ToObject(typeof(EsabRating), data[1].Value);
                phase = (PhaseType)Enum.ToObject(typeof(PhaseType), data[2].Value);
                phasecol = (PhaseColour)Enum.ToObject(typeof(PhaseColour), data[3].Value);
                endType1 = (EsabConnectorType)Enum.ToObject(typeof(EsabConnectorType), data[4].Value);
                endType2 = (EsabConnectorType)Enum.ToObject(typeof(EsabConnectorType), data[5].Value);
                for (int i = 6; i < data.Length; i++)
                {
                        featureIds.Add((ObjectId)data[i].Value);
                }

            }

        }

        #endregion transformers

    }

    public class EsabConductor
    {
        EsabConnectorType connectionType;
    }

    #region Enumerators

    public enum EsabRating
    {
        kv400, kv275
    }

    public enum EsabFeatureType
    {
        PI, ESW, CVT, SA, XX
    }

    public enum EsabConnectorType
    {
        SGT, CSE, GIS, OHC, Junction, Null
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


    #endregion Enumerators
}
