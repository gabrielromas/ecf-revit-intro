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
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapplication = commandData.Application;
            UIDocument uidocument = uiapplication.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application application = uiapplication.Application;
            Document document = uidocument.Document;

            var wall = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Walls)
                .FirstOrDefault(w => w.Name == "INT_G_GK3 2xRB+ISO+CW/UW 75/600+2xRB_125 EI60");

            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == "Parter");

            var ceiling = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Ceilings)
                .FirstOrDefault(f => f.Name == "Plafon casetat 600x600");

            WallType wallType = document.GetElement(wall.Id) as WallType;
            double width = wallType.Width/2;

            var lengthOrizontal = UnitUtils.Convert(400, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var lengthVertical = UnitUtils.Convert(600, UnitTypeId.Centimeters, UnitTypeId.Feet);
            var hCeiling = UnitUtils.Convert(3930, UnitTypeId.Millimeters, UnitTypeId.Feet);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create Ceiling");
                ElementId ceilingTypeId = ceiling.GetTypeId();

                XYZ buttomLeft = new XYZ(width, width, 0);
                XYZ buttomRight = new XYZ(lengthOrizontal - width, width, 0);
                XYZ topLeft = new XYZ(width, lengthVertical - width, 0);
                XYZ topRight = new XYZ(lengthOrizontal - width, lengthVertical - width, 0);

                CurveLoop profile = new CurveLoop();

                profile.Append(Line.CreateBound(buttomLeft, topLeft));
                profile.Append(Line.CreateBound(topLeft, topRight));
                profile.Append(Line.CreateBound(topRight, buttomRight));
                profile.Append(Line.CreateBound(buttomRight, buttomLeft));

                var ceilingCreate = Ceiling.Create(document, new List<CurveLoop> { profile }, ceilingTypeId, level.Id);

                Parameter param = ceilingCreate.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM);
                param.Set(hCeiling);

                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
