namespace _12306ByXX
{
    partial class LoginForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tb_userName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_passWord = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.captchaCheck = new _12306ByXX.CaptchaCheck();
            this.cb_remember = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(78, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "账号";
            // 
            // tb_userName
            // 
            this.tb_userName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_userName.Font = new System.Drawing.Font("微软雅黑", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_userName.Location = new System.Drawing.Point(144, 56);
            this.tb_userName.Margin = new System.Windows.Forms.Padding(4);
            this.tb_userName.Name = "tb_userName";
            this.tb_userName.Size = new System.Drawing.Size(288, 39);
            this.tb_userName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(78, 124);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码";
            // 
            // tb_passWord
            // 
            this.tb_passWord.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_passWord.Font = new System.Drawing.Font("微软雅黑", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_passWord.Location = new System.Drawing.Point(146, 114);
            this.tb_passWord.Margin = new System.Windows.Forms.Padding(4);
            this.tb_passWord.Name = "tb_passWord";
            this.tb_passWord.PasswordChar = '*';
            this.tb_passWord.Size = new System.Drawing.Size(288, 39);
            this.tb_passWord.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.captchaCheck);
            this.groupBox1.Controls.Add(this.cb_remember);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(32, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(780, 732);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "12306";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 170);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 32);
            this.label3.TabIndex = 3;
            this.label3.Text = "验证码";
            // 
            // captchaCheck
            // 
            this.captchaCheck.Agent = null;
            this.captchaCheck.Cookie = ((System.Net.CookieContainer)(resources.GetObject("captchaCheck.Cookie")));
            this.captchaCheck.Location = new System.Drawing.Point(112, 164);
            this.captchaCheck.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.captchaCheck.Name = "captchaCheck";
            this.captchaCheck.RandCode = null;
            this.captchaCheck.Size = new System.Drawing.Size(588, 534);
            this.captchaCheck.TabIndex = 2;
            this.captchaCheck.Token = null;
            this.captchaCheck.Type = null;
            // 
            // cb_remember
            // 
            this.cb_remember.AutoSize = true;
            this.cb_remember.Location = new System.Drawing.Point(428, 108);
            this.cb_remember.Margin = new System.Windows.Forms.Padding(4);
            this.cb_remember.Name = "cb_remember";
            this.cb_remember.Size = new System.Drawing.Size(116, 36);
            this.cb_remember.TabIndex = 1;
            this.cb_remember.Text = "记住我";
            this.cb_remember.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 7.539268F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(512, 694);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 28);
            this.label4.TabIndex = 0;
            this.label4.Text = "Copyright © 2017 by Liu";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 751);
            this.Controls.Add(this.tb_passWord);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_userName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(860, 820);
            this.MinimumSize = new System.Drawing.Size(860, 820);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_userName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_passWord;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cb_remember;
        private CaptchaCheck captchaCheck;
        private System.Windows.Forms.Label label3;

    }
}

