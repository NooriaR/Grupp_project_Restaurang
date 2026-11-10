using RestaurangSystem.Models;
using RestaurangSystem.UI;
namespace RestaurangSystem.Services
{
    public static class  AuthService
    {
        // Attribute
        public static Staff currentStaff { get; set; }
        // Methods

        public static bool IsValidEmail(string email) // this method checks if the email follows structure
        {
            int at = email.IndexOf('@');
            int dot = email.LastIndexOf('.');
            return at > 0 && dot > at + 1 && dot < email.Length - 1; // checks if there is letters infront and behind the @ and .
        }


        public static bool LogIn(string email, string password, DataStore<Staff> staffList  ) //this method logs in and create a currentStaff
        {
            try // tests the log in function, if there is no user prints an error
            {
                if (staffList.items.Count == 0) // checks if list is empty
                {
                    MenuHelper.ShowError("Please Register");
                    return false;
                }
                // finds a staff with the correct email and password
                currentStaff = staffList.FindItem(item => item.Email == email && item.Password == password);

                return currentStaff != null; // returns true if currentStaff isnt null
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError("Wrong Email or Password");
                return false;
            }

        }

    }
}
