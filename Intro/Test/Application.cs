using Autodesk.Revit.UI;
using Autodesk.Windows;
using System.Collections.Generic;
using System.Reflection;

namespace Test
{
    public class Application : IExternalApplication
    {
        public static Dictionary<string, JoinPair> wallsDictionary = new Dictionary<string, JoinPair>();

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonTab(application);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public RibbonTab RibbonTab(UIControlledApplication app)
        {
            string tab = "Intro";
            RibbonTab ribbonTab = null;
            app.CreateRibbonTab(tab);

            Autodesk.Revit.UI.RibbonPanel ribbonPanel = app.CreateRibbonPanel(tab, "Intro Panel");

            var Item1 = CreatePushButton("createWalls", "   1   ", typeof(CreateWall).FullName);
            var Item2 = CreatePushButton("createFourWalls", "   4   ", typeof(CreateWalls).FullName);
            
            PulldownButtonData groupData = new PulldownButtonData("pulldownBtnData", "Create\nWalls");
            PulldownButton group = ribbonPanel.AddItem(groupData) as PulldownButton;

            CreatePushButtonForPullDownButton(ribbonPanel, group, Item1);
            CreatePushButtonForPullDownButton(ribbonPanel, group, Item2);

            
            var Floor = CreatePushButton("createFloor", "Create\nFloors", typeof(CreateFloor).FullName);
            _ = ribbonPanel.AddItem(Floor);

            var Ceilings = CreatePushButton("createCeilings", "Create\nCeilings", typeof(CreateCeilings).FullName);
            _ = ribbonPanel.AddItem(Ceilings);

            var Exterior = CreatePushButton("createExterior", "Create\nExterior", typeof(CreateExterior).FullName);
            _ = ribbonPanel.AddItem(Exterior);

            var Interior = CreatePushButton("createInterior", "Create\nInterior", typeof(CreateInterior).FullName);
            _ = ribbonPanel.AddItem(Interior);

            var Doors = CreatePushButton("createDoors", "Create\nDoors", typeof(CreateDoors).FullName);
            _ = ribbonPanel.AddItem(Doors);

            var Windows = CreatePushButton("createWindows", "Create\nWindows", typeof(CreateWindows).FullName);
            _ = ribbonPanel.AddItem(Windows);

            var JoinWalls = CreatePushButton("joinWalls", "Join\nWalls", typeof(JoinWalls).FullName);
            _ = ribbonPanel.AddItem(JoinWalls);

            return ribbonTab;
        }

        public PushButton CreatePushButtonForPullDownButton(Autodesk.Revit.UI.RibbonPanel panel, PulldownButton group, PushButtonData buttonName)
        {
            PushButton item = group.AddPushButton(buttonName) as PushButton;

            return item;
        }

        public PushButtonData CreatePushButton(string nameButton, string textButton, string className)
        {
            PushButtonData pushButton = new PushButtonData(nameButton, textButton, Assembly.GetExecutingAssembly().Location, className);

            return pushButton;
        }
    }
}
