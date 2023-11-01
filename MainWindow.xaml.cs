using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindData();
            
            
        }

        private void BindData()
        {
            Dictionary<string, double> dictData = new Dictionary<string, double>();
            dictData.Add("SELECT", 0);
            dictData.Add("USD", 1);
            dictData.Add("EUR", 0.95);
            dictData.Add("PLN", 4.23);
            dictData.Add("GBP", 0.82);

            cbFrom.ItemsSource = dictData;
            cbTo.ItemsSource = dictData;
            cbFrom.DisplayMemberPath = "Key";
            cbTo.DisplayMemberPath = "Key";

            cbFrom.SelectedIndex = 0;
            cbTo.SelectedIndex = 0;
            
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
    }
}
