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
using System.Linq;
using AppLauncher.Settings;
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace AppLauncher.Models
{
  public class AppLauncherSettingsEdit : IWorkflowModel
  {
    public const string MODEL_ID_STR = "873EB147-C998-4632-8F86-D5E24062BE2E";
    public const string ID = "id";
    public static App CurrentApp;
    public static ItemsList Items = new ItemsList();
    public Apps _apps;

    public void Select(ListItem item)
    {
      foreach (var a in _apps.AppsList.Where(a => Convert.ToString(a.Id) == (string)item.AdditionalProperties[ID]))
      {
        CurrentApp = a;
      }
      ServiceRegistration.Get<IWorkflowManager>().NavigatePushAsync(new Guid("63B0EBCE-8B52-4DE6-9B8F-D902507CC53D"));
    }

    private void Init()
    {
      CurrentApp = null;
      _apps = new Apps();
      _apps = Helper.Help.LoadApps();

      FillItems();
    }

    private void FillItems()
    {
       Items.Clear();
       foreach (var a in _apps.AppsList)
      {
        var item = new ListItem();
        item.AdditionalProperties[ID] = Convert.ToString(a.Id);
        item.SetLabel("Name", a.ShortName);
        item.SetLabel("ImageSrc", a.IconPath);
        Items.Add(item);
      }
      Items.FireChange();
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
      Helper.Help.SaveApps(_apps);
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
      Init();
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