using System;
using System.Collections.Generic;
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
using Editor_Lib;

namespace RapportViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cb_DisableExtension.IsThreeState = false;
        }

        private void btn_ChangeFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result == true)
            {
                lb_File.Content = ofd.FileName;
            }
        }

        private void btn_Anonymize_Click(object sender, RoutedEventArgs e)
        {
            if(lb_File.Content != null)
            {
                EditorRapport.DisableExtension = (bool)cb_DisableExtension.IsChecked;
                EditorRapport.GenerateAnonymousRapport((string) lb_File.Content);
            }
        }
    }
}
