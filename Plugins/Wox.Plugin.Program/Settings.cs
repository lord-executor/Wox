using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.Program.Programs;

namespace Wox.Plugin.Program
{
    public class Settings
    {
		[JsonIgnore]
		public IEnumerable<IProgramSource> ProgramSources => Sources.Cast<IProgramSource>().ToList();

		public List<ProgramSource> Sources { get; set; } = new List<ProgramSource>();

        public string[] ProgramSuffixes { get; set; } = {"bat", "appref-ms", "exe", "lnk"};

        public bool EnableStartMenuSource { get; set; } = true;

        public bool EnableRegistrySource { get; set; } = true;

        internal const char SuffixSeperator = ';';

        public class ProgramSource : IProgramSource
        {
            public string Location { get; set; }
        }
    }
}
