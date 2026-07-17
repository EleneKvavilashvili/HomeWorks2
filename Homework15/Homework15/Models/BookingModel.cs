using System.ComponentModel.DataAnnotations;

namespace Homework15.Models
{
    public class BookingModel
    {
        [Required(ErrorMessage = "Name is required!")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Surname is required!")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Doctor's name is required!")]
        public string Doctor { get; set; }


        [Required(ErrorMessage = "Booking time is required!")]
        [BookingTimeRange(ErrorMessage = "Working Hours are 10:00-19:00!")]
        public string Time { get; set; }
    }

    public class BookingTimeRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string timeString = value.ToString();

            
            if (TimeSpan.TryParse(timeString, out TimeSpan bookingTime))
            {
                TimeSpan startTime = new TimeSpan(10, 0, 0); // 10:00
                TimeSpan endTime = new TimeSpan(19, 0, 0);   // 19:00

                if (bookingTime >= startTime && bookingTime <= endTime)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage ?? "Can't book out of working hours");
        }
    }
}
