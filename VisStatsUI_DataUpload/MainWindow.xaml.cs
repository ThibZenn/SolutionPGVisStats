using System.Data;
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
using Microsoft.Win32;
using VisStatsBL.Interfaces;
using VisStatsBL.Managers;
using VisStatsDL_File;
using VisStatsDL_SQL;

namespace VisStatsUI_DataUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog dialog = new OpenFileDialog(); //zorgt ervoor dat je bestanden kunt selecteren.
        string conn = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=PGVisStats;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
        IFileProcessor _fileProcessor;
        IVisStatsRepository _visStatsRepository;
        VisStatsManager _visStatsManager;

        public MainWindow() //Dit is de constructor
        {
            InitializeComponent();
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
            dialog.InitialDirectory = @"C:\Users\thiba\Documents\HOGENT\Semester2\ProgGevorderd1\Vis\Data"; //De directory waar je wilt starten met zoeken.
            dialog.Multiselect = true;
            _fileProcessor = new FileProcessor();
            _visStatsRepository = new VisStatsRepository(conn);
            _visStatsManager = new VisStatsManager(_fileProcessor, _visStatsRepository);
        }

        private void Button_Click_Vissoorten(object sender, RoutedEventArgs e)
        {
            bool? result = dialog.ShowDialog();
            if (result == true) 
            {
                var filenames = dialog.FileNames;
                VissoortenFileListBox.ItemsSource = filenames;
                dialog.FileName = null;
            }
            else VissoortenFileListBox.ItemsSource = null; //Dit gebeurd als je op 'cancel' klikt.
        }

        private void Button_Click_UploadVissoorten(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in VissoortenFileListBox.ItemsSource)
            {
                _visStatsManager.UploadVissoorten(fileName);
            }
            MessageBox.Show("Upload klaar","VisStats");
        }

        private void Button_Click_Havens(object sender, RoutedEventArgs e)
        {
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var filenames = dialog.FileNames;
                HavensFileListBox.ItemsSource = filenames;
                dialog.FileName = null;
            }
            else HavensFileListBox.ItemsSource = null; //Dit gebeurd als je op 'cancel' klikt.
        }

        private void Button_Click_UploadHavens(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in HavensFileListBox.ItemsSource)
            {
                _visStatsManager.UploadHaven(fileName);
            }
            MessageBox.Show("Upload klaar", "VisStats");
        }

        private void Button_Click_statistieken(object sender, RoutedEventArgs e)
        {
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var filenames = dialog.FileNames;
                StatistiekenFileListBox.ItemsSource = filenames;
                dialog.FileName = null;
            }
            else StatistiekenFileListBox.ItemsSource = null; //Dit gebeurd als je op 'cancel' klikt.
        }

        private void Button_Click_UploadStatistieken(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in StatistiekenFileListBox.ItemsSource)
            {
                _visStatsManager.UploadStatistieken(fileName);
            }
            MessageBox.Show("Upload klaar", "VisStats");
        }
    }
}