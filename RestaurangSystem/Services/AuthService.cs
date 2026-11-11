using RestaurangSystem.Models;
using RestaurangSystem.UI;
namespace RestaurangSystem.Services
{
    public static class AuthService
    {
        // Attributer
        public static Staff currentStaff { get; set; }

        private static Staff? resetStaff { get; set; }  // personalen som håller på att återställa sitt lösenord
        private static string? resetCode { get; set; }  // den tillfälliga koden


        // Metoder

        public static bool StartReset(string email, DataStore<Staff> staffList, out string code) // letar upp personalen och skapar en engångskod
        {
            code = "";
            resetStaff = staffList.FindItem(s => s.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));

            if (resetStaff == null) return false;

            resetCode = Random.Shared.Next(100000, 1000000).ToString(); // sexsiffrig kod
            code = resetCode;
            return true;
        }

        public static bool CompleteReset(string code, string newPassword, DataStore<Staff> staffList) // byter lösenordet om koden stämmer
        {
            if (resetStaff == null || code != resetCode) return false;

            resetStaff.Password = newPassword;
            staffList.SaveToJson(); // sparar till JSON-filen

            resetStaff = null;
            resetCode = null; // koden kan inte användas två gånger
            return true;
        }

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

        public static void LogOut()
        {
            if (currentStaff == null) return; // kollar om det finns en staff

            currentStaff.IsLoggedIn = false; // sätter isloggedin till false, vilket stoppar staff att kunna se staffmenu
            Logger.Info($"Logout: {currentStaff.Email}"); // skickar logg

            currentStaff = null; // glöm bort vem som var inloggad
        }

    }
}