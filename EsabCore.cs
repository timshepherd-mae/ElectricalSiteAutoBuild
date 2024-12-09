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
        EsabFeatureType featureType;

    }

    public class EsabRoute
    {
        public ObjectId id;
        public EsabRating rating = EsabRating.kv400;
        public EsabConnectorType endType1 = EsabConnectorType.SGT;
        public EsabConnectorType endType2 = EsabConnectorType.CSE;

        public int phaseCount = 3;
        public ObjectIdCollection featureIds = new ObjectIdCollection();

        // constructors
        //

        #region transformers
        // xdict to class transformers

        public void ToXdictionary(DBObject dbo)
        {
            int tvCount = featureIds.Count + 7;
            TypedValue[] xdata = new TypedValue[tvCount];

            xdata[0] = new TypedValue((int)DxfCode.SoftPointerId, id);
            xdata[1] = new TypedValue((int)DxfCode.Int32, rating);
            xdata[2] = new TypedValue((int)DxfCode.Int32, phaseCount);
            xdata[3] = new TypedValue((int)DxfCode.Int32, endType1);
            xdata[4] = new TypedValue((int)DxfCode.Int32, endType2);
            xdata[5] = new TypedValue((int)DxfCode.ExtendedDataControlString, "{");
            
            for (int i = 0; i < featureIds.Count; i++)
            {
                xdata[i + 6] = new TypedValue((int)DxfCode.SoftPointerId, id);
            }

            xdata[tvCount - 1] = new TypedValue((int)DxfCode.ExtendedDataControlString, "}");

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
                phaseCount = (int)data[2].Value;
                endType1 = (EsabConnectorType)data[3].Value;
                endType2 = (EsabConnectorType)data[4].Value;
                if ((string)data[5].Value != "{")
                {
                    int i = 6;
                    while ((string)data[i++].Value != "}")
                    {
                        featureIds.Add((ObjectId)data[i].Value);
                    }

                }

            }

        }

        #endregion transformers

    }

    public class EsabConnector
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

    #endregion Enumerators
}
