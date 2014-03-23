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
using System.Drawing;
using System.IO;
using AppLauncher.Settings;
using MediaPortal.Common;
using MediaPortal.Common.PathManager;

namespace AppLauncher.Helper
{
  public class Help
  {
    public static string AppLauncherFolder = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\AppLauncher");

    public static string GetIconPfad(string title, Image bmp)
    {
      var path = AppLauncherFolder + "\\" + title + ".bmp";

      if (!IconExists(path))
      {
        bmp.Save(path);
      }
      return path;
    }

    private static bool IconExists(string title)
    {
      if (!Directory.Exists(AppLauncherFolder))
        Directory.CreateDirectory(AppLauncherFolder);
      return File.Exists(title);
    }

    public static void SetIds(Apps apps)
    {
      int x = 0;
      foreach (App a in apps.AppsList)
      {
        x ++;
        a.Id = Convert.ToString(x);
      }
    }
  }
}