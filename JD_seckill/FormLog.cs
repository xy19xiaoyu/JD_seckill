using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace JD_seckill
{
    public partial class FormLog : DockContent
    {
        public FormLog()
        {
            InitializeComponent();
        }
        public void AddLog(string line)
        {
            if (this.Disposing || this.IsDisposed) return;
            this.Invoke((Action)delegate ()
            {
                this.txtLogs.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\t{line}{Environment.NewLine}");
            });
            log.Debug(line);
        }

    }
}
