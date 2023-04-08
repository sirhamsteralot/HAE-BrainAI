using IAIBrainAI.Network;

namespace IAIBrainAI
{
    internal class Program
    {
        static NeuronPool neuronPool = new NeuronPool();

        static void Main(string[] args)
        {
            Console.WriteLine("IAIBRAINAI INITIALIZING!");

            int seed = 123173;
            Random random = new Random(seed);

            var activationlist = new List<float> { 1, 1};
            var outputList = new List<float> { 1 };

            neuronPool.InitializeTopology(10);
            neuronPool.AddProbes(1, activationlist.Count);
            neuronPool.RandomConnectionStep(random, 150);

            GeneticTrainer genTrain = new GeneticTrainer(neuronPool);

            genTrain.TrainLoop(activationlist, outputList, 1000);

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
        }

        static void PrintOutputs()
        {
            var outputSnapshot = neuronPool.OutputSnapshot();
            for (int i = 0; i < outputSnapshot.Count; i++)
                Console.WriteLine($"outputvalue: {outputSnapshot[i]}");
        }
    }
}