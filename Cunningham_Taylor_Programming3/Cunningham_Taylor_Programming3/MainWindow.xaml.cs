using Cunningham_Taylor_Programming3;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Data;
using System.Windows.Automation.Peers;

namespace Cunningham_Taylor_Programming3
{
    /// <summary>
    /// Resources used:
    /// https://www.c-sharpcorner.com/UploadFile/mahesh/datagrid-in-wpf/
    /// https://help.syncfusion.com/wpf/datagrid/selection
    /// https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.tabitem?view=windowsdesktop-10.0
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/stringbuilder
    /// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/
    /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/nested-types
    /// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
    /// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct
    /// https://learn.microsoft.com/en-us/dotnet/csharp/linq/
    /// https://learn.microsoft.com/en-us/dotnet/api/system.string.substring?view=net-10.0
    /// </summary>
    /// This code allows the user to input cipher text from the column permutation cipher
    /// and use all possible rectangle sizes, vowel difference and centiban calculations to 
    /// successfully decrypt the cipher text returning the plaintext.
    /// Test vector containing 102 characters used for possible rectangles
    ///abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx
    ///Test vector for autosolve function created using Claude
    ///Ciphertext:
    ///itfulutstesrsettvnhaoaofishoec
    ///Key - (3,1,4,2,5)
    ///Plaintext (expected plaintext - 
    ///thisisatestoftheautosolverfunc
    ///Link to conversation with Claude to create the autosolve feature, code can be seen far below and is at the very end
    ///of the cs code, everything prior to the spacing is written with no use of AI
    ///https://claude.ai/share/e28ab212-dfd6-4c32-a765-3371cc092cae


    public class Rectangles
        //Rectangles class used for the column and row possibilities
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public double Vowels { get; set; }
    };

    public class Data
        //Data class for the left hand data grid in the GUI
    {
        public int Rows { set; get; }
        public int Columns { set; get; }
        public double Centiban { get; set; }
    };
    public partial class MainWindow : Window
    {
        //Variables used across classes and things go here
        //List of factors used for determining possible columns
        List<Tuple<int, int>> Factors = new List<Tuple<int, int>>();
        List<Tuple<int, int>> CentibanColumns = new List<Tuple<int, int>>();
        const int ASCII = 97;
        int rows = 0;
        String CipherText = "";
        int len = 0;

        // crossScore[a,b] = sum over rows r=0..rows-2 of
        //   CentibanValues[char at (r, col a), char at (r+1, col b)]
        // Used to score cross-row transitions without re-scanning the text each time.
        double[,] crossScore;

        double[,] CentibanValues = { { 0.331093, 0.459178, 0.615747, 0.737111, 0.128084, 0.384253, 0.459178, 0.256169, 0.651624, 0.128084, 0.256169, 0.768506, 0.615747, 0.89659, 0.256169, 0.587262, 0, 0.827352, 0.814303, 0.83954, 0.602053, 0.487663, 0.331093, 0, 0.587262, 0 }, { 0.256169, 0, 0, 0, 0.662186, 0, 0, 0, 0.256169, 0.128084, 0, 0.459178, 0.128084, 0, 0.384253, 0, 0, 0.256169, 0.128084, 0.128084, 0.256169, 0, 0, 0, 0.487663, 0 }, { 0.553571, 0, 0.331093, 0.128084, 0.768506, 0.128084, 0, 0.615747, 0.487663, 0, 0.384253, 0.425487, 0.128084, 0.128084, 0.814303, 0, 0, 0.384253, 0.128084, 0.615747, 0.384253, 0, 0.128084, 0, 0.128084, 0 }, { 0.640422, 0.384253, 0.384253, 0.512337, 0.774192, 0.512337, 0.256169, 0.256169, 0.737111, 0.128084, 0, 0.331093, 0.425487, 0.384253, 0.640422, 0.425487, 0.256169, 0.587262, 0.602053, 0.628496, 0.425487, 0.331093, 0.384253, 0, 0.128084, 0 }, { 0.656981, 0.384253, 0.768506, 0.884665, 0.818756, 0.662186, 0.384253, 0.487663, 0.737111, 0.128084, 0, 0.750316, 0.615747, 0.998343, 0.587262, 0.681656, 0.587262, 0.953325, 0.865195, 0.795334, 0.331093, 0.681656, 0.487663, 0.487663, 0.384253, 0.128084 }, { 0.297403, 0, 0.256169, 0.128084, 0.553571, 0.571183, 0.128084, 0, 0.805062, 0, 0, 0.256169, 0.128084, 0, 0.80974, 0.128084, 0, 0.534102, 0.331093, 0.571183, 0.331093, 0, 0.128084, 0, 0.128084, 0 }, { 0.359578, 0, 0.256169, 0.128084, 0.615747, 0.256169, 0.128084, 0.681656, 0.425487, 0.128084, 0, 0.256169, 0.128084, 0.331093, 0.459178, 0.256169, 0, 0.425487, 0.331093, 0.384253, 0.256169, 0, 0.128084, 0, 0, 0 }, { 0.553571, 0.128084, 0.331093, 0.256169, 0.681656, 0.425487, 0, 0, 0.774192, 0, 0, 0.128084, 0.256169, 0.331093, 0.681656, 0.128084, 0.128084, 0.651624, 0.384253, 0.743831, 0.512337, 0, 0.128084, 0, 0.128084, 0 }, { 0.384253, 0.256169, 0.699268, 0.459178, 0.602053, 0.553571, 0.672177, 0, 0, 0, 0.256169, 0.707482, 0.534102, 0.925899, 0.814303, 0.487663, 0, 0.737111, 0.785065, 0.737111, 0, 0.72289, 0, 0.628496, 0, 0.256169 }, { 0, 0, 0, 0, 0.256169, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.256169, 0, 0, 0, 0, 0, 0.256169, 0, 0, 0, 0, 0 }, { 0, 0, 0.128084, 0, 0.459178, 0, 0, 0, 0.256169, 0, 0, 0.128084, 0, 0.128084, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0, 0, 0 }, { 0.384253, 0.331093, 0.331093, 0.534102, 0.795334, 0.331093, 0.128084, 0.128084, 0.681656, 0, 0, 0.737111, 0, 0.128084, 0.602053, 0.331093, 0, 0.256169, 0.459178, 0.512337, 0.256169, 0.256169, 0.256169, 0, 0.553571, 0 }, { 0.662186, 0.459178, 0.331093, 0.128084, 0.730137, 0.128084, 0, 0.128084, 0.534102, 0, 0, 0, 0.602053, 0, 0.553571, 0.512337, 0, 0.256169, 0.384253, 0.256169, 0.256169, 0, 0, 0, 0.256169, 0 }, { 0.602053, 0.331093, 0.672177, 0.858221, 0.875186, 0.534102, 0.737111, 0.384253, 0.75658, 0.128084, 0.256169, 0.425487, 0.425487, 0.512337, 0.662186, 0.331093, 0.128084, 0.384253, 0.715346, 0.942387, 0.487663, 0.331093, 0.331093, 0, 0.425487, 0 }, { 0.359578, 0.384253, 0.512337, 0.587262, 0.331093, 0.72289, 0.256169, 0.331093, 0.425487, 0.128084, 0.256169, 0.672177, 0.72289, 0.930762, 0.459178, 0.72289, 0, 0.89659, 0.615747, 0.672177, 0.795334, 0.487663, 0.512337, 0.128084, 0.256169, 0 }, { 0.487663, 0.128084, 0.128084, 0.128084, 0.707482, 0.256169, 0, 0.331093, 0.459178, 0, 0, 0.602053, 0.384253, 0.128084, 0.651624, 0.571183, 0, 0.662186, 0.459178, 0.512337, 0.331093, 0.128084, 0.128084, 0, 0.128084, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0.128084, 0, 0, 0.628496, 0, 0, 0, 0, 0 }, { 0.676977, 0.256169, 0.534102, 0.651624, 0.975325, 0.459178, 0.487663, 0.331093, 0.75658, 0.128084, 0.128084, 0.425487, 0.534102, 0.487663, 0.743831, 0.602053, 0, 0.571183, 0.762639, 0.818756, 0.425487, 0.425487, 0.384253, 0, 0.534102, 0 }, { 0.587262, 0.331093, 0.602053, 0.425487, 0.847241, 0.587262, 0.256169, 0.730137, 0.779709, 0, 0.128084, 0.256169, 0.331093, 0.384253, 0.628496, 0.553571, 0, 0.425487, 0.672177, 0.89368, 0.571183, 0.128084, 0.384253, 0, 0.128084, 0 }, { 0.615747, 0.331093, 0.459178, 0.459178, 0.915771, 0.487663, 0.128084, 0.933146, 0.831505, 0, 0, 0.425487, 0.459178, 0.487663, 0.850974, 0.256169, 0.128084, 0.651624, 0.672177, 0.672177, 0.425487, 0, 0.790271, 0, 0.814303, 0.128084 }, { 0.297403, 0.331093, 0.331093, 0.331093, 0.571183, 0.128084, 0.512337, 0, 0.425487, 0, 0, 0.459178, 0.425487, 0.690671, 0.128084, 0, 0, 0.762639, 0.587262, 0.587262, 0, 0.128084, 0, 0, 0, 0 }, { 0.331093, 0, 0, 0, 0.875186, 0, 0, 0, 0.587262, 0, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0, 0 }, { 0.459178, 0, 0, 0, 0.699268, 0, 0, 0.384253, 0.602053, 0, 0, 0.128084, 0, 0.256169, 0.672177, 0, 0, 0.128084, 0.128084, 0, 0, 0, 0, 0, 0.128084, 0 }, { 0.128084, 0, 0.256169, 0.128084, 0.128084, 0.128084, 0, 0.128084, 0.256169, 0, 0, 0, 0, 0.128084, 0.128084, 0.256169, 0, 0.128084, 0.128084, 0.487663, 0, 0, 0, 0, 0, 0 }, { 0.331093, 0.256169, 0.384253, 0.384253, 0.534102, 0.571183, 0.128084, 0.128084, 0.331093, 0, 0, 0.256169, 0.256169, 0.459178, 0.553571, 0.331093, 0, 0.384253, 0.571183, 0.628496, 0.128084, 0, 0.128084, 0, 0, 0 }, { 0, 0, 0, 0, 0.256169, 0, 0, 0, 0.128084, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
        public List<Tuple<int, int>> FactorPairs(int len) //Function to determine factor pairs of the ciphertext length
        {
            CipherText = Regex.Replace(txtCipher.Text.ToLower(), "[^a-z]", "");
            txtCipher.Text = CipherText;
            len = CipherText.Length;
            List<Tuple<int, int>> Factorpairs = new List<Tuple<int, int>>(); // List that contains each set of factors
            for (int i = 1; i * i <= len; i++) // Loops up to the square root of length
            {
                if (len % i == 0) //If there is no remainder, the number is a factor
                {
                    int factor1 = i;
                    int factor2 = len / i;
                    Factorpairs.Add(Tuple.Create(factor1, factor2));
                }
            }
            return Factorpairs;
        }

        public List<Tuple<int, int>> CentibanCombos(int columns)
        {
            //Function to determine possible column combinations of 2 for centiban calculations
            List<Tuple<int, int>> CentibanCombos = new List<Tuple<int, int>>();
            for (int i = 1; i < columns; i++)
            {
                for (int j = i + 1; j <= columns; j++)
                {
                    CentibanCombos.Add(Tuple.Create(i, j));
                }
            }
            return CentibanCombos;
        }

        double CentibanSum (int ColumnA, int ColumnB)
        {
            double sum = 0;
            for (int i = 0; i < rows; i++)
            {
                //Get characters A and B from the columns at row i
                Char A = CipherText[i + (ColumnA - 1) * rows];
                Char B = CipherText[i + (ColumnB - 1) * rows];
                //Convert to ASCII value 
                int IndexA = A - ASCII;
                int IndexB = B - ASCII;
                sum += CentibanValues[IndexA, IndexB];
            }
            return Math.Round(sum,4);
        }

        double voweldifference(int Columns)
        {
            //Function to determine the percent difference in vowels compared against the expected 0.4 or 40%
            string vowelslist = "aeiou";
            int rowCount = CipherText.Length / Columns;
            double totaldifference = 0;
            double expected = Columns * 0.4;
            for (int i = 0; i < rowCount; i++)
            {
                int vowelCount = 0;
                for (int j = 0; j < Columns; j++)
                {
                    //Split the text into the dimensions of the rectangle and count vowels
                    char c = CipherText[i + j * rowCount];
                    if (vowelslist.Contains(c))
                    {
                        vowelCount++;
                    }
                }
                totaldifference += Math.Abs(vowelCount - expected);
            }
            return Math.Round(totaldifference,3);
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<Rectangles> RectData(string CipherText, int len)
        {
            //Generates rectangles and vowel differences for the RectanglesVowels data grid that is inside the Rectangles and vowels tab in the xaml
            List<Rectangles> Rectangles = new List<Rectangles>();
            for (int i = 0; i < Factors.Count; i++)
            {
                //Var index is used to access each tuple in the list of Factors, storing each item from the tuple as a factor for iterating rectangles
                var index = Factors[i];
                int factor1 = index.Item1;
                int factor2 = index.Item2;
                //Iterates a rectangle for each factor pair both possible ways
                Rectangles.Add(new Rectangles() { Rows = factor1, Columns = factor2, Vowels = voweldifference(factor1) });
                Rectangles.Add(new Rectangles() { Rows = factor2, Columns = factor1, Vowels = voweldifference(factor2) });
            }
            return Rectangles;
        }

        public List<Data> CentibanData(string CipherText, int rows, int columns)
        {
            //Generates centiban data for the Centibans
            List<Data> CentibanData = new List<Data>();
            for (int i = 0; i < CentibanColumns.Count; i++)
            {
                //Var index is used to access each tuple in the list of CentibanColumns, storing each item from the tuple as a factor for iterating centibans
                var index = CentibanColumns[i];
                int factor1 = index.Item1;
                int factor2 = index.Item2;
                CentibanData.Add(new Data() { Rows = factor1, Columns = factor2, Centiban = CentibanSum(factor1, factor2)});
            }
            return CentibanData;
        }


        public void txtCipher_TextChanged(object sender, TextChangedEventArgs e)
        {
            ///Error checks that are vital for this code to work and I don't question since it threw so many errors
            ///Most of these checks could have been avoided if I used buttons but whats the fun in using buttons?
            if (string.IsNullOrEmpty(txtCipher.Text))
            {
                return;
            }
            if (RectanglesVowels == null)
            {
                return;
            }
            //Remove formatting, call on previous functions and fill the data grid
            CipherText = Regex.Replace(txtCipher.Text.ToLower(), "[^a-z]", "");
            Factors = FactorPairs(CipherText.Length);
            List<Rectangles> rectangles = RectData(CipherText, len);
            RectanglesVowels.ItemsSource = rectangles;
        }
        public void RectanglesVowels_Selected(object sender, RoutedEventArgs e)
        {
            if (RectanglesVowels.SelectedItem is Rectangles selected)
            {
                //Get User selection from the datagrid
                rows = selected.Rows;
                int columns = selected.Columns;
                //Create a datatable to fill the datagrid for centibans
                DataTable Centibans = new DataTable();
                //Create columns
                for (int i = 0;  i < columns; i++)
                {
                    Centibans.Columns.Add($"{i+1}");
                }
                //fill the datatable using same logic as calculating vowel difference
                for (int i = 0;i < rows; i++)
                {
                    DataRow row = Centibans.NewRow();
                    for (int j = 0; j < columns; j++)
                    {
                        char c = CipherText[i + j * rows];
                        row[j] = c.ToString();
                    }
                    Centibans.Rows.Add(row);
                }
                CentibanCol.ItemsSource = Centibans.DefaultView;
                CentibanColumns = CentibanCombos(columns);
                List<Data> centibanData = CentibanData(CipherText, rows, columns);
                Dat.ItemsSource = centibanData;
            }
        }
        public void CentibanCol_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            //Using LINQ to get order to take datagrid into plaintext
            var columns = CentibanCol.Columns
                .OrderBy(c => c.DisplayIndex)
                .Select(c => c.Header?.ToString() ?? "")
                .ToList();

            StringBuilder Plaintext = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                foreach (var column in columns)
                {
                    int columnIndex = int.Parse(column) - 1;
                    Plaintext.Append(CipherText[i + columnIndex * rows]);
                }
            }
            txtPlain.Text = Plaintext.ToString();
        }




        //Beginning of auto solve feature methods using Claude
        // ── Auto solve button — runs heavy work on background thread ─────────────────
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CmdAutoSolve.IsEnabled = false;
            await Task.Run(() => AutoSolve());
            CmdAutoSolve.IsEnabled = true;
        }

        // ── Auto solve ───────────────────────────────────────────────────────────────
        private void AutoSolve()
        {
            if (string.IsNullOrEmpty(CipherText)) return;

            int localRows;
            int columns;
            string localCipher;
            List<Rectangles> allRects = null;
            Rectangles bestRect = null;

            Dispatcher.Invoke(() =>
            {
                Factors = FactorPairs(CipherText.Length);
                allRects = RectData(CipherText, len);
                RectanglesVowels.ItemsSource = allRects;

                var candidates = allRects
                    .Where(r => r.Rows > 1 && r.Columns > 1)
                    .Where(r => (double)Math.Min(r.Rows, r.Columns) /
                                        Math.Max(r.Rows, r.Columns) >= 0.5)
                    .ToList();

                // First fallback: drop aspect ratio requirement
                if (!candidates.Any())
                    candidates = allRects.Where(r => r.Rows > 1 && r.Columns > 1).ToList();

                // Second fallback: accept everything
                if (!candidates.Any())
                    candidates = allRects.ToList();

                // Final guard: ciphertext too short
                if (!candidates.Any())
                {
                    txtPlain.Text = "Ciphertext too short to analyse.";
                    return;
                }

                bestRect = candidates.OrderBy(r => r.Vowels).First();
                RectanglesVowels.SelectedItem = bestRect;
                RectanglesVowels.ScrollIntoView(bestRect);

                rows = bestRect.Rows;
                CentibanColumns = CentibanCombos(bestRect.Columns);
                Dat.ItemsSource = CentibanData(CipherText, rows, bestRect.Columns);
            });

            if (bestRect == null) return;

            // Snapshot values for background use
            localRows = rows;
            localCipher = CipherText;
            columns = bestRect.Columns;

            // ── Build within-row score matrix ────────────────────────────────────────
            double[,] scoreMatrix = new double[columns, columns];
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < columns; j++)
                    if (i != j)
                        for (int r = 0; r < localRows; r++)
                        {
                            char A = localCipher[r + i * localRows];
                            char B = localCipher[r + j * localRows];
                            scoreMatrix[i, j] += CentibanValues[A - ASCII, B - ASCII];
                        }

            // ── Build cross-row score matrix ─────────────────────────────────────────
            crossScore = new double[columns, columns];
            for (int a = 0; a < columns; a++)
                for (int b = 0; b < columns; b++)
                    for (int r = 0; r < localRows - 1; r++)
                    {
                        char A = localCipher[r + a * localRows];
                        char B = localCipher[(r + 1) + b * localRows];
                        crossScore[a, b] += CentibanValues[A - ASCII, B - ASCII];
                    }

            // ── Find best column ordering ────────────────────────────────────────────
            int[] beamBest = columns <= 9
                ? BruteForceOrder(columns, scoreMatrix)
                : BeamSearch(columns, scoreMatrix, beamWidth: 5000);
            int[] bestOrder = columns <= 9
                ? beamBest
                : MultiStartILS(columns, scoreMatrix, beamBest);

            // ── Update UI on main thread ─────────────────────────────────────────────
            Dispatcher.Invoke(() =>
            {
                DataTable SolvedTable = new DataTable();
                for (int i = 0; i < columns; i++)
                    SolvedTable.Columns.Add($"{bestOrder[i] + 1}");
                for (int i = 0; i < localRows; i++)
                {
                    DataRow row = SolvedTable.NewRow();
                    for (int j = 0; j < columns; j++)
                        row[j] = localCipher[i + bestOrder[j] * localRows].ToString();
                    SolvedTable.Rows.Add(row);
                }
                CentibanCol.ItemsSource = SolvedTable.DefaultView;

                var sb = new StringBuilder();
                for (int i = 0; i < localRows; i++)
                    foreach (int col in bestOrder)
                        sb.Append(localCipher[i + col * localRows]);
                txtPlain.Text = sb.ToString();
            });
        }

        // ── FastScore: within-row bigrams + cross-row transitions (O(n) per call) ────
        private double FastScore(int[] order, double[,] scoreMatrix)
        {
            int n = order.Length;
            double total = 0;
            for (int i = 0; i < n - 1; i++)
                total += scoreMatrix[order[i], order[i + 1]];
            if (crossScore != null)
                total += crossScore[order[n - 1], order[0]];
            return total;
        }

        // ── Beam search ──────────────────────────────────────────────────────────────
        private int[] BeamSearch(int columns, double[,] scoreMatrix, int beamWidth = 5000)
        {
            if (columns <= 1)
                return Enumerable.Range(0, columns).ToArray();

            // Bitmask only works safely for columns <= 30; fall back to greedy above that
            if (columns > 30)
            {
                bool[] used = new bool[columns];
                int[] order = new int[columns];
                order[0] = 0; used[0] = true;
                for (int step = 1; step < columns; step++)
                {
                    int bestNext = -1;
                    double bestScore = double.MinValue;
                    for (int j = 0; j < columns; j++)
                        if (!used[j] && scoreMatrix[order[step - 1], j] > bestScore)
                        { bestScore = scoreMatrix[order[step - 1], j]; bestNext = j; }
                    order[step] = bestNext;
                    used[bestNext] = true;
                }
                return order;
            }

            var beams = new List<(int[] order, double score, int usedMask)>(beamWidth);
            for (int start = 0; start < columns; start++)
                beams.Add((new int[] { start }, 0.0, 1 << start));

            for (int step = 1; step < columns; step++)
            {
                var next = new List<(int[] order, double score, int usedMask)>(
                    beams.Count * (columns - step));
                foreach (var (order, score, usedMask) in beams)
                {
                    int last = order[step - 1];
                    for (int nx = 0; nx < columns; nx++)
                    {
                        if ((usedMask & (1 << nx)) != 0) continue;
                        int[] newOrder = new int[step + 1];
                        Array.Copy(order, newOrder, step);
                        newOrder[step] = nx;
                        next.Add((newOrder, score + scoreMatrix[last, nx],
                                  usedMask | (1 << nx)));
                    }
                }

                if (!next.Any())
                    return Enumerable.Range(0, columns).ToArray();

                next.Sort((a, b) => b.score.CompareTo(a.score));
                beams = next.GetRange(0, Math.Min(beamWidth, next.Count));
            }

            if (!beams.Any())
                return Enumerable.Range(0, columns).ToArray();

            int[] best = beams[0].order;
            double bestScore2 = FastScore(best, scoreMatrix);
            foreach (var (order, _, __) in beams.Take(100))
            {
                double s = FastScore(order, scoreMatrix);
                if (s > bestScore2) { best = order; bestScore2 = s; }
                int[] rev = order.Reverse().ToArray();
                double rs = FastScore(rev, scoreMatrix);
                if (rs > bestScore2) { best = rev; bestScore2 = rs; }
            }
            return best;
        }

        // ── Multi-start ILS ──────────────────────────────────────────────────────────
        private int[] MultiStartILS(int columns, double[,] scoreMatrix, int[] beamBest)
        {
            int[] globalBest = ILS(columns, scoreMatrix, beamBest, iterations: 200);
            double globalScore = FastScore(globalBest, scoreMatrix);

            Random seedRng = new Random(42);
            for (int restart = 0; restart < 10; restart++)
            {
                int[] randomStart = Enumerable.Range(0, columns).ToArray();
                for (int i = columns - 1; i > 0; i--)
                {
                    int j = seedRng.Next(i + 1);
                    int tmp = randomStart[i]; randomStart[i] = randomStart[j];
                    randomStart[j] = tmp;
                }
                int[] candidate = ILS(columns, scoreMatrix, randomStart, iterations: 150);
                double cScore = FastScore(candidate, scoreMatrix);
                if (cScore > globalScore) { globalScore = cScore; globalBest = candidate.ToArray(); }
            }

            globalBest = TwoOpt(globalBest, scoreMatrix);
            globalBest = OrOpt(globalBest, scoreMatrix);
            return globalBest;
        }

        // ── Iterated local search ────────────────────────────────────────────────────
        private int[] ILS(int columns, double[,] scoreMatrix, int[] initial, int iterations = 200)
        {
            Random rng = new Random(Guid.NewGuid().GetHashCode());

            int[] best = TwoOpt(initial.ToArray(), scoreMatrix);
            best = OrOpt(best, scoreMatrix);
            double bestScore = FastScore(best, scoreMatrix);
            int[] incumbent = best.ToArray();
            double incScore = bestScore;
            int noImprove = 0;

            for (int iter = 0; iter < iterations; iter++)
            {
                int[] perturbed = iter % 3 == 0
                    ? DoubleBridge(incumbent, rng)
                    : RandomSwap(incumbent, rng, 2 + rng.Next(4), columns);

                perturbed = TwoOpt(perturbed, scoreMatrix);
                perturbed = OrOpt(perturbed, scoreMatrix);

                double pertScore = FastScore(perturbed, scoreMatrix);
                if (pertScore > incScore)
                { incumbent = perturbed.ToArray(); incScore = pertScore; noImprove = 0; }
                else noImprove++;

                if (noImprove >= 40)
                { incumbent = best.ToArray(); incScore = bestScore; noImprove = 0; }

                if (incScore > bestScore)
                { bestScore = incScore; best = incumbent.ToArray(); }
            }
            return best;
        }

        // ── Double-bridge perturbation ────────────────────────────────────────────────
        private int[] DoubleBridge(int[] order, Random rng)
        {
            int n = order.Length;
            if (n < 8) return order;
            var pos = Enumerable.Range(1, n - 1)
                                 .OrderBy(_ => rng.Next())
                                 .Take(3)
                                 .OrderBy(x => x)
                                 .ToArray();
            int a = pos[0], b = pos[1], c = pos[2];
            return order[..a]
                   .Concat(order[b..c])
                   .Concat(order[a..b])
                   .Concat(order[c..])
                   .ToArray();
        }

        // ── Random swap perturbation ──────────────────────────────────────────────────
        private int[] RandomSwap(int[] source, Random rng, int swapCount, int columns)
        {
            int[] result = source.ToArray();
            for (int k = 0; k < swapCount; k++)
            {
                int i = rng.Next(columns), j = rng.Next(columns);
                while (j == i) j = rng.Next(columns);
                int tmp = result[i]; result[i] = result[j]; result[j] = tmp;
            }
            return result;
        }

        // ── 2-opt: reverse sub-segments ──────────────────────────────────────────────
        private int[] TwoOpt(int[] order, double[,] scoreMatrix)
        {
            bool improved = true;
            int[] best = order.ToArray();
            while (improved)
            {
                improved = false;
                double bestScore = FastScore(best, scoreMatrix);
                for (int i = 0; i < best.Length - 1; i++)
                    for (int j = i + 2; j < best.Length; j++)
                    {
                        int[] candidate = best.ToArray();
                        Array.Reverse(candidate, i, j - i + 1);
                        double s = FastScore(candidate, scoreMatrix);
                        if (s > bestScore)
                        { best = candidate; bestScore = s; improved = true; }
                    }
            }
            return best;
        }

        // ── Or-opt: relocate single elements ─────────────────────────────────────────
        private int[] OrOpt(int[] order, double[,] scoreMatrix)
        {
            bool improved = true;
            int[] best = order.ToArray();
            double bestScore = FastScore(best, scoreMatrix);
            while (improved)
            {
                improved = false;
                for (int i = 0; i < best.Length; i++)
                {
                    int elem = best[i];
                    var without = best.Where((_, idx) => idx != i).ToArray();
                    for (int j = 0; j <= without.Length; j++)
                    {
                        var cand = without.ToList();
                        cand.Insert(j, elem);
                        int[] arr = cand.ToArray();
                        double s = FastScore(arr, scoreMatrix);
                        if (s > bestScore)
                        { bestScore = s; best = arr; improved = true; break; }
                    }
                    if (improved) break;
                }
            }
            return best;
        }

        // ── Brute force for small column counts (≤ 9) ────────────────────────────────
        private int[] BruteForceOrder(int columns, double[,] scoreMatrix)
        {
            int[] initial = Enumerable.Range(0, columns).ToArray();
            int[] bestOrder = initial.ToArray();
            double bestScore = FastScore(bestOrder, scoreMatrix);
            foreach (int[] perm in AllPermutations(initial))
            {
                double s = FastScore(perm, scoreMatrix);
                if (s > bestScore) { bestScore = s; bestOrder = perm.ToArray(); }
            }
            return bestOrder;
        }

        // ── Permutation generator ─────────────────────────────────────────────────────
        private IEnumerable<int[]> AllPermutations(int[] arr)
        {
            if (arr.Length <= 1) { yield return arr.ToArray(); yield break; }
            for (int i = 0; i < arr.Length; i++)
            {
                int[] rest = arr.Where((_, idx) => idx != i).ToArray();
                foreach (int[] perm in AllPermutations(rest))
                    yield return new[] { arr[i] }.Concat(perm).ToArray();
            }
        }

        private async void CentibanAutoSolve_Click(object sender, RoutedEventArgs e)
        {
            // Guard: a rectangle must already be selected
            if (rows == 0 || string.IsNullOrEmpty(CipherText))
            {
                MessageBox.Show("Please select a rectangle first by double-clicking a row " +
                                "in the Rectangles and Vowels tab.",
                                "No rectangle selected",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            CentibanAutoSolve.IsEnabled = false;
            await Task.Run(() => SolveColumns());
            CentibanAutoSolve.IsEnabled = true;
        }


        // ── Column-only solver — rectangle already chosen by the user ────────────────
        private void SolveColumns()
        {
            // Snapshot current state
            int localRows = rows;
            string localCipher = CipherText;
            int columns = Dispatcher.Invoke(() =>
            {
                // Read column count from the current CentibanCol DataTable
                if (CentibanCol.ItemsSource is DataView dv)
                    return dv.Table.Columns.Count;
                return 0;
            });

            if (columns == 0) return;

            // ── Build within-row score matrix ────────────────────────────────────────
            double[,] scoreMatrix = new double[columns, columns];
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < columns; j++)
                    if (i != j)
                        for (int r = 0; r < localRows; r++)
                        {
                            char A = localCipher[r + i * localRows];
                            char B = localCipher[r + j * localRows];
                            scoreMatrix[i, j] += CentibanValues[A - ASCII, B - ASCII];
                        }

            // ── Build cross-row score matrix ─────────────────────────────────────────
            crossScore = new double[columns, columns];
            for (int a = 0; a < columns; a++)
                for (int b = 0; b < columns; b++)
                    for (int r = 0; r < localRows - 1; r++)
                    {
                        char A = localCipher[r + a * localRows];
                        char B = localCipher[(r + 1) + b * localRows];
                        crossScore[a, b] += CentibanValues[A - ASCII, B - ASCII];
                    }

            // ── Find best column ordering ────────────────────────────────────────────
            int[] beamBest = columns <= 9
                ? BruteForceOrder(columns, scoreMatrix)
                : BeamSearch(columns, scoreMatrix, beamWidth: 5000);
            int[] bestOrder = columns <= 9
                ? beamBest
                : MultiStartILS(columns, scoreMatrix, beamBest);

            // ── Update UI on main thread ─────────────────────────────────────────────
            Dispatcher.Invoke(() =>
            {
                // Rebuild CentibanCol in solved order
                DataTable SolvedTable = new DataTable();
                for (int i = 0; i < columns; i++)
                    SolvedTable.Columns.Add($"{bestOrder[i] + 1}");
                for (int i = 0; i < localRows; i++)
                {
                    DataRow row = SolvedTable.NewRow();
                    for (int j = 0; j < columns; j++)
                        row[j] = localCipher[i + bestOrder[j] * localRows].ToString();
                    SolvedTable.Rows.Add(row);
                }
                CentibanCol.ItemsSource = SolvedTable.DefaultView;

                // Write plaintext row by row
                var sb = new StringBuilder();
                for (int i = 0; i < localRows; i++)
                    foreach (int col in bestOrder)
                        sb.Append(localCipher[i + col * localRows]);
                txtPlain.Text = sb.ToString();
            });
        }
    }
}