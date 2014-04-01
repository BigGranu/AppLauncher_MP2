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
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Presentation.Workflow;

namespace AppLauncher.Dialoges
{
  public class DlgSwitchPlaying : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "90DB10F3-FD09-42F9-9DED-12E6804FD098";

    #endregion

    #region Propertys

    private static readonly AbstractProperty _primaryTitle = new WProperty(typeof(string), string.Empty);

    public AbstractProperty PrimaryTitleProperty
    {
      get { return _primaryTitle; }
    }

    public static string PrimaryTitle
    {
      get { return (string)_primaryTitle.GetValue(); }
      set { _primaryTitle.SetValue(value); }
    }

    private static readonly AbstractProperty _secondaryTitle = new WProperty(typeof(string), string.Empty);

    public AbstractProperty SecondaryTitleProperty
    {
      get { return _secondaryTitle; }
    }

    public static string SecondaryTitle
    {
      get { return (string)_secondaryTitle.GetValue(); }
      set { _secondaryTitle.SetValue(value); }
    }

    public bool SecondaryVisible = false;

    #endregion

    #region public Members

    public void Primary_Stop()
    {
      ServiceRegistration.Get<IPlayerContextManager>().PrimaryPlayerContext.Stop();
    }

    public void Primary_Pause()
    {
      ServiceRegistration.Get<IPlayerContextManager>().PrimaryPlayerContext.Pause();
    }

    public void Secondary_Stop()
    {
      ServiceRegistration.Get<IPlayerContextManager>().SecondaryPlayerContext.Stop();
    }

    public void Secondary_Pause()
    {
      ServiceRegistration.Get<IPlayerContextManager>().SecondaryPlayerContext.Pause();
    }

    #endregion

    #region private Members

    private void Init()
    {
      var pp = ServiceRegistration.Get<IPlayerContextManager>().PrimaryPlayerContext;
      PrimaryTitle = pp.CurrentPlayer.MediaItemTitle;

      if (ServiceRegistration.Get<IPlayerContextManager>().NumActivePlayerContexts != 2) return;
      var sp = ServiceRegistration.Get<IPlayerContextManager>().SecondaryPlayerContext;
      SecondaryTitle = sp.CurrentPlayer.MediaItemTitle;
      SecondaryVisible = true;
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
