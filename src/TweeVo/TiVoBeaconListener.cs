using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace TweeVo
{
	public static class TiVoBeaconListener
	{
		public static event EventHandler<TiVoFoundEventArgs> TiVoFound;

		private static Thread _thread;
		private static UdpClient _listener;

		public static void Start()
		{
			// start a new thread
			if(_thread == null)
			{
				_thread = new Thread(DiscoverTiVos);
				_thread.IsBackground = true;
				_thread.Start();
			}
		}

		public static void Stop()
		{
			_listener.Close();
		}

		private static void DiscoverTiVos()
		{
			MessageBoxResult dr = MessageBoxResult.None;

			do
			{
				try
				{
					// bind to port 2190
					_listener = new UdpClient(2190);
				}
				catch(SocketException ex)
				{
					// this error indicates something else is listening on the port, most likely TiVo Desktop
					if(ex.ErrorCode == 10048)
					{
						dr = MessageBox.Show("There is an application running already listening for TiVo beacons.  This means you're likely running TiVo Desktop on this computer.  If the TiVo list has not yet been loaded, please disable TiVo Desktop.  Once the list is filled in and settings are saved, TiVo Desktop can be restarted.  Try again?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
						if(dr == MessageBoxResult.No)
							return;
					}
					else
						throw;
				}
			}
			while(dr == MessageBoxResult.Yes);

			// receive data from any IP on this port
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, 2190);

			while(_listener != null)
			{
				// get a beacon packet
				byte[] bytes = _listener.Receive(ref ep);
				string beacon = Encoding.ASCII.GetString(bytes);

				// parse it out
				TiVo t = ParseBeacon(beacon);
				// assign the IP of the incoming data (that's the TiVo's IP)
				t.IpAddress = ep.Address;

				// if it's a new TiVo, add it to the hashtable
				if((TweeVoSettings.Default.TiVos == null || !TweeVoSettings.Default.TiVos.ContainsKey(t.Identity)) && !t.Platform.StartsWith("pc"))
				{
					if(TweeVoSettings.Default.TiVos == null)
						TweeVoSettings.Default.TiVos = new Dictionary<string,TiVo>();

					t.Active = true;
					t.LastPolled = DateTime.Now;
					TweeVoSettings.Default.TiVos[t.Identity] = t;

					// raise the event to whoever is listening
					if(TiVoFound != null)
					{
						TiVoFoundEventArgs args = new TiVoFoundEventArgs {TiVo = t};
						TiVoFound(null, args);
					}
				}
			}
		}

		private static TiVo ParseBeacon(string beacon)
		{
			TiVo t = new TiVo();

			// parse the beacon packet into properties
			string[] lines = beacon.Split('\n');
			foreach(string line in lines)
			{
				// parse the name/value pairs
				string[] values = line.Split('=');
				switch(values[0].ToUpperInvariant())
				{
					case "PLATFORM":
						t.Platform = values[1];
						break;
					case "MACHINE":
						t.Machine = values[1];
						break;
					case "IDENTITY":
						t.Identity = values[1];
						break;
				}
			}

			return t;
		}
	}

	public class TiVoFoundEventArgs : EventArgs
	{
		public TiVo TiVo;
	}
}
