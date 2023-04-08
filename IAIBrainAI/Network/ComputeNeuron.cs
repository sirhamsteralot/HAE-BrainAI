using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI.Network
{
    internal struct ComputeNeuron : INeuron
    {
        public Axon Axon { get; init; }

        private const float epsilon = 0.1f;

        public float OutputValue { get; private set; }
        public bool Activated { get; private set; }

        public ComputeNeuron()
        {
            Axon = new Axon();
        }

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
            if (inputValue > 0)
            {
                OutputValue = 0.1f * inputValue + 0.9f * OutputValue;
            }
            else
            {
                float computedValue = epsilon * (MathF.Exp(inputValue / epsilon) - 1);
                OutputValue = 0.1f * computedValue + 0.9f * OutputValue;
                Activated = true;
            }
        }
    }
}
