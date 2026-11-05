using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.UI
{
    public enum StartMenuChoice // this enum is used to simplify collecting choice from StartMenu, could set each state as a int for ease of access
    {
        Order,
        LogIn,
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
}
