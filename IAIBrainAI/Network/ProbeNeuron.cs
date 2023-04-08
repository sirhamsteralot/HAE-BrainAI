using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI.Network
{
    internal class ProbeNeuron : INeuron
    {
        public bool Accumulative = true;

        public Axon Axon { get; init; } = new();
        public event Action<float>? ActivationCallback;

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
            ActivationCallback?.Invoke(inputValue);
            OutputValue = 0.1f * inputValue + 0.9f * OutputValue;
            Activated = true;
        }
    }
}
