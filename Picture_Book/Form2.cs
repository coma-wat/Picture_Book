using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SQLite;

namespace Picture_Book
{
    public partial class Form2 : Form
    {
        int id;
        string picPath = "";
        DataTable dt1, dt2;
        DataControl dc;
        PictureBox pb;
        Button[] bt = new Button[2];
        Label[] lb1 = new Label[6];
        Label[] lb2 = new Label[7];

        public Form2(int _id)
        {
            this.Text = "Picture Book";
            this.Width = 800; this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            id = _id;

            dt1 = new DataTable();
            dt2 = new DataTable();
            dc = new DataControl();
            dc.DataRead(dt1);
            dc.PictureDataRead(dt2);
            DataRow[] dr = dt1.Select($"ID = {id}");
            int index = dt1.Rows.IndexOf(dr[0]);
            picPath = dc.GetImageFilePath(dt2, id);

            for (int i = 0; i < bt.Length; i++)
            {
                bt[i] = new Button();
                bt[i].Top = 10;
                bt[i].Left = i == 0 ? this.Width / 4 : this.Width / 2 + bt[i].Width;
                bt[i].Text = i == 0 ? "編集" : "閉じる";
            }

            pb = new PictureBox();
            pb.Width = this.Width / 2; pb.Height = pb.Width / 4 * 3;
            pb.Location = new Point(20, bt[0].Bottom + 30);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = Image.FromFile(picPath);

            for(int i = 0; i < lb1.Length; i++)
            {
                lb1[i] = new Label();
                lb1[i].Location = i == 0 ? new Point(pb.Right + 20, pb.Top + 10) : new Point(lb1[0].Left, lb1[i-1].Bottom + 20);
                lb1[i].Font = new Font(lb1[i].Font, FontStyle.Bold);
            }
            lb1[0].Text = "ID";
            lb1[1].Text = "植物名";
            lb1[2].Text = "学名";
            lb1[3].Text = "科名";
            lb1[4].Text = "属名";
            lb1[5].Text = "自生環境";

            // 植物データ格納用のラベル(lb2)にデータを格納(lb2[6]は「説明」なのでforループ外で設定)
            for (int i = 0; i < lb2.Length - 1; i++)
            {
                lb2[i] = new Label();
                lb2[i].Location = i == 0 ? new Point(lb1[0].Right, lb1[i].Top) : new Point(lb2[i - 1].Left, lb1[i].Top);
                lb2[i].AutoSize = false;
                lb2[i].Width = 220;
                lb2[i].Text = dt1.Rows[index][i].ToString();
            }
            lb2[5].Height = 70;
            lb2[6] = new Label();
            lb2[6].Location = new Point(pb.Left, lb2[5].Bottom + 30);
            lb2[6].AutoSize = false;
            lb2[6].Width = this.Width - 60; lb2[6].Height = this.Height / 3;
            lb2[6].Text = dt1.Rows[index][6].ToString();

            for (int i = 0; i < bt.Length; i++)
            {
                bt[i].Parent = this;
            }
            pb.Parent = this;
            for(int i = 0; i < lb1.Length; i++)
            {
                lb1[i].Parent = this;
            }
            for(int i = 0; i < lb2.Length; i++)
            {
                lb2[i].Parent = this;
            }
            lb2[6].Parent = this;

            bt[0].Click += new EventHandler(Bt_Click); 
            bt[1].Click += new EventHandler(Bt_Click);
        }

        public void Bt_Click(Object sender, EventArgs e)
        {
            if(sender == bt[0])
            {
                Program.mainFormContext.MainForm = new Form3(id);
                Program.mainFormContext.MainForm.Show();
                this.Close();
            }
            else if(sender == bt[1])
            {
                Program.mainFormContext.MainForm = new Form1();
                Program.mainFormContext.MainForm.Show();
                this.Close();
            }
        }
    }
}
