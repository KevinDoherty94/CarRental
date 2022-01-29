using System;

namespace Car_Rental
{
    public partial class Car
    {
        public static DateTime RentalStartDate { get; set; }
        public static DateTime RentalEndDate { get; set; }

        //ToString Method
        public override string ToString()
        {
            return string.Format($"{Make} - {Model}");
        }

        public string GetCarDetails()
        {
            return string.Format
                ($"CarID:{Id}" +
                $"\nMake: {Make}" +
                $"\nModel: {Model}" +
                $"\nRental Date: {RentalStartDate.ToShortDateString()}" +
                $"\nReturn Date: {RentalEndDate.ToShortDateString()}");
        }
    }
}