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
        public int maxPopulationSize = 250;
        public int mutationsToMake = 100;
        public float deleteChance = 0.5f;
        public float weightMutationMultiplier = 5f;
        public int runsToProcess = 5;

        Random random = new Random();

        public GeneticTrainer(NeuronPool initialPool) { 
            population = new List<NeuronPool>();
            population.Add(initialPool);
        }

        public void TrainLoop(List<List<float>> inputs, List<List<float>> outputs, int epochs)
        {
            if (inputs.Count != outputs.Count)
                throw new Exception("input count does not match output count for training!");

            for (int i = 0; i < epochs; i++)
            {
                double bestOfGeneration = Train(inputs, outputs);
                Console.WriteLine($"Epoch [{i}]: {bestOfGeneration}");
            }
        }

        public double Train(List<List<float>> inputs, List<List<float>> outputs)
        {
            Dictionary<int, double> fitnesses= new();

            for (int k = 0; k < population.Count; k++)
            {
                double fitness = 0;
                NeuronPool testPool = population[k];
                

                for (int j = 0; j < inputs.Count; j++)
                {
                    List<float> testInputs = inputs[j];
                    List<float> expectedOutputs = outputs[j];

                    for (int i = 0; i < runsToProcess; i++)
                    {
                        testPool.Activate(testInputs);
                        testPool.PushNeurons();
                    }

                    var actualOutput = testPool.OutputSnapshot();

                    for (int i = 0; i < expectedOutputs.Count; i++)
                    {
                        fitness += Math.Abs(expectedOutputs[i] - actualOutput[i]);
                    }
                }

                fitnesses[k] = fitness;
            }

            var sortedFitni = fitnesses.OrderBy(x => x.Value);

            List<NeuronPool> oldGenerationList = new();
            foreach (var keyvaluePair in sortedFitni)
            {
                oldGenerationList.Add(population[keyvaluePair.Key]);
            }

            population.Clear();
            population.AddRange(GenerateNextGeneration(oldGenerationList));

            return sortedFitni.First().Value;
        }

        // assumes sourcegeneration list is sorted by fitness with the fittest at index 0
        public List<NeuronPool> GenerateNextGeneration(List<NeuronPool> sourceGeneration)
        {
            List<NeuronPool> nextGeneration = new List<NeuronPool> ();

            int carriedOver = Math.Max( maxPopulationSize / 5, 1);

            if (sourceGeneration.Count > carriedOver)
            {
                for(int i = 0; i < population.Count && i < carriedOver; i++)
                {
                    NeuronPool newPool = DeepCopyPool(sourceGeneration[i]);
                    nextGeneration.Add(newPool);
                }
            } else
            {
                foreach (var network in sourceGeneration)
                {
                    NeuronPool newPool = DeepCopyPool(network);
                    nextGeneration.Add(newPool);
                }
            }

            carriedOver = nextGeneration.Count;
            int leftToAdd = maxPopulationSize - carriedOver;

            for (int i = 0; i < leftToAdd; i++)
            {
                int randomSelection = random.Next(carriedOver);
                NeuronPool newPool = DeepCopyPool(sourceGeneration[randomSelection]);
                newPool.MutateConnections(random, mutationsToMake, weightMutationMultiplier, deleteChance);

                nextGeneration.Add(newPool);
            }

            return nextGeneration;
        }

        public NeuronPool DeepCopyPool(NeuronPool currentPool)
        {
            NeuronPool newPool = new NeuronPool();
            newPool.MutationCounter = currentPool.MutationCounter;
            newPool.addedConnectionCounter = currentPool.addedConnectionCounter;
            newPool.removedConnectionCounter = currentPool.removedConnectionCounter;
            newPool.GenerationCounter = currentPool.GenerationCounter + 1;

            foreach (var neuron in currentPool.NeuronSet)
            {
                if (neuron is ProbeNeuron)
                    continue;

                if (neuron is ComputeNeuron)
                {
                    ComputeNeuron computeNeuron = new ComputeNeuron();
                    computeNeuron.Axon.Weight = neuron.Axon.Weight;

                    newPool.NeuronSet.Add(computeNeuron);
                    continue;
                }

                if (neuron is MemoryNeuron)
                {
                    MemoryNeuron memoryNeuron = new MemoryNeuron();
                    memoryNeuron.Axon.Weight = neuron.Axon.Weight;

                    newPool.NeuronSet.Add(memoryNeuron);
                    continue;
                }
            }

            foreach (var neuron in currentPool.ReadProbes)
            {
                ProbeNeuron probeNeuron = new ProbeNeuron();

                probeNeuron.Axon.Weight = neuron.Axon.Weight;
                newPool.NeuronSet.Add(probeNeuron);
                newPool.ReadProbes.Add(probeNeuron);
            }

            foreach (var neuron in currentPool.ActivationProbes)
            {
                ProbeNeuron probeNeuron = new ProbeNeuron();

                probeNeuron.Axon.Weight = neuron.Axon.Weight;
                newPool.NeuronSet.Add(probeNeuron);
                newPool.ActivationProbes.Add(probeNeuron);
            }

            for(int i = 0; i <  currentPool.NeuronSet.Count; i++)
            {
                foreach (var connectTo in currentPool.NeuronSet[i].Axon.ConnectedTo.Keys)
                {
                    newPool.NeuronSet[i].Axon.ConnectTo(connectTo, newPool.NeuronSet[connectTo]);
                }
            }

            return newPool;
        }

    }
}
