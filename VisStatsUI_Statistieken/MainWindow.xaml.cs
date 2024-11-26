using System.Collections.ObjectModel;
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
using VisStatsBL.Interfaces;
using VisStatsBL.Managers;
using VisStatsBL.Model;
using VisStatsDL_File;
using VisStatsDL_SQL;

namespace VisStatsUI_Statistieken
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VisStatsManager visStatsManager;
        IFileProcessor fileProcessor;
        IVisStatsRepository visStatsRepository;
        string connectionString = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=PGVisStats;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
        ObservableCollection<VisSoort> AlleVissoorten;
        ObservableCollection<VisSoort> GeselecteerdeVissoorten;


        public MainWindow()
        {
            InitializeComponent();
            fileProcessor = new FileProcessor();
            visStatsRepository = new VisStatsRepository(connectionString);
            visStatsManager = new VisStatsManager(fileProcessor, visStatsRepository);
            HavenComboBox.ItemsSource = visStatsManager.GeefHavens();
            HavenComboBox.SelectedIndex = 0;
            JaarComboBox.ItemsSource = visStatsManager.GeefJaartallen();
            JaarComboBox.SelectedIndex = 0;
            AlleVissoorten = new ObservableCollection<VisSoort>(visStatsManager.GeefVissoorten());
            GeselecteerdeVissoorten = new ObservableCollection<VisSoort>();
            AlleSoortenListBox.ItemsSource = AlleVissoorten;
            GeselecteerdeSoortenLisBox.ItemsSource = GeselecteerdeVissoorten;


        }

        private void VoegAlleSoortenToeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (VisSoort v in AlleVissoorten)
            {
                GeselecteerdeVissoorten.Add(v);
            }
            AlleVissoorten.Clear();
        }

        private void VoegSoortenToeButton_Click(object sender, RoutedEventArgs e)
        {
            List<VisSoort> soorten = new();

            foreach (VisSoort v in AlleSoortenListBox.SelectedItems)
            {
                soorten.Add(v);
            }
            foreach (VisSoort v in soorten)
            {
                GeselecteerdeVissoorten.Add(v);
                AlleVissoorten.Remove(v);
            }
        }

        private void VerwijderSoortenButton_Click(object sender, RoutedEventArgs e)
        {
            List<VisSoort> soorten = new();

            foreach (VisSoort v in GeselecteerdeSoortenLisBox.SelectedItems) soorten.Add(v);
            foreach (VisSoort v in soorten)
            {
                GeselecteerdeVissoorten.Remove(v);
                AlleVissoorten.Add(v);
            }
        }

        private void VerwijderAlleSoortenButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (VisSoort v in GeselecteerdeVissoorten)
            {
                AlleVissoorten.Add(v);
            }
            GeselecteerdeVissoorten.Clear();
        }
        private void ToonStatistiekenButton_Click(object sender, RoutedEventArgs e)
        {
            Eenheid eenheid;
            if ((bool)KgRadioButton.IsChecked)
            {
                eenheid = Eenheid.kg;
            }
            else
            {
                eenheid = Eenheid.euro;
            }
            List<Jaarvangst> vangst = visStatsManager.GeefVangst((int)JaarComboBox.SelectedItem, (Haven)HavenComboBox.SelectedItem, GeselecteerdeVissoorten.ToList(), eenheid);
            //
            StatistiekenWindow w = new StatistiekenWindow((int)JaarComboBox.SelectedItem, (Haven)HavenComboBox.SelectedItem, vangst, eenheid);
            w.ShowDialog();
        }
    }

}