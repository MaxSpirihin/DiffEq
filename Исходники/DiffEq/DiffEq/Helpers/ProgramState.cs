using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffEq.Helpers
{
    public class ProgramState
    {
        public Preferences pref { get; set; }
        public List<GraphInfo> graphInfos { get; set; }

        public ProgramState()
        {
            pref = new Preferences();
            graphInfos = new List<GraphInfo>();
        }


        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }


        public static ProgramState DeSerialize(string Str)
        {
            ProgramState result = JsonConvert.DeserializeObject<ProgramState>(Str);
            return result;
        }
    }
}
