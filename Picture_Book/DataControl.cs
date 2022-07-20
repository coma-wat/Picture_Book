using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace Picture_Book
{
    class DataControl
    {
        // data.db にテーブルがない場合、テーブルを作成
        /* base_info テーブルの各フォームの見出し対応表「(データベース上のカラム名) = (見出し名)」
            id = ID
            name => 植物名
            scientific_name => 学名
            family => 科名
            genus => 属名
            native_environment => 自生環境
            explanation => 説明
            created_at => 登録日
            updated_at => 更新日
         */
        public void CreateTable(SQLiteCommand cmd)
        {
            // base_info テーブルを作成
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS base_info(" +
                                "id INTEGER NOT NULL PRIMARY KEY," +
                                "name TEXT NOT NULL," +
                                "scientific_name TEXT NOT NULL," +
                                "family TEXT NOT NULL," +
                                "genus TEXT NOT NULL," +
                                "native_environment TEXT NOT NULL," +
                                "explanation TEXT NOT NULL," +
                                "created_at TEXT NOT NULL," +
                                "updated_at TEXT NOT NULL)";
            cmd.ExecuteNonQuery();

            // picture_info テーブルを作成
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS picture_info(" +
                               "id INTEGER NOT NULL," +
                               "filepath TEXT NOT NULL," +
                               "FOREIGN KEY(id) REFERENCES base_info(id))";
            cmd.ExecuteNonQuery();
        }

        // DataGridView用のデータテーブル(dt)に data.db 内の base_info テーブルを読み込む
        public void DataGridViewRead(DataTable dt)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = "data.db" };
            using (SQLiteConnection cn = new SQLiteConnection(sb.ToString()))
            {
                cn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cn))
                {
                    CreateTable(cmd);

                    cmd.CommandText = @"SELECT id, name, scientific_name, family, genus, created_at, updated_at, native_environment, explanation " +
                                        "FROM base_info " +
                                        "ORDER BY name ASC";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
        }

        // Form2、Form3に表示する base_info テーブルをデータテーブル(dt1)に読み込む(※ DataGridViewRead とは列の順番が異なる)
        public void DataRead(DataTable dt1)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = "data.db" };
            using (SQLiteConnection cn = new SQLiteConnection(sb.ToString()))
            {
                cn.Open();
                using(SQLiteCommand cmd = new SQLiteCommand(cn))
                {
                    CreateTable(cmd);

                    cmd.CommandText = @"SELECT id, name, scientific_name, family, genus, native_environment, explanation, created_at, updated_at FROM base_info";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    da.Fill(dt1);
                }
            }
        }

        // Form2、Form3で表示する画像の情報をデータテーブルに格納
        public void PictureDataRead(DataTable dt2)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = "data.db" };
            using (SQLiteConnection cn = new SQLiteConnection(sb.ToString()))
            {
                cn.Open();
                using(SQLiteCommand cmd = new SQLiteCommand(cn))
                {
                    cmd.CommandText = @"SELECT id, filepath FROM picture_info";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    da.Fill(dt2);
                }
            }
        }

        // データテーブルから filepath を取り出す
        public string GetImageFilePath(DataTable dt2, int _id)
        {
            DataRow[] picRow = dt2.AsEnumerable()
                                .Where(i => i["id"].ToString() == _id.ToString())
                                .ToArray();
            string filepath = picRow[0][1].ToString();
            return filepath;
        }
    }
}
