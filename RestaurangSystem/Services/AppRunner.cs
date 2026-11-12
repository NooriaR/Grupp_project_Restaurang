using RestaurangSystem.UI;
using RestaurangSystem.Models;
using Spectre.Console;

namespace RestaurangSystem.Services
{
    public static class AppRunner
    {

        public static void Run()
        {
            DataStore<Staff> stafflist = new DataStore<Staff>();
            stafflist.LoadFromJson();
            StartMenuLogic(stafflist);
        }
        public static void StaffMenuLogic() // Det här är logiken för personalmenyn, som körs när en personalmedlem är inloggad
        {
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>();
            menuDataStore.LoadFromJson();
            RestaurantMenu menu = new RestaurantMenu();
            while (AuthService.currentStaff.IsLoggedIn==true)
            {
                var choice = MenuHelper.StaffMenu();
                switch (choice)
                {
                    case StaffMenuChoice.AddToMenu:
                        menu.AddMenuItem(menuDataStore);
                        break;
                    case StaffMenuChoice.RemovefromMenu:
                        menu.RemoveMenuItem(menuDataStore);
                        break;
                    case StaffMenuChoice.UpdateitemonMenu:
                        menu.UppdateMenuItem(menuDataStore);
                        break;
                    case StaffMenuChoice.ManageOrders:
                        ShowOrderMenuLogic();
                        break;
                    case StaffMenuChoice.ShowSchedule:
                        AuthService.currentStaff.WorkingSchedule();
                        break;
                    case StaffMenuChoice.Logout:
                        menuDataStore.SaveToJson();
                        AuthService.LogOut();
                        break;
 
                }
            }
        }
        public static void GuestMenuLogic()
        {
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>(); // skapar en instans av DataStore för MenuItem
            menuDataStore.LoadFromJson();
            RestaurantMenu menu = new RestaurantMenu(); // skapar en instans av RestaurantMenu
            bool isBrowsing = true; 
            Guest guest = new Guest(); // skapar en instans av Guest
            guest.Name = AnsiConsole.Ask<string>("What's your name?"); // frågar användaren efter deras namn och sparar det i guest-objektet

            while (isBrowsing == true)
            {
                // gör en meny för gästen att välja mellan olika alternativ
                var choice = MenuHelper.GuestMenu(guest);
                switch (choice)
                {
                    //Gö em order för att skapa en beställning
                    case GuestMenuChoice.CreateOrder:
                        guest.CreateOrder(menu.MenuItems);
                        break;
                    // Visar menyn för gästen
                    case GuestMenuChoice.ShowMenu:
                        RestaurantMenu.ShowMenu(menuDataStore);
                        break;

                    case GuestMenuChoice.ShowOrderStatus:
                        guest.ShowOrderStatus();
                        break;

                    case GuestMenuChoice.MakeReservation:
                        guest.MakeReservation();
                        break;

                    case GuestMenuChoice.Back:
                        // Slutar loopen och går tillbaka till startmenyn
                        menuDataStore.SaveToJson();
                        isBrowsing = false;
                        break;
                }
            }
        }

        public static void LogInAndRegisterMenuLogic(DataStore<Staff> stafflist) // Det här är logiken för inloggnings- och registreringsmenyn, som körs när en personalmedlem vill logga in eller registrera sig
        {
            // Loopa tills användaren väljer att gå tillbaka
            {
                bool isRunning = true;
                while (isRunning)
                {
                    var choice = MenuHelper.StaffPortal();
                    switch (choice)
                    {
                        case "Log In":
                            bool loginsuccesfull = MenuHelper.LogInMenu(stafflist);
                            if (loginsuccesfull == true)
                            {
                                StaffMenuLogic();
                                break;
                            }
                            break;
                        case "Register":
                            Staff staff = new Staff();
                            staff.Register();
                            break;
                        case "Back":
                            isRunning = false;
                            break;
                    }
                }
            }
        }

        public static void StartMenuLogic(DataStore<Staff> stafflist) // det här är logiken för startmenyn, som körs när programmet startar
        {
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
                        AppRunner.LogInAndRegisterMenuLogic(stafflist);
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

        public static void ShowOrderMenuLogic() // det här är logik för orderhanteringsmenyn, som körs när en personalmedlem vill hantera beställningar
        {
            DataStore<Order> orderDataStore = new DataStore<Order>();
            orderDataStore.LoadFromJson();
            bool running = true;

            while (running)
            {
                var choice =  MenuHelper.ShowOrderMenu();

                switch (choice)
                {
                    case "Show orders":
                        AuthService.currentStaff.ShowOrders();
                        break;
                    case "Update Order Status":
                        var order = orderDataStore.FindItem(o => o.Id == AnsiConsole.Ask<int>("Enter the order ID to update:"));
                        if (order != null)
                        {
                            var newStatus = AnsiConsole.Prompt(
                                new SelectionPrompt<OrderStatus>()
                                    .Title("Select the new status:")
                                    .AddChoices(Enum.GetValues<OrderStatus>()));
                            order.UpdateStatus(newStatus);
                            orderDataStore.SaveToJson();
                        }
                        break;
                    case "Back":
                        running = false;
                        break;
                }
            }
        }
    }
}

