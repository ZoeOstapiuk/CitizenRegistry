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
        private DateTime birthDate;
        private string firstName;
        private string lastName;
        private Gender gender;

        public Citizen(string firstName, string lastName, DateTime birthDate, Gender gender)
        {
            if (birthDate > SystemDateTime.Now())
            {
                throw new ArgumentException("BirthDate");
            }

            if (!Enum.IsDefined(typeof(Gender), gender))
            {
                throw new ArgumentOutOfRangeException("Gender");
            }

            if (String.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException("FirstName");
            }

            if (String.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException("LastName");
            }

            this.firstName = firstName.Transform(To.LowerCase, To.TitleCase);
            this.lastName = lastName.Transform(To.LowerCase, To.TitleCase);
            this.birthDate = birthDate.Date;
            this.gender = gender;
        }

        public DateTime BirthDate { get { return birthDate; } }

        public string FirstName { get { return firstName; } }

        public Gender Gender { get { return gender; } }

        public string LastName { get { return lastName; } }

        public string VatId { get; set; }
    }
}
