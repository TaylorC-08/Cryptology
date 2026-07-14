//LINQ lets number of letters in string get calculated easier, documentation is above
using System.Collections.Generic;
using System.Linq; //https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries
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

namespace Index_of_Coincidence
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

        // Decrypts the ciphertext using the Vigenere cipher with the given keyword
        public static string Decrypt(string ciphertext, string keyword)
        {
            StringBuilder plaintext = new StringBuilder();

            ciphertext = ciphertext.ToLower();
            keyword = keyword.ToLower();

            int keywordIndex = 0;

            foreach (char c in ciphertext)
            {
                int shift = keyword[keywordIndex % keyword.Length] - 'a';
                char decryptedChar = (char)((((c - 'a' - shift) + 26) % 26) + 'a');
                plaintext.Append(decryptedChar);
                keywordIndex++;
            }

            return plaintext.ToString();
        }
        private void txtCipher_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtError.Content = "";
            txtIoC.Text = "";
            string CipherText = Regex.Replace(txtCipher.Text.ToLower(), "[^a-z]", "");
            int N = CipherText.Length;
            double IndexofCoincidence = 0;
            if (string.IsNullOrEmpty(CipherText)) // Checks if cipher text is empty since there is no use of a button
            {
                txtError.Content = "There is no Cipher text.";
                txtIoC.Text = "";
                return;
            }
            var characterCounts = CipherText //Use of LINQ for going by each letter in the string and calculating the index of coincidence
                .Where(c => Char.IsLetter(c))
                .GroupBy(c => c)
                .Select(g => new { Letter = g.Key, Count = g.Count() })
                .OrderBy(item => item.Letter);

            foreach (var item in characterCounts) //Goes through the string for letters a-z
            {
                IndexofCoincidence += (item.Count * (item.Count-1));
            }
            IndexofCoincidence = IndexofCoincidence / (N * (N - 1));
            txtIoC.Text = $"{IndexofCoincidence}";
        }

        private void txtPeriod_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtError.Content = "";
            txtIndexes.Text = "";
            long Period;
            bool SuccessPeriod = long.TryParse(txtPeriod.Text, out Period);
            string CipherText = Regex.Replace(txtCipher.Text.ToLower(), "[^a-z]", "");
            double IndexofCoincidence = 0;
            if (!SuccessPeriod)
            {
                txtError.Content = "Please enter a test period.";
                txtIndexes.Text = "";
                return;
            }
            var characterCounts = CipherText
                .Where(c => Char.IsLetter(c))
                .GroupBy(c => c)
                .Select(g => new { Letter = g.Key, Count = g.Count() })
                .OrderBy(item => item.Letter);
            for (int i = 0; i < Period; i++)
            {
                StringBuilder TestPeriod = new StringBuilder(); //https://learn.microsoft.com/en-us/dotnet/standard/base-types/stringbuilder
                for (int j = i; j < CipherText.Length; j += (int)Period)
                {
                    TestPeriod.Append(CipherText[j]);
                }
                string txtTestPeriod = TestPeriod.ToString();
                int N = txtTestPeriod.Length;
                var TestPeriodFrequency = txtTestPeriod
                    .GroupBy(c => c)
                    .Select(g => new { Letter = g.Key, Count = g.Count() })
                    .OrderBy(item => item.Letter);
                foreach (var item in TestPeriodFrequency)
                {
                    IndexofCoincidence += item.Count * (item.Count - 1);
                }
                IndexofCoincidence = IndexofCoincidence / (N * (N - 1.0));
                txtIndexes.Text += $"{i + 1}: {IndexofCoincidence}\n";
            }
        }
    }
}