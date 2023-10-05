using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DeleteElementsB
    {
        public Result Execute(ExternalCommandData commandData)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            using (Transaction tx = new Transaction(document, "Delete All Elements"))
            {
                tx.Start();
                try 
                {
                    document.Delete(App.elementIDS);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Element is not found: {ex.Message}");
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
