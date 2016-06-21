using System;
using System.Text;
using Humanizer;

namespace Citizens
{
    public class CitizenRegistry : ICitizenRegistry
    {
        private readonly DateTime dateToCompare = new DateTime(1899, 12, 31);
        private ICitizen[] allCitizens;
        private int registeredCitizensCount;
        private DateTime lastRegisterTime;

        public CitizenRegistry()
        {
            allCitizens = new ICitizen[0];
            registeredCitizensCount = 0;
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
                    return;
                }
            }

            citizen.VatId = GetID(citizen);
            AddCitizenToArray(citizen);
        }

        public string Stats()
        {
            int female = 0;
            int male = 0;
            for (int i = 0; i < registeredCitizensCount; i++)
            {
                if (allCitizens[i].Gender == Gender.Female)
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
            + ". Last registration was " + lastRegisterTime.Humanize(dateToCompareAgainst: SystemDateTime.Now());
        }

        private string GetID(ICitizen citizen)
        {
            if (citizen == null)
            {
                throw new ArgumentNullException("citizen");
            }

            var result = new StringBuilder();
            result.Append(String.Format("{0:00000}", citizen.BirthDate.Subtract(dateToCompare).TotalDays));

            int? ordinalNumber = null;
            for (int i = registeredCitizensCount - 1; i >= 0; i--)
            {
                if (allCitizens[i].VatId.StartsWith(result.ToString()))
                {
                    ordinalNumber = Convert.ToInt32(allCitizens[i].VatId.Substring(6, 4));
                    break;
                }
            }

            result.Append(String.Format("{0:0000}", ordinalNumber != null ? ordinalNumber + 2 :
                         (citizen.Gender == Gender.Male ? 1 : 0)));
            result.Append(FindKey(result.ToString()));

            return result.ToString();
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

            if (registeredCitizensCount == allCitizens.Length)
            {
                Array.Resize<ICitizen>(ref allCitizens, allCitizens.Length * 2);
            }

            allCitizens[registeredCitizensCount] = new Citizen(citizen.FirstName, citizen.LastName, citizen.BirthDate, citizen.Gender);
            allCitizens[registeredCitizensCount].VatId = citizen.VatId;
            lastRegisterTime = SystemDateTime.Now();
            registeredCitizensCount++;
        }

        private char FindKey(string preId)
        {
            return (((Convert.ToByte(preId[0].ToString()) * (-1) + Convert.ToByte(preId[1].ToString()) * 5 +
                   Convert.ToByte(preId[2].ToString()) * 7 + Convert.ToByte(preId[3].ToString()) * 9 +
                   Convert.ToByte(preId[4].ToString()) * 4 + Convert.ToByte(preId[5].ToString()) * 6 +
                   Convert.ToByte(preId[6].ToString()) * 10 + Convert.ToByte(preId[7].ToString()) * 5 +
                   Convert.ToByte(preId[8].ToString()) * 7) % 11) % 10).ToString()[0];
        }
    }
}
