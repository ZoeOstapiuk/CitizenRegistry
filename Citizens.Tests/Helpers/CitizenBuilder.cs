using System;

namespace Citizens.Tests.Helpers
{
    internal sealed class CitizenBuilder
    {
        private Gender gender;
        private DateTime dateOfBirth;
        private string firstName;
        private string lastName;
        private string vatId;

        private CitizenBuilder(string firstName, string lastName, Gender gender)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.gender = gender;
        }

        public static CitizenBuilder NewMan()
        {
            var builder = new CitizenBuilder("Roger", "Pierce", Gender.Male);
            return builder;
        }

        public static CitizenBuilder NewWoman()
        {
            var builder = new CitizenBuilder("Anne", "Jensen", Gender.Female);
            return builder;
        }

        public CitizenBuilder WithDate(DateTime birthDate)
        {
            dateOfBirth = birthDate;
            return this;
        }

        public CitizenBuilder WithVatId(string vatId)
        {
            this.vatId = vatId;
            return this;
        }

        public ICitizen Build()
        {
            var citizen = new Citizen(firstName, lastName, dateOfBirth, gender);
            citizen.VatId = vatId;
            return citizen;
        }
    }
}
