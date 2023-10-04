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
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var wallTopId = App.walls[WallSide.Top];
            var wallBottomId = App.walls[WallSide.Bottom];

            var wallTopElement = document.GetElement(wallTopId.WallID);
            var wallBottomElement = document.GetElement(wallBottomId.WallID);

            var window = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(d => d.FamilyName == ("EXT Fereastra un canat") && d.Name == "1000X800");

            double lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double heigh = UnitUtils.Convert(1000, UnitTypeId.Millimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document, "Adaugare Usa"))
            {
                tx.Start();

                if (window != null)
                {
                    XYZ location = new XYZ(lengthOrizontal / 2, lengthVertical / 2, heigh);

                    FamilyInstance door1Instance = document.Create.NewFamilyInstance(
                        location,
                        window,
                        wallTopElement,
                        StructuralType.NonStructural);

                    FamilyInstance door2Instance = document.Create.NewFamilyInstance(
                        location,
                        window,
                        wallBottomElement,
                        StructuralType.NonStructural);
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
