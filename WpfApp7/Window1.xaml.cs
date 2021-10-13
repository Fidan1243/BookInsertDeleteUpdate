using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp7
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        SqlConnection conn;

        private string welcomeText;
        public string WelcomeText
        {
            get { return welcomeText; }
            set { welcomeText = value; OnPropertyChanged(); }
        }

        public int IdStudent { get; set; }
        public int BookId { get; set; } = -1;
        BooksDataContext bd = new BooksDataContext();
        public Window1()
        {
            InitializeComponent();
            GetAllData();
            
        }
        public void Welcome()
        {
            string fname = (from a in bd.Students
                            where a.Id == IdStudent
                            select a.FirstName).Take(1).First();
            string lname = (from a in bd.Students
                            where a.Id == IdStudent
                            select a.LastName).Take(1).First();

            WelcomeText = $"Welcome {fname} {lname}";
            txtb1.Text = welcomeText;
        }
        
        public void GetAllData()
        {
            BooksDg.ItemsSource = from b in bd.Books select new { b.Id,b.Name,b.Pages,b.Quantity,b.YearPress,b.Comment };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void BooksDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gd = (DataGrid)sender;
            dynamic book = gd.SelectedItem;
            if (book != null)
            {
                BookId = book.Id;
                
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"]
                    .ConnectionString;
                await conn.OpenAsync();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = "InsertIntoSCard";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter p1 = new SqlParameter();
                p1.Value = BookId;
                p1.SqlDbType = SqlDbType.NVarChar;
                p1.ParameterName = "@bookId";
                command.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter();
                p2.Value = IdStudent;
                p2.SqlDbType = SqlDbType.NVarChar;
                p2.ParameterName = "@studentId";
                command.Parameters.Add(p2);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("The Book Added your S_card");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bt.IsEnabled = false;
            BooksDg.ItemsSource = from s in bd.S_Cards
                                  where s.Id_Student == IdStudent
                                  select new { s.Id, s.Book.Name, s.Id_Lib, s.DateOut, s.DateIn };

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            bt.IsEnabled = true;
            GetAllData();
        }
    }
}
