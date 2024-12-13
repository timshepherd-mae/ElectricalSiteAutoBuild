using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using acApp = Autodesk.AutoCAD.ApplicationServices;


namespace ElectricalSiteAutoBuild
{

    public partial class InspectionTool : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void InspectionTool_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private ObjectId lastSelection;

        public InspectionTool()
        {
            InitializeComponent();
            this.lastSelection = ObjectId.Null;
        }

        private void InspectionTool_Load(object sender, EventArgs e)
        {
            this.MouseDown += InspectionTool_MouseDown;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.lvwItems.Items.Clear();
            

            acApp.Document acDoc = acApp.Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            acEd.Regen();

            PromptEntityOptions peo = new PromptEntityOptions("\nSelect entity to inspect:\n");
            PromptEntityResult per = acEd.GetEntity(peo);

            if (per.Status != PromptStatus.OK)
                return;

            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                Entity ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                ResultBuffer rb = ent.GetXDictionaryXrecordData(Constants.XappName);

                if (rb != null)
                {
                    var data = rb.AsArray();
                    var type = (EsabXdType)Enum.ToObject(typeof(EsabXdType), data[1].Value);
                    this.lvwItems.Items.Add("Item Class: " + type.ToString());

                    if (type == EsabXdType.Route)
                    {
                        this.lvwItems.Items.Add("Rating:     " + (EsabRating)Enum.ToObject(typeof(EsabRating), data[2].Value));
                        this.lvwItems.Items.Add("Phase:      " + (EsabPhaseType)Enum.ToObject(typeof(EsabPhaseType), data[3].Value));
                        this.lvwItems.Items.Add("Separation: " + data[4].Value);
                        this.lvwItems.Items.Add("Colour:     " + (EsabPhaseColour)Enum.ToObject(typeof(EsabPhaseColour), data[5].Value));
                        this.lvwItems.Items.Add("Conductor:  " + (EsabConductorType)Enum.ToObject(typeof(EsabConductorType), data[6].Value));
                    }
                    if (type == EsabXdType.Feature)
                    {
                        this.lvwItems.Items.Add("Type: " + (EsabFeatureType)Enum.ToObject(typeof(EsabFeatureType), data[4].Value));
                    }
                    if (type == EsabXdType.Terminator)
                    {
                        this.lvwItems.Items.Add("Type: " + (EsabTerminatorType)Enum.ToObject(typeof(EsabTerminatorType), data[4].Value));
                    }
                    if (type == EsabXdType.Junction)
                    {
                        this.lvwItems.Items.Add("Type: " + (EsabJunctionType)Enum.ToObject(typeof(EsabJunctionType), data[4].Value));
                    }

                }
                else
                {
                    this.lvwItems.Items.Add("No ESAB data found");
                }

                this.lastSelection = ent.ObjectId;
                tr.Commit();
            }

            this.lvwItems.Focus();

        }
/*
        private void btnLocate_Click(object sender, EventArgs e)
        {
            if (this.lastSelection != null)
            {
                acApp.Document acDoc = acApp.Application.DocumentManager.MdiActiveDocument;
                Editor acEd = acDoc.Editor;

                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    Entity ent = (Entity)tr.GetObject(this.lastSelection, OpenMode.ForRead);
                    ent.Highlight();
                    acEd.UpdateScreen();

                    tr.Commit();
                }
            }
        }
*/
        private void btnLocate_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.lastSelection != null)
            {
                acApp.Document acDoc = acApp.Application.DocumentManager.MdiActiveDocument;
                Editor acEd = acDoc.Editor;

                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    Entity ent = (Entity)tr.GetObject(this.lastSelection, OpenMode.ForRead);
                    ent.Highlight();
                    acEd.UpdateScreen();

                    tr.Commit();
                }
            }
        }

        private void btnLocate_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.lastSelection != null)
            {
                acApp.Document acDoc = acApp.Application.DocumentManager.MdiActiveDocument;
                Editor acEd = acDoc.Editor;

                using (Transaction tr = acDoc.TransactionManager.StartTransaction())
                {
                    Entity ent = (Entity)tr.GetObject(this.lastSelection, OpenMode.ForRead);
                    ent.Unhighlight();
                    acEd.UpdateScreen();

                    tr.Commit();
                }
            }
        }
    }
}
