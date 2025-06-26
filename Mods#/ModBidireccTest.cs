using MSharp.ModAPI;
using System;
using MSharp.Launcher.Core.Bridge;
using ShpCore.Logging;

namespace ModDeEjemplo
{
	// -- Esto sería un mod de ejemplo que implementa la interfaz IMsharpMod --
	public class ModVioleta : IMsharpMod
	{
		private readonly IBridgeConnection _bridge;

		public ModVioleta(IBridgeConnection bridge) => this._bridge = bridge;

		public void OnStart() => Console.WriteLine("[example mod] Mod Violeta iniciado.");


		public void OnEvent(string type, object? payload = null)
		{
			Console.WriteLine($"[example mod] Evento recibido desde Java: {type} | Payload: {payload}");

			if (type == "BRIDGE_MSG") _bridge.Send($"[example mod] ACK: {payload}");

		}
		public void OnTick() { }

	}
}
