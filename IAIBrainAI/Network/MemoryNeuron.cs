using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI.Network
{
    internal class MemoryNeuron : INeuron
    {
        public Axon Axon { get; init; } = new();

        public float OutputValue { get; private set; }
        public bool Activated { get; private set; }

        public void PushValue()
        {
            if (!Activated)
                return;

            foreach (INeuron neuron in Axon.ConnectedTo.Values)
            {
                neuron.ActivationFunction(OutputValue);
                Activated = false;
            }
        }

        public void ActivationFunction(float inputValue)
        {
            OutputValue = inputValue * 0.1f + OutputValue * 0.9f;
            Activated = true;
        }
    }
}
