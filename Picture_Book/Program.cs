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

        public static ApplicationContext mainFormContext;

        [STAThread]
        public static void Main(string[] args)
        {
            mainFormContext = new ApplicationContext();
            mainFormContext.MainForm = new Form1();
            Application.Run(mainFormContext);
        }
    }
}