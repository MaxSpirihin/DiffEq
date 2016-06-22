using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffEq.Helpers
{
    public class StateSaver
    {
        List<string> states = new List<string>();
        int currentState;

        public StateSaver()
        {
            UpdateState(new List<GraphInfo>());
        }

        public bool AllowCancel()
        {
            return currentState > 0;
        }

        public bool AllowReturn()
        {
            return currentState < states.Count() - 1;
        }

        public List<GraphInfo> Cancel()
        {
            if (currentState <= 0)
                throw new Exception("Нет действия для отмены");
            currentState--;
            return JsonConvert.DeserializeObject<List<GraphInfo>>(states[currentState]);
        }

        public List<GraphInfo> Return()
        {
            if (currentState >= states.Count() - 1)
                throw new Exception("Нет действия для возврата");
            currentState++;
            return JsonConvert.DeserializeObject<List<GraphInfo>>(states[currentState]);
        }

        public void UpdateState(List<GraphInfo> infos)
        {
            states.Add(JsonConvert.SerializeObject(infos));
            currentState = states.Count() - 1;
        }

        public void ChangePrefs(double newLeft, double newRight)
        {
            for (int i = 0; i < states.Count; i++)
            {
                List<GraphInfo> oldState = JsonConvert.DeserializeObject<List<GraphInfo>>(states[i]);
                List<GraphInfo> newState = new List<GraphInfo>();
                foreach (GraphInfo gi in oldState)
                {
                    if (!(gi.IsEquation && (gi.x0 < newLeft || gi.x0 > newRight)))
                        newState.Add(gi);
                }
                states[i] = JsonConvert.SerializeObject(newState);
            }

        }

    }
}
