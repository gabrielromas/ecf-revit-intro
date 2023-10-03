using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]   
    public class CreateDoors : IExternalCommand
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

            var left = document.GetElement(Application.wallsDictionary["leftWall"].WallID);
            var right = document.GetElement(Application.wallsDictionary["rightWall"].WallID);

            var doorType = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(d => d.FamilyName == ("EXT Usa metalica un canat") && d.Name == "UP 1 900 x 2400mm");

            var lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document, "Adaugare Usa"))
            {
                transaction.Start();

                if (doorType != null)
                {
                    XYZ location = new XYZ(lengthOrizontal / 2, lengthVertical / 2, 0);

                    FamilyInstance door1Instance = document.Create.NewFamilyInstance(
                        location,
                        doorType,
                        left,
                        StructuralType.NonStructural);

                    FamilyInstance door2Instance = document.Create.NewFamilyInstance(
                        location,
                        doorType,
                        right,
                        StructuralType.NonStructural);
                }
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
