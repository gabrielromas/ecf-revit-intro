using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace Test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class JoinWalls : IExternalCommand
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

            //Convert String to Wall
            var bottom = document.GetElement(Application.wallsDictionary["bottomWall"].WallID) as Wall;
            var bottomExt = document.GetElement(Application.wallsDictionary["bottomWall"].WallExteriorID) as Wall;
            var bottomInt = document.GetElement(Application.wallsDictionary["bottomWall"].WallInteriorID) as Wall;

            var top = document.GetElement(Application.wallsDictionary["topWall"].WallID) as Wall;
            var topExt = document.GetElement(Application.wallsDictionary["topWall"].WallExteriorID) as Wall;
            var topInt = document.GetElement(Application.wallsDictionary["topWall"].WallInteriorID) as Wall;

            var left = document.GetElement(Application.wallsDictionary["leftWall"].WallID) as Wall;
            var leftExt = document.GetElement(Application.wallsDictionary["leftWall"].WallExteriorID) as Wall;
            var leftInt = document.GetElement(Application.wallsDictionary["leftWall"].WallInteriorID) as Wall;

            var right = document.GetElement(Application.wallsDictionary["rightWall"].WallID) as Wall;
            var rightExt = document.GetElement(Application.wallsDictionary["rightWall"].WallExteriorID) as Wall;
            var rightInt = document.GetElement(Application.wallsDictionary["rightWall"].WallInteriorID) as Wall;

            using (Transaction transaction = new Transaction(document, "Join Walls"))
            {
                transaction.Start();

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, bottom, bottomExt);
                    JoinGeometryUtils.JoinGeometry(document, bottom, bottomInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Processing failed: {ex.Message}");
                }

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, top, topExt);
                    JoinGeometryUtils.JoinGeometry(document, top, topInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Processing failed: {ex.Message}");
                }

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, left, leftExt);
                    JoinGeometryUtils.JoinGeometry(document, left, leftInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Processing failed: {ex.Message}");
                }

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, right, rightExt);
                    JoinGeometryUtils.JoinGeometry(document, right, rightInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Processing failed: {ex.Message}");
                }

                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
