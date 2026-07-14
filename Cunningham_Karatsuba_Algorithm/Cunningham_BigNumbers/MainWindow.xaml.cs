using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

namespace Cunningham_BigNumbers
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.zip?view=netframework-4.8.1
    /// https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger?view=netframework-4.8.1
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //Variable to determine how many times karatsuba multiplication is called
        int multiplyCallCount = 0;
        //Addition algorithm, starts from the back of the numbers to handle carrying
        string Addition(string string1, string string2)
        {
            string Output = "";
            string1 = new string(string1.Where(char.IsDigit).ToArray());
            string2 = new string(string2.Where(char.IsDigit).ToArray());
            int maxLen = Math.Max(string1.Length, string2.Length);
            string1 = string1.PadLeft(maxLen, '0');
            string2 = string2.PadLeft(maxLen, '0');
            var (carry, Out) = string1.Reverse().Zip(string2.Reverse(),
                (num1, num2) => (int)char.GetNumericValue(num1) + (int)char.GetNumericValue(num2)
                ).Aggregate((carry: 0, outString: ""),
                (Acc, sum) => { 
                    int total = sum + Acc.carry;
                    return (total/10, Acc.outString + (total %10).ToString()); 
                }
            );
            if (carry > 0)
            {
                Out = Out + carry.ToString();
            }
            Output = Out;
            return new string(Output.Reverse().ToArray());
        }
        //Subtraction method from BigInteger struct
        string Subtraction(string string1, string string2)
        {
            BigInteger result = BigInteger.Parse(string1) - BigInteger.Parse(string2);
            return result.ToString();
        }
        //Multiplication using karatsuba multiplication
        string BigMultiply (string string1, string string2)
        {
            multiplyCallCount++;
            string1 = new string(string1.Where(char.IsDigit).ToArray());
            string2 = new string(string2.Where(char.IsDigit).ToArray());
            if (string1.Length <= 9 && string2.Length <= 9)
            {
                return (long.Parse(string1) * long.Parse(string2)).ToString();
            }
            int maxLen = Math.Max(string1.Length, string2.Length);
            string1 = string1.PadLeft(maxLen, '0');
            string2 = string2.PadLeft(maxLen, '0');
            int m = maxLen / 2;
            string x1 = string1.Substring(0, maxLen - m);
            string x0 = string1.Substring(maxLen - m);
            string y1 = string2.Substring(0, maxLen - m);
            string y0 = string2.Substring(maxLen-m);
            string z2 = BigMultiply(x1, y1);
            string z0 = BigMultiply(x0, y0);
            string w = BigMultiply(Addition(x1, x0), Addition(y1, y0));
            string z1 = Subtraction(Subtraction(w, z2), z0);
            string result = Addition(Addition(z2 + new string('0', 2 * m),z1 + new string('0', m)),z0);
            result = result.TrimStart('0');
            return result;
        }

        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            string Num1 = txtNum1.Text.ToString();
            string Num2 = txtNum2.Text.ToString();
            string Output = "";
            if (string.IsNullOrEmpty(Num1) || string.IsNullOrEmpty(Num2))
            {
                txtOut.Text = "Error. Please enter both numbers";
                return;
            }
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Output = Addition(Num1, Num2);
            stopwatch.Stop();
            MessageBox.Show($"Calculation completed in {stopwatch.Elapsed.TotalMilliseconds:F3} ms",
                "Time Taken",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            txtOut.Text = Output;
        }

        public void cmdMult_Click(object sender, RoutedEventArgs e)
        {
            string Num1 = txtNum1.Text.ToString();
            string Num2 = txtNum2.Text.ToString();
            string Output = "";
            multiplyCallCount = 0;
            if (string.IsNullOrEmpty(Num1) || string.IsNullOrEmpty(Num2))
            {
                txtOut.Text = "Error. Please enter both numbers";
                return;
            }
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Output = BigMultiply(Num1, Num2);
            stopwatch.Stop();
            MessageBox.Show(
        $"Calculation completed in {stopwatch.Elapsed.TotalMilliseconds:F3} ms\n" +
        $"BigMultiply called {multiplyCallCount} times",
        "Time Taken",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            txtOut.Text = Output;
        }
    }
}