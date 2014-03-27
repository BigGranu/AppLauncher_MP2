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
using System.Diagnostics;
using System.Linq;
using System.Security;
using AppLauncher.Settings;
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace AppLauncher.Models
{
  public class AppLauncherHome : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "624339C2-0D3B-437B-8046-6F540D704A93";
    public const string ID = "id";

    #endregion

    public static ItemsList items = new ItemsList();

    private static Apps _apps;
    private static ProcessStartInfo _pInfo;

    #region public Methods


    public static void StartApp(ListItem item)
    {
      Start(_apps.AppsList.FirstOrDefault(a => Convert.ToString(a.Id) == (string)item.AdditionalProperties[ID]));
    }

    #endregion

    #region private Methods

    private static void Start(App app)
    {
      try
      {
        _pInfo = new ProcessStartInfo { FileName = app.ApplicationPath, Arguments = app.Arguments };
        
        _pInfo.WindowStyle = app.WindowStyle;

        if (app.Admin == false & app.Username != "" & app.Password != "")
        {
          _pInfo.UserName = app.Username;
          _pInfo.Password = ToSecureString(app.Password);
        }

        var p = new Process { StartInfo = _pInfo };
        p.Start();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.StackTrace);
      }
    }

    private static SecureString ToSecureString(string password)
    {
      var pass = new SecureString();
      foreach (var c in password)
      {
        pass.AppendChar(c);
      }
      return pass;
    }

    private static void Init()
    {
      items.Clear();

      var settingsManager = ServiceRegistration.Get<ISettingsManager>();
      _apps = settingsManager.Load<Apps>();

      foreach (var a in _apps.AppsList)
      {
        var item = new ListItem();
        item.AdditionalProperties[ID] = Convert.ToString(a.Id);
        item.SetLabel("ImageSrc", a.IconPath);
        item.SetLabel("Description", a.Description);
        item.SetLabel("Name", a.ShortName);
        items.Add(item);
      }
      items.FireChange();
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