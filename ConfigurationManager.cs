using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public class ConfigurationManager
    {
        private ConfigurationManager() { }

        public static ConfigurationManager Source { get { return Nested.source; } }

        private class Nested
        {
            static Nested(){ }
            internal static readonly ConfigurationManager source = new ConfigurationManager();
        }

        public int OutputDevice;
        public int InputDevice;
    }
}
