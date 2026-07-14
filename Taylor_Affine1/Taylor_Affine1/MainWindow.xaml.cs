using System.IO; // For text files
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
using Microsoft.Win32;
using Path = System.IO.Path; // For SaveFileDialog 

namespace Taylor_Affine1
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

        const int ASCII = 97; // 97 is a, this number is used to shift letters down and back up when encrypting/decrypting
        long GCD(long a, long b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        long Modulo(long a) // function to prevent negative outputs
        {
            a = a % 26;
            if (a < 0)
            {
                a = a + 26;
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
            long tempa = a; //Used in case the modulo returns a negative to shift it back

            while (b != 0)
            {
                q = a / b;
                (a, b) = (b, a % b);
                (x, tempx) = (tempx - q * x, x);
                (y, tempy) = (tempy - q * y, y);
            }

            if (tempy < 0)
            {
                tempy += tempa;
            }

            return tempy;
        }
        private void cmdEncrypt_Click(object sender, RoutedEventArgs e)
        {
            lblError.Content = "";
            long a = 0;
            long b = 0;
            String plainText = "";
            String cipherText = "";
            bool successA = long.TryParse(txtA.Text, out a);
            bool successB = long.TryParse(txtB.Text, out b);

            if (!successA || !successB)
            {
                lblError.Content = "Please enter integers!";
                return;
            }
            long gcd = GCD(a, 26);
            if (gcd != 1)
            {
                lblError.Content += "\nNo inverse.";
                return;
            }
            long Inverse = inverse(a, 26);
            plainText = Regex.Replace(txtPlain.Text.ToLower(), "[^a-z]", ""); // removes formatting and keeps only lowercase characters
            txtPlain.Text = plainText;

            foreach (char character in plainText) // For loop that iterates through each character in the specified string
            {
                long letter = (int)character - ASCII;
                long encryptValue = Modulo((a * letter) + b);
                long ASCIIVal = encryptValue + ASCII;
                cipherText += (char)ASCIIVal;
            }
            txtCipher.Text = cipherText;
        }

        private void cmdDecrypt_Click(object sender, RoutedEventArgs e)
        {
            lblError.Content = "";
            long a = 0;
            long b = 0;
            String plainText = "";
            String cipherText = "";
            bool successA = long.TryParse(txtA.Text, out a);
            bool successB = long.TryParse(txtB.Text, out b);

            if (!successA || !successB)
            {
                lblError.Content = "Please enter integers!";
                return;
            }
            long gcd = GCD(a, 26);
            long Inverse = inverse(26, a);
            if (gcd != 1)
            {
                lblError.Content += "\nNo inverse.";
                return;
            }
            cipherText = Regex.Replace(txtCipher.Text.ToLower(), "[^a-z]", "");
            txtCipher.Text = cipherText;

            foreach (char character in cipherText)
            {
                long letter = (int)character - ASCII;
                long decryptValue = Modulo((Inverse * (letter - b)));
                long ASCIIVal = decryptValue + ASCII;
                plainText += (char)ASCIIVal;
            }
            txtPlain.Text = plainText;
        }

        private void cmdPlainOpen_Click(object sender, RoutedEventArgs e)
        {
            // Taken from https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/how-to-open-common-system-dialog-box
            lblError.Content = "";
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                txtPlain.Text = File.ReadAllText(filename);
            }
            else
            {
                lblError.Content = "Error opening file.";
            }
        }

        private void cmdCipherOpen_Click(object sender, RoutedEventArgs e)
        {
            lblError.Content = "";
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                txtCipher.Text = File.ReadAllText(filename);
            }
            else
            {
                lblError.Content = "Error opening file.";
            }
        }
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-10.0
            //https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            var dialog = new Microsoft.Win32.SaveFileDialog();

            string fullPath = saveFileDialog1.FileName;
            string fileName = Path.GetFileName(fullPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);

            // Create a string array with the lines of text
            string[] lines = { "Plain Text: " + txtPlain, "Cipher Text: " + txtCipher };
            // Set a variable to the Documents path.
            string docPath =
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName + ".txt")))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }
    }
}