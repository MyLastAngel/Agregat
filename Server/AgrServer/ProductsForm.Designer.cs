namespace AgrServer
{
    partial class ProductsForm
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.add_user_btn = new System.Windows.Forms.ToolStripButton();
            this.remove_user_btn = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.user_list = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.product_list = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.project_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.apply_btn = new System.Windows.Forms.Button();
            this.description_text = new System.Windows.Forms.TextBox();
            this.name_text = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.add_user_btn,
            this.remove_user_btn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(636, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // add_user_btn
            // 
            this.add_user_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.add_user_btn.Image = global::AgrServer.Properties.Resources.user_green;
            this.add_user_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.add_user_btn.Name = "add_user_btn";
            this.add_user_btn.Size = new System.Drawing.Size(23, 22);
            this.add_user_btn.Text = "Добавить пользователя";
            this.add_user_btn.Click += new System.EventHandler(this.AddUserBtnClick);
            // 
            // remove_user_btn
            // 
            this.remove_user_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.remove_user_btn.Image = global::AgrServer.Properties.Resources.user_red;
            this.remove_user_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.remove_user_btn.Name = "remove_user_btn";
            this.remove_user_btn.Size = new System.Drawing.Size(23, 22);
            this.remove_user_btn.Text = "Удалить пользователя";
            this.remove_user_btn.Click += new System.EventHandler(this.RemoveUserBtnClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.user_list);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.product_list);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.project_list);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.apply_btn);
            this.splitContainer1.Panel2.Controls.Add(this.description_text);
            this.splitContainer1.Panel2.Controls.Add(this.name_text);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Enabled = false;
            this.splitContainer1.Size = new System.Drawing.Size(636, 495);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 3;
            // 
            // user_list
            // 
            this.user_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5});
            this.user_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.user_list.FullRowSelect = true;
            this.user_list.GridLines = true;
            this.user_list.HideSelection = false;
            this.user_list.Location = new System.Drawing.Point(0, 0);
            this.user_list.MultiSelect = false;
            this.user_list.Name = "user_list";
            this.user_list.Size = new System.Drawing.Size(250, 495);
            this.user_list.TabIndex = 1;
            this.user_list.UseCompatibleStateImageBehavior = false;
            this.user_list.View = System.Windows.Forms.View.Details;
            this.user_list.SelectedIndexChanged += new System.EventHandler(this.UserListSelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Номер";
            this.columnHeader2.Width = 49;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Имя";
            this.columnHeader4.Width = 71;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Описание";
            this.columnHeader5.Width = 96;
            // 
            // product_list
            // 
            this.product_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.product_list.CheckBoxes = true;
            this.product_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.product_list.FullRowSelect = true;
            this.product_list.GridLines = true;
            this.product_list.HideSelection = false;
            this.product_list.Location = new System.Drawing.Point(62, 210);
            this.product_list.MultiSelect = false;
            this.product_list.Name = "product_list";
            this.product_list.Size = new System.Drawing.Size(305, 129);
            this.product_list.TabIndex = 12;
            this.product_list.UseCompatibleStateImageBehavior = false;
            this.product_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Имя";
            this.columnHeader3.Width = 288;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Изделия";
            // 
            // project_list
            // 
            this.project_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.project_list.CheckBoxes = true;
            this.project_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.project_list.FullRowSelect = true;
            this.project_list.GridLines = true;
            this.project_list.HideSelection = false;
            this.project_list.Location = new System.Drawing.Point(62, 61);
            this.project_list.MultiSelect = false;
            this.project_list.Name = "project_list";
            this.project_list.Size = new System.Drawing.Size(305, 129);
            this.project_list.TabIndex = 10;
            this.project_list.UseCompatibleStateImageBehavior = false;
            this.project_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Имя";
            this.columnHeader1.Width = 288;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Проекты";
            // 
            // apply_btn
            // 
            this.apply_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply_btn.Enabled = false;
            this.apply_btn.Location = new System.Drawing.Point(286, 460);
            this.apply_btn.Name = "apply_btn";
            this.apply_btn.Size = new System.Drawing.Size(84, 23);
            this.apply_btn.TabIndex = 6;
            this.apply_btn.Text = "Применить";
            this.apply_btn.UseVisualStyleBackColor = true;
            this.apply_btn.Click += new System.EventHandler(this.ApplyBtnClick);
            // 
            // description_text
            // 
            this.description_text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.description_text.Location = new System.Drawing.Point(62, 35);
            this.description_text.Name = "description_text";
            this.description_text.Size = new System.Drawing.Size(308, 20);
            this.description_text.TabIndex = 4;
            this.description_text.TextChanged += new System.EventHandler(this.DescriptionTextTextChanged);
            // 
            // name_text
            // 
            this.name_text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.name_text.Location = new System.Drawing.Point(62, 9);
            this.name_text.Name = "name_text";
            this.name_text.Size = new System.Drawing.Size(308, 20);
            this.name_text.TabIndex = 3;
            this.name_text.TextChanged += new System.EventHandler(this.NameTextTextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Описание";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя";
            // 
            // ProductsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 520);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ProductsForm";
            this.Text = "ProductsForm";
            this.Load += new System.EventHandler(this.ProductsFormLoad);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton add_user_btn;
        private System.Windows.Forms.ToolStripButton remove_user_btn;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button apply_btn;
        private System.Windows.Forms.TextBox description_text;
        private System.Windows.Forms.TextBox name_text;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView product_list;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView project_list;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView user_list;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}