using RestaurangSystem.Models;
using RestaurangSystem.Services;
using RestaurangSystem.UI;
namespace RestaurangSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataStore<Staff> stafflist = new DataStore<Staff>();
            stafflist.LoadFromJson();
            bool isRunning = true;
            while (isRunning)
            {
                // Visa huvudmenyn och få användarens val.
                var choice = MenuHelper.StartMenu();
                // Hantera användarens val.
                switch (choice)
                {
                    case StartMenuChoice.GuestView:
                        AppRunner.GuestMenuLogic();
                        break;
                    case StartMenuChoice.StaffView:
                        AppRunner.LogInAndRegisterLogic(stafflist);
                        break;

                       
                    case StartMenuChoice.Exit:
                        // Avsluta programmet.
                        Console.WriteLine("Avslutar programmet...");
                        stafflist.SaveToJson(); // spara personaldata innan programmet avslutas
                        isRunning = false;
                        return;
                }
            }


        }
          
    }

}

