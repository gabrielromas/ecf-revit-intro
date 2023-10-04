using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class CreateExterior : IExternalCommand
    {
        private string EXTERIOR_FAMILY = "EXT_INSexp-5cm";
        private string LEVEL_FAMILY = "Parter";
        private string WALL_FAMILY = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var exterior = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_Walls)
               .FirstOrDefault(w => w.Name == EXTERIOR_FAMILY);

            var level = new FilteredElementCollector(document)
               .OfClass(typeof(Level))
               .Cast<Level>()
               .FirstOrDefault(l => l.Name == LEVEL_FAMILY);

            var wall = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_Walls)
               .FirstOrDefault(w => w.Name == WALL_FAMILY);

            var widthWall = Width(document, wall);
            var widthExterior = Width(document, exterior);
            double width = widthExterior + widthWall;

            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Exterior Walls");

                XYZ start1 = new XYZ(-width, -width, 0);
                XYZ end1 = new XYZ(lengthX + width, -width, 0);
                var bottom = CreateW(start1, end1, document, exterior.Id, level, commandData);

                XYZ start2 = new XYZ(-width, -width, 0);
                XYZ end2 = new XYZ(-width, lengthY + width, 0);
                var left = CreateW(start2, end2, document, exterior.Id, level, commandData);

                XYZ start3 = new XYZ(-width, lengthY + width, 0);
                XYZ end3 = new XYZ(lengthX + width, lengthY + width, 0);
                var top = CreateW(start3, end3, document, exterior.Id, level, commandData);

                XYZ start4 = new XYZ(lengthX + width, lengthY + width, 0);
                XYZ end4 = new XYZ(lengthX + width, -width, 0);
                var right = CreateW(start4, end4, document, exterior.Id, level, commandData);

                App.walls[WallSide.Bottom].ExteriorID = bottom.UniqueId;
                App.walls[WallSide.Left].ExteriorID = left.UniqueId;
                App.walls[WallSide.Top].ExteriorID = top.UniqueId;
                App.walls[WallSide.Right].ExteriorID = right.UniqueId;

                tx.Commit();
            }

            return Result.Succeeded;
        }

        public Wall CreateW(XYZ start, XYZ end, Document document, ElementId wallTypeId, Element level, ExternalCommandData commandData)
        {
            var height = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            Line line1 = Line.CreateBound(start, end);
            var wall = Wall.Create(document, line1, wallTypeId, level.Id, height, 0, false, false);

            return wall;
        }

        public double Width(Document document, Element wall)
        {
            WallType wallType = document.GetElement(wall.Id) as WallType;
            double width = wallType.Width;

            return width / 2;
        }
    }
}
