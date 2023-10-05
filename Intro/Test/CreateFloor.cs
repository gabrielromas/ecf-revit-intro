using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateFloor : IExternalCommand
    {
        private string WALL_FAMILY = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";
        private string LEVEL_NAME = "Parter";
        private string FLOOR_FAMILY = "INT_P_Parchet 1 cm + sapa 6 cm";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            var wall = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .FirstOrDefault(w => w.Name == WALL_FAMILY);

            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault(l => l.Name == LEVEL_NAME);

            var floorType = new FilteredElementCollector(document)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .FirstOrDefault(f => f.Name == FLOOR_FAMILY);

            var wallId = document.GetElement(wall.Id) as WallType;
            double width = wallId.Width / 2;
            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Floor");

                //ElementId floorId = Floor.GetDefaultFloorType(document, false);
                XYZ bottomLeft = new XYZ(width, width, 0);
                XYZ bottomRight = new XYZ(lengthX - width, width, 0);
                XYZ topLeft = new XYZ(width, lengthY - width, 0);
                XYZ topRight = new XYZ(lengthX - width, lengthY - width, 0);

                CurveLoop profile = new CurveLoop();
                profile.Append(Line.CreateBound(bottomLeft, topLeft));
                profile.Append(Line.CreateBound(topLeft, topRight));
                profile.Append(Line.CreateBound(topRight, bottomRight));
                profile.Append(Line.CreateBound(bottomRight, bottomLeft));

                var floor = Floor.Create(document, new List<CurveLoop> { profile }, floorType.Id, level.Id);

                App.elementIDS.Add(floor.Id);

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
