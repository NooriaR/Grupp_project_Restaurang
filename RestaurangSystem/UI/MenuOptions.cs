using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.UI
{
    public enum StartMenuChoice // this enum is used to simplify collecting choice from StartMenu, could set each state as a int for ease of access
    {
        GuestView,
        StaffView,
        Exit
    }

    public enum StaffMenuChoice // this enum is used to simplify collecting choice from StaffMenu, could set each state as a int for ease of access
    {
        AddToMenu,
        RemovefromMenu,
        UpdateitemonMenu,
        ManageOrders,
        ShowSchedule,
        Logout,
        Exit
    }
    public enum GuestMenuChoice // this enum is used to simplify collecting choice from GuestMenu, could set each state as a int for ease of access
    {
        CreateOrder,
        ShowMenu,
        ShowOrderStatus,
        Back
    }
}
