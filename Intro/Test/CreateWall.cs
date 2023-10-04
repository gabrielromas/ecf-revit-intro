using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateWall : IExternalCommand
    {
        private string LEVEL_FAMILY = "Parter";
        private string WALL_FAMILY = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault(l => l.Name == LEVEL_FAMILY);

            var wall = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .FirstOrDefault(w => w.Name == WALL_FAMILY);
                
            double length = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Wall");

                XYZ start = new XYZ(0, 0, 0);
                XYZ end = new XYZ(length, 0, 0);
                _ = Create(start, end, document, wall.Id, level);

                tx.Commit();
            }

            return Result.Succeeded;
        }

        public Wall Create(XYZ start, XYZ end, Document document, ElementId wallTypeId, Element level)
        {
            var height = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            Line line = Line.CreateBound(start, end);
            var wall = Wall.Create(document, line, wallTypeId, level.Id, height, 0, false, false);

            return wall;
        }
    }
}
