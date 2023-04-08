using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI.Network
{
    internal class Axon
    {
        public Dictionary<int ,INeuron> ConnectedTo { get; } = new();
        public float Weight { get; set; }

        public void ConnectTo(int id, INeuron neuron)
        {
            ConnectedTo.TryAdd(id, neuron);
        }
    }
}
