using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateCeilings : IExternalCommand
    {
        private string FAMILY_WALL = "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60";
        private string FAMILY_CEILING = "Plafon casetat 600x600";
        private string LEVEL_FAMILY = "Parter";

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
                .FirstOrDefault(w => w.Name == FAMILY_WALL);

            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == LEVEL_FAMILY);

            var ceiling = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Ceilings)
                .FirstOrDefault(f => f.Name == FAMILY_CEILING);

            var wallType = document.GetElement(wall.Id) as WallType;
            double width = wallType.Width / 2;
            double lengthX = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double lengthY = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);
            double height = UnitUtils.Convert(3930, UnitTypeId.Millimeters, UnitTypeId.Feet);

            using (Transaction tx = new Transaction(document))
            {
                tx.Start("Create Ceiling");

                ElementId ceilingTypeId = ceiling.GetTypeId();
                XYZ bottomLeft = new XYZ(width, width, 0);
                XYZ bottomRight = new XYZ(lengthX - width, width, 0);
                XYZ topLeft = new XYZ(width, lengthY - width, 0);
                XYZ topRight = new XYZ(lengthX - width, lengthY - width, 0);

                CurveLoop profile = new CurveLoop();
                profile.Append(Line.CreateBound(bottomLeft, topLeft));
                profile.Append(Line.CreateBound(topLeft, topRight));
                profile.Append(Line.CreateBound(topRight, bottomRight));
                profile.Append(Line.CreateBound(bottomRight, bottomLeft));

                var create = Ceiling.Create(document, new List<CurveLoop> { profile }, ceilingTypeId, level.Id);

                Parameter param = create.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM);
                param.Set(height);

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
