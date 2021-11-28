using MyPKITools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace MyWpfPublicKeyApp
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

        private string encryptedText;
        public string EncryptedText
        {
            get { return encryptedText; }
            set { SetField(ref encryptedText, value, nameof(EncryptedText)); }
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
        /// Button_Encrypt_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Encrypt_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                EncryptedText = PKITools.EncryptTextFromPublicKey(p.PlainText,p.PublicKey);
                Message = "Text successfully encrypted"; 
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = EncryptedText = "ERROR failed to encrypt !!";
            }
        }

        /// <summary>
        /// Button_DeletePlainText_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_DeletePlainText_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;
            p.PlainText = "";
        }

        /// <summary>
        /// Button_PastePublicKey_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PastePublicKey_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                p.PublicKey = Clipboard.GetData(DataFormats.Text) as string;
                Message = "Paste Encrypted Text from Clipboard";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = p.PublicKey = "ERROR copy from Clipboard !!";
            }
        }

        /// <summary>
        /// Button_CopyEncryptedText_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CopyEncryptedText_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;

            try
            {
                Clipboard.SetDataObject(EncryptedText);
                Message = "Copy Encrypted Text to Clipboard";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Message = "ERROR copy to Clipboard !!";
            }


        }

        /// <summary>
        /// Button_DeletePublicKey_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_DeletePublicKey_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;
            p.PublicKey = "";
            Message = "Delete PublicKey";
        }

        /// <summary>
        /// Button_ClearAll_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var p = Properties.Settings.Default;
            p.PublicKey = "";
            p.PlainText = "";
            EncryptedText = "";
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
                if (button_Encrypt != null)
                    button_Encrypt.IsEnabled = false;
            }
            else
            {
                if (button_Encrypt != null)
                    button_Encrypt.IsEnabled = true;
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
