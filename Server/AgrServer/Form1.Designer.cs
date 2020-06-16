namespace AgrServer
{
    partial class Form1
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
            this.users_btn = new System.Windows.Forms.Button();
            this.clients_btn = new System.Windows.Forms.Button();
            this.products_btn = new System.Windows.Forms.Button();
            this.projects_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // users_btn
            // 
            this.users_btn.Location = new System.Drawing.Point(24, 21);
            this.users_btn.Name = "users_btn";
            this.users_btn.Size = new System.Drawing.Size(115, 23);
            this.users_btn.TabIndex = 0;
            this.users_btn.Text = "Пользователи";
            this.users_btn.UseVisualStyleBackColor = true;
            this.users_btn.Click += new System.EventHandler(this.UsersBtnClick);
            // 
            // clients_btn
            // 
            this.clients_btn.Location = new System.Drawing.Point(24, 50);
            this.clients_btn.Name = "clients_btn";
            this.clients_btn.Size = new System.Drawing.Size(115, 23);
            this.clients_btn.TabIndex = 1;
            this.clients_btn.Text = "Клиенты";
            this.clients_btn.UseVisualStyleBackColor = true;
            this.clients_btn.Click += new System.EventHandler(this.ClientsBtnClick);
            // 
            // products_btn
            // 
            this.products_btn.Location = new System.Drawing.Point(24, 79);
            this.products_btn.Name = "products_btn";
            this.products_btn.Size = new System.Drawing.Size(115, 23);
            this.products_btn.TabIndex = 2;
            this.products_btn.Text = "Изделия";
            this.products_btn.UseVisualStyleBackColor = true;
            this.products_btn.Click += new System.EventHandler(this.ProductsBtnClick);
            // 
            // projects_btn
            // 
            this.projects_btn.Location = new System.Drawing.Point(24, 108);
            this.projects_btn.Name = "projects_btn";
            this.projects_btn.Size = new System.Drawing.Size(115, 23);
            this.projects_btn.TabIndex = 3;
            this.projects_btn.Text = "Проекты";
            this.projects_btn.UseVisualStyleBackColor = true;
            this.projects_btn.Click += new System.EventHandler(this.ProjectsBtnClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 314);
            this.Controls.Add(this.projects_btn);
            this.Controls.Add(this.products_btn);
            this.Controls.Add(this.clients_btn);
            this.Controls.Add(this.users_btn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button users_btn;
        private System.Windows.Forms.Button clients_btn;
        private System.Windows.Forms.Button products_btn;
        private System.Windows.Forms.Button projects_btn;
    }
}

