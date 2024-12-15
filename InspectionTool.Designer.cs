namespace ElectricalSiteAutoBuild
{
    partial class InspectionTool
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelect = new System.Windows.Forms.Button();
            this.lvwItems = new System.Windows.Forms.ListView();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLocate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(5, 5);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(50, 25);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lvwItems
            // 
            this.lvwItems.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwItems.HideSelection = false;
            this.lvwItems.Location = new System.Drawing.Point(5, 36);
            this.lvwItems.MultiSelect = false;
            this.lvwItems.Name = "lvwItems";
            this.lvwItems.Size = new System.Drawing.Size(190, 190);
            this.lvwItems.TabIndex = 1;
            this.lvwItems.UseCompatibleStateImageBehavior = false;
            this.lvwItems.View = System.Windows.Forms.View.List;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(170, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(25, 25);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnLocate
            // 
            this.btnLocate.Location = new System.Drawing.Point(60, 5);
            this.btnLocate.Name = "btnLocate";
            this.btnLocate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnLocate.Size = new System.Drawing.Size(50, 25);
            this.btnLocate.TabIndex = 3;
            this.btnLocate.Text = "locate";
            this.btnLocate.UseVisualStyleBackColor = true;
            this.btnLocate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLocate_MouseDown);
            this.btnLocate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnLocate_MouseUp);
            // 
            // InspectionTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 230);
            this.Controls.Add(this.btnLocate);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lvwItems);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InspectionTool";
            this.Text = "ESAB Inspection Tool";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InspectionTool_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.ListView lvwItems;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnLocate;
    }
}