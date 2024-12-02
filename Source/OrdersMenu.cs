using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Reflection;

namespace ShortcutForOrdersMenu
{

    [DefOf]
    public static class Defs
    {
        public static KeyBindingDef ArchitectOrders;
    }

    [DefOf]
    public static class MainButtonDefs
    {
        public static MainButtonDef Architect;
    }

    [HarmonyPatch(typeof(MainButtonsRoot))]
    public static class MainButtonsRoot_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainButtonsOnGUI))]
        public static void MainButtonsOnGUI()
        {
            if(Defs.ArchitectOrders.KeyDownEvent)
            {
                Event.current.Use();
                MainTabWindow_Architect mainWindow = (MainTabWindow_Architect)MainButtonDefs.Architect.TabWindow;
                ArchitectCategoryTab tab = mainWindow.desPanelsCached.Find( x => x.def.defName == "Orders" );
                if( mainWindow.selectedDesPanel == tab )
                    MainButtonDefs.Architect.Worker.InterfaceTryActivate(); // deactivate
                else if( Find.MainTabsRoot.OpenTab == MainButtonDefs.Architect )
                {
                    mainWindow.selectedDesPanel = tab; // change menu
                    SoundDefOf.ArchitectCategorySelect.PlayOneShotOnCamera();
                }
                else
                {   // activate and change menu
                    MainButtonDefs.Architect.Worker.InterfaceTryActivate();
                    mainWindow.selectedDesPanel = tab;
                }
            }
        }
    }

    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("llunak.ShortcutsForOrdersMenu");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
