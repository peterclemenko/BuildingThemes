﻿using System;
using BuildingThemes.GUI;
using ColossalFramework;
using ICities;

namespace BuildingThemes
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            int currentConfigVersion = BuildingThemesManager.instance.Configuration.version;

            Debugger.Initialize();

            Debugger.Log("ON_CREATED");
            Debugger.Log("Building Themes: Initializing Mod...");

            PolicyPanelEnabler.Register();

            BuildingThemesManager.instance.Reset();

            BuildingVariationManager.instance.Reset();

            if (BuildingVariationManager.Enabled)
            {
                bool cloneFeatureUsed = false;

                if (BuildingThemesManager.instance.Configuration.version == 0)
                {

                    foreach (var theme in BuildingThemesManager.instance.Configuration.themes)
                    {
                        foreach (var building in theme.buildings)
                        {
                            if (building.baseName != null)
                            {
                                cloneFeatureUsed = true;
                                break;
                            }
                        }

                        if (cloneFeatureUsed) break;
                    }
                }
                else cloneFeatureUsed = true;

                if (cloneFeatureUsed)
                {
                    try { Detour.BuildingInfoDetour.Deploy(); }
                    catch (Exception e) { Debugger.LogException(e); }
                }
                else
                {
                    BuildingVariationManager.Enabled = false;
                }
            }

            try { Detour.BuildingManagerDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.DistrictManagerDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.ZoneBlockDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.ImmaterialResourceManagerDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<ResidentialBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<CommercialBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<IndustrialBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PrivateBuildingAIDetour<OfficeBuildingAI>.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.PoliciesPanelDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            try { Detour.DistrictWorldInfoPanelDetour.Deploy(); }
            catch (Exception e) { Debugger.LogException(e); }

            BuildingThemesManager.instance.Configuration.version = 1;
            BuildingThemesManager.instance.SaveConfig();

            Debugger.Log("Building Themes: Mod successfully intialized.");
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Don't load if it's not a game
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame) return;

            Debugger.Log("ON_LEVEL_LOADED");

            Debugger.AppendModList();
            Debugger.AppendThemeList();

            Debugger.OnLevelLoaded();

            PolicyPanelEnabler.UnlockPolicyToolbarButton();
            BuildingThemesManager.instance.ImportStylesAsThemes();
            UIThemeManager.Initialize();
            UIStyleButtonReplacer.ReplaceStyleButton();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            BuildingThemesManager.instance.Reset();

            Debugger.Log("ON_LEVEL_UNLOADING");

            Debugger.OnLevelUnloading();

            UIThemeManager.Destroy();
        }

        public override void OnReleased()
        {
            base.OnReleased();

            Debugger.Log("ON_RELEASED");

            BuildingVariationManager.instance.Reset();

            Debugger.Log("Building Themes: Reverting detoured methods...");

            Singleton<BuildingThemesManager>.instance.Reset();

            try
            {
                Detour.BuildingInfoDetour.Revert();
                Detour.BuildingManagerDetour.Revert();
                Detour.DistrictManagerDetour.Revert();
                Detour.ZoneBlockDetour.Revert();
                Detour.ImmaterialResourceManagerDetour.Revert();
                Detour.PrivateBuildingAIDetour<ResidentialBuildingAI>.Revert();
                Detour.PrivateBuildingAIDetour<CommercialBuildingAI>.Revert();
                Detour.PrivateBuildingAIDetour<IndustrialBuildingAI>.Revert();
                Detour.PrivateBuildingAIDetour<OfficeBuildingAI>.Revert();
                Detour.PoliciesPanelDetour.Revert();
                Detour.DistrictWorldInfoPanelDetour.Revert();
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }

            PolicyPanelEnabler.Unregister();

            Debugger.Log("Building Themes: Done!");

            Debugger.Deinitialize();
        }
    }
}