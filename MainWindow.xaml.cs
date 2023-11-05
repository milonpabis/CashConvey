using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

// TODO:
// - create listBox *
// - create class that converts DataTable 2col data into Dictionary *
// - make dictionary a property *
// - connect database *
// - implement logic with database

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dt = new DataTable();
        SqlConnection sqlConnection;
        string connectionString;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=F:\\Desktop\\repos\\C#\\CurrencyConverter\\DataBase\\currencyDatamdf.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            BindData();
        }

        private void BindData()
        {
            dt = new DataTable();
            string query = "select * from Currency_Data";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataAdapter SDA = new SqlDataAdapter(command);

            using (SDA)
            {
                SDA.Fill(dt);
            }

            var myTestDict = ConvertDataTable(dt);

            cbFrom.ItemsSource = myTestDict;
            cbTo.ItemsSource = myTestDict;
            cbFrom.DisplayMemberPath = "Key";
            cbTo.DisplayMemberPath = "Key";

            cbFrom.SelectedIndex = 0;
            cbTo.SelectedIndex = 0;

            var dbData = new Dictionary<string, double>(myTestDict);
            dbData.Remove("SELECT");

            lbData.ItemsSource = dbData;
            
            


        }

        private Dictionary<string, double> ConvertDataTable(DataTable dt)
        {
            Dictionary<string, double> readyDictionary = new Dictionary<string, double>();
            foreach( DataRow row in dt.Rows )
            {
                if (row[1] != null && row[2] != null )
                {
                    string key = row[2].ToString();
                    double value = double.Parse(row[1].ToString());
                    readyDictionary[key] = value;
                }
               
                   
            }
            return readyDictionary;

        }

        private void btConvert_Click(object sender, RoutedEventArgs e)
        {
            if (cbFrom.SelectedItem != null && cbTo.SelectedItem != null && cbTo.SelectedIndex != 0 && cbFrom.SelectedIndex != 0)
            {
                
                var selectedFrom = ((KeyValuePair<string, double>)cbFrom.SelectedItem).Value;
                var selectedTo = ((KeyValuePair<string, double>)cbTo.SelectedItem).Value;

                double amount;
                double result;

                if (double.TryParse(tbAmount.Text, out amount))
                {
                    result = CalculateValue(selectedFrom, selectedTo, amount);
                    lResult.Content = string.Format(amount + " " + cbFrom.Text + "  =  " + result + " " + cbTo.Text);
                }
            }
            tbAmount.Focus();
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            cbFrom.SelectedIndex = 0;
            cbTo.SelectedIndex = 0;
            lResult.Content = string.Empty;
            tbAmount.Text = string.Empty;
            tbAmount.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string currentText = tbAmount.Text.Trim();

            if ( !double.TryParse(e.Text, out _))
                e.Handled = true;
        }

        private double CalculateValue( double fromRate, double toRate, double value )
        {
            double result = value * (toRate / fromRate);
            return Math.Round(result, 2);
        }

        private void tbAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbData.SelectedItem != null)
                {
                    string nameToUpdate = ((KeyValuePair<string, double>)lbData.SelectedItem).Key;

                    string query = "update Currency_Data set Name=@Name, Rate=@Rate where Name=@OldName";
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    command.Parameters.AddWithValue("@Name", tbName.Text);
                    command.Parameters.AddWithValue("@Rate", double.Parse(tbRate.Text));
                    command.Parameters.AddWithValue("@OldName", nameToUpdate);
                    command.ExecuteScalar();
                    
                }
                else
                {
                    string query = "insert into Currency_Data (Rate, Name) values (@Rate, @Name)";
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    command.Parameters.AddWithValue("@Rate", tbRate.Text);
                    command.Parameters.AddWithValue("@Name", tbName.Text);
                    command.ExecuteScalar();
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                BindData();
            }
            
            
            
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            lbData.SelectedItem = null;
            tbName.Text = String.Empty;
            tbRate.Text = String.Empty;
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if( lbData.SelectedItem != null )
            {
                string nameToDel = ((KeyValuePair<string, double>)(lbData.SelectedItem)).Key;
                string query = "delete from Currency_Data where Name = @Name";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                command.Parameters.AddWithValue("@Name", nameToDel);
                command.ExecuteScalar();
                sqlConnection.Close();
                BindData();

                lbData.SelectedItem = null;
            }
        }

        private void lbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( lbData.SelectedItem != null )
            {
                tbName.Text = ((KeyValuePair<string, double>)lbData.SelectedItem).Key;
                tbRate.Text = ((KeyValuePair<string, double>)lbData.SelectedItem).Value.ToString();
            }
            
        }

        private void tbRate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void tbRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if( !int.TryParse(e.Text, out _) )
                e.Handled = true;
        }
    }
}
