using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class CreateInterior : IExternalCommand
    {
        private string INTERIOR_WALL_FAMILY = "INT_F_PLA_LAV gri-CTI10";
        private string WALL_FAMILY = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";
        private string LEVEL_NAME = "Parter";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Document document = uidocument.Document;

            var interior = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_StackedWalls)
               .FirstOrDefault(w => w.Name == INTERIOR_WALL_FAMILY);

            var wall = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_Walls)
               .FirstOrDefault(w => w.Name == WALL_FAMILY);

            var level = new FilteredElementCollector(document)
               .OfClass(typeof(Level))
               .Cast<Level>()
               .FirstOrDefault(l => l.Name == LEVEL_NAME);

            double widthWall = Width(document, wall);
            double widthInterior = Width(document, interior);
            double width = widthInterior + widthWall;
            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Interior Walls");

                XYZ start1 = new XYZ(width, width, 0);
                XYZ end1 = new XYZ(lengthX - width, width, 0);
                var bottom = Create(document, interior.Id, start1, end1, level);

                XYZ start2 = new XYZ(width, width, 0);
                XYZ end2 = new XYZ(width, lengthY - width, 0);
                var left = Create(document, interior.Id, start2, end2, level);

                XYZ start3 = new XYZ(width, lengthY - width, 0);
                XYZ end3 = new XYZ(lengthX - width, lengthY - width, 0);
                var top = Create(document, interior.Id, start3, end3, level);

                XYZ start4 = new XYZ(lengthX - width, lengthY - width, 0);
                XYZ end4 = new XYZ(lengthX - width, width, 0);
                var right = Create(document, interior.Id, start4, end4, level);

                App.walls[WallSide.Bottom].InteriorID = bottom.UniqueId;
                App.walls[WallSide.Left].InteriorID = left.UniqueId;
                App.walls[WallSide.Top].InteriorID = top.UniqueId;
                App.walls[WallSide.Right].InteriorID = right.UniqueId;

                App.elementIDS.Add(left.Id);
                App.elementIDS.Add(right.Id);
                App.elementIDS.Add(top.Id);
                App.elementIDS.Add(bottom.Id);

                tx.Commit();
            }

            return Result.Succeeded;
        }

        public Wall Create(Document document, ElementId wallId, XYZ start, XYZ end, Element level)
        {
            if (wallId != null)
            {
                var line = Line.CreateBound(start, end);
                var height = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
                var wall = Wall.Create(document, line, wallId, level.Id, height, 0, false, false);

                return wall;
            }

            return default;
        }

        public double Width(Document document, Element wall)
        {
            if (wall != null)
            {
                var wallID = document.GetElement(wall.Id) as WallType;
                double width = wallID.Width;

                return width / 2;
            }

            return default;
        }
    }
}
