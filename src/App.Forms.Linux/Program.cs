// <eddie_source_header>
// This file is part of Eddie/AirVPN software.
// Copyright (C)2014-2023 AirVPN (support@airvpn.org) / https://airvpn.org
//
// Eddie is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Eddie is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Eddie. If not, see <http://www.gnu.org/licenses/>.
// </eddie_source_header>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Eddie.Core;
using Eddie.Forms;

namespace Eddie.Forms.Linux
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 

		private static UiClient m_client;

		[STAThread]
		static void Main()
		{
			try
			{
				//Application.EnableVisualStyles();
				System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

				Application.ThreadException += new ThreadExceptionEventHandler(ApplicationThreadException);
				//Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException); // Mono Not Supported
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				Core.Platform.Instance = new Eddie.Platform.Linux.Platform();

				Eddie.Platform.Linux.NativeMethods.Signal((int)Eddie.Platform.Linux.NativeMethods.Signum.SIGHUP, SignalCallback);
				Eddie.Platform.Linux.NativeMethods.Signal((int)Eddie.Platform.Linux.NativeMethods.Signum.SIGINT, SignalCallback);
				Eddie.Platform.Linux.NativeMethods.Signal((int)Eddie.Platform.Linux.NativeMethods.Signum.SIGTERM, SignalCallback);
				Eddie.Platform.Linux.NativeMethods.Signal((int)Eddie.Platform.Linux.NativeMethods.Signum.SIGUSR1, SignalCallback);
				Eddie.Platform.Linux.NativeMethods.Signal((int)Eddie.Platform.Linux.NativeMethods.Signum.SIGUSR2, SignalCallback);

				m_client = new UiClient();
				m_client.Engine = new Engine(Environment.CommandLine);
				if (m_client.Init(Environment.CommandLine) == false)
					return;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Constants.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			// Application.Run must be outside the catch above, otherwise it's not unhandled
			if ((m_client != null) && (m_client.AppContext != null))
				System.Windows.Forms.Application.Run(m_client.AppContext);
		}

		private static void SignalCallback(int signum)
		{
			Engine.Instance.ExitStart();
		}

		public static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (m_client != null)
				m_client.OnUnhandledException("ApplicationThread", e.Exception);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (m_client != null)
			{
				Exception ex = (Exception)e.ExceptionObject;
				m_client.OnUnhandledException("CurrentDomain", ex);
			}
		}
	}
}