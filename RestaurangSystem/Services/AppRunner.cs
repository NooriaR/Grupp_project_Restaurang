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
            // Skapar ett DataStore-objekt som används för att hantera restaurangens meny.
            // MenuItem anger att objektet ska lagra information om maträtterna på menyn.
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>();
            // Hämtar tidigare sparad menyinformation från en JSON-fil.
            // På så sätt finns menyinformationen kvar även när programmet startas om.
            menuDataStore.LoadFromJson();
            RestaurantMenu menu = new RestaurantMenu();
            // Menyn fortsätter att visas så länge den inloggade personalmedlemmen
            // fortfarande är inloggad i systemet.
            while (AuthService.currentStaff.IsLoggedIn == true)
            {
                // Visar personalmenyn och hämtar det alternativ som personalen väljer.
                var choice = MenuHelper.StaffMenu();

                // Kontrollerar om personalen har valt att logga ut.
                if (choice == StaffMenuChoice.Logout)
                {
                    // Sparar eventuella ändringar i menyn till JSON-filen
                    // innan personalen loggas ut.
                    menuDataStore.SaveToJson();
                    // Anropar LogOut för att avsluta den aktuella personalsessionen.
                    AuthService.LogOut();
                    // Hoppar tillbaka till början av while-loopen.
                    // Eftersom personalen nu är utloggad avslutas loopen. 
                    continue;
                }
                if (choice == StaffMenuChoice.ManageOrders)
                {
                    ShowOrderMenuLogic(); // har en egen meny och loop, så ingen paus behövs
                    continue;
                }
                // RunScreen används för att köra den valda funktionen på en skärm.
                // Den hanterar skärmens visning och eventuell paus innan man återgår
                // till personalmenyn.
                MenuHelper.RunScreen(() =>
                {
                    // Switch kontrollerar vilket alternativ personalen har valt
                    // och kör den funktion som hör till det alternativet.
                    switch (choice)
                    {
                        case StaffMenuChoice.AddToMenu:
                            // Anropar metoden AddMenuItem för att lägga till en ny maträtt
                            // i restaurangens meny.
                            menu.AddMenuItem(menuDataStore);
                            Logger.Info($"MenuItem added by: {AuthService.currentStaff.Email}");
                            break;

                        case StaffMenuChoice.RemovefromMenu:
                            // Anropar metoden RemoveMenuItem för att ta bort
                            // en befintlig maträtt från restaurangens meny. 
                            menu.RemoveMenuItem(menuDataStore);
                            Logger.Info($"MenuItem removed by: {AuthService.currentStaff.Email}");
                            break;
                        case StaffMenuChoice.UpdateitemonMenu:
                            // Anropar UppdateMenuItem för att ändra information
                            // om en maträtt som redan finns på menyn.
                            menu.UppdateMenuItem(menuDataStore);
                            Logger.Info($"MenuItem updated by: {AuthService.currentStaff.Email}");
                            break;
                        case StaffMenuChoice.ShowLog:
                            Logger.ShowLog();
                            break;
                        case StaffMenuChoice.ShowSchedule:
                            // Visar arbetsschemat för den personalmedlem
                            // som för tillfället är inloggad.
                            AuthService.currentStaff.WorkingSchedule();
                            break;
                    }
                });
            }
        }
        public static void GuestMenuLogic()
        {
            // Skapar en instans av DataStore som används för att hantera
            // restaurangens menyartiklar och kopplingen till JSON-filen.  
            DataStore<MenuItem> menuDataStore = new DataStore<MenuItem>();
            // Läser in tidigare sparade menyartiklar från JSON-filen.
            menuDataStore.LoadFromJson();
            // Skapar en instans av RestaurantMenu som innehåller
            // funktioner för att hantera och visa restaurangens meny.
            RestaurantMenu menu = new RestaurantMenu();
            // Kopierar menyartiklarna från DataStore till RestaurantMenu.
            // På så sätt kan gästen komma åt alla tillgängliga maträtter.
            menu.MenuItems = menuDataStore.items;
            // Skapar en bool-variabel som styr while-loopen.
            // Så länge isBrowsing är true fortsätter gästen att använda menyn.
            bool isBrowsing = true;
            // Skapar ett Guest-objekt som representerar den aktuella gästen.
            Guest guest = new Guest();
            // frågar användaren efter deras namn och sparar det i guest-objektet
            guest.Name = AnsiConsole.Ask<string>("What's your name?");

            // Fortsätter visa gästmenyn så länge gästen inte väljer att gå tillbaka.
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

                MenuHelper.RunScreen(() => //här körs alla metoder i switch casen med hjälp av RunScreen
                {
                    //Går genom användarens val
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
                        // Visar information om gästens tidigare beställningar
                        // och deras aktuella status.
                        case GuestMenuChoice.ShowOrderStatus:
                            guest.ShowOrderStatus();
                            break;
                        // Anropar metoden för att skapa en bordsreservation.
                        case GuestMenuChoice.MakeReservation:
                            guest.MakeReservation();
                            Logger.Info($"Reservation made by: {guest.Name}"); // skickar logg
                            break;
                        // Visar gästens befintliga reservationer.
                        case GuestMenuChoice.ShowReservation:
                            guest.ShowReservation();
                            break;
                        // Anropar metoden för att avboka en befintlig reservation.
                        case GuestMenuChoice.CancelReservation:
                            guest.CancelReservation();
                            Logger.Info($"Reservation canceled by: {guest.Name}"); // skickar logg
                            break;
                    }
                });
            }
        }
        public static void LogInAndRegisterMenuLogic(DataStore<Staff> stafflist)
        {
            // Den här metoden hanterar personalens inloggning och registrering.
            // stafflist skickas in som parameter för att metoden ska kunna
            // komma åt information om registrerad personal.

            // Skapar en bool-variabel som styr while-loopen.
            // Så länge isRunning är true fortsätter personalportalen att visas.
            {
                bool isRunning = true;
                while (isRunning)
                {
                    // Visar personalportalen där användaren kan välja
                    // att logga in, registrera sig eller gå tillbaka.
                    // Valet sparas i variabeln choice. 
                    var choice = MenuHelper.StaffPortal();
                    // Switch kontrollerar vilket alternativ användaren har valt
                    // och kör den kod som hör till alternativet.
                    switch (choice)
                    {
                        case "Log In":
                            // Anropar LogInMenu och skickar med stafflist.
                            // Metoden kontrollerar användarens inloggningsuppgifter
                            // och returnerar true om inloggningen lyckas,
                            // annars returneras false.
                            bool loginsuccesfull = MenuHelper.LogInMenu(stafflist);
                            // Kontrollerar om inloggningen lyckades. 
                            if (loginsuccesfull == true)
                            {
                                // Om inloggningen lyckas öppnas personalmenyn.
                                // Där kan personalen hantera restaurangens funktioner.
                                StaffMenuLogic();
                                break;
                            }
                            // Om inloggningen misslyckas stannar användaren
                            // kvar i personalportalen och kan försöka igen.
                            break;
                        case "Register":
                            // Skapar ett nytt Staff-objekt som representerar
                            // den personalmedlem som vill registrera sig.
                            Staff staff = new Staff();
                            // Anropar Register-metoden som hanterar registreringen
                            // och samlar in den nya personalmedlemmens information.  
                            staff.Register();
                            Logger.Info($"Staff registered: {staff.Email}"); // skickar logg
                            break;
                        case "Back":
                            // Ändrar isRunning till false.
                            // Det gör att while-loopen avslutas och användaren
                            // återgår till programmets startmeny.
                            isRunning = false;
                            break;
                    }
                }
            }
        }
        public static void StartMenuLogic(DataStore<Staff> stafflist)
        {
            // Skapar en bool-variabel som används för att styra while-loopen.
            // Variabeln sätts till true eftersom startmenyn ska visas
            // så länge användaren inte väljer att avsluta programmet.
            bool isRunning = true;
            // While-loopen gör att startmenyn visas om och om igen.
            // Den fortsätter så länge isRunning har värdet true.
            while (isRunning)
            {
                // Visa huvudmenyn och få användarens val.
                var choice = MenuHelper.StartMenu();
                // Switch används för att kontrollera vilket alternativ
                // användaren har valt och bestämma vad programmet ska göra.
                switch (choice)
                {
                    case StartMenuChoice.GuestView:
                        // Om användaren väljer GuestView öppnas gästmenyn.
                        // Där kan gästen exempelvis visa restaurangens meny,
                        // skapa beställningar och göra reservationer
                        GuestMenuLogic();
                        // Avslutar det aktuella switch-fallet.
                        // När gästmenyn stängs fortsätter programmet
                        // till nästa varv i while-loopen.
                        break;
                    case StartMenuChoice.StaffView:
                        // Om användaren väljer StaffView öppnas personalportalen.
                        // Där kan personalen logga in eller registrera sig.
                        // stafflist skickas med som parameter så att
                        // inloggningsmetoden kan komma åt personalinformationen.
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
            // Skapar en instans av DataStore som används för att hantera beställningar.
            // <Order> anger att DataStore ska hantera objekt av typen Order.
            DataStore<Order> orderDataStore = new DataStore<Order>();
            // Läser in tidigare sparade beställningar från JSON-filen.
            // Detta gör att programmet kan komma åt beställningar
            // som har skapats och sparats tidigare.
            orderDataStore.LoadFromJson();
            // Skapar en bool-variabel som styr while-loopen.
            // running sätts till true eftersom beställningsmenyn ska visas
            // tills personalen väljer att gå tillbaka. 
            bool running = true;
            // While-loopen gör att beställningsmenyn fortsätter visas
            // så länge running har värdet true. 
            while (running)
            {
                // Anropar ShowOrderMenu som visar de olika alternativen
                // för beställningshantering.
                // Personalens val sparas i variabeln choice.
                var choice = MenuHelper.ShowOrderMenu();
                // Switch kontrollerar vilket alternativ personalen har valt
                // och kör den kod som hör till det valda alternativet.
                switch (choice)
                {
                    case "Show orders":
                        // Anropar RunScreen för att visa beställningarna på en separat skärm.
                        // AuthService.currentStaff hämtar den personalmedlem
                        // som för tillfället är inloggad.
                        // ShowOrders-metoden visar beställningarna för personalen.
                        MenuHelper.RunScreen(() => AuthService.currentStaff.ShowOrders());
                        break;
                    case "Update Order Status":
                        // Frågar personalen efter ID-numret på beställningen
                        // som ska uppdateras.
                        // AnsiConsole.Ask<int> innebär att användaren ska ange ett heltal.
                        int orderId = AnsiConsole.Ask<int>("Enter the order ID to update:");
                        // Söker efter en beställning i DataStore med hjälp av dess ID.
                        // Lambda-uttrycket o => o.Id == orderId jämför varje beställnings ID
                        // med det ID-nummer som personalen har angett.
                        // Den hittade beställningen sparas i variabeln order.
                        var order = orderDataStore.FindItem(o => o.Id == orderId);
                        // Kontrollerar om beställningen hittades.
                        // Om order inte är null betyder det att beställningen finns.
                        // Detta förhindrar att programmet försöker ändra en beställning
                        // som inte hittades i systemet.
                        if (order != null)
                        {
                            // Visar en lista med alla tillgängliga statusalternativ.
                            // SelectionPrompt gör det möjligt för personalen
                            // att välja en ny status från listan.
                            var newStatus = AnsiConsole.Prompt(
                                new SelectionPrompt<OrderStatus>()
                                    .Title("Select the new status:")
                                    // Hämtar alla värden som finns definierade i enum OrderStatus.
                                    // Dessa värden visas som valbara alternativ för personalen.
                                    .AddChoices(Enum.GetValues<OrderStatus>()));
                            // Anropar UpdateStatus för att ändra beställningens status.
                            // Den nya statusen skickas med som parameter till metoden.
                            order.UpdateStatus(newStatus);
                            // Sparar den uppdaterade beställningen till JSON-filen.
                            // På så sätt finns ändringen kvar även efter att programmet stängs.
                            orderDataStore.SaveToJson();
                        }
                        break;
                    case "Back":
                        // Ändrar running till false.
                        // Eftersom while-loopen bara fortsätter när running är true
                        // kommer loopen att avslutas och personalen återgår till personalmenyn.
                        running = false;
                        break;
                }
            }
        }
    }
}