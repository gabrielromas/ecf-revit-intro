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
        private string LEVEL_FAMILY = "Parter";
        private string FLOOR_FAMILY = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var wall = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .FirstOrDefault(w => w.Name == WALL_FAMILY);

            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == LEVEL_FAMILY);

            var floor = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Floors)
                .FirstOrDefault(f => f.Name == FLOOR_FAMILY);

            var wallId = document.GetElement(wall.Id) as WallType;
            double width = wallId.Width / 2;

            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Floor");

                ElementId floorTypeId = Floor.GetDefaultFloorType(document, false);

                XYZ buttomLeft = new XYZ(width, width, 0);
                XYZ buttomRight = new XYZ(lengthX - width, width, 0);
                XYZ topLeft = new XYZ(width, lengthY - width, 0);
                XYZ topRight = new XYZ(lengthX - width, lengthY - width, 0);

                CurveLoop profile = new CurveLoop();

                profile.Append(Line.CreateBound(buttomLeft, topLeft));
                profile.Append(Line.CreateBound(topLeft, topRight));
                profile.Append(Line.CreateBound(topRight, buttomRight));
                profile.Append(Line.CreateBound(buttomRight, buttomLeft));

                _ = Floor.Create(document, new List<CurveLoop> { profile }, floorTypeId, level.Id);

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
