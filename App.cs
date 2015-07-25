#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Drawing;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace PrintIndex
{
    class App : IExternalApplication
    {
        
        public Result OnStartup(UIControlledApplication a)
        {
            
            // Add a new ribbon panel
            RibbonPanel ribbonPanel = a.CreateRibbonPanel("Sheet Index Tools");

            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string AddInPath = typeof(App).Assembly.Location;
            string AddFolder = Path.GetDirectoryName(AddInPath);
            string helpPath = AddFolder + "\\PrintIndexHelp.html";
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.Url,helpPath);

            PushButtonData button1Data = new PushButtonData("cmdPfI",
                "Print Index", thisAssemblyPath, "PrintIndex.Command");

            PushButton pushButton1 = ribbonPanel.AddItem(button1Data) as PushButton;
            pushButton1.ToolTip = "Click to select your sheet index and save as a printable set.";
            pushButton1.LargeImage = new BitmapImage(new Uri(Path.Combine(AddFolder, "PanelIconButton.bmp"),UriKind.Absolute));
            pushButton1.SetContextualHelp(contextHelp);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}

