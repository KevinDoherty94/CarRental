using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Car_Rental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CarAndBookingsEntities db = new CarAndBookingsEntities();

        private string[] types = { "All", "Small", "Medium", "Large" };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //limits the dates that can be picked between now and a year from now and selects todays date and a date 14 days from now automatically

                dpStartDate.DisplayDateStart = DateTime.Now;
                dpStartDate.DisplayDateEnd = DateTime.Now.AddYears(1);
                dpStartDate.SelectedDate = DateTime.Now;

                dpEndDate.DisplayDateStart = DateTime.Now;
                dpEndDate.DisplayDateEnd = DateTime.Now.AddYears(1);
                dpEndDate.SelectedDate = DateTime.Now.AddDays(14);

                //Displays the booking date in the tblSelectedCar text block automatically when window is loaded

                Car.RentalStartDate = DateTime.Now;
                Car.RentalEndDate = DateTime.Now.AddDays(14);

                //ComboBox updated with an enum Type

                cbxCarType.ItemsSource = types.ToList();

                //Displays automatically the first entry
                cbxCarType.SelectedIndex = 0;
                lbAvailableCars.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load application correctly. Message: {ex.Message}");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Converting to a DateTime type
                DateTime startDate = (DateTime)dpStartDate.SelectedDate;
                DateTime endDate = (DateTime)dpEndDate.SelectedDate;

                //Dates get stored to a static property in the Car class
                Car.RentalStartDate = startDate;
                Car.RentalEndDate = endDate;

                IQueryable<Car> query;

                //Using an if statement to display all cars or the ones that are available on a particular date

                if (cbxCarType.SelectedValue == types[0])
                {
                    query = from b in db.Bookings
                            join c in db.Cars on b.CarId equals c.Id
                            where !(startDate >= b.StartDate && endDate <= b.EndDate)
                            orderby c.Make
                            select c;
                }
                else
                {
                    query = from b in db.Bookings
                            join c in db.Cars on b.CarId equals c.Id
                            where !(startDate >= b.StartDate && endDate <= b.EndDate) && c.Size == cbxCarType.SelectedValue
                            orderby c.Make
                            select c;
                }

                //Add items to a list and gets rid off duplicates
                lbAvailableCars.ItemsSource = query.ToList().Distinct();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to pick these dates, please try again");
            }
        }

        private void lbAvailableCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Creates an object with the selected field
                Car selectedCar = lbAvailableCars.SelectedItem as Car;

                //Selects what image to use

                if (selectedCar != null)
                {
                    switch (selectedCar.Make)
                    {
                        case "Opel":
                            image.Source = new BitmapImage(new Uri("/Images/Opel.png", UriKind.Relative));
                            break;
                        case "Toyota":
                            image.Source = new BitmapImage(new Uri("/Images/Toyota.png", UriKind.Relative));
                            break;
                        case "Skoda":
                            image.Source = new BitmapImage(new Uri("/Images/Skoda.png", UriKind.Relative));
                            break;
                        case "Citreon":
                            image.Source = new BitmapImage(new Uri("/Images/Citroen.png", UriKind.Relative));
                            break;
                        case "Ford":
                            image.Source = new BitmapImage(new Uri("/Images/Ford.png", UriKind.Relative));
                            break;
                        default:
                            image.Source = new BitmapImage(new Uri("/Images/Peugeot.png", UriKind.Relative));
                            break;
                    }
                }
                else
                {
                    string.Format("No available cars");
                }

                //Displays the booking details
                if (selectedCar != null)
                {
                    tblSelectedCar.Text = selectedCar.GetCarDetails();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to select this car");
            }
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Creates an object when a car is selected
                Car selectedCar = lbAvailableCars.SelectedItem as Car;

                Booking date = new Booking()
                {
                    StartDate = Car.RentalStartDate,
                    EndDate = Car.RentalEndDate,
                    CarId = selectedCar.Id
                };

                db.Bookings.Add(date);
                db.SaveChanges();

                //Displays a message box
                MessageBox.Show
                        ($"Booking Confirmation" +
                        $"\n\n{selectedCar.GetCarDetails()}");
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to book this car");
            }
        }

        private void cbxCarType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //A query to add the correct cars from the comboBox

                var query = from s in db.Cars
                            orderby s.Make
                            select s;

                //Need to get selected as a string

                string selected = cbxCarType.SelectedItem as string;

                switch (selected)
                {
                    case "All":
                        //Do nothing query above
                        break;

                    case "Small":
                        query = from s in db.Cars
                                where s.Size.Equals("Small")
                                orderby s.Make
                                select s;
                        break;

                    case "Medium":
                        query = from s in db.Cars
                                where s.Size.Equals("Medium")
                                orderby s.Make
                                select s;
                        break;

                    case "Large":
                        query = from s in db.Cars
                                where s.Size.Equals("Large")
                                orderby s.Make
                                select s;
                        break;
                }

                //Adds cars to the listBox

                lbAvailableCars.ItemsSource = query.ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to select this size");
            }
        }
    }
}