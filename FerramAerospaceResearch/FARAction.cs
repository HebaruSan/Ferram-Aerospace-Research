﻿/*
Ferram Aerospace Research v0.14.3
Copyright 2014, Michael Ferrara, aka Ferram4

    This file is part of Ferram Aerospace Research.

    Ferram Aerospace Research is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Ferram Aerospace Research is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Ferram Aerospace Research.  If not, see <http://www.gnu.org/licenses/>.

    Serious thanks:		a.g., for tons of bugfixes and code-refactorings
            			Taverius, for correcting a ton of incorrect values
            			sarbian, for refactoring code for working with MechJeb, and the Module Manager 1.5 updates
            			ialdabaoth (who is awesome), who originally created Module Manager
                        Regex, for adding RPM support
            			Duxwing, for copy editing the readme
 * 
 * Kerbal Engineer Redux created by Cybutek, Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
 *      Referenced for starting point for fixing the "editor click-through-GUI" bug
 *
 * Part.cfg changes powered by sarbian & ialdabaoth's ModuleManager plugin; used with permission
 *	http://forum.kerbalspaceprogram.com/threads/55219
 *
 * Toolbar integration powered by blizzy78's Toolbar plugin; used with permission
 *	http://forum.kerbalspaceprogram.com/threads/60863
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;

namespace ferram4
{
    //Added by DaMichel: 
    public class FARAction : KSPAction
    {
        // Is constructed each time a part module instance is created.
        // The current AG seems to be stored elsewhere so base.actionGroup
        // is only used for the initial assignment.
        public FARAction(string guiName, int actionIdentifier)
            : base(guiName)
        {
            base.actionGroup = FARActionGroupConfiguration.map(actionIdentifier);
        }
    };



    public class FARActionGroupConfiguration
    {
        public const int ID_SPOILER = 0;
        public const int ID_INCREASE_FLAP_DEFLECTION = 1;
        public const int ID_DECREASE_FLAP_DEFLECTION = 2;
        public const int ACTION_COUNT = 3;
        // private lookup tables
        static KSPActionGroup[] id2actionGroup = { KSPActionGroup.Brakes, KSPActionGroup.None, KSPActionGroup.None };
        // keys in the configuration file
        static string[] configKeys = {  "actionGroupSpoiler",
                                        "actionGroupIncreaseFlapDeflection",
                                        "actionGroupDecreaseFlapDeflection" };
        // for the gui
        static string[] guiLabels = { "Spoilers",
                                      "Increase Flap Deflection",
                                      "Decrease Flap Deflection" };
        static string[] currentGuiStrings = { id2actionGroup[0].ToString(), 
                                              id2actionGroup[1].ToString(),
                                              id2actionGroup[2].ToString() };

        public static KSPActionGroup map(int id)
        {
            return id2actionGroup[id];
        }

        public static void LoadConfiguration()
        {
            // straight forward, reading the (action name, action group) tuples
            KSP.IO.PluginConfiguration config = FARDebugOptions.config;
            for (int i = 0; i < ACTION_COUNT; ++i)
            {
                try
                {
                    id2actionGroup[i] = (KSPActionGroup)Enum.Parse(typeof(KSPActionGroup), config.GetValue(configKeys[i], id2actionGroup[i].ToString())); ;
                    currentGuiStrings[i] = id2actionGroup[i].ToString(); // don't forget to initialize the gui
                    Debug.Log(String.Format("FAR: loaded AG {0} as {1}", configKeys[i], id2actionGroup[i]));
                }
                catch (Exception e)
                {
                    Debug.LogWarning("FAR: error reading config key '" + configKeys[i] + "' with value '" + config.GetValue(configKeys[i], "n/a") + "' gave " + e.ToString());
                }
            }
        }

        public static void SaveConfigruration()
        {
            KSP.IO.PluginConfiguration config = FARDebugOptions.config;
            for (int i = 0; i < ACTION_COUNT; ++i)
            {
                Debug.Log(String.Format("FAR: save AG {0} as {1}", configKeys[i], id2actionGroup[i]));
                config.SetValue(configKeys[i], id2actionGroup[i].ToString());
            }
        }

        public static void DrawGUI()
        {
            GUIStyle label = new GUIStyle(GUI.skin.label);
            label.normal.textColor = GUI.skin.toggle.normal.textColor;
            GUILayout.Label("Default Action Group Assignments");
            GUILayout.BeginHorizontal(); // left column: label, right column: text field
            GUILayout.BeginVertical();
            for (int i = 0; i < FARActionGroupConfiguration.ACTION_COUNT; ++i)
            {
                GUILayout.Label(FARActionGroupConfiguration.guiLabels[i], label);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            for (int i = 0; i < FARActionGroupConfiguration.ACTION_COUNT; ++i)
            {
                GUILayout.BeginHorizontal();
                currentGuiStrings[i] = GUILayout.TextField(currentGuiStrings[i], GUILayout.Width(150));
                bool ok = false;
                try
                {
                    id2actionGroup[i] = (KSPActionGroup)Enum.Parse(typeof(KSPActionGroup), currentGuiStrings[i]);
                    ok = true;
                    //Debug.Log(String.Format("FAR: set AG {0} to {1}", guiLabels[i], id2actionGroup[i]));
                }
                catch   //FIXME with a dropdown list
                {
                }
                GUILayout.Label(ok ? " Ok" : " Invalid", GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal(); // end of columns
            GUILayout.BeginHorizontal(); // list admissible entries for ease of use. Unity has no comboboxes, so this has to do for now ...
            GUILayout.Label("Admissible Entries ", label);
            string[] names = Enum.GetNames(typeof(KSPActionGroup));
            var sb = new System.Text.StringBuilder(256);
            for (int i = 0; i < names.Length; ++i)
            {
                sb.Append(names[i]);
                if (i < names.Length - 1) sb.Append(',');
            }
            GUILayout.Label(sb.ToString());
            GUILayout.EndHorizontal();
        }
    }
}
