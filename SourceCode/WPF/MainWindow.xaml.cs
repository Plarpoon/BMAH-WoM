using BMAH_WoM.SourceCode;
using System.Windows;
using System.Windows.Input;

namespace BMAH_WoM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Program program;
        private readonly ExportToExcel exporttoexcel;

        public MainWindow()
        {
            InitializeComponent();

            program = new Program();
            DataContext = program;

            exporttoexcel = new ExportToExcel();
            DataContext = exporttoexcel;
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set tooltip visibility

            if (Tg_Btn.IsChecked == true)
            {
                tt_home.Visibility = Visibility.Collapsed;
                tt_info.Visibility = Visibility.Collapsed;
                tt_share.Visibility = Visibility.Collapsed;
                tt_settings.Visibility = Visibility.Collapsed;
                tt_logout.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_home.Visibility = Visibility.Visible;
                tt_info.Visibility = Visibility.Visible;
                tt_share.Visibility = Visibility.Visible;
                tt_settings.Visibility = Visibility.Visible;
                tt_logout.Visibility = Visibility.Visible;
            }
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 1;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 0.3;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            exporttoexcel.ToExcel();
        }

        private void BtnScraper_Click(object sender, RoutedEventArgs e)
        {
            program.ScrapeData();
        }
    }
}