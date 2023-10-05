using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DeleteElementsB : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            for (int i = App.elementIDS.Count - 1; i >= 0; i--)
            {
                var elementId = App.elementIDS[i];

                var element = document.GetElement(elementId);

                if (element is null || !element.IsValidObject)
                {
                    _ = App.elementIDS.Remove(elementId);
                }
            }

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
