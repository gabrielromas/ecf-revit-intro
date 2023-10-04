using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class CreateWalls : IExternalCommand
    {
        public string WALL_FAMILY = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";
        public string LEVEL_FAMILY = "Parter";

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
               .FirstOrDefault(l => l.Name == LEVEL_FAMILY);

            //conversion feet to centimeters
            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Walls");

                XYZ start1 = new XYZ(0, 0, 0);
                XYZ end1 = new XYZ(lengthX, 0, 0);
                var bottom = Create(start1, end1, document, wall.Id, level);

                XYZ start2 = new XYZ(0, 0, 0);
                XYZ end2 = new XYZ(0, lengthY, 0);
                var left = Create(start2, end2, document, wall.Id, level);

                XYZ start3 = new XYZ(0, lengthY, 0);
                XYZ end3 = new XYZ(lengthX, lengthY, 0);
                var top = Create(start3, end3, document, wall.Id, level);

                XYZ start4 = new XYZ(lengthX, lengthY, 0);
                XYZ end4 = new XYZ(lengthX, 0, 0);
                var right = Create(start4, end4, document, wall.Id, level);

                App.walls[WallSide.Bottom] = new JoinPair
                {
                    WallID = bottom.UniqueId
                };
                App.walls[WallSide.Left] = new JoinPair
                {
                    WallID = left.UniqueId
                }; 
                App.walls[WallSide.Top] = new JoinPair
                {
                    WallID = top.UniqueId
                };
                App.walls[WallSide.Right] = new JoinPair
                {
                    WallID = right.UniqueId
                };

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

