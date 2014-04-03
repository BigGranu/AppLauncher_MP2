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
  public class AppLauncherSettingsDelete : IWorkflowModel
  {
    public const string MODEL_ID_STR = "3C19B13A-D64C-4918-9AD6-17EC2D9AAE29";
    public const string ID = "id";
    public static ItemsList Items = new ItemsList();
    public Apps _apps = new Apps();

    /// <summary>
    /// Delete the selected Application
    /// </summary>
    public void Select(ListItem item)
    {
      foreach (var a in _apps.AppsList.Where(a => Convert.ToString(a.Id)  == (string)item.AdditionalProperties[ID]))
      {
        _apps.AppsList.Remove(a);
        break;
      }
      FillItems();
    }

    /// <summary>
    /// Read the Applications from MP Registration
    /// </summary>
    private void Init()
    {
      _apps = Helper.Help.LoadApps();
      FillItems();
    }

    /// <summary>
    /// Fill the Itemslist with all Applications
    /// </summary>
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