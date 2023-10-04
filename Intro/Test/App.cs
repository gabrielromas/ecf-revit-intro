using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Reflection;

namespace Test
{
    public class App : IExternalApplication
    {
        public static Dictionary<WallSide, JoinPair> walls = new Dictionary<WallSide, JoinPair>();

        public Result OnStartup(UIControlledApplication app)
        {
            RibbonTab(app);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public void RibbonTab(UIControlledApplication app)
        {
            string tab = "Intro";
            app.CreateRibbonTab(tab);

            Autodesk.Revit.UI.RibbonPanel ribbonPanel = app.CreateRibbonPanel(tab, "Intro Panel");

            var create1 = CreatePushButton("createWalls", "   1   ", typeof(CreateWall).FullName);
            var create4 = CreatePushButton("createFourWalls", "   4   ", typeof(CreateWalls).FullName);

            PulldownButtonData groupData = new PulldownButtonData("pulldownBtnData", "Create\nWalls");
            PulldownButton group = ribbonPanel.AddItem(groupData) as PulldownButton;

            CreatePushButtonForPullDownButton(group, create1);
            CreatePushButtonForPullDownButton(group, create4);

            var floor = CreatePushButton("createFloor", "Create\nFloor", typeof(CreateFloor).FullName);
            _ = ribbonPanel.AddItem(floor);

            var ceiling = CreatePushButton("createCeilings", "Create\nCeiling", typeof(CreateCeilings).FullName);
            _ = ribbonPanel.AddItem(ceiling);

            var exterior = CreatePushButton("createExterior", "Create\nExterior", typeof(CreateExterior).FullName);
            _ = ribbonPanel.AddItem(exterior);

            var interior = CreatePushButton("createInterior", "Create\nInterior", typeof(CreateInterior).FullName);
            _ = ribbonPanel.AddItem(interior);

            var doors = CreatePushButton("createDoors", "Create\nDoors", typeof(CreateDoors).FullName);
            _ = ribbonPanel.AddItem(doors);

            var windows = CreatePushButton("createWindows", "Create\nWindows", typeof(CreateWindows).FullName);
            _ = ribbonPanel.AddItem(windows);

            var joinWalls = CreatePushButton("joinWalls", "Join\nWalls", typeof(JoinWalls).FullName);
            _ = ribbonPanel.AddItem(joinWalls);

        }

        public PushButton CreatePushButtonForPullDownButton(PulldownButton group, PushButtonData buttonName)
        {
            PushButton pushButton = group.AddPushButton(buttonName);

            return pushButton;
        }

        public PushButtonData CreatePushButton(string nameButton, string textButton, string className)
        {
            PushButtonData pushButton = new PushButtonData(nameButton, textButton, Assembly.GetExecutingAssembly().Location, className);

            return pushButton;
        }
    }
}
