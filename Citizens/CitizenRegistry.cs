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
        private readonly DateTime dateToCompare = new DateTime(1899, 12, 31);
        private ICitizen[] allCitizens;
        private DateTime lastRegister;

        public CitizenRegistry()
        {
            allCitizens = new ICitizen[0];
        }

        public ICitizen this[string id]
        {
            get
            {
                if (String.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException("id");
                }

                return Array.Find<ICitizen>(allCitizens, human => human.VatId == id);
            }
        }

        public void Register(ICitizen citizen)
        {
            if (citizen == null)
            {
                throw new ArgumentNullException("citizen");
            }

            if (!String.IsNullOrWhiteSpace(citizen.VatId))
            {
                long tryParseId;
                if (long.TryParse(citizen.VatId, out tryParseId) && citizen.VatId.Length == 10)
                {
                    if (Array.Find<ICitizen>(allCitizens, human => human.VatId == citizen.VatId) != null)
                    {
                        throw new InvalidOperationException();
                    }

                    AddCitizenToArray(citizen);
                    lastRegister = SystemDateTime.Now();
                    return;
                }
            }

            citizen.VatId = GetID(citizen);
            AddCitizenToArray(citizen);
            lastRegister = SystemDateTime.Now();
        }

        public string Stats()
        {
            int female = 0;
            int male = 0;
            foreach (var person in allCitizens)
            {
                if (person == null)
                {
                    break;
                }
                else if (person.Gender == Gender.Female)
                {
                    female++;
                }
                else
                {
                    male++;
                }
            }

            if (female == 0 && male == 0)
            {
                return "0 men and 0 women";
            }

            return "man".ToQuantity(male) + " and " + "woman".ToQuantity(female)
            + ". Last registration was " + lastRegister.Humanize(dateToCompareAgainst: SystemDateTime.Now());
        }

        private string GetID(ICitizen citizen)
        {
            if (citizen == null)
            {
                throw new ArgumentNullException("citizen");
            }

            string result = String.Format("{0:00000}", citizen.BirthDate.Subtract(dateToCompare).TotalDays);
            int? ordinalNumber = null;
            foreach (var person in allCitizens)
            {
                if (person == null)
                {
                    break;
                }
                else if ((person.VatId.StartsWith(result) && ordinalNumber.HasValue && 
                         ordinalNumber < Convert.ToInt32(person.VatId.Substring(6, 4))) ||
                         (person.VatId.StartsWith(result) && !ordinalNumber.HasValue))
                {
                    ordinalNumber = Convert.ToInt32(person.VatId.Substring(6, 4));
                }
            }

            result += String.Format("{0:0000}", ordinalNumber != null ? ordinalNumber + 2 :
                      (citizen.Gender == Gender.Male ? 1 : 0));
            int sum = Convert.ToByte(result[0].ToString()) * (-1) + Convert.ToByte(result[1].ToString()) * 5 +
                      Convert.ToByte(result[2].ToString()) * 7 + Convert.ToByte(result[3].ToString()) * 9 +
                      Convert.ToByte(result[4].ToString()) * 4 + Convert.ToByte(result[5].ToString()) * 6 +
                      Convert.ToByte(result[6].ToString()) * 10 + Convert.ToByte(result[7].ToString()) * 5 +
                      Convert.ToByte(result[8].ToString()) * 7;
            result += (sum % 11) % 10;
            return result;
        }

        private void AddCitizenToArray(ICitizen citizen)
        {
            if (citizen == null)
            {
                throw new ArgumentNullException("citizen");
            }

            if (allCitizens.Length == 0)
            {
                Array.Resize<ICitizen>(ref allCitizens, 1);
            }

            ICitizen current;
            for (int i = 0; i < allCitizens.Length; i++)
            {
                if (allCitizens[i] == null)
                {
                    current = new Citizen(citizen.FirstName, citizen.LastName, citizen.BirthDate, citizen.Gender);
                    current.VatId = citizen.VatId;
                    allCitizens[i] = current;
                    return;
                }
            }

            Array.Resize<ICitizen>(ref allCitizens, allCitizens.Length * 2);
            current = new Citizen(citizen.FirstName, citizen.LastName, citizen.BirthDate, citizen.Gender);
            current.VatId = citizen.VatId;
            allCitizens[allCitizens.Length / 2] = current;
        }
    }
}
