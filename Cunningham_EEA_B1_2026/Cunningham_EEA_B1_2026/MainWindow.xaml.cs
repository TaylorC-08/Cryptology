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

namespace Cunningham_EEA_B1_2026
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        long counter = 0; //keeps track of the number of times the loop executes in the inverse
        long GCD(long a, long b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        long inverse(long a, long b)
        {
            long x = 0;
            long y = 1;
            long tempx = 1;
            long tempy = 0;
            long q = 0;
            counter = 0;
            long tempa = a; //Used in case the modulo returns a negative to shift it back

            while(b != 0)
            {
                q = a / b;
                (a, b) = (b, a % b);
                (x, tempx) = (tempx - q * x, x);
                (y, tempy) = (tempy - q * y, y);
                counter++;
            }

            if (tempy < 0)
            {
                tempy += tempa;
            }

            return tempy;
        }
        private void cmdCalculator_Click(object sender, RoutedEventArgs e)
        {
            long a = 0;
            long b = 0;

            bool successA = long.TryParse(txtA.Text, out a);
            bool successB = long.TryParse(txtB.Text, out b);

            if (!successA || !successB)
            {
                lblOutput.Content = "Please enter integers!";
                return;
            }

            if (a < b)
            {
                lblOutput.Content = "a must be bigger than b.";
                return;
            }

            long gcd = GCD(a, b);
            lblOutput.Content = "GCD = " + gcd.ToString();  

            if(gcd != 1)
            {
                lblOutput.Content += "\nNo inverse.";
                return;
            }

            long Inverse = inverse(a, b);
            lblOutput.Content += $"\nInverse = {Inverse:n0}\n{counter}, steps"; // n0 puts comma if big number
        }
    }
}