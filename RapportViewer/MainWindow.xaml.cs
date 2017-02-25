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
            string fullPath = @"C:\Users\rprii\Downloads\Wizards _ RTE - Node Backup Report  2_17_2017 9_30 AM created on 17_02_2017 09_31_16 (1)\Notification.html";
            lb_RapportContent.ItemsSource = EditorRapport.CreateRapportDictonary(fullPath)["All"];
        }
    }
}
