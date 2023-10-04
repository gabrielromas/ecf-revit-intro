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
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var topId = App.walls[WallSide.Top];
            var bottomId = App.walls[WallSide.Bottom];
            var topElement = document.GetElement(topId.WallID);
            var bottomElement = document.GetElement(bottomId.WallID);

            var window = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(d => d.FamilyName == (WINDOW_FAMILY_NAME) && d.Name == WINDOW_NAME);

            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double heigh = UnitUtils.Convert(1000, UnitTypeId.Millimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document, "Create Doors"))
            {
                tx.Start();

                if (window != null)
                {
                    XYZ location = new XYZ(lengthX / 2, lengthY / 2, heigh);

                    FamilyInstance door1Instance = document.Create.NewFamilyInstance(
                        location,
                        window,
                        topElement,
                        StructuralType.NonStructural);

                    FamilyInstance door2Instance = document.Create.NewFamilyInstance(
                        location,
                        window,
                        bottomElement,
                        StructuralType.NonStructural);
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
