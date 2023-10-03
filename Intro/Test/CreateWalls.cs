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

            var levelType = new FilteredElementCollector(document)
               .OfClass(typeof(Level))
               .FirstOrDefault(l => l.Name == "Parter");

            //conversion feet to centimeters
            var lOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create Wall1");

                XYZ start1 = new XYZ(0, 0, 0);
                XYZ end1 = new XYZ(lOrizontal, 0, 0);
                var bottom = CreateWall(start1, end1, document, wallType.Id, levelType);

                XYZ start2 = new XYZ(0, 0, 0);
                XYZ end2 = new XYZ(0, lVertical, 0);
                var left = CreateWall(start2, end2, document, wallType.Id, levelType);

                XYZ start3 = new XYZ(0, lVertical, 0);
                XYZ end3 = new XYZ(lOrizontal, lVertical, 0);
                var top = CreateWall(start3, end3, document, wallType.Id, levelType);

                XYZ start4 = new XYZ(lOrizontal, lVertical, 0);
                XYZ end4 = new XYZ(lOrizontal, 0, 0);
                var right = CreateWall(start4, end4, document, wallType.Id, levelType);

                //set wallID
                JoinPair joinLeft = new JoinPair
                {
                    WallID = left.UniqueId
                };
                JoinPair joinRight = new JoinPair
                {
                    WallID = right.UniqueId
                };
                JoinPair joinBottom = new JoinPair
                {
                    WallID = bottom.UniqueId
                };
                JoinPair joinTop = new JoinPair
                {
                    WallID = top.UniqueId
                };

                Application.wallsDictionary["bottomWall"] = joinBottom;
                Application.wallsDictionary["leftWall"] = joinLeft;
                Application.wallsDictionary["topWall"] = joinTop;
                Application.wallsDictionary["rightWall"] = joinRight;

                transaction.Commit();
            }
            return Result.Succeeded;
        }

        public Wall CreateWall(XYZ start, XYZ end, Document document, ElementId wallTypeId, Element level)
        {
            var height = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            Line line = Line.CreateBound(start, end);
            var wall = Wall.Create(document, line, wallTypeId, level.Id, height, 0, false, false);

            return wall;
        }

    }
}

