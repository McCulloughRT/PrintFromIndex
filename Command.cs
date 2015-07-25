#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace PrintIndex
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static Nullable<bool> dialogResult = false;
        public static IList<string> nameSchedule = new List<string>();

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ElementId eId = new ElementId(-2003100);

            //Collect Schedules from active document
            foreach (ViewSchedule vSched in new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .Where(q => !q.IsTitleblockRevisionSchedule && !q.IsInternalKeynoteSchedule))
            {
                if (vSched.Definition.CategoryId == eId)
                {
                    nameSchedule.Add(vSched.Name);
                }
                
            }

            //Instantiate Form
            MainWindow mWindow = new MainWindow();
            mWindow.ShowDialog();
            //Error handling
            if(dialogResult == false)
            {
                GarbageCollect();
                return Result.Failed;
            }
            if(MainWindow.selectedName == null)
            {
                TaskDialog.Show("Error", "No schedule was selected!");
                GarbageCollect();
                return Result.Failed;
            }

            //get ElementId of selected schedule
            var viewId = doc.ActiveView.Id;
            foreach (ViewSchedule vSched2 in new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .Where(q => !q.IsTitleblockRevisionSchedule && !q.IsInternalKeynoteSchedule))
            {
                if (vSched2.Name == MainWindow.selectedName)
                {
                    viewId = vSched2.Id;
                }
            }

            //Get sheets from selected schedule, catch error for invalid schedules
            ViewSet vs = new ViewSet();
            try
            {
                foreach (ViewSheet vSh in new FilteredElementCollector(doc, viewId))
                {
                    //Ignore dummy sheets
                    if(vSh.CanBePrinted == true)
                    {
                        vs.Insert(vSh);
                    }
                
                }
            }
            catch (System.InvalidCastException)
            {
                TaskDialog.Show("Error","Cannot use '" + MainWindow.selectedName + "'" + Environment.NewLine + "'" + MainWindow.selectedName + "' is not a sheet index.");
                GarbageCollect();
                return Result.Failed;
            }

            // Save ViewSet with sheets from selected schedule using provided name
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("SaveSet");

                PrintManager printManager = doc.PrintManager;
                printManager.PrintRange = PrintRange.Select;
                ViewSheetSetting viewSheetSetting = printManager.ViewSheetSetting;
                viewSheetSetting.CurrentViewSheetSet.Views = vs;

                try
                {
                    viewSheetSetting.SaveAs(MainWindow.setName);
                }
                catch (Autodesk.Revit.Exceptions.InvalidOperationException)
                {
                    TaskDialog.Show("Error", "The name '" + MainWindow.setName + "' is already in use!" + Environment.NewLine + "Pick a different name.");
                    GarbageCollect();
                    return Result.Failed;
                }
                
                tx.Commit();
            }
            TaskDialog.Show("View Set", vs.Size + " sheets added to '" + MainWindow.setName + "'");
            GarbageCollect();
            return Result.Succeeded;
        }

        //Reset public variables for next program run
        static void GarbageCollect()
        {
            dialogResult = false;
            nameSchedule.Clear();
            MainWindow.selectedName = null;
            MainWindow.setName = null;
        }
    }
}
