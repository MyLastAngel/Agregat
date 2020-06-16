using Microsoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation.Peers;

namespace ProjectControlSystem
{
    public class CustomDataGrid : DataGrid
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return null;
        }
    }
}
