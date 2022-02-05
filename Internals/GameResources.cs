using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace WiiPlayTanksRemake.Internals
{
	public static class GameResources
	{
		private static Dictionary<string, object> ResourceCache { get; set; } = new();

		public static T GetResource<T>(this ContentManager manager, string name) where T : class
		{
			if (ResourceCache.TryGetValue(Path.Combine(manager.RootDirectory, name), out var val) && val is T content)
			{
				return content;
			}
			return LoadResource<T>(manager, name);
		}
		public static T LoadResource<T>(ContentManager manager, string name) where T : class
		{
			OtherContentManager otherContent = new OtherContentManager(new GameServiceContainer(), "C:/Users/Cuno/Documents/WiiPlayTanksRemake/Content/bin/Debug/net5.0/Content");
			T loaded = otherContent.LoadContentExclusive<T>(name);

			ResourceCache[name] = loaded;
			return loaded;
		}

		public static T GetGameResource<T>(string name) where T : class
        {
			return GetResource<T>(TankGame.Instance.Content, name);
        }
	}

	public class OtherContentManager : ContentManager
    {
		public OtherContentManager(IServiceProvider serviceProvider, string RootDirectory) : base(serviceProvider, RootDirectory)
        {

        }

		public T LoadContentExclusive<T>(string assetName) where T : class
        {
			return ReadAsset<T>(assetName, null);
        }

		public void Unload(IDisposable contentItem)
        {
			contentItem.Dispose();
        }
    }
}