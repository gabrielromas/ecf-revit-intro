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
        private string DOOR_FAMILY_NAME = "EXT Usa metalica un canat";
        private string DOOR_NAME = "UP 1 900 x 2400mm";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            var left = document.GetElement(App.walls[WallSide.Left].WallID);
            var right = document.GetElement(App.walls[WallSide.Right].WallID);

            var door = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(d => d.FamilyName == (DOOR_FAMILY_NAME) && d.Name == DOOR_NAME);

            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document, "Create Doors"))
            {
                tx.Start();

                if (door != null)
                {
                    XYZ location = new XYZ(lengthX / 2, lengthY / 2, 0);

                    _ = document.Create.NewFamilyInstance(
                        location,
                        door,
                        left,
                        StructuralType.NonStructural);

                    _ = document.Create.NewFamilyInstance(
                        location,
                        door,
                        right,
                        StructuralType.NonStructural);
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
