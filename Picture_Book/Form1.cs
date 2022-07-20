using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Picture_Book
{
    public partial class Form1 : Form
    {
        Label topLb;                        // Form1のタイトル「検索」
        Label[] lb = new Label[4];
        TextBox[] tb = new TextBox[4];
        Button[] bt = new Button[3];        // 「絞り込み」「編集」「データ追加」ボタン
        DataTable dt;
        DataControl dc;                     // 自作した DataControl クラス
        DataGridView dgv;
        Button updateBt;                    // 「更新」ボタン
        Form2 fm2;
        Form3 fm3;

        public Form1()
        {
            this.Text = "Picture Book";
            this.Width = 800; this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            topLb = new Label();
            topLb.Text = "検索";
            topLb.AutoSize = true;
            topLb.Location = new Point(this.Width / 2 - topLb.Width / 2, 10);
            topLb.Font = new Font("Arial", 12);

            for (int i = 0; i < lb.Length; i++)
            {
                lb[i] = new Label();
                if(i % 2 == 0)
                {
                    lb[i].Location = new Point(30 , topLb.Bottom + 30 * i);
                }
                else if(i % 2 == 1)
                {
                    lb[i].Location = new Point(this.Width / 2 , lb[i - 1].Top);
                }
            }
            lb[0].Text = "科名";
            lb[1].Text = "属名";
            lb[2].Text = "植物名";
            lb[3].Text = "キーワード";

            for(int i = 0; i < tb.Length; i++)
            {
                tb[i] = new TextBox();
                tb[i].Width = this.Width / 2 - 60;
                tb[i].Location = new Point(lb[i].Left, lb[i].Bottom);
            }

            for(int i = 0; i < bt.Length; i++)
            {
                bt[i] = new Button();
                bt[i].AutoSize = true;
                bt[i].Location = new Point(this.Width / 5 * (i + 1) + (i * 40) , tb[tb.Length - 1].Bottom + 20);
            }
            bt[0].Text = "絞り込み";
            bt[1].Text = "編集";
            bt[2].Text = "データ追加";

            dt = new DataTable();

            dc = new DataControl();         // データベースファイル操作用のクラスインスタンス
            dc.DataGridViewRead(dt);        // データグリッドビュー用のデータを data.db から dt に読み込む
            ColumnNameChange(dt);           // datatable のカラム名(列名)を日本語に変更

            dgv = new DataGridView();
            dgv.Location = new Point(30, bt[0].Bottom + 20);
            dgv.Width = Convert.ToInt32(this.Width * 0.9);
            dgv.Height = Convert.ToInt32(this.Height * 0.6);
            dgv.ScrollBars = ScrollBars.Both;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.DataSource = dt;

            updateBt = new Button();
            updateBt.Text = "更新";
            updateBt.Location = new Point(this.Width / 2 - updateBt.Width / 2, dgv.Bottom + 10);

            topLb.Parent = this;

            for(int i = 0; i < lb.Length; i++)
            {
                lb[i].Parent = this;
            }

            for(int i = 0; i < tb.Length; i++)
            {
                tb[i].Parent = this;
            }

            for(int i = 0; i < bt.Length; i++)
            {
                bt[i].Parent = this;
            }

            dgv.Parent = this;

            updateBt.Parent = this;

            for (int i = 0; i < tb.Length; i++)
            {
                tb[i].KeyPress += new KeyPressEventHandler(Tb_KeyPress);
            }
            bt[0].Click += new EventHandler(Bt_Click);
            bt[1].Click += new EventHandler(Bt_Click);
            bt[2].Click += new EventHandler(Bt_Click);
            updateBt.Click += new EventHandler(Bt_Click);
            dgv.CellDoubleClick += new DataGridViewCellEventHandler(Cell_DoubleClick);
        }

        // テキストボックスにフォーカス時、ビープ音を鳴らさないためのイベント
        public void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }

        // dt のカラム名を日本語に変更
        public void ColumnNameChange(DataTable dt)
        {
            dt.Columns["id"].ColumnName = "ID";
            dt.Columns["name"].ColumnName = "植物名";
            dt.Columns["scientific_name"].ColumnName = "学名";
            dt.Columns["family"].ColumnName = "科名";
            dt.Columns["genus"].ColumnName = "属名";
            dt.Columns["created_at"].ColumnName = "登録日";
            dt.Columns["updated_at"].ColumnName = "更新日";
            dt.Columns["native_environment"].ColumnName = "自生環境";
            dt.Columns["explanation"].ColumnName = "説明";
        }

        // ボタンクリック時のイベント
        public void Bt_Click(Object sender, EventArgs e)
        {
            if(sender == bt[0])                 // 「絞り込み」ボタンの処理
            {
                DataTable_Extract(dt, dgv);
            }
            else if(sender == bt[1])            // 「編集」ボタンの処理
            {
                int id = int.Parse(dgv[0, dgv.CurrentCell.RowIndex].Value.ToString());
                fm3 = new Form3(id);
                fm3.Show();
            }
            else if(sender == bt[2])            // 「データ追加」ボタンの処理
            {
                fm3 = new Form3();
                fm3.Show();
            }
            else if(sender == updateBt)         // 「更新」ボタンの処理
            {
                dgv.Columns.Clear();
                dt = new DataTable();
                dc.DataGridViewRead(dt);
                ColumnNameChange(dt);
                dgv.DataSource = dt;
            }
        }

        // DataGridView のセルをダブルクリックした時の処理
        public void Cell_DoubleClick(Object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int id = int.Parse(dgv[0, dgv.CurrentCell.RowIndex].Value.ToString());
                fm2 = new Form2(id);
                fm2.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 「絞り込み」ボタンクリック時のデータ抽出機能
        public void DataTable_Extract(DataTable dt, DataGridView dgv)
        {
            try
            {
                dgv.Columns.Clear();        // データグリッドビュー内のデータをクリア

                // テキストボックス内の文字列にヒットするレコードをデータテーブルから抽出(カラム名は日本語)
                DataTable result = dt.AsEnumerable()
                                        .Where(i => i["科名"].ToString().Contains(tb[0].Text)
                                        && i["属名"].ToString().Contains(tb[1].Text)
                                        && i["植物名"].ToString().Contains(tb[2].Text))
                                        .Where(i => i["科名"].ToString().Contains(tb[3].Text)
                                        || i["属名"].ToString().Contains(tb[3].Text)
                                        || i["植物名"].ToString().Contains(tb[3].Text)
                                        || i["自生環境"].ToString().Contains(tb[3].Text)
                                        || i["説明"].ToString().Contains(tb[3].Text))
                                        .CopyToDataTable();
                dgv.DataSource = result;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
