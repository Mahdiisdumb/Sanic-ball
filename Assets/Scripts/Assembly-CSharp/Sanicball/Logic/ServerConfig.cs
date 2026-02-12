namespace Sanicball.Logic
{
	public struct ServerConfig
	{
		public string ServerName { get; set; }

		public bool ShowInBrowser { get; set; }

		public int PrivatePort { get; set; }

		public string PublicIP { get; set; }

		public int PublicPort { get; set; }

		public int MaxPlayers { get; set; }
	}
}
