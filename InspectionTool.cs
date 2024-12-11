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


        public InspectionTool()
        {
            InitializeComponent();
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

            PromptEntityOptions peo = new PromptEntityOptions("\nSelect entity to inspect: ");
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
                    var type = (xdType)Enum.ToObject(typeof(xdType), data[1].Value);
                    this.lvwItems.Items.Add("Dictionary Type: " + type.ToString());

                    if (type == xdType.Route)
                    {
                        this.lvwItems.Items.Add("Rating: " + (EsabRating)Enum.ToObject(typeof(EsabRating), data[2].Value));
                        this.lvwItems.Items.Add("Phase: " + (PhaseType)Enum.ToObject(typeof(PhaseType), data[3].Value));
                        this.lvwItems.Items.Add("Colour: " + (PhaseColour)Enum.ToObject(typeof(PhaseColour), data[4].Value));
                        this.lvwItems.Items.Add("End1: " + (EsabTerminatorType)Enum.ToObject(typeof(EsabTerminatorType), data[5].Value));
                        this.lvwItems.Items.Add("End2: " + (EsabTerminatorType)Enum.ToObject(typeof(EsabTerminatorType), data[6].Value));
                        

                    }
                    if (type == xdType.Feature)
                    {
                        this.lvwItems.Items.Add("Type: " + (EsabFeatureType)Enum.ToObject(typeof(EsabFeatureType), data[4].Value));
                    }
                }
                else
                {
                    this.lvwItems.Items.Add("No ESAB data found");
                }
            }

            this.lvwItems.Focus();

        }
    }
}
