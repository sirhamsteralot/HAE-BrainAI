using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI.Network
{
    internal class NeuronPool
    {
        public int computeToMemoryRatio = 5;

        public bool DynamicallyAddProbes { get; set; }

        public List<INeuron> NeuronSet = new();
        public List<ProbeNeuron> ActivationProbes = new();
        public List<ProbeNeuron> ReadProbes = new();

        public void InitializeTopology(int PoolSize)
        {
            for (int i = 0; i < PoolSize; i++)
            {
                if (i < (NeuronSet.Count / computeToMemoryRatio))
                    NeuronSet.Add(new MemoryNeuron());
                else
                    NeuronSet.Add(new ComputeNeuron());
            }
        }

        public void RandomConnectionStep(Random random, int connectionsToMake)
        {
            for (int i = 0; i < connectionsToMake; i++)
            {
                int connectFromIndex = random.Next(NeuronSet.Count);
                int connectToIndex = random.Next(NeuronSet.Count);

                NeuronSet[connectFromIndex].Axon.ConnectTo(connectToIndex, NeuronSet[connectToIndex]);

                Console.WriteLine($"connecting [{connectFromIndex}] to [{connectToIndex}]");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="connectionsToMutate"> amount of connections to mutate </param>
        /// <param name="mutationDeviationMultiplier"> multiplier to the weight addition</param>
        /// <param name="mutateConnectedChance"> value between 0 and 1</param>
        public void MutateConnections(Random random, int connectionsToMutate, float mutationDeviationMultiplier, float mutateConnectedChance)
        {
            for (int i = 0; i < connectionsToMutate; i++)
            {
                int connectFromIndex = random.Next(NeuronSet.Count);
                var axon = NeuronSet[connectFromIndex].Axon;

                axon.Weight += mutationDeviationMultiplier * ((random.NextSingle() * 2) - 1f);

                int connectToIndex = random.Next(axon.ConnectedTo.Count);

                if (random.NextSingle() < mutateConnectedChance)
                {
                    if (!axon.ConnectedTo.ContainsKey(connectToIndex))
                    {
                        axon.ConnectTo(connectToIndex, NeuronSet[connectToIndex]);
                    }
                    else
                    {
                        axon.ConnectedTo.Remove(connectToIndex);
                        //Console.WriteLine($"Removed connection! [{connectFromIndex}]-[{connectToIndex}]");
                    }
                }

            }
        }

        public bool Activate(List<float> activations)
        {
            int nettoProbeCount = ActivationProbes.Count - activations.Count;

            for(int i = 0; i < ActivationProbes.Count; i++)
            {
                ActivationProbes[i].ActivationFunction(activations[i]);
            }

            if (!DynamicallyAddProbes || nettoProbeCount < 0)
                return false;

            for (int i = 0; i < nettoProbeCount; i++)
            {
                var neuron = new ProbeNeuron();
                neuron.ActivationFunction(activations[ActivationProbes.Count - 1]);

                ActivationProbes.Add(neuron);
                NeuronSet.Add(neuron);
            }

            return true;
        }

        public void PushNeurons()
        {
            foreach (var neuron in NeuronSet)
            {
                neuron.PushValue();
            }
        }

        public void AddProbes(int readProbes, int writeProbes)
        {
            for (int i = 0; i < readProbes; i++)
            {
                var probeNeuron = new ProbeNeuron();
                ReadProbes.Add(probeNeuron);
                NeuronSet.Add(probeNeuron);
            }

            for (int i = 0; i < writeProbes; i++)
            {
                var probeNeuron = new ProbeNeuron();
                ActivationProbes.Add(probeNeuron);
                NeuronSet.Add(probeNeuron);
            }
        }

        public List<float> OutputSnapshot()
        {
            List<float> returnList = new();

            foreach (var probe in ReadProbes)
            {
                returnList.Add(probe.OutputValue);
            }

            return returnList;
        }
    }
}
