using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI.Network
{
    internal interface INeuron
    {
        public Axon Axon { get; }
        public bool Activated { get;}
        public float OutputValue { get; }

        public void ActivationFunction(float inputValue);
        public void PushValue();
    }
}
