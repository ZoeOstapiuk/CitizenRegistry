using System;
using Humanizer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citizens
{
    public class Citizen : ICitizen
    {
        private DateTime _birthDate;
        private string _firstName;
        private string _lastName;
        private Gender _gender;
        private string _vatId;

        public Citizen(string FirstName, string LastName, DateTime BirthDate, Gender gender)
        {
            if (BirthDate > SystemDateTime.Now())
            {
                throw new ArgumentException();
            }
            if ((int)gender > 1 || (int)gender < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            _firstName = FirstName.Transform(To.LowerCase, To.TitleCase);
            _lastName = LastName.Transform(To.LowerCase, To.TitleCase);
            _birthDate = BirthDate.Date;
            _gender = gender;
        }

        public DateTime BirthDate { get { return _birthDate; } }

        public string FirstName { get { return _firstName; } }

        public Gender Gender { get { return _gender; } }

        public string LastName { get { return _lastName; } }

        public string VatId
        {
            get { return _vatId; }
            set { _vatId = value; }
        }
    }
}
