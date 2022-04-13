using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WebApp.Models
{
    public class User
    {
        private DateTime _dob;
        private string _nric;
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Age { get; private set; }
        public string NRIC
        {
            get { return _nric; }
            set { _nric = value; if (_nric != null) ExtractDOB(); }
        }

        public string DOB
        {
            get { return _dob.ToString("dd/MM/yyyy"); }
            set { ConvertDOB(value); }
        }

        private void ExtractDOB()
        {
            if (_nric?.Length != 12) return;

            int year, month, day;
            bool isNumericYear = int.TryParse(_nric.Substring(0, 2), out year);
            bool isNumericMonth = int.TryParse(_nric.Substring(2, 2), out month);
            bool isNumericDay = int.TryParse(_nric.Substring(4, 2), out day);

            if (!isNumericYear || !isNumericMonth || !isNumericDay) return;

            year = CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);


            DOB = $"{day}/{month}/{year}";
        }

        private void ConvertDOB(string sDOB)
        {
            if (string.IsNullOrEmpty(sDOB)) return;
            string[] dmy = Regex.Split(sDOB, @"[^\d]");
            if (dmy.Length != 3) return;

            int dd = validate(dmy[0]);
            int mm = validate(dmy[1]);
            int yy = validate(dmy[2]);

            if (dd == -1 || mm == -1 || yy == -1) return;

            _dob = new DateTime(yy, mm, dd);

            CalculateAge();
        }

        private void ConvertDOB(DateTime sDOB)
        {
            if (sDOB == DateTime.MinValue) return;
            _dob = sDOB;

            CalculateAge();
        }

        public static string ConvertDOBFormat(string sDOB, string newFormat)
        {
            if (string.IsNullOrEmpty(sDOB)) return "Empty Input";
            string[] dmy = Regex.Split(sDOB, @"[^\d]");
            if (dmy.Length != 3) return "Only accept day month year delimited with single character";

            var newDate = new DateTime(Convert.ToInt32(dmy[2]), Convert.ToInt32(dmy[1]), Convert.ToInt32(dmy[0]));
            Console.WriteLine($"Date from ConvertDOBFormat: { newDate.ToString(newFormat)}");
            return newDate.ToString(newFormat);
        }

        private void CalculateAge()
        {
            Age = DateTime.Now.Year - _dob.Year;
        }

        private int validate(string dmy)
        {
            int _dmy;
            bool isNumeric = int.TryParse(dmy, out _dmy);
            return (isNumeric) ? _dmy : -1;
        }

    }
}
