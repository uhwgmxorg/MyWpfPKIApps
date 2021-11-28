using MyPKITools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace MyWpfPrivatKeyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PKITools MyPki { get; set; }

        #region INotify Changed Properties  
        private string message;
        public string Message
        {
            get { return message; }
            set { SetField(ref message, value, nameof(Message)); }
        }

        private string plainText;
        public string PlainText
        {
            get { return plainText; }
            set { SetField(ref plainText, value, nameof(PlainText)); }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

#if DEBUG
            Title += "    Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#else
            Title += "    Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#endif
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_GenerateKeys_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_GenerateKeys_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            p.PrivateKey = "";
            p.PublicKey = "";
            p.EncryptedText = "";
            PlainText = "";

            MyPki = new PKITools(p.KeySize);

            string publicKey = MyPki.PublicKey;
            string privatKey = MyPki.PrivateKey;

            p.PublicKey = publicKey;
            p.PrivateKey = privatKey;

            // Just to calculate the length of the public and private keys
            publicKey = publicKey.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");
            privatKey = privatKey.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "");
            Message = $"Keys generated - private key PEM length {privatKey.Length}  -  public key PEM length {publicKey.Length}";
        }

        /// <summary>
        /// Button_CopyPrivateKey_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CopyPrivateKey_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                Clipboard.SetDataObject(p.PrivateKey);
                Message = "Copy Private Key to Clipboard";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = PlainText = "ERROR copy to Clipboard !!";
            }
        }

        /// <summary>
        /// Button_Decrypt_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                PlainText = PKITools.DecryptTextWithPrivateKey(p.EncryptedText, p.PrivateKey);
                Message = "Text successfully decrypted";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = PlainText = "ERROR failed to decrypt !!";
            }
        }

        /// <summary>
        /// Button_CopyPublicKey_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CopyPublicKey_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                Clipboard.SetDataObject(p.PublicKey);
                Message = "Copy Public Key to Clipboard";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = PlainText = "ERROR copy to Clipboard !!";
            }
        }

        /// <summary>
        /// Button_PasteEncryptedText_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PasteEncryptedText_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                p.EncryptedText = Clipboard.GetData(DataFormats.Text) as string;
                Message = "Paste Encrypted Text from Clipboard";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = PlainText = "ERROR copy from Clipboard !!";
            }
        }

        /// <summary>
        /// Button_DeleteEncryptedText_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_DeleteEncryptedText_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;
            p.EncryptedText = "";
            Message = "Delete Encrypted Text";
        }

        /// <summary>
        /// Button_ClearAll_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;
            p.PrivateKey = "";
            p.PublicKey = "";
            p.EncryptedText = "";
            PlainText = "";
            Message = "All TextBoxes cleared";
        }

        /// <summary>
        /// Button_Close_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Lable_Message_MouseDown
        /// Clear Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lable_Message_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Message = "";
        }

        /// <summary>
        /// Hyperlink_RequestNavigate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var p = Properties.Settings.Default;

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = p.PrivatPublicKeyTestPage,
                UseShellExecute = true
            }); ;
        }

        /// <summary>
        /// textBox_PublicKey_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_PublicKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var p = Properties.Settings.Default;
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            string newText = textBox.Text;
            if (string.IsNullOrEmpty(newText))
            {
                if (button_Decrypt != null)
                    button_Decrypt.IsEnabled = false;
            }
            else
            {
                if (button_Decrypt != null)
                    button_Decrypt.IsEnabled = true;
            }
        }

        /// <summary>
        /// Window_Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// SetField
        /// for INotify Changed Properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
