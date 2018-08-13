using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfComponentApp.Model;

namespace WpfComponentApp.UI
{
    public class MainWindowViewModel
    {
        #region fields

        #endregion fields


        #region constructor

        public MainWindowViewModel()
        {
            this.Persons = new ObservableCollection<Person>();
            this.Persons.Add(new Person("Bob", "Boberson", "Bobby@hotmail.edu", 96, Gender.Male, false));
            this.Persons.Add(new Person("Sally", "Sallerson", "Sally@duckduck.go", 36, Gender.Female, false));
            this.Persons.Add(new Person("Jack", "Jackson", "Jack@gmail.org", 22, Gender.Male, true));
            this.Persons.Add(new Person("Jill", "Jillian", "Jill@outlook.xyz", 20, Gender.Female, true));
            this.Persons.Add(new Person("Barack", "Obama", "Barack@Obama.gov", 20, Gender.Male, false));

            this.GroupedPersons = new ListCollectionView(this.Persons);
            this.GroupedPersons.GroupDescriptions.Add(new PropertyGroupDescription("Gender"));
        }

        #endregion constructor


        #region properties

        private ObservableCollection<Person> persons;
        public ObservableCollection<Person> Persons
        {
            get
            {
                return persons;
            }
            set
            {
                this.persons = value;
            }
        }

        public ICollectionView GroupedPersons { get; private set; }



        #endregion properties


        #region methods

        #endregion methods
    }
}
