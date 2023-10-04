using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateWindows : IExternalCommand
    {
        private string WINDOW_FAMILY_NAME = "EXT Fereastra un canat";
        private string WINDOW_NAME = "1000X800";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            var topId = App.walls[WallSide.Top];
            var topElement = document.GetElement(topId.WallID);

            var bottomId = App.walls[WallSide.Bottom];
            var bottomElement = document.GetElement(bottomId.WallID);

            var windowSymbol = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(d => d.FamilyName == WINDOW_FAMILY_NAME && d.Name == WINDOW_NAME);

            if (windowSymbol != null)
            {
                double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
                double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);
                double heigh = UnitUtils.Convert(100, UnitTypeId.Centimeters, UnitTypeId.Feet);

                using (Transaction tx = new Transaction(document, "Create Windows"))
                {
                    tx.Start();

                    XYZ location = new XYZ(lengthX / 2, lengthY / 2, heigh);

                    var window1 = document.Create.NewFamilyInstance(
                        location,
                        windowSymbol,
                        topElement,
                        StructuralType.NonStructural);

                    var window2 = document.Create.NewFamilyInstance(
                        location,
                        windowSymbol,
                        bottomElement,
                        StructuralType.NonStructural);

                    App.elementIDS.Add(window1.Id);
                    App.elementIDS.Add(window2.Id);

                    tx.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
