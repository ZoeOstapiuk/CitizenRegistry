namespace Citizens.Tests
{
    using System;
    using System.Text.RegularExpressions;
    using Citizens;
    using Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CitizenRegistryTests : TestsBase
    {
        private ICitizenRegistry registry;
        private readonly DateTime TestBirthDate = new DateTime(1991, 8, 24);
        private const string TestVatIdForMan = "3347300011";
        private const string TestVatIdForWoman = "3347300005";

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            registry = CreateCitizenRegistry();
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Register_ForAlreadyRegisteredVatId_ThrowsInvalidOperationException()
        {
            string vatId = TestVatIdForMan;
            var citizen1 = CitizenBuilder.NewMan().WithVatId(vatId).Build();
            var citizen2 = CitizenBuilder.NewMan().WithVatId(vatId).Build();
            registry.Register(citizen1);
            registry.Register(citizen2);
        }
        
        [TestMethod]
        public void Register_ForCitizenWithEmptyVatId_AssignsVatId()
        {
            var citizen = CitizenBuilder.NewWoman().WithDate(SystemDateTime.Now()).Build();
            registry.Register(citizen);

            Assert.IsFalse(String.IsNullOrWhiteSpace(citizen.VatId));
        }
        
        [TestMethod]
        public void Register_ForCitizenWithEmptyVatId_AssignsVatIdBasedOnBirthDate()
        {
            var citizen = CitizenBuilder.NewWoman().WithDate(new DateTime(1991, 8, 24)).Build();
            registry.Register(citizen);

            StringAssert.StartsWith(citizen.VatId, "33473");
        }
        
        [TestMethod]
        public void Register_ForManWithEmptyVatId_AssignsVatIdWithOddCounter()
        {
            var citizen = CitizenBuilder.NewMan().WithDate(TestBirthDate).Build();
            registry.Register(citizen);

            int counter = Int32.Parse(citizen.VatId.Substring(5, 4));
            Assert.AreEqual(1, counter & 1);
        }
        
        [TestMethod]
        public void Register_ForWomanWithEmptyVatId_AssignsVatIdWithEvenCounter()
        {
            var citizen = CitizenBuilder.NewWoman().WithDate(TestBirthDate).Build();
            registry.Register(citizen);

            int counter = Int32.Parse(citizen.VatId.Substring(5, 4));
            Assert.AreEqual(0, counter & 1);
        }
        
        [TestMethod]
        public void Register_ForTwoWomenWithSameBirthDate_AssignsDifferentVatIds()
        {
            var w1 = CitizenBuilder.NewWoman().WithDate(TestBirthDate).Build();
            var w2 = CitizenBuilder.NewWoman().WithDate(TestBirthDate).Build();
            registry.Register(w1);
            registry.Register(w2);

            Assert.AreNotEqual(w1.VatId, w2.VatId);
        }
        
        [TestMethod]
        public void Register_ForCitizenWithEmptyVatId_AssignedVatIdContainsDigitsOnly()
        {
            var citizen = CitizenBuilder.NewWoman().WithDate(SystemDateTime.Now()).Build();
            registry.Register(citizen);

            StringAssert.Matches(citizen.VatId, new Regex("^[0-9]+$"));
        }
        
        [TestMethod]
        public void Register_ForCitizenWithEmptyVatId_AssignedVatIdIsTenCharsLength()
        {
            var citizen = CitizenBuilder.NewWoman().WithDate(new DateTime(1909, 7, 5)).Build();
            registry.Register(citizen);

            Assert.AreEqual(10, citizen.VatId.Length);
        }
        
        [TestMethod]
        public void Indexer_WithRegisteredVatId_ReturnsCitizenFoundByVatId()
        {
            var citizen = CitizenBuilder.NewMan().WithDate(SystemDateTime.Now()).Build();
            registry.Register(citizen);

            ICitizen foundCitizen = registry[citizen.VatId];

            Assert.AreEqual(citizen.VatId, foundCitizen.VatId);
            Assert.AreEqual(citizen.FirstName, foundCitizen.FirstName);
            Assert.AreEqual(citizen.LastName, foundCitizen.LastName);
            Assert.AreEqual(citizen.BirthDate, foundCitizen.BirthDate);
        }

        [TestMethod]
        public void Indexer_WhenCitizenChanges_RegistryHasNoChanges()
        {
            var citizen = CitizenBuilder.NewMan().WithDate(SystemDateTime.Now()).Build();
            registry.Register(citizen);

            string id = citizen.VatId;
            citizen.VatId = "0123456789";
            ICitizen foundCitizen = registry[id];

            Assert.AreNotEqual(citizen.VatId, foundCitizen.VatId);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Indexer_WithNull_ThrowsArgumentNullException()
        {
            var result = registry[null];
        }
        
        [TestMethod]
        public void Indexer_WithNotRegisteredVatId_ReturnsNull()
        {
            var result = registry["0123456789"];
            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void Stats_WithEmptyRegistry_ReturnsMessageZeroMenAndZeroWomen()
        {
            var msg = registry.Stats();
            Assert.AreEqual("0 men and 0 women", msg);
        }

        [TestMethod]
        public void Stats_WithOneManInRegistry_ReturnsMessageOneManAndZeroWomenWithLastRegistrationTimeInfo()
        {
            var bd = TestTodayDate.AddYears(-1);
            var citizen = CitizenBuilder.NewMan().WithDate(bd).Build();

            var yesterday = TestTodayDate.AddDays(-1);
            SystemDateTime.Now = () => yesterday;
            registry.Register(citizen);
            SystemDateTime.Now = () => TestTodayDate;

            var msg = registry.Stats();
            Assert.AreEqual("1 man and 0 women. Last registration was yesterday", msg);
        }

        // New test method:
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullArgument_throwsNullArgumentException()
        {
            ICitizen citizen = null;

            registry.Register(citizen);
        }

        private ICitizenRegistry CreateCitizenRegistry()
        {
            return new CitizenRegistry();
        }
    }
}
