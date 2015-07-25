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

namespace PrintIndex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string selectedName;
        public static string setName;
        public MainWindow()
        {
            InitializeComponent();
            ScheduleListBox.Items.Clear();
            ScheduleListBox.ItemsSource = Command.nameSchedule;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selectedName = ScheduleListBox.SelectedItem as string;
            setName = TextBox1.Text;
            Command.dialogResult = true;
            Close();
        }
    }
}
