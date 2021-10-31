using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HuajiTech.CoolQ;
using System.Threading;

namespace XBridge
{
    public partial class XBridgeForm : Form
    {
        public XBridgeForm()
        {
            InitializeComponent();
            listBox1.Items.Add($"[{DateTime.Now}] XBridge init!");
            foreach(var i in Main.playerdatas)
            {
                try
                {
                    listBox2.Items.Add(i.Value.xboxid);
                }
                catch { }
            }
        }

        private void 打开窗体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                listBox1.Items.Add($"[{DateTime.Now}] " + Func.XB_CMD.runcode(textBox1.Text));
                this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);
                textBox1.Clear();
            }
        }

        private void 查看详情ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listBox2.SelectedIndex != -1)
            {
                var pl = Main.playerdatas.ToArray()[listBox2.SelectedIndex].Key;
                string t = $"QQ:{pl}\nxboxid:{Main.playerdatas[pl].xboxid}";
                Task.Run(() =>
                {
                    MessageBox.Show(t,pl.ToString());
                });
            }

        }

        private void 移除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                var pl = Main.playerdatas.ToArray()[listBox2.SelectedIndex].Key;
                listBox1.Items.Add($"[{DateTime.Now}] 移除了 {Utils.Data.get_xboxid(pl)} 的 白名单");
                Utils.Data.wl_remove(pl);
                Utils.Data.SAVE();
                listBox2.Items.Clear();
                foreach (var i in Main.playerdatas)
                {
                    try
                    {
                        listBox2.Items.Add(i.Value.xboxid);
                    }
                    catch { }
                }
                Task.Run(() =>
                {
                    MessageBox.Show("移除成功");
                });
            }
        }

        private void 刷新列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            foreach (var i in Main.playerdatas)
            {
                try
                {
                    listBox2.Items.Add(i.Value.xboxid);
                }
                catch { }
            }
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    listBox1.Items.Add($"[{DateTime.Now}] " + Func.XB_CMD.runcode(textBox1.Text));
                    this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);
                    textBox1.Clear();
                }
            }
        }

    }
}
