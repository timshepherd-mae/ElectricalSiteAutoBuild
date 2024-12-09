using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;



namespace ElectricalSiteAutoBuild
{
    static class Extension
    {
        public static void Check(this ErrorStatus es, bool condition, string msg = null)
        {
            if (!condition)
            {
                if (msg == null)
                    throw new Autodesk.AutoCAD.Runtime.Exception(es);
                else
                    throw new Autodesk.AutoCAD.Runtime.Exception(es, msg);
            }
        }

        public static T GetObject<T>(
            this ObjectId id,
            OpenMode mode = OpenMode.ForRead,
            bool openErased = false,
            bool forceOpenOnLockedLayer = false)
            where T : DBObject
        {
            ErrorStatus.NullObjectId.Check(!id.IsNull);
            Transaction tr = id.Database.TransactionManager.TopTransaction;
            ErrorStatus.NoActiveTransactions.Check(tr != null);

            return (T)tr.GetObject(id, mode, openErased, forceOpenOnLockedLayer);
        }

        public static DBDictionary TryGetExtensionDictionary(this DBObject source)
        {
            Assert.IsNotNull(source, nameof(source));
            ObjectId dictId = source.ExtensionDictionary;
            if (dictId == ObjectId.Null)
            {
                return null;
            }
            return dictId.GetObject<DBDictionary>();
        }

        public static DBDictionary GetOrCreateExtensionDictionary(this DBObject source)
        {
            Assert.IsNotNull(source, nameof(source));
            if (source.ExtensionDictionary == ObjectId.Null)
            {
                source.UpgradeOpen();
                source.CreateExtensionDictionary();
            }
            return source.ExtensionDictionary.GetObject<DBDictionary>();
        }

        public static ResultBuffer GetXDictionaryXrecordData(this DBObject source, string key)
        {
            Assert.IsNotNull(source, nameof(source));
            Assert.IsNotNullOrWhiteSpace(key, nameof(key));
            DBDictionary xdict = source.TryGetExtensionDictionary();
            if (xdict == null)
            {
                return null;
            }
            return xdict.GetXrecordData(key);
        }

        public static void SetXDictionaryXrecordData(this DBObject target, string key, params TypedValue[] values)
        {
            target.SetXDictionaryXrecordData(key, new ResultBuffer(values));
        }

        public static void SetXDictionaryXrecordData(this DBObject target, string key, ResultBuffer data)
        {
            Assert.IsNotNull(target, nameof(target));
            Assert.IsNotNullOrWhiteSpace(key, nameof(key));
            target.GetOrCreateExtensionDictionary().SetXrecordData(key, data);
        }

        public static ResultBuffer GetXrecordData(this DBDictionary dict, string key)
        {
            Assert.IsNotNull(dict, nameof(dict));
            if (!dict.Contains(key))
                return null;
            ObjectId id = (ObjectId)dict[key];
            return id.GetObject<Xrecord>().Data;
        }

        public static void SetXrecordData(this DBDictionary dict, string key, params TypedValue[] values)
        {
            dict.SetXrecordData(key, new ResultBuffer(values));
        }

        public static void SetXrecordData(this DBDictionary dict, string key, ResultBuffer data)
        {
            Assert.IsNotNull(dict, nameof(dict));
            Assert.IsNotNullOrWhiteSpace(key, nameof(key));
            Xrecord xrec;
            if (dict.Contains(key))
            {
                xrec = ((ObjectId)dict[key]).GetObject<Xrecord>(OpenMode.ForWrite);
            }
            else
            {
                dict.UpgradeOpen();
                xrec = new Xrecord();
                dict.SetAt(key, xrec);
                dict.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(xrec, true);
            }
            xrec.Data = data;
        }
    }

    static class Assert
    {
        public static void IsNotNull<T>(T obj, string paramName) where T : class
        {
            if (obj == null)
                throw new System.ArgumentNullException(paramName);
        }

        public static void IsNotNullOrWhiteSpace(string str, string paramName)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new System.ArgumentException("eNullOrWhiteSpace", paramName);
        }
    }
}
