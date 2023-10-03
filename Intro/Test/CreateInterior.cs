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
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var interiorType = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_StackedWalls)
               .FirstOrDefault(wInterior => wInterior.Name == "INT_F_PLA_LAV gri-CTI10");

            var wallType = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_Walls)
               .FirstOrDefault(w => w.Name == "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60");

            var levelType = new FilteredElementCollector(document)
               .OfClass(typeof(Level))
               .FirstOrDefault(l => l.Name == "Parter");

            var widthWall = Width(document, wallType);
            var widthInterior = Width(document, interiorType);
            var width = widthInterior + widthWall;

            //conversion to centimeters
            var lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create InteriorWalls");

                XYZ start1 = new XYZ(width, width, 0);
                XYZ end1 = new XYZ(lengthOrizontal - width, width, 0);
                var bottom = CreateW(start1, end1, document, interiorType.Id, levelType);
               
                XYZ start2 = new XYZ(width, width, 0);
                XYZ end2 = new XYZ(width, lengthVertical - width, 0);
                var left = CreateW(start2, end2, document, interiorType.Id, levelType);
               
                XYZ start3 = new XYZ(width, lengthVertical - width, 0);
                XYZ end3 = new XYZ(lengthOrizontal - width, lengthVertical - width, 0);
                var top = CreateW(start3, end3, document, interiorType.Id, levelType);
                
                XYZ start4 = new XYZ(lengthOrizontal - width, lengthVertical - width, 0);
                XYZ end4 = new XYZ(lengthOrizontal - width, width, 0);
                var right = CreateW(start4, end4, document, interiorType.Id, levelType);

                //add interior wall id to dictionary
                Application.wallsDictionary["bottomWall"].WallInteriorID = bottom.UniqueId;
                Application.wallsDictionary["leftWall"].WallInteriorID = left.UniqueId;
                Application.wallsDictionary["topWall"].WallInteriorID = top.UniqueId;
                Application.wallsDictionary["rightWall"].WallInteriorID = right.UniqueId;

                transaction.Commit();
            }
            return Result.Succeeded;
        }

        public Wall CreateW(XYZ start, XYZ end, Document document, ElementId wallTypeId, Element level)
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

            return width/2;
        }
    }
}
