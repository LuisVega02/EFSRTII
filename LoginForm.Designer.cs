namespace ProyectoFinal
{
    partial class LoginForm
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
            this.bttValidar = new System.Windows.Forms.Button();
            this.bttCerrar = new System.Windows.Forms.Button();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bttValidar
            // 
            this.bttValidar.Location = new System.Drawing.Point(157, 187);
            this.bttValidar.Name = "bttValidar";
            this.bttValidar.Size = new System.Drawing.Size(75, 23);
            this.bttValidar.TabIndex = 0;
            this.bttValidar.Text = "Ingresar";
            this.bttValidar.UseVisualStyleBackColor = true;
            this.bttValidar.Click += new System.EventHandler(this.button1_Click);
            // 
            // bttCerrar
            // 
            this.bttCerrar.Location = new System.Drawing.Point(157, 231);
            this.bttCerrar.Name = "bttCerrar";
            this.bttCerrar.Size = new System.Drawing.Size(75, 23);
            this.bttCerrar.TabIndex = 1;
            this.bttCerrar.Text = "Cerrar";
            this.bttCerrar.UseVisualStyleBackColor = true;
            this.bttCerrar.Click += new System.EventHandler(this.bttCerrar_Click);
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(157, 103);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(100, 20);
            this.txtUser.TabIndex = 2;
            this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(157, 146);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(149, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 46);
            this.label1.TabIndex = 4;
            this.label1.Text = "LOGIN";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 451);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.bttCerrar);
            this.Controls.Add(this.bttValidar);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bttValidar;
        private System.Windows.Forms.Button bttCerrar;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
    }
}