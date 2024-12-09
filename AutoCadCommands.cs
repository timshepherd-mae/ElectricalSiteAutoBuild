using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;


namespace ElectricalSiteAutoBuild
{

    public class AutoCadCommands
    {


        [CommandMethod("test")]
        public void test()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline or Circle: ");
            peo.SetRejectMessage("\nOnly LW Poly or Circle entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            peo.AddAllowedClass(typeof(Circle), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            // check for valid input
            //
            if (per.Status != PromptStatus.OK)
                return;

            acEd.WriteMessage("\nEntityObjectId: {0}", per.ObjectId.ToString());

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                acEd.WriteMessage("\n" + ent.Handle.ToString() + "\n");

                // check for attached Xdictionary
                //
                ObjectId extId = ent.ExtensionDictionary;
                if (extId == ObjectId.Null)
                {
                    ent.UpgradeOpen();
                    ent.CreateExtensionDictionary();
                    extId = ent.ExtensionDictionary;
                }

                acEd.WriteMessage("\nXDictionaryObjectId: {0}", extId.ToString());
                // now extId is valid
                //
                DBDictionary extObj = (DBDictionary)tr.GetObject(extId, OpenMode.ForRead);

                // if app-specific data not present, then add
                //
                if (!extObj.Contains("ESAB"))
                {
                    Xrecord xRec = new Xrecord();
                    ResultBuffer rb = new ResultBuffer();

                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, "TestData"));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataReal, 0.45));
                    rb.Add(new TypedValue((int)DxfCode.SoftPointerId, extId));

                    // set the data
                    //
                    extObj.UpgradeOpen();

                    xRec.Data = rb;
                    extObj.SetAt("ESAB", xRec);
                    tr.AddNewlyCreatedDBObject(xRec, true);
                }
                // otherwise, display data 
                //
                else
                {
                    Xrecord xRec = (Xrecord)tr.GetObject(extObj.GetAt("ESAB"), OpenMode.ForRead, false);
                    if (xRec != null)
                    {
                        ResultBuffer rb = xRec.Data;

                        if (rb != null)
                        {
                            var rbArray = rb.AsArray();
                            string s = (string)rbArray[0].Value;
                            double d = (double)rbArray[1].Value;
                            ObjectId o = (ObjectId)rbArray[2].Value;
                            acEd.WriteMessage($"data0: {s}\ndata1: {d}\ndata2: {o}");
                        }

                    }

                    acEd.WriteMessage("\nEntity already contains ESAB data\n");
                }

                tr.Commit();

            }

        }

        [CommandMethod("setxd")]
        public void setxd()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select polyline");
            peo.SetRejectMessage("Polyline only");
            peo.AddAllowedClass(typeof(Polyline), true);

            PromptEntityResult result = ed.GetEntity(peo);

            if (result.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {

                Polyline pline = (Polyline)tr.GetObject(result.ObjectId, OpenMode.ForRead);

                pline.SetXDictionaryXrecordData(
                    Constants.XappName,
                    new TypedValue(1000, "thing"),
                    new TypedValue(1040, 34.56),
                    new TypedValue((int)DxfCode.SoftPointerId, result.ObjectId)
                    );

                tr.Commit();
            }
        }

        [CommandMethod("getxd")]
        public void getxd()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select polyline");
            peo.SetRejectMessage("Polyline only");
            peo.AddAllowedClass(typeof(Polyline), true);

            PromptEntityResult result = ed.GetEntity(peo);

            if (result.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(result.ObjectId, OpenMode.ForRead);

                ResultBuffer rb = pline.GetXDictionaryXrecordData(Constants.XappName);

                if (rb != null)
                {
                    var data = rb.AsArray();
                    string str = (string)data[0].Value;
                    double dbl = (double)data[1].Value;
                    ObjectId oid = (ObjectId)data[2].Value;

                    ed.WriteMessage($"\nString {str}");
                    ed.WriteMessage($"\nReal {dbl}");
                    ed.WriteMessage($"\nObjId {oid}");

                }
                else
                {
                    ed.WriteMessage($"\n{Constants.XappName} data not found");
                }

                tr.Commit();


            }
        }

        [CommandMethod("kwdtest")]
        public void kwdtest()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = acDoc.Editor;

            PromptPointOptions ppo = new PromptPointOptions("Pick a point: ");
            ppo.Keywords.Add("teSt");
            ppo.Keywords.Add("Tree");
            ppo.Keywords.Add("fEnce");

            ppo.AppendKeywordsToMessage = true;

            PromptPointResult res = ed.GetPoint(ppo);
            ed.WriteMessage(res.Status.ToString() + "\n");
            ed.WriteMessage(res.StringResult + "\n");


        }

        [CommandMethod("loadXdata")]
        public void loadxdata()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                EsabRoute route = new EsabRoute()
                {
                    id = pline.ObjectId,
                    rating = EsabRating.kv400,
                    phaseCount = 3,
                    endType1 = EsabConnectorType.CSE,
                    endType2 = EsabConnectorType.SGT,
                    featureIds = new ObjectIdCollection()
                };
                route.featureIds.Add(pline.ObjectId);
                route.featureIds.Add(pline.ObjectId);

                route.ToXdictionary(pline);

                tr.Commit();               
            }
        }

        [CommandMethod("readxdata")]
        public void readxdata()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("Select LW Polyline: ");
            peo.SetRejectMessage("\nOnly LW Poly entities allowed: ");
            peo.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Polyline pline = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                EsabRoute route = new EsabRoute();
                route.FromXdictionary(pline);

                acEd.WriteMessage(route.phaseCount.ToString());
                acEd.WriteMessage(route.id.ToString());
                foreach (ObjectId objid in route.featureIds)
                {
                    acEd.WriteMessage(objid.ToString());
                }

                tr.Commit();
            }

        }


    }


}
