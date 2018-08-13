using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfComponentApp.Model
{
    public enum Gender
    {
        Male, Female
    }


    public class Person : INotifyPropertyChanged
    {
        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

        public Person(string firstName, string lastName, string email, int age, Gender gender, bool isStudent)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Age = age;
            this.Gender = gender;
            this.IsStudent = isStudent;
        }

        #endregion constructor


        #region properties

        private string firstName;
        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            set
            {
                this.firstName = value;
                NotifyPropertyChanged(nameof(this.FirstName));
            }
        }

        private string lastName;
        public string LastName
        {
            get
            {
                return this.lastName;
            }
            set
            {
                this.lastName = value;
                NotifyPropertyChanged(nameof(this.LastName));
            }
        }

        private string email;
        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
                NotifyPropertyChanged(nameof(this.Email));
            }
        }

        private int age;
        public int Age
        {
            get
            {
                return this.age;
            }
            set
            {
                this.age = value;
                NotifyPropertyChanged(nameof(this.Age));
            }
        }
        
        private Gender gender;
        public Gender Gender
        {
            get
            {
                return this.gender;
            }
            set
            {
                this.gender = value;
                NotifyPropertyChanged(nameof(this.Gender));
            }
        }

        private bool isStudent;
        public bool IsStudent
        {
            get
            {
                return this.isStudent;
            }
            set
            {
                this.isStudent = value;
                NotifyPropertyChanged(nameof(this.IsStudent));
            }
        }


        #endregion properties


        #region methods

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion methods
    }
}
