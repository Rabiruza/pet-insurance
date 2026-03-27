namespace PetInsurance.Tests.Models
{
    public class BookingBuilder
    {
        private readonly Booking _booking = new Booking();

        public BookingBuilder WithFirstName(string firstName)
        {
            _booking.FirstName = firstName;
            return this;
        }

        public BookingBuilder WithLastName(string lastName)
        {
            _booking.LastName = lastName;
            return this;
        }

        public BookingBuilder WithTotalPrice(int price)
        {
            _booking.TotalPrice = price;
            return this;
        }

        public BookingBuilder WithDepositPaid(bool paid)
        {
            _booking.DepositPaid = paid;
            return this;
        }

        public BookingBuilder WithDates(string checkIn, string checkOut)
        {
            _booking.BookingDates = new BookingDates
            {
                CheckIn = checkIn,
                CheckOut = checkOut
            };
            return this;
        }

        public BookingBuilder WithAdditionalNeeds(string needs)
        {
            _booking.AdditionalNeeds = needs;
            return this;
        }

        public BookingBuilder WithRandomData()
        {
            var random = new Random();
            _booking.FirstName = "User" + random.Next(1000, 9999);
            _booking.LastName = "Test" + random.Next(1000, 9999);
            _booking.TotalPrice = random.Next(50, 500);
            _booking.DepositPaid = random.Next(0, 2) == 1;
            _booking.BookingDates = new BookingDates
            {
                CheckIn = DateTime.Now.AddDays(random.Next(1, 30)).ToString("yyyy-MM-dd"),
                CheckOut = DateTime.Now.AddDays(random.Next(31, 60)).ToString("yyyy-MM-dd")
            };
            _booking.AdditionalNeeds = "None";
            return this;
        }

        // Метод для створення "стандартного" об'єкта з дефолтними даними
        public Booking Build()
        {
            if (string.IsNullOrEmpty(_booking.FirstName))
                _booking.FirstName = "John";
            if (string.IsNullOrEmpty(_booking.LastName))
                _booking.LastName = "Doe";
            if (_booking.TotalPrice == 0)
                _booking.TotalPrice = 100;
            if (_booking.BookingDates == null)
                _booking.BookingDates = new BookingDates
                {
                    CheckIn = "2024-01-01",
                    CheckOut = "2024-01-02"
                };
            return _booking;
        }
    }
}