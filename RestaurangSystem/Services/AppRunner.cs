using RestaurangSystem.UI;
using RestaurangSystem.Models;
using Spectre.Console;
namespace RestaurangSystem.Services
{
    public static class AppRunner
    {
        public static void Run()
        {
            try
            {
                // Skapar en instans av DataStore för Staff och laddar personaldata från JSON-filen
                DataStore<Staff> stafflist = new DataStore<Staff>();
                stafflist.LoadFromJson();
                Logger.Info($"App Started Succesfully"); // skickar logg
                StartMenuLogic(stafflist);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while running the application: {ex.Message}", ex);
                MenuHelper.ShowError($"An error occurred while running the application: {ex.Message}");
            }

        }
        public static void StaffMenuLogic() // Det här är logiken för personalmenyn, som körs när en personalmedlem är inloggad
        {
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>();
            menuDataStore.LoadFromJson();
            RestaurantMenu menu = new RestaurantMenu();
            while (AuthService.currentStaff.IsLoggedIn == true)
            {
                var choice = MenuHelper.StaffMenu();

                if (choice == StaffMenuChoice.Logout)
                {
                    menuDataStore.SaveToJson();
                    AuthService.LogOut();
                    continue;
                }
                if (choice == StaffMenuChoice.ManageOrders)
                {
                    ShowOrderMenuLogic(); // har en egen meny och loop, så ingen paus behövs
                    continue;
                }
                MenuHelper.RunScreen(() =>
                {
                    switch (choice)
                    {
                        case StaffMenuChoice.AddToMenu:
                            menu.AddMenuItem(menuDataStore);
                            Logger.Info($"MenuItem added by: {AuthService.currentStaff.Email}");
                            break;
                        case StaffMenuChoice.RemovefromMenu:
                            menu.RemoveMenuItem(menuDataStore);
                            Logger.Info($"MenuItem removed by: {AuthService.currentStaff.Email}");
                            break;
                        case StaffMenuChoice.UpdateitemonMenu:
                            menu.UppdateMenuItem(menuDataStore);
                            Logger.Info($"MenuItem updated by: {AuthService.currentStaff.Email}");
                            break;
                        case StaffMenuChoice.ShowLog:
                            Logger.ShowLog();
                            break;
                        case StaffMenuChoice.ShowSchedule:
                            AuthService.currentStaff.WorkingSchedule();
                            break;
                    }
                });
            }
        }
        public static void GuestMenuLogic()
        {
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>(); // skapar en instans av DataStore för MenuItem
            menuDataStore.LoadFromJson();
            RestaurantMenu menu = new RestaurantMenu(); // skapar en instans av RestaurantMenu
            menu.MenuItems = menuDataStore.items;
            bool isBrowsing = true; 
            Guest guest = new Guest(); // skapar en instans av Guest
            guest.Name = AnsiConsole.Ask<string>("What's your name?"); // frågar användaren efter deras namn och sparar det i guest-objektet

            while (isBrowsing)
            {
                // gör en meny för gästen att välja mellan olika alternativ
                var choice = MenuHelper.GuestMenu(guest);

                if (choice == GuestMenuChoice.Back)
                {
                    // Slutar loopen och går tillbaka till startmenyn
                    menuDataStore.SaveToJson();
                    isBrowsing = false;
                    continue;
                }

                MenuHelper.RunScreen(() =>
                {
                    switch (choice)
                    {
                        // Gör en order för att skapa en beställning
                        case GuestMenuChoice.CreateOrder:
                            guest.CreateOrder(menu.MenuItems);
                            Logger.Info($"Order created by: {guest.Name}"); // skickar logg
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
                            Logger.Info($"Reservation made by: {guest.Name}"); // skickar logg
                            break;
                        case GuestMenuChoice.ShowReservation:
                            guest.ShowReservation();
                            break;
                        case GuestMenuChoice.CancelReservation:
                            guest.CancelReservation();
                            Logger.Info($"Reservation canceled by: {guest.Name}"); // skickar logg
                            break;
                    }
                });
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
                            Logger.Info($"Staff registered: {staff.Email}"); // skickar logg
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
                        GuestMenuLogic();
                        break;
                    case StartMenuChoice.StaffView:
                        LogInAndRegisterMenuLogic(stafflist);
                        break;

                    case StartMenuChoice.Exit:
                        // Avsluta programmet.
                        Console.WriteLine("Exiting the program...");
                        Logger.Info($"Application closed"); // skickar logg
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
                        MenuHelper.RunScreen(() => AuthService.currentStaff.ShowOrders());
                        break;
                    case "Update Order Status":
                        int orderId = AnsiConsole.Ask<int>("Enter the order ID to update:");
                        var order = orderDataStore.FindItem(o => o.Id == orderId);
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