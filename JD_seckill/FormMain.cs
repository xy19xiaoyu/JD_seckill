using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JD_seckill
{
    public partial class FormMain : Form, IShowLog
    {
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            CreateItem();
            CreateLogItem();
            AddLog("启动成功");
        }

        private void Frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }
        public FormLog FrmLog { get; set; }

        public void CreateLogItem()
        {
            FrmLog = new FormLog();
            FrmLog.Show(this.DPContent);
            FrmLog.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;

        }


        private void CreateItem()
        {
            FormWBItem frm = new FormWBItem();
            frm.Show(this.DPContent);
            frm.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            frm.FormClosed += Frm_FormClosed;
            frm.ShowLogEvent += Frm_ShowLogEvent;
        }

        private void Frm_ShowLogEvent(string log)
        {
            AddLog(log);
        }

        public void AddLog(string line)
        {
            FrmLog.AddLog(line);
        }
    }
}
