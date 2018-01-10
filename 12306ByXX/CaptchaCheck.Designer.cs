namespace _12306ByXX
{
    partial class CaptchaCheck
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pb_image = new System.Windows.Forms.PictureBox();
            this.btn_Refresh = new System.Windows.Forms.Button();
            this.btn_Confirm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb_image)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_image
            // 
            this.pb_image.Location = new System.Drawing.Point(2, 2);
            this.pb_image.Margin = new System.Windows.Forms.Padding(2);
            this.pb_image.Name = "pb_image";
            this.pb_image.Size = new System.Drawing.Size(293, 190);
            this.pb_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pb_image.TabIndex = 0;
            this.pb_image.TabStop = false;
            this.pb_image.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pb_image_MouseDown);
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Refresh.Location = new System.Drawing.Point(212, 2);
            this.btn_Refresh.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(40, 20);
            this.btn_Refresh.TabIndex = 1;
            this.btn_Refresh.Text = "刷新";
            this.btn_Refresh.UseVisualStyleBackColor = true;
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Confirm.Location = new System.Drawing.Point(2, 158);
            this.btn_Confirm.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(250, 20);
            this.btn_Confirm.TabIndex = 2;
            this.btn_Confirm.Text = "确定";
            this.btn_Confirm.UseVisualStyleBackColor = true;
            this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // CaptchaCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_Refresh);
            this.Controls.Add(this.btn_Confirm);
            this.Controls.Add(this.pb_image);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CaptchaCheck";
            this.Size = new System.Drawing.Size(294, 225);
            this.Load += new System.EventHandler(this.CaptchaCheck_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_image;
        private System.Windows.Forms.Button btn_Refresh;
        private System.Windows.Forms.Button btn_Confirm;
    }
}
