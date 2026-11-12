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
        public static void StaffMenuLogic()
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
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>();
            menuDataStore.LoadFromJson();
            RestaurantMenu menu = new RestaurantMenu();
            bool isGuest = true;
            Guest guest = new Guest();
            guest.Name = AnsiConsole.Ask<string>("What's your name?");

            while (isGuest == true)
            {
                // Perform the action the guest picked
                var choice = MenuHelper.GuestMenu();
                switch (choice)
                {
                    //Make a order
                    case GuestMenuChoice.CreateOrder:
                        guest.CreateOrder(menu.MenuItems);
                        break;
                    // Show all menu items in a table
                    case GuestMenuChoice.ShowMenu:
                        RestaurantMenu.ShowMenu(menuDataStore);
                        break;

                    case GuestMenuChoice.ShowOrderStatus:
                        guest.ShowOrderStatus();
                        break;

                    case GuestMenuChoice.Back:
                        // End the loop and return to the previous menu
                        menuDataStore.SaveToJson();
                        isGuest = false;
                        break;
                }
            }
        }

        public static void LogInAndRegisterMenuLogic(DataStore<Staff> stafflist)
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

        public static void StartMenuLogic(DataStore<Staff> stafflist)
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

        public static void ShowOrderMenuLogic()
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

