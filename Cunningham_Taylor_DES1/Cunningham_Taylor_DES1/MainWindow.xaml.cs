using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace Cunningham_Taylor_DES1
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
    /// https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select?view=net-10.0
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // PC-1 permutation table from Fthe NIST document
        private static readonly int[] PC1 =
        {
            57, 49, 41, 33, 25, 17, 9,
            1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27,
            19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29,
            21, 13, 5, 28, 20, 12, 4
        };
        private void txtASCII_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Error checks
            String ASCII = txtASCII.Text.ToString();
            if (string.IsNullOrEmpty(ASCII))
            {
                txtBinary.Text = "";
                return;
            }
            if (ASCII.Length != 7)
            {
                txtBinary.Text = "Type more text.";
                return;
            }
            //Get input text to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(ASCII);
            string Binary = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
            String BinaryAppend = "";
            int numbytes = ASCII.Length;
            //Append for odd parity
            for (int i = 0; i < 8; i++)
            {
                String CurrentByte = Binary.Substring(i * 7, 7);
                int num1s = CurrentByte.Count(c => c == '1');
                if (num1s % 2 == 0)
                {
                    CurrentByte += "1";
                } else
                {
                    CurrentByte += "0";
                }
                BinaryAppend += CurrentByte.ToString();
            }
            txtBinary.Text = BinaryAppend;
            string PC1Input = txtBinary.Text;
            //Error checks for PC1
            if (string.IsNullOrEmpty(PC1Input))
            {
                txtPC1.Text = "";
                return;
            }
            if (PC1Input.Length != 64 || !PC1Input.All(c => c == '0' || c == '1'))
            {
                txtPC1.Text = "Error: Input must be exactly 64 binary digits.";
                return;
            }
            //Calculate PC1
            string result = string.Join("", PC1.Select(position => PC1Input[position - 1]));
            txtPC1.Text = result;
        }
    }
}