using IAIBrainAI.Network;

namespace IAIBrainAI
{
    internal class Program
    {
        static NeuronPool neuronPool = new NeuronPool();

        static void Main(string[] args)
        {
            Console.WriteLine("IAIBRAINAI INITIALIZING!");

            int seed = 12311273;
            Random random = new Random();

            var activationlist = new List<List<float>> 
            { 
                new List<float> { 1, 1 },
                new List<float> { 1, 0 },
                new List<float> { 0, 1 },
                new List<float> { 0, 0 },
            };
            var outputList = new List<List<float>> { 
                new List<float> { 0 },
                new List<float> { 1 },
                new List<float> { 1 },
                new List<float> { 0 },
            };

            neuronPool.InitializeTopology(10);
            neuronPool.AddProbes(1, 2);
            neuronPool.RandomConnectionStep(random, 25);

            GeneticTrainer genTrain = new GeneticTrainer(neuronPool);

            genTrain.TrainLoop(activationlist, outputList, 100);

            //PrintOutputs();
            //int cycleNr = 0;
            //neuronPool.Activate(activationlist);

            //while (true)
            //{
            //    cycleNr++;

            //    neuronPool.PushNeurons();

            //    if (cycleNr % 10 == 0)
            //    {
            //        Console.WriteLine($"Mutating connections {cycleNr} ....");
            //        neuronPool.MutateConnections(random, 10, 1, 0.2f);
            //    }
            //    if (cycleNr % 100 == 0)
            //    {
            //        Console.WriteLine($"<===========================[ {cycleNr} ]===========================>");
            //        PrintOutputs();
            //        Console.ReadLine();
            //    }
            //}

            for (int i = 0; i < activationlist.Count; i++)
            {
                Console.WriteLine($"<===========================[ {i} ]===========================>");
                Console.Write($"Inputs: [ ");

                foreach (var input in activationlist[i])
                    Console.Write($"{input} ");

                Console.WriteLine("]");

                genTrain.population[0].Activate(activationlist[i]);

                for (int j = 0; j < genTrain.runsToProcess; j++)
                {
                    genTrain.population[0].PushNeurons();
                }

                PrintOutputs(genTrain.population[0]);
            }
        }

        static void PrintOutputs(NeuronPool pool)
        {
            var outputSnapshot = pool.OutputSnapshot();
            for (int i = 0; i < outputSnapshot.Count; i++)
                Console.WriteLine($"outputvalue: {outputSnapshot[i]}");
        }
    }
}