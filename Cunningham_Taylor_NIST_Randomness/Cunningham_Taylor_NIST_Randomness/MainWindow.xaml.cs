using System.Security.Cryptography.Xml;
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

namespace Cunningham_Taylor_NIST_Randomness
{
    /// <summary>
    /// Sources:
    /// https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.convert?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.all?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.range?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.string.padleft?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.string.split?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/csharp/how-to/parse-strings-using-split
    /// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.count?view=net-10.0
    /// Most of the code in this is just copy and pasted from my SRT II assignments or used the above sources for reference
    /// LINQ is amazing and is used for most of this but I don't understand the syntax for it at all - hence the 10+ microsoft learn tabs I had open coding this.
    /// </summary>
    public partial class MainWindow : Window
    {
        //Random scary math code provided by Mr. Evans

        //Code provided in cooperation between Microsoft and NIST (Government Special Issue 2013 Code Downloads)

        public class GammaFunctions
        {
            public static double GammaLower(double a, double x)
            {
                // incomplete Gamma 'P' (lower) aka 'igam'
                if (x < 0.0 || a <= 0.0)
                    throw new Exception("Bad args in GammaLower");
                if (x < a + 1)
                    return GammaLowerSer(a, x); // no surprise
                else
                    return 1.0 - GammaUpperCont(a, x); // indirectly is faster
            }

            public static double GammaUpper(double a, double x)
            {
                // incomplete Gamma 'Q' (upper) == (1 - GammaP) but computed for efficiency
                // aka 'igamc' (incomplete gamma complement)
                if (x < 0.0 || a <= 0.0)
                    throw new Exception("Bad args in GammaUpper");
                if (x < a + 1)
                    return 1.0 - GammaLowerSer(a, x); // indirect is faster
                else
                    return GammaUpperCont(a, x);
            }

            // -------------------------------------------------------------------------------

            private static double LogGamma(double x)
            {
                // Log of Gamma from Lanczos with g=5, n=6/7
                // not in A & S 
                double[] coef = new double[6] { 76.18009172947146, -86.50532032941677,24.01409824083091, -1.231739572450155, 0.1208650973866179E-2, -0.5395239384953E-5 };
                double LogSqrtTwoPi = 0.91893853320467274178;
                double denom = x + 1;
                double y = x + 5.5;
                double series = 1.000000000190015;
                for (int i = 0; i < 6; ++i)
                {
                    series += coef[i] / denom;
                    denom += 1.0;
                }
                return (LogSqrtTwoPi + (x + 0.5) * Math.Log(y) - y + Math.Log(series / x));
            }

            private static double GammaLowerSer(double a, double x)
            {
                // incomplete gamma lower (computed by series expansion)
                if (x < 0.0)
                    throw new Exception("x param less than 0.0 in GammaLowerSer");

                double gln = LogGamma(a);
                double ap = a;
                double del = 1.0 / a;
                double sum = del;
                for (int n = 1; n <= 100; ++n)
                {
                    ++ap;
                    del *= x / ap;
                    sum += del;
                    if (Math.Abs(del) < Math.Abs(sum) * 3.0E-7) // close enough?
                        return sum * Math.Exp(-x + a * Math.Log(x) - gln);
                }
                throw new Exception("Unable to compute GammaLowerSer to desired accuracy");
            }

            private static double GammaUpperCont(double a, double x)
            {
                // incomplete gamma upper computed by continuing fraction
                if (x < 0.0)
                    throw new Exception("x param less than 0.0 in GammaUpperCont");
                double gln = LogGamma(a);
                double b = x + 1.0 - a;
                double c = 1.0 / 1.0E-30; // div by close to double.MinValue
                double d = 1.0 / b;
                double h = d;
                for (int i = 1; i <= 100; ++i)
                {
                    double an = -i * (i - a);
                    b += 2.0;
                    d = an * d + b;
                    if (Math.Abs(d) < 1.0E-30) d = 1.0E-30; // got too small?
                    c = b + an / c;
                    if (Math.Abs(c) < 1.0E-30) c = 1.0E-30;
                    d = 1.0 / d;
                    double del = d * c;
                    h *= del;
                    if (Math.Abs(del - 1.0) < 3.0E-7)
                     return Math.Exp(-x + a * Math.Log(x) - gln) * h;  // close enough?
                }
                throw new Exception("Unable to compute GammaUpperCont to desired accuracy");
            }

        }
        static double Erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return 1 - (sign * y);
        }

        static double Erfc(double x)
        {
            double s = 0;
            double c = 1.128379167; //2/sqrt(Pi)
            for (int i = 0; i < 15; i++)
            {
                s += (Math.Pow(-1, i) * Math.Pow(x, 2 * i + 1)) / ((2 * i + 1) * Factorial(i));
            }
            return 1 - (c * s);
        }

        static double Factorial(double x)
        {
            double fact = 1;
            for (int i = 1; i <= x; i++)
            {
                fact = fact * i;
            }
            return fact;
        }
        //Hex determination for hex chacking
        private bool IsHex(string text)
        {
            foreach (char c in text)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }
            return true;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        //Start of my (not as scary) code
        //Hex change remains untouched because all the block text examples are in binary
        private void txtHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            double mean = 0;
            int sum1 = 0;
            int sum0 = 0;
            int S = 0;
            //Error checks
            if (string.IsNullOrEmpty(txtHex.Text))
            {
                txtBinary.Text = "Enter some Hexidecimal or Binary input.";
                return;
            }
            String Input = txtHex.Text.ToString();
            txtBinary.Text = "";
            txtPvalue.Text = "";
            //Convert to binary using LINQ to do each character at a time
            string binary = string.Join("", Input.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));            
            //Output binary
            txtBinary.Text = binary;
            //Counts for means
            foreach (char c in binary)
            {
                mean += (c - '0');
                if(c == '1')
                {
                    sum1++;
                } else
                {
                    sum0++;
                }
            }
            //Determine P value
            mean = (mean/binary.Length);
            S = sum1 - sum0;
            double S_obs = Math.Abs(sum1 - sum0) / Math.Sqrt(binary.Length);
            double Pvalue = Erfc(S_obs / Math.Sqrt(2));
            txtPvalue.Text = Pvalue.ToString();
        }
        //Checks Hex input for Hex characters
        private void txtHex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsHex(e.Text);
        }

        private void txtBinary_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Error checks
            if (string.IsNullOrEmpty(txtBinary.Text))
            {
                txtBinary.Text = "Enter some Hexidecimal or Binary input.";
                return;
            }
            //Declarations
            int M = 0;
            int N = 0;
            double mean = 0;
            int sum1 = 0;
            int sum0 = 0;
            int S = 0;
            String binary = txtBinary.Text.ToString();
            N = binary.Length;
            //Counts
            foreach (char c in binary)
            {
                mean += (c - '0');
                if (c == '1')
                {
                    sum1++;
                }
                else
                {
                    sum0++;
                }
            }
            //Determine P value for monobits
            mean = (mean / binary.Length);
            S = sum1 - sum0;
            double S_obs = Math.Abs(sum1 - sum0) / Math.Sqrt(binary.Length);
            double Pvalue = Erfc(S_obs / Math.Sqrt(2));
            txtPvalue.Text = Pvalue.ToString();

            //Counts & split string - start of block test
            //Gets the M value; since there is no check for if the M value changes, the M value must be inputted before the binary string
            //This is communicated by the Enter first or else on the label so its not like the user wasn't aware.
            if (!int.TryParse(txtM.Text, out M) || M <= 0 || M > N)
            {
                txtBinary.Text = "Invalid block size.";
                return;
            }
            //Discard remainder bits and take only full blocks
            int numBlocks = N / M;
            var partitions = Enumerable.Range(0, numBlocks)
                .Select(i => binary.Substring(i * M, M));
            //Determine Chi Square value
            double chiSquare = 0;
            for (int i = 0; i < numBlocks; i++)
            {
                //Adds value for each block to chi square
                string currentBlock = binary.Substring(i * M, M);

                double num1s = currentBlock.Count(c => c == '1') / (double)M;

                chiSquare += Math.Pow(num1s - 0.5, 2);
            }
            chiSquare = chiSquare * 4 * M;
            //Determine p-value using gamma function
            double BlockPvalue = GammaFunctions.GammaUpper(numBlocks / 2.0, chiSquare / 2.0);
            txtPvalueBlocks.Text = BlockPvalue.ToString();
        }
        //Ensures that only 1's and 0's are inputted into binary
        private void txtBinary_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => c == '0' || c == '1');
        }
    }
}