using IAIBrainAI.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAIBrainAI
{
    internal class GeneticTrainer
    {
        public List<NeuronPool> population;
        public int maxPopulationSize = 50;
        public int mutationsToMake = 20;
        public float deleteChance = 0.2f;
        public float weightMutationMultiplier = 1f;

        public int runsBeforeMutate = 25;

        Random random = new Random();

        public GeneticTrainer(NeuronPool initialPool) { 
            population = new List<NeuronPool>();
            population.Add(initialPool);
        }

        public void TrainLoop(List<float> inputs, List<float> outputs, int epochs)
        {
            for (int i = 0; i < epochs; i++)
            {
                double bestOfGeneration = Train(inputs, outputs);
                Console.WriteLine($"Epoch [{i}]: {bestOfGeneration}");
            }
        }

        public double Train(List<float> inputs, List<float> outputs)
        {
            SortedDictionary<double, int> fitnesses= new();

            for (int k = 0; k < population.Count; k++)
            {
                population[k].Activate(inputs);

                for (int i = 0; i < runsBeforeMutate; i++)
                {
                    population[k].PushNeurons();
                }

                var actualOutput = population[k].OutputSnapshot();

                double fitness = 0;

                for (int i = 0; i < outputs.Count; i++)
                {
                    fitness += Math.Abs(outputs[i] - actualOutput[i]);
                }

                fitnesses.TryAdd(fitness, k);
            }

            List<NeuronPool> oldGenerationList = new();
            foreach (var keyvaluePair in fitnesses)
            {
                oldGenerationList.Add(population[keyvaluePair.Value]);
            }

            population = GenerateNextGeneration(oldGenerationList);

            return fitnesses.First().Key;
        }

        // assumes sourcegeneration list is sorted by fitness with the fittest at index 0
        public List<NeuronPool> GenerateNextGeneration(List<NeuronPool>sourceGeneration)
        {
            List<NeuronPool> nextGeneration = new List<NeuronPool> ();

            int carryOver = Math.Max( maxPopulationSize / 10, 1);

            if (sourceGeneration.Count > carryOver)
            {
                for(int i = 0; i < population.Count && i < sourceGeneration.Count; i++)
                {
                    nextGeneration.Add(sourceGeneration[i]);
                }
            } else
            {
                nextGeneration.AddRange(sourceGeneration);
            }

            carryOver = nextGeneration.Count;
            int leftToAdd = maxPopulationSize - carryOver;

            for (int i = 0; i < leftToAdd; i++)
            {
                NeuronPool newPool = CopyPool(sourceGeneration[random.Next(carryOver)]);
                newPool.MutateConnections(random, mutationsToMake, weightMutationMultiplier, deleteChance);

                nextGeneration.Add(newPool);
            }

            return nextGeneration;
        }

        public NeuronPool CopyPool(NeuronPool currentPool)
        {
            NeuronPool nextGenPool = new NeuronPool();
            foreach (var neuron in currentPool.NeuronSet)
            {
                if (neuron is ProbeNeuron)
                    continue;

                if (neuron is ComputeNeuron)
                {
                    ComputeNeuron computeNeuron = new ComputeNeuron();
                    computeNeuron.Axon.Weight = neuron.Axon.Weight;

                    nextGenPool.NeuronSet.Add(computeNeuron);
                }

                if (neuron is MemoryNeuron)
                {
                    MemoryNeuron memoryNeuron = new MemoryNeuron();
                    memoryNeuron.Axon.Weight = neuron.Axon.Weight;

                    nextGenPool.NeuronSet.Add(memoryNeuron);
                }
            }

            foreach (var neuron in currentPool.ReadProbes)
            {
                ProbeNeuron probeNeuron = new ProbeNeuron();

                probeNeuron.Axon.Weight = neuron.Axon.Weight;
                nextGenPool.NeuronSet.Add(probeNeuron);
                nextGenPool.ReadProbes.Add(probeNeuron);
            }

            foreach (var neuron in currentPool.ActivationProbes)
            {
                ProbeNeuron probeNeuron = new ProbeNeuron();

                probeNeuron.Axon.Weight = neuron.Axon.Weight;
                nextGenPool.NeuronSet.Add(probeNeuron);
                nextGenPool.ActivationProbes.Add(probeNeuron);
            }

            for(int i = 0; i <  currentPool.NeuronSet.Count; i++)
            {
                foreach (var connectTo in currentPool.NeuronSet[i].Axon.ConnectedTo.Keys)
                {
                    nextGenPool.NeuronSet[i].Axon.ConnectTo(connectTo, nextGenPool.NeuronSet[connectTo]);
                }
            }

            return nextGenPool;
        }

    }
}
