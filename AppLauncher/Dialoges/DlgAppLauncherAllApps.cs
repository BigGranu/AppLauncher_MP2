#region Copyright (C) 2007-2014 Team MediaPortal

/*
    Copyright (C) 2007-2014 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AppLauncher.Helper;
using AppLauncher.Models;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;
using Microsoft.Win32;

namespace AppLauncher.Dialoges
{
  public class DlgAppLauncherAllApps : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "ABA8802E-3E55-49DC-A446-E128D5B4A1D1";
    public const string NAME = "name";
    public const string ICON = "icon";
    public const string PATH = "path";

    #endregion

    #region Vars

    public static ItemsList items = new ItemsList();

    #endregion

    #region Propertys

    /// <summary>
    /// Hold the Length of the largest Path (Only to view the Dialog in one width)
    /// </summary>
    private static readonly AbstractProperty _maxStringProperty = new WProperty(typeof(string), string.Empty);
    public AbstractProperty MaxStringProperty
    {
      get { return _maxStringProperty; }
    }
    public string MaxString
    {
      get { return (string)_maxStringProperty.GetValue(); }
      set { _maxStringProperty.SetValue(value); }
    }

    #endregion

    #region public Members

    public void Init()
    {
      // Read the Softwarekey from Regiytry (only for installed Software)
      var rKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths");

      if (rKey == null) return;
      var sKeyNames = rKey.GetSubKeyNames();

      // Loop over all Keys
      foreach (var sKeyName in sKeyNames)
      {
        var sKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths\" + sKeyName);

        if (sKey == null) continue;

        // Read the Applicationpath
        var path = (string)sKey.GetValue("", "", RegistryValueOptions.None);

        if (path == null | !File.Exists(path)) continue;

        // only executable Files
        if (!path.EndsWith(".exe")) continue;

        // Set the maxLength for Dialodwidth
        if (path.Length > MaxString.Length)
          MaxString = path;

        // Fill the List with Items
        var item = new ListItem();
        item.AdditionalProperties[NAME] = sKeyName.Replace(".exe", "");
        item.AdditionalProperties[PATH] = path;
        item.SetLabel("Name", sKeyName.Replace(".exe", ""));

        // Extract the Icon
        var icon = Icon.ExtractAssociatedIcon(path);

        if (icon != null)
        {
          // Check if Icon allready exists and save it then not
          string iconPath = Help.GetIconPfad(sKeyName, icon.ToBitmap());
          item.SetLabel("ImageSrc", iconPath);
          item.AdditionalProperties[ICON] = iconPath;
        }
        items.Add(item);
      }
      items.FireChange();
    }

    public void Select(ListItem item)
    {
      // Added the selected Application to the Screen
      AppLauncherSettingsAdd.AppPath = (string)item.AdditionalProperties[PATH];
      AppLauncherSettingsAdd.IconPath = (string)item.AdditionalProperties[ICON];
      if (AppLauncherSettingsAdd.ShortName == "")
        AppLauncherSettingsAdd.ShortName = (string)item.AdditionalProperties[NAME];

      // Close the Dialog
      ServiceRegistration.Get<IScreenManager>().CloseTopmostDialog();
    }

    #endregion

    #region IWorkflowModel implementation

    public Guid ModelId
    {
      get { return new Guid(MODEL_ID_STR); }
    }

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      Init();
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
      // We could initialize some data here when changing the media navigation state
    }

    public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
    {
      // Todo: select any or the Last ListItem
    }

    public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
    {
    }

    public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
    {
      return ScreenUpdateMode.AutoWorkflowManager;
    }

    #endregion
  }
}