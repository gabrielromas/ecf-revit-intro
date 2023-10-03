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
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var exteriorType = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_Walls)
               .FirstOrDefault(w => w.Name == "EXT_INSexp-5cm");

            var levelType = new FilteredElementCollector(document)
               .OfClass(typeof(Level))
               .FirstOrDefault(l => l.Name == "Parter");

            var wallType = new FilteredElementCollector(document)
               .OfCategory(BuiltInCategory.OST_Walls)
               .FirstOrDefault(w => w.Name == "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60");

            var widthWall = Width(document, wallType);
            var widthExterior = Width(document, exteriorType);
            var width = widthExterior + widthWall;

            //conversion feet to centimeters
            var lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create ExteriorWalls");

                //create exterior walls
                XYZ start1 = new XYZ(-width, -width, 0);
                XYZ end1 = new XYZ(lengthOrizontal + width, -width, 0);
                var bottom = CreateW(start1, end1, document, exteriorType.Id, levelType, commandData);
                
                XYZ start2 = new XYZ(-width, -width, 0);
                XYZ end2 = new XYZ(-width, lengthVertical + width, 0);
                var left = CreateW(start2, end2, document, exteriorType.Id, levelType, commandData);
                
                XYZ start3 = new XYZ(-width, lengthVertical + width, 0);
                XYZ end3 = new XYZ(lengthOrizontal + width, lengthVertical + width, 0);
                var top = CreateW(start3, end3, document, exteriorType.Id, levelType, commandData);
                
                XYZ start4 = new XYZ(lengthOrizontal + width, lengthVertical + width, 0);
                XYZ end4 = new XYZ(lengthOrizontal + width, -width, 0);
                var right = CreateW(start4, end4, document, exteriorType.Id, levelType, commandData);

                //add exterior wall id to dictionary
                Application.wallsDictionary["bottomWall"].WallExteriorID = bottom.UniqueId;
                Application.wallsDictionary["leftWall"].WallExteriorID = left.UniqueId;
                Application.wallsDictionary["topWall"].WallExteriorID = top.UniqueId;
                Application.wallsDictionary["rightWall"].WallExteriorID = right.UniqueId;

                transaction.Commit();
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

            return width/2;
        }
    }
}


/*public double GetStackedWallWidth(Document document, WallType wallType)
{
    double result = 0;

    Level firstLevel = new FilteredElementCollector(document)
        .OfType<Level>()
        .FirstOrDefault();

    using (Transaction tx = new Transaction(document, "Get stacked wall width"))
    {
        tx.Start();

        XYZ start1 = new XYZ(0, 0, 0);
        XYZ end1 = new XYZ(0, 0, 0);
        Line line1 = Line.CreateBound(start1, end1);
        var createdWall = Wall.Create(document, line1, wallType.Id, firstLevel.Id, UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet), 0, false, false);

        document.Regenerate();

        double width = createdWall.Width;

        var stackWallWidth = createdWall.GetStackedWallMemberIds();

        //return width 

        tx.RollBack();
    }


    return result;
}*/
