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

            var wall1Id = Application.wallsDictionary["topWall"];
            var wall2Id = Application.wallsDictionary["bottomWall"];

            var wall1Element = document.GetElement(wall1Id.WallID);
            var wall2Element = document.GetElement(wall2Id.WallID);

            var windowType = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(d => d.FamilyName == ("EXT Fereastra un canat") && d.Name == "1000X800");

            var lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var heigh = UnitUtils.Convert(1000, UnitTypeId.Millimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document, "Adaugare Usa"))
            {
                transaction.Start();

                if (windowType != null)
                {
                    XYZ location = new XYZ(lengthOrizontal / 2, lengthVertical / 2, heigh);

                    FamilyInstance door1Instance = document.Create.NewFamilyInstance(
                        location,
                        windowType,
                        wall1Element,
                        StructuralType.NonStructural);

                    FamilyInstance door2Instance = document.Create.NewFamilyInstance(
                        location,
                        windowType,
                        wall2Element,
                        StructuralType.NonStructural);

                }
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
