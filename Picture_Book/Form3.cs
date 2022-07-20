using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Picture_Book
{
    public partial class Form3 : Form
    {
        string picPath = "";            // 画像のファイルパスを格納する変数
        DataTable dt1, dt2;             // dt1:base_info    dt2:picture_info からのデータを格納するdatatable
        DataControl dc;                 // データベース(data.db)を操作するための自作クラス
        PictureBox pb;                  // 植物の画像を表示するためのオブジェクト
        Button[] bt = new Button[3];    // bt[0]:保存して閉じる   bt[1]:データ削除  bt[2]:キャンセル 各ボタン
        Button pictureButton;           // 「画像を設定」ボタン
        Label[] lb1 = new Label[6];     // 見出し
        TextBox[] tb = new TextBox[7];  // データ入力用のテキストボックス

        public Form3()                  // Form1 で「データ追加」ボタンを押したときの Form3(引数なし)
        {
            this.Text = "Picture Book";
            this.Width = 800; this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            for (int i = 0; i < bt.Length; i++)
            {
                bt[i] = new Button();
                bt[i].Width = 100;
                bt[i].Top = 10;
                bt[i].Left = this.Width / 4 * (i + 1) - bt[i].Width / 2;
            }
            bt[0].Text = "保存して閉じる";
            bt[1].Text = "データ削除";
            bt[1].Enabled = false;          // Form1で「データ追加」を選択したとき、「データ削除」ボタン(bt[1])を非アクティブに
            bt[2].Text = "キャンセル";

            picPath = @"Image/noimage.png";         // 最初は「Image」フォルダ内の「noimage.png」を画像に設定
            pb = new PictureBox();
            pb.Width = this.Width / 2; pb.Height = pb.Width / 4 * 3;
            pb.Location = new Point(20, bt[0].Bottom + 30);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = Image.FromFile(picPath);

            pictureButton = new Button();
            pictureButton.Text = "画像を設定";
            pictureButton.Location = new Point(pb.Left + pb.Width / 2 - pictureButton.Width / 2, pb.Bottom + 10);

            for (int i = 0; i < lb1.Length; i++)
            {
                lb1[i] = new Label();
                lb1[i].Location = i == 0 ? new Point(pb.Right + 20, pb.Top + 10) : new Point(lb1[0].Left, lb1[i - 1].Bottom + 20);
                lb1[i].Font = new Font(lb1[i].Font, FontStyle.Bold);
            }
            lb1[0].Text = "ID";
            lb1[1].Text = "植物名";
            lb1[2].Text = "学名";
            lb1[3].Text = "科名";
            lb1[4].Text = "属名";
            lb1[5].Text = "自生環境";

            for (int i = 0; i < tb.Length - 1; i++)
            {
                tb[i] = new TextBox();
                tb[i].Location = i == 0 ? new Point(lb1[0].Right, lb1[i].Top) : new Point(tb[i - 1].Left, lb1[i].Top);
                tb[i].AutoSize = false;
                tb[i].Width = 220;
            }
            tb[0].Enabled = false;          // IDは自動で割り振るため tb[0] は非アクティブに
            tb[5].Height = 70;
            tb[5].Multiline = true;         // 「自生環境(tb[5])」を複数行に
            tb[5].AcceptsReturn = true;     // 「自生環境(tb[5])」内で改行を有効に
            tb[5].ScrollBars = ScrollBars.Vertical;
            tb[6] = new TextBox();
            tb[6].Location = new Point(pb.Left, pb.Bottom + 50);
            tb[6].AutoSize = false;
            tb[6].Width = this.Width - 60; tb[6].Height = this.Height / 3;
            tb[6].Multiline = true;         // 「説明(tb[6])」を複数行に
            tb[6].AcceptsReturn = true;     // 「説明(tb[6])」内で改行を有効に
            tb[6].ScrollBars = ScrollBars.Vertical;

            for (int i = 0; i < bt.Length; i++)
            {
                bt[i].Parent = this;
            }
            pb.Parent = this;
            pictureButton.Parent = this;
            for (int i = 0; i < lb1.Length; i++)
            {
                lb1[i].Parent = this;
            }
            for (int i = 0; i < tb.Length; i++)
            {
                tb[i].Parent = this;
            }

            for (int i = 0; i < tb.Length; i++)
            {
                if (i == 5) break;                                          // 「自生環境(tb[5])」と「説明(tb[6])」には音がならない処理が必要ない
                tb[i].KeyPress += new KeyPressEventHandler(Tb_KeyPress);    // テキストボックスでEnterキーを押したときに音がならない処理
            }
            for(int i = 0; i < bt.Length; i++)
            {
                bt[i].Click += new EventHandler(Bt_Click);                  // 「保存して閉じる」「データ削除」「キャンセル」ボタンのイベント
            }
            pictureButton.Click += new EventHandler(PictureButton_Click);   // 「画像を設定」ボタンのイベント
        }

        public Form3(int _id) : this()      // Form1、Form2 で「編集」ボタンを選択したときの Form3(引数にID)
        {
            dt1 = new DataTable();
            dt2 = new DataTable();
            dc = new DataControl();
            dc.DataRead(dt1);               // dt1 に base_info のデータをコピー
            dc.PictureDataRead(dt2);        // dt2 に picture_info のデータをコピー

            // _id と一致するレコードから各カラムごとに対応するテキストボックスに格納する
            try
            {
                // dt1 からコンストラクタ引数(_id)と一致するレコードを抽出
                DataRow[] row = dt1.AsEnumerable()
                                    .Where(i => i["id"].ToString() == _id.ToString())
                                    .ToArray();
                for (int i = 0; i < tb.Length; i++)
                {
                    tb[i].Text = row[0][i].ToString();      // 抽出したレコードから各カラムのデータを対応するテキストボックスに格納
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // pictureboxに画像を読み込む
            try
            {
                picPath = dc.GetImageFilePath(dt2, _id);    // dt2 からコンストラクタ引数(_id)に対応する filepath を picPath にコピー
                pb.Image = Image.FromFile(picPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            if (tb[0].Text != "")       // ID(tb[0]) に数字が入っているとき(新規作成ではないとき)「データ削除」ボタンをアクティブに
            {
                bt[1].Enabled = true;
            }
        }

        // テキストボックス内でEnterキーを押しても音が鳴らないようにする
        public void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }

        public void Bt_Click(Object sender, EventArgs e)
        {
            if(sender == bt[0])     // 「保存して閉じる」ボタンのイベント
            {
                SQLiteConnectionStringBuilder sqlConnectionSb = new SQLiteConnectionStringBuilder { DataSource = @"data.db" };
                using (SQLiteConnection cn = new SQLiteConnection(sqlConnectionSb.ToString()))
                {
                    cn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(cn))
                    {
                        dt1 = new DataTable();
                        int newId = 0;

                        switch(tb[0].Text)
                        {
                            case "":            // Form1 で「データ追加」(引数にIDがない)を選択したときの処理
                                try
                                {
                                    try
                                    {
                                        cmd.CommandText = "SELECT MIN(id + 1) AS id " +
                                                            "FROM base_info " +
                                                            "WHERE (id + 1) NOT IN (SELECT id FROM base_info)";
                                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                                        da.Fill(dt1);
                                        newId = Convert.ToInt32(dt1.Rows[0][0]);
                                    }
                                    catch
                                    {
                                        
                                    }

                                    DialogResult dr = MessageBox.Show($"「{tb[1].Text}」 をデータベースに追加します。"
                                                                        , "確認", MessageBoxButtons.OKCancel
                                                                        , MessageBoxIcon.Question);
                                    if (dr == DialogResult.OK)
                                    {
                                        cmd.CommandText = "INSERT INTO base_info VALUES (" +
                                                         $"'{newId}'," +
                                                         $"'{tb[1].Text}'," +
                                                         $"'{tb[2].Text}'," +
                                                         $"'{tb[3].Text}'," +
                                                         $"'{tb[4].Text}'," +
                                                         $"'{tb[5].Text}'," +
                                                         $"'{tb[6].Text}'," +
                                                         $"'{DateTime.Now.ToShortDateString()}'," +
                                                         $"'{DateTime.Now.ToShortDateString()}')";
                                        cmd.ExecuteNonQuery();      // base_info テーブルにデータを追加

                                        cmd.CommandText = "INSERT INTO picture_info VALUES (" +
                                                            $"'{newId}'," +
                                                            $@"'{picPath}')";
                                        cmd.ExecuteNonQuery();      // picture_info テーブルにデータを追加

                                        this.Close();
                                    }
                                }
                                catch(SQLiteException ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                break;

                            default:                    // 既存のデータ(すでにIDがある場合)を更新する処理
                                try
                                {
                                    cmd.CommandText = $"SELECT name FROM base_info WHERE id = {Convert.ToInt32(tb[0].Text)}";
                                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                                    da.Fill(dt1);
                                    DialogResult dr = MessageBox.Show($"ID:{tb[0].Text}\n" +
                                                                        $"「{dt1.Rows[0][0]}」→ 「{tb[1].Text}」に上書きします。"
                                                                        , "確認"
                                                                        , MessageBoxButtons.OKCancel
                                                                        , MessageBoxIcon.Question);
                                    if(dr == DialogResult.OK)
                                    {
                                        cmd.CommandText = "UPDATE base_info SET " +
                                                         $"name = '{tb[1].Text}', " +
                                                         $"scientific_name = '{tb[2].Text}', " +
                                                         $"family = '{tb[3].Text}', " +
                                                         $"genus = '{tb[4].Text}', " +
                                                         $"native_environment = '{tb[5].Text}', " +
                                                         $"explanation = '{tb[6].Text}', " +
                                                         $"updated_at = '{DateTime.Now.ToShortDateString()}' " +
                                                         $"WHERE id = '{Convert.ToInt32(tb[0].Text)}'";
                                        cmd.ExecuteNonQuery();

                                        cmd.CommandText = $@"UPDATE picture_info SET " +
                                                            $@"filepath = '{picPath}'" +
                                                            $"WHERE id = '{Convert.ToInt32(tb[0].Text)}'";
                                        cmd.ExecuteNonQuery();

                                        this.Close();
                                    }
                                }
                                catch(SQLiteException ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    MessageBox.Show(picPath);
                                }
                                break;
                        }
                    }
                }
            }
            else if(sender == bt[1])            // 「データ削除」ボタンのイベント
            {
                SQLiteConnectionStringBuilder sqlConnectionSb = new SQLiteConnectionStringBuilder { DataSource = @"data.db" };
                using (SQLiteConnection cn = new SQLiteConnection(sqlConnectionSb.ToString()))
                {
                    cn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(cn))          // cmd に SQLiteConnection(cn) を設定
                    {
                        try
                        {
                            dt1 = new DataTable();
                            cmd.CommandText = $"SELECT id, name FROM base_info WHERE id = {Convert.ToInt32(tb[0].Text)}";
                            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                            da.Fill(dt1);
                            DialogResult dr = MessageBox.Show($"ID: {dt1.Rows[0][0]}\n" +
                                                                $"植物名: {dt1.Rows[0][1]}\n" +
                                                                $"を削除します。よろしいですか？"
                                                                , "確認"
                                                                , MessageBoxButtons.OKCancel
                                                                , MessageBoxIcon.Question);
                            if (dr == DialogResult.OK)
                            {
                                cmd.CommandText = $"DELETE FROM picture_info WHERE id = {Convert.ToInt32(tb[0].Text)}";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = $"DELETE FROM base_info WHERE id = {Convert.ToInt32(tb[0].Text)}";
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("削除しました。");
                                this.Close();
                            }
                        }catch(SQLiteException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            else if(sender == bt[2])        // 「キャンセル」ボタンの処理
            {
                this.Close();
            }
        }

        // 「画像を設定」ボタンの処理
        public void PictureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)         // 画像ファイルを選択したときの処理
            {
                dt2 = new DataTable();
                dc = new DataControl();
                dc.PictureDataRead(dt2);
                try
                {
                    string picFullPath = ofd.FileName;
                    picPath = Path.GetFileName(picFullPath);
                    string folderName = Path.GetDirectoryName(picFullPath);
                    if (folderName.Substring(folderName.Length - "Pictures".Length) == "Pictures")
                    {
                        picPath = $@"Pictures/{picPath}";
                        pb.Image = Image.FromFile(picPath);
                    }
                    else
                    {
                        picPath = picFullPath;
                        pb.Image = Image.FromFile(picPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
