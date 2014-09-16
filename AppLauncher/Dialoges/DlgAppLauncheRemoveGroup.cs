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
using AppLauncher.Helper;
using AppLauncher.Settings;
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;

namespace AppLauncher.Dialoges
{
  public class DlgAppLauncheRemoveGroup : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "3C38DF86-AE80-4411-8C3D-9480E7AAB279";
    public const string GROUP = "group";

    #endregion

    #region Vars

    public static ItemsList items = new ItemsList();
    private static Apps _apps;
    private static List<string> _groups;

    #endregion

    #region public Members

    public void Init()
    {
      var settingsManager = ServiceRegistration.Get<ISettingsManager>();
       _apps = settingsManager.Load<Apps>();
       _groups = new List<string>();

      items.Clear();

      foreach (var a in _apps.AppsList.Where(a => !_groups.Contains(a.Group) & a.Group != ""))
      {
        _groups.Add(a.Group);
        var item = new ListItem();
        item.AdditionalProperties[GROUP] = a.Group;
        item.SetLabel("Name", a.Group);
        items.Add(item);
      }
      items.FireChange();
    }

    public void Select(ListItem item)
    {
      item.Selected = item.Selected != true;
      item.FireChange();
    }

    public void Delete()
    {
      foreach (var a in items.Where(item => item.Selected).SelectMany(item => _apps.AppsList.Where(a => a.Group == (string)item.AdditionalProperties[GROUP])))
      {
        a.Group = "";
      }
      Help.SaveApps(_apps);

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
