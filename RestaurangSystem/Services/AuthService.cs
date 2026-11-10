using RestaurangSystem.Models;
using RestaurangSystem.UI;
namespace RestaurangSystem.Services
{
    public static class AuthService
    {
        // Attributer
        public static Staff currentStaff { get; set; }
        // Metoder

        public static bool IsValidEmail(string email) // den här metoden kontrollerar om epostadressen följer rätt struktur
        {
            int at = email.IndexOf('@');
            int dot = email.LastIndexOf('.');
            return at > 0 && dot > at + 1 && dot < email.Length - 1; // kontrollerar att det finns bokstäver före och efter @ och .
        }
        public static bool LogIn(string email, string password, DataStore<Staff> staffList) // den här metoden loggar in och skapar en currentStaff
        {
            try // testar inloggningen, om det inte finns någon användare visas ett felmeddelande
            {
                if (staffList.items.Count == 0) // kontrollerar om listan är tom
                {
                    MenuHelper.ShowError("Please Register");
                    return false;
                }
                // hittar en personal med rätt e-post och lösenord
                currentStaff = staffList.FindItem(item => item.Email == email && item.Password == password);

                return currentStaff != null; // returnerar true om currentStaff inte är null
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError("Wrong Email or Password");
                return false;
            }

        }

    }
}