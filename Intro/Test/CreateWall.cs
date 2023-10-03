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
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var levelType = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == "Parter");

            var wallType = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .FirstOrDefault(w => w.Name == "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60");
                
            var length = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create Wall");

                XYZ start = new XYZ(0, 0, 0);
                XYZ end = new XYZ(length, 0, 0);

                CreateW(start, end, document, wallType.Id, levelType, commandData);

                transaction.Commit();
            }
            return Result.Succeeded;
        }

        public Wall CreateW(XYZ start, XYZ end, Document document, ElementId wallTypeId, Element level, ExternalCommandData commandData)
        {
            Line line = Line.CreateBound(start, end);
            var wall = Wall.Create(document, line, wallTypeId, level.Id, UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet), 0, false, false);

            return wall;
        }

    }
}
