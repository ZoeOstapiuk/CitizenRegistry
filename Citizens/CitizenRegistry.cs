using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

namespace Citizens
{
    public class CitizenRegistry : ICitizenRegistry
    {
        private ICitizen[] AllCitizens;
        private DateTime _lastRegister;

        public CitizenRegistry()
        {
            AllCitizens = new ICitizen[0];
        }

        public ICitizen this[string id]
        {
            get
            {
                if (String.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException();
                }
                for (int i = 0; i < AllCitizens.Length; i++)
                {
                    if (AllCitizens[i].VatId == id)
                    {
                        return AllCitizens[i];
                    }
                }
                return null;
            }
        }

        public void Register(ICitizen citizen)
        {
            if (citizen == null)
            {
                throw new ArgumentNullException();
            }
            if (!String.IsNullOrWhiteSpace(citizen.VatId))
            {
                long TryParse;
                if (long.TryParse(citizen.VatId, out TryParse) && citizen.VatId.Length == 10)
                {
                    foreach (var Person in AllCitizens)
                    {
                        if (Person.VatId == citizen.VatId)
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    Array.Resize<ICitizen>(ref AllCitizens, AllCitizens.Length + 1);
                    AllCitizens[AllCitizens.Length - 1] = new Citizen(citizen.FirstName, citizen.LastName, citizen.BirthDate, citizen.Gender);
                    AllCitizens[AllCitizens.Length - 1].VatId = citizen.VatId;

                    _lastRegister = DateTime.Now;
                    return;
                }
                throw new InvalidOperationException();
            }

            citizen.VatId = GetID(citizen);
            Array.Resize<ICitizen>(ref AllCitizens, AllCitizens.Length + 1);
            
            AllCitizens[AllCitizens.Length - 1] = new Citizen(citizen.FirstName, citizen.LastName, citizen.BirthDate, citizen.Gender);
            AllCitizens[AllCitizens.Length - 1].VatId = citizen.VatId;

            _lastRegister = SystemDateTime.Now();
        }

        public string Stats()
        {
            int Female = 0;
            int Male = 0;
            foreach (var Person in AllCitizens)
            {
                if (Person.Gender == Gender.Female) Female++;
                else Male++;
            }

            if (Female == 0 && Male == 0)
            {
                return "0 men and 0 women";
            }
            string Result = Male.ToString() + (Male != 1 ? " men" : " man") + " and " + Female.ToString() +
                            (Female != 1 ? " women. " : " woman. ") + "Last registration was " +
                            _lastRegister.Humanize(dateToCompareAgainst: SystemDateTime.Now());
            return Result;
        }

        private string GetID(ICitizen citizen)
        {
            if (citizen == null || citizen.BirthDate == null)
            {
                throw new ArgumentException();
            }
            string Result = citizen.BirthDate.Subtract(new DateTime(1899, 12, 31)).TotalDays.ToString();
            if (Result.Length < 5)
            {
                Result = "0" + Result;
            }

            int OrdinalNumber = 0;
            foreach (var Person in AllCitizens)
            {
                if (Person.VatId.StartsWith(Result) &&
                    OrdinalNumber < Convert.ToInt32(Person.VatId.Substring(6, 3)))
                {
                    OrdinalNumber = Convert.ToInt32(Person.VatId.Substring(6, 3));
                }
            }

            Result += (++OrdinalNumber).ToString("000");
            
            Result += citizen.Gender == Gender.Female ? "0" : "1";

            int Sum = Convert.ToByte(Result[0]) * (-1) + Convert.ToByte(Result[1]) * 5 +
                      Convert.ToByte(Result[2]) * 7 + Convert.ToByte(Result[3]) * 9 +
                      Convert.ToByte(Result[4]) * 4 + Convert.ToByte(Result[5]) * 6 +
                      Convert.ToByte(Result[6]) * 10 + Convert.ToByte(Result[7]) * 5 +
                      Convert.ToByte(Result[8]) * 7;
            Sum = (Sum % 11) % 10;

            Result += Sum.ToString();
            return Result;
        }
    }
}
