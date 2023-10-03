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
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var wallType = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .FirstOrDefault(w => w.Name == "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60");

            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == "Parter");

            var floor = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Floors)
                .FirstOrDefault(f => f.Name == "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60");

            WallType wallId = document.GetElement(wallType.Id) as WallType;
            double width = wallId.Width/2;

            var lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create Floor");

                ElementId floorTypeId = Floor.GetDefaultFloorType(document, false);

                XYZ buttomLeft = new XYZ(width, width, 0);
                XYZ buttomRight = new XYZ(lengthOrizontal-width, width, 0);
                XYZ topLeft = new XYZ(width, lengthVertical-width, 0);
                XYZ topRight = new XYZ(lengthOrizontal-width, lengthVertical-width, 0);

                CurveLoop profile = new CurveLoop();

                profile.Append(Line.CreateBound(buttomLeft, topLeft));
                profile.Append(Line.CreateBound(topLeft, topRight));
                profile.Append(Line.CreateBound(topRight, buttomRight));
                profile.Append(Line.CreateBound(buttomRight, buttomLeft));

                Floor.Create(document, new List<CurveLoop> { profile }, floorTypeId, level.Id);

                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
