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
using AppLauncher.Helper;
using AppLauncher.Settings;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Utilities;
using MediaPortal.UI.Presentation.Workflow;

namespace AppLauncher.Models
{
  public class AppLauncherSettingsAdd : IWorkflowModel
  {
    #region Const

    public const string MODEL_ID_STR = "D47E22A3-3D0F-4A28-8EF6-1121B811508C";
    public const string VIEW_NOW = "[AppLauncher.Settings.Add.NoWindow]";
    public const string VIEW_MIN = "[AppLauncher.Settings.Add.Minimum]";
    public const string VIEW_NOR = "[AppLauncher.Settings.Add.Normal]";
    public const string VIEW_MAX = "[AppLauncher.Settings.Add.Maximum]";
    public const string S_ICO = "[AppLauncher.Settings.Add.SearchIcon]";
    public const string S_APP = "[AppLauncher.Settings.Add.SearchApp]";

    #endregion

    #region Propertys

    private static readonly AbstractProperty      _asAdmin = new WProperty(typeof(bool), true);

    public AbstractProperty AsAdminProperty
    {
      get { return _asAdmin; }
    }

    public bool AsAdmin
    {
      get { return (bool) _asAdmin.GetValue(); }
      set { _asAdmin.SetValue(value); }
    }

    private static readonly AbstractProperty _shortName = new WProperty(typeof(string), string.Empty);

    public AbstractProperty ShortNameProperty
    {
      get { return _shortName; }
    }

    public static string ShortName
    {
      get { return (string)_shortName.GetValue(); }
      set { _shortName.SetValue(value); }
    }

    private static readonly AbstractProperty _appPath = new WProperty(typeof(string), string.Empty);

    public AbstractProperty AppPathProperty
    {
      get { return _appPath; }
    }

    public static string AppPath
    {
      get { return (string)_appPath.GetValue(); }
      set { _appPath.SetValue(value); }
    }

    private static readonly AbstractProperty _arguments = new WProperty(typeof(string), string.Empty);

    public AbstractProperty ArgumentsProperty
    {
      get { return _arguments; }
    }

    public static string Arguments
    {
      get { return (string)_arguments.GetValue(); }
      set { _arguments.SetValue(value); }
    }

    private static readonly AbstractProperty _description = new WProperty(typeof(string), string.Empty);

    public AbstractProperty DescriptionProperty
    {
      get { return _description; }
    }

    public string Description
    {
      get { return (string)_description.GetValue(); }
      set { _description.SetValue(value); }
    }

    private static readonly AbstractProperty _username = new WProperty(typeof(string), string.Empty);

    public AbstractProperty UsernameProperty
    {
      get { return _username; }
    }

    public string Username
    {
      get { return (string)_username.GetValue(); }
      set { _username.SetValue(value); }
    }

    private static readonly AbstractProperty _password = new WProperty(typeof(string), string.Empty);

    public AbstractProperty PasswordProperty
    {
      get { return _password; }
    }

    public string Password
    {
      get { return (string)_password.GetValue(); }
      set { _password.SetValue(value); }
    }

    private static readonly AbstractProperty _iconPath = new WProperty(typeof(string), string.Empty);

    public AbstractProperty IconPathProperty
    {
      get { return _iconPath; }
    }

    public static string IconPath
    {
      get { return (string)_iconPath.GetValue(); }
      set { _iconPath.SetValue(value); }
    }

    private static readonly AbstractProperty _screenView = new WProperty(typeof(string), VIEW_MAX);

    public AbstractProperty ScreenViewProperty
    {
      get { return _screenView; }
    }

    public string ScreenView
    {
      get { return (string)_screenView.GetValue(); }
      set { _screenView.SetValue(value); }
    }

    #endregion

    protected PathBrowserCloseWatcher _pathBrowserCloseWatcher = null;

    private Apps _apps = new Apps();
    private string _screenMode = "3";

    public string Fallback = "no-icon.png";

    public void Init()
    {
      Clear();

      var settingsManager = ServiceRegistration.Get<ISettingsManager>();
      _apps = settingsManager.Load<Apps>() ?? new Apps(new List<App>());

      if (AppLauncherSettingsEdit.CurrentApp != null)
      {
        ShortName = AppLauncherSettingsEdit.CurrentApp.ShortName;
        Arguments = AppLauncherSettingsEdit.CurrentApp.Arguments;
        AppPath = AppLauncherSettingsEdit.CurrentApp.ApplicationPath;
        Description = AppLauncherSettingsEdit.CurrentApp.Description;
        IconPath = AppLauncherSettingsEdit.CurrentApp.IconPath;
        Password = AppLauncherSettingsEdit.CurrentApp.Password;
        Username = AppLauncherSettingsEdit.CurrentApp.Username;
        ScreenModeToString(AppLauncherSettingsEdit.CurrentApp.ScreenMode);
      }

      Maximum();
    }

    public void SearchApp()
    {
      string initialPath = "C:\\";
      Guid dialogHandle = ServiceRegistration.Get<IPathBrowser>().ShowPathBrowser(S_APP, true, false,
        string.IsNullOrEmpty(initialPath) ? null : LocalFsResourceProviderBase.ToResourcePath(initialPath),
        path =>
        {
          string choosenPath = LocalFsResourceProviderBase.ToDosPath(path.LastPathSegment.Path);
          if (string.IsNullOrEmpty(choosenPath))
            return false;

          return true;
        });

      if (_pathBrowserCloseWatcher != null)
        _pathBrowserCloseWatcher.Dispose();

      _pathBrowserCloseWatcher = new PathBrowserCloseWatcher(this, dialogHandle, choosenPath =>
      {
        AppPath = LocalFsResourceProviderBase.ToDosPath(choosenPath);
        var icon = Icon.ExtractAssociatedIcon(AppPath);

        if (icon != null)
        {
          IconPath = Help.GetIconPfad(choosenPath.FileName, icon.ToBitmap());
        }
      }, null);
    }

    public void SelectApp()
    {
      ServiceRegistration.Get<IWorkflowManager>().NavigatePushAsync(new Guid("5FB79A89-3CA6-4DD0-A867-7B934470CFC2"));
    }

    public void SearchIcon()
    {
      string initialPath = "C:\\";
      Guid dialogHandle = ServiceRegistration.Get<IPathBrowser>().ShowPathBrowser(S_ICO, true, false,
        string.IsNullOrEmpty(initialPath) ? null : LocalFsResourceProviderBase.ToResourcePath(initialPath),
        path =>
        {
          string choosenPath = LocalFsResourceProviderBase.ToDosPath(path.LastPathSegment.Path);
          if (string.IsNullOrEmpty(choosenPath))
            return false;

          return true;
        });

      if (_pathBrowserCloseWatcher != null)
        _pathBrowserCloseWatcher.Dispose();

      _pathBrowserCloseWatcher = new PathBrowserCloseWatcher(this, dialogHandle, choosenPath => { IconPath = LocalFsResourceProviderBase.ToDosPath(choosenPath); }, null);
    }

    public void NoWindow()
    {
      ScreenView = VIEW_NOW;
      _screenMode = "0";
    }

    public void Minimum()
    {
      ScreenView = VIEW_MIN;
      _screenMode = "1";
    }

    public void Normal()
    {
      ScreenView = VIEW_NOR;
      _screenMode = "2";
    }

    public void Maximum()
    {
      ScreenView = VIEW_MAX;
      _screenMode = "3";
    }

    public void Add()
    {
      if (AppPath != "")
      {
        if (AppLauncherSettingsEdit.CurrentApp != null)
          _apps.AppsList.Remove(AppLauncherSettingsEdit.CurrentApp);

        var app = new App { ShortName = ShortName, ApplicationPath = AppPath, Arguments = Arguments, Description = Description, IconPath = IconPath, Password = Password, Username = Username, ScreenMode = _screenMode, Admin = AsAdmin};
        _apps.AppsList.Add(app);
      }
      Clear();
    }

    private void ScreenModeToString(string mode)
    {
      if (mode == "0")
        NoWindow();

      if (mode == "1")
        Minimum();

      if (mode == "2")
        Normal();

      if (mode == "3")
        Maximum();
    }

    private void Clear()
    {
      ShortName = "";
      Arguments = "";
      AppPath = "";
      Description = "";
      IconPath = "";
      Password = "";
      Username = "";
      ScreenView = VIEW_MAX;
      _screenMode = "3";
    }

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
      Help.SetIds(_apps);
      ServiceRegistration.Get<ISettingsManager>().Save(_apps);
      AppLauncherSettingsEdit.CurrentApp = null;
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