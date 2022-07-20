using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace Picture_Book
{
    internal class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
      
        [STAThread]
        public static void Main(string[] args)
        {
            Form1 fm1 = new Form1();
            Application.Run(fm1);
        }
    }
}