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
            Document document = uidocument.Document;

            //Convert String to Wall
            var bottom = document.GetElement(App.walls[WallSide.Bottom].WallID) as Wall;
            var bottomExt = document.GetElement(App.walls[WallSide.Bottom].ExteriorID) as Wall;
            var bottomInt = document.GetElement(App.walls[WallSide.Bottom].InteriorID) as Wall;

            var top = document.GetElement(App.walls[WallSide.Top].WallID) as Wall;
            var topExt = document.GetElement(App.walls[WallSide.Top].ExteriorID) as Wall;
            var topInt = document.GetElement(App.walls[WallSide.Top].InteriorID) as Wall;

            var left = document.GetElement(App.walls[WallSide.Left].WallID) as Wall;
            var leftExt = document.GetElement(App.walls[WallSide.Left].ExteriorID) as Wall;
            var leftInt = document.GetElement(App.walls[WallSide.Left].InteriorID) as Wall;

            var right = document.GetElement(App.walls[WallSide.Right].WallID) as Wall;
            var rightExt = document.GetElement(App.walls[WallSide.Right].ExteriorID) as Wall;
            var rightInt = document.GetElement(App.walls[WallSide.Right].InteriorID) as Wall;

            using (Transaction tx = new Transaction(document, "Join Walls"))
            {
                tx.Start();

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, bottom, bottomExt);
                    JoinGeometryUtils.JoinGeometry(document, bottom, bottomInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Bottom wall Join failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Bottom wall Join failed: {ex.Message}");
                }

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, top, topExt);
                    JoinGeometryUtils.JoinGeometry(document, top, topInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Top wall Join failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Top wall Join failed: {ex.Message}");
                }

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, left, leftExt);
                    JoinGeometryUtils.JoinGeometry(document, left, leftInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Left wall Join failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Left wall Join failed: {ex.Message}");
                }

                try
                {
                    JoinGeometryUtils.JoinGeometry(document, right, rightExt);
                    JoinGeometryUtils.JoinGeometry(document, right, rightInt);
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Right wall Join failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Right wall Join failed: {ex.Message}");
                }

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
