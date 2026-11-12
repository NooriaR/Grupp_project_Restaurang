using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.UI
{
    public enum StartMenuChoice // den här enumen används för att förenkla hur valet från StartMenu hämtas, och varje läge kan ges ett heltal för enklare åtkomst
    {
        GuestView,
        StaffView,
        Exit
    }

    public enum StaffMenuChoice // den här enumen används för att förenkla hur valet från StaffMenu hämtas, och varje läge kan ges ett heltal för enklare åtkomst
    {
        AddToMenu,
        RemovefromMenu,
        UpdateitemonMenu,
        ManageOrders,
        ShowSchedule,
        Logout

    }
    public enum GuestMenuChoice // den här enumen används för att förenkla hur valet från GuestMenu hämtas, och varje läge kan ges ett heltal för enklare åtkomst
    {
        CreateOrder,
        ShowMenu,
        ShowOrderStatus,
        Back
    }
}
