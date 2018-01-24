using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.ViewModel
{
    public class NameList : ObservableCollection<PersonName>
    {
        public NameList() : base()
        {
            Add(new PersonName("Willa", "Cather"));
            Add(new PersonName("Isak", "Dinesem"));
            Add(new PersonName("Viktor", "Hugo"));
            Add(new PersonName("Jules", "Verne"));
        }

    }

    public class PersonName
    {
        private string firstName;
        private string lastName;

        public PersonName(string first, string last)
        {
            this.firstName = first;
            this.lastName = last;
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
    }
}
