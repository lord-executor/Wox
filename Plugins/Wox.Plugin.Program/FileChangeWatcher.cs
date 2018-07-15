using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wox.Infrastructure.Logger;

namespace Wox.Plugin.Program
{
	internal class FileChangeWatcher
	{
		private readonly IDictionary<string, IList<FileSystemWatcher>> WatchedPath = new Dictionary<string, IList<FileSystemWatcher>>();
		private bool _indexing = false;

		public void RegisterSources(List<IProgramSource> sources, string[] suffixes)
		{
			foreach (var watchers in WatchedPath.Values)
			{
				foreach (var w in watchers)
				{
					w.Dispose();
				}
			}

			WatchedPath.Clear();

			foreach (var s in sources)
			{
				if (Directory.Exists(s.Location))
				{
					AddWatch(s.Location, suffixes);
				}
			}

			_indexing = false;
		}

		private void AddWatch(string path, string[] programSuffixes, bool includingSubDirectory = true)
		{
			if (WatchedPath.ContainsKey(path)) return;

			if (!Directory.Exists(path))
			{
				Log.Warn($"|FileChangeWatcher|{path} doesn't exist");
				return;
			}

			var watchers = new List<FileSystemWatcher>();
			foreach (string fileType in programSuffixes)
			{
				FileSystemWatcher watcher = new FileSystemWatcher
				{
					Path = path,
					IncludeSubdirectories = includingSubDirectory,
					Filter = $"*.{fileType}",
					EnableRaisingEvents = true
				};
				watcher.Changed += FileChanged;
				watcher.Created += FileChanged;
				watcher.Deleted += FileChanged;
				watcher.Renamed += FileChanged;

				watchers.Add(watcher);
			}

			WatchedPath.Add(path, watchers);
		}

		private void FileChanged(object source, FileSystemEventArgs e)
		{
			if (!_indexing)
			{
				// only the first file watch event will actually trigger indexing to avoid
				// race conditions.
				_indexing = true;

				Task.Run(() =>
				{
					Main.IndexPrograms();
				});
			}
		}
	}
}
