using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"]
                    .ConnectionString;
                await conn.OpenAsync();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = "SignInOpStdnt";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter p1 = new SqlParameter();
                p1.Value = UN.Text;
                p1.SqlDbType = SqlDbType.NVarChar;
                p1.ParameterName = "@un";
                command.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter();
                p2.Value = Convert.ToString(PW.Password);
                p2.SqlDbType = SqlDbType.NVarChar;
                p2.ParameterName = "@pw";
                command.Parameters.Add(p2);

                SqlParameter p3 = new SqlParameter();
                p3.Value = -1;
                p3.Direction = ParameterDirection.Output;
                p3.SqlDbType = SqlDbType.Int;
                p3.ParameterName = "@st_id";
                command.Parameters.Add(p3);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show(Convert.ToString(p3.Value));
                    var w1 = new Window1();
                    w1.IdStudent = int.Parse(Convert.ToString(p3.Value));
                    w1.Welcome();
                    w1.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
    }
}
