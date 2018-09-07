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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace FilterObservableCollection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<City> sourceLst = new List<City>();
        private ObservableCollection<City> cityList = new ObservableCollection<City>();
        private string searchText;

        public ObservableCollection<City> CityList
        {
            get { return cityList; }
            set
            {
                cityList = value;
                OnPropertyChanged("CityList");
            }
        }
        public ICollectionView CityListView
        {
            get { return CollectionViewSource.GetDefaultView(CityList); }
        }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (value != searchText)
                {
                    searchText = value;
                    OnPropertyChanged("SearchText");
                    //CityListView.Refresh();

                    var filter = (from p in sourceLst
                                 let nm = p.Name
                                 where nm.ToLower().Contains(searchText.ToLower()) ||
                                 nm.Contains(searchText)
                                 orderby nm
                                 select p).Take(10);

                    CityList = new ObservableCollection<City>(filter); 
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            populateData();
            //CityListView.Filter = new Predicate<object>(o => SearchData(o as City));
        }

        private void populateData()
        {
            sourceLst.Add(new City { ID = 1, Name = "New York" });
            sourceLst.Add(new City { ID = 2, Name = "Boston" });
            sourceLst.Add(new City { ID = 3, Name = "Seattle" });
            sourceLst.Add(new City { ID = 4, Name = "Los Angeles" });
            sourceLst.Add(new City { ID = 5, Name = "Houston" });

            for (int i = 6; i < 10000; i++)
            {
                sourceLst.Add(new City { ID = i, Name = "_" + RandomString(5) + i.ToString() });
            }

            //cityList = sourceLst;
        }

        //https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool SearchData(City city)
        {
            return SearchText == null
                || city.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) != -1;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null) return;
            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);

        }

        #endregion
    }
}
