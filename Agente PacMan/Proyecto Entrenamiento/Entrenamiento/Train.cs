using System;
using System.Diagnostics;

namespace Entrenamiento
{
    class Train
    {
        static void Main(string[] args)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork();
            neuralNetwork.Constructor(new int[] { 8, 5, 4 });
            string pathWeights = "../../../../../Assets/Files/Weights.txt";
            string[] text = System.IO.File.ReadAllLines("../../../../../Assets/Files/TrainingSet.txt");
            float[] input = new float[8];
            float[] output = new float[4];
            int epoch = 0;
            int batch;
            

            neuralNetwork.Load(pathWeights);
            ///*
            do
            {
                batch = 0;
                foreach (string line in text)
                {
                 
                    if (!line.Equals("")) { 
                    string[] values = line.Split(" ");
                    
                    for (int index = 0; index < values.Length; ++index)
                    {
                        if (index < 8)
                            input[index] = float.Parse(values[index]);
                        else
                            output[index % 8] = float.Parse(values[index]);
                        //System.Console.WriteLine("Output: " + output[0] + " " + output[1] + " " + output[2] + " " + output[3] + "\n");
                    }
                    //System.Console.WriteLine("wenas");
                    //System.Console.WriteLine("Output: " + input[0] + " " + input[1] + " " + input[2] + " " + input[3] + " " +  input[4] + " " + input[5] + " " + input[6] + " " + input[7] + "\n");
                    //System.Console.WriteLine("Epoch: " + epoch + ", Batch: " + batch + "\n");
                    if (output[0] != 0 || output[1] != 0 || output[2] != 0 || output[3] != 0 )
                        neuralNetwork.BackPropagate(input, output);
                    ++batch;
                    }
                }
                if (epoch % 50 == 0) {
                    
                    System.Console.WriteLine("Epoch: " + epoch + ", Batch: " + batch + "\n");
                    
                }
                if (epoch % 5000 == 0)
                {

                    neuralNetwork.Save(pathWeights);
                    System.Console.WriteLine("***********Pesos guardados*************** ");
                }

                batch = 0;
                ++epoch;
            } while (epoch < 5000);
            neuralNetwork.Save(pathWeights);
            //*/
            neuralNetwork.Load(pathWeights);
            float[] test = neuralNetwork.FeedForward(new float[] { 2, 0, 1, 1, 0, 1 ,1,1});
            System.Console.WriteLine("Prueba: " + test[0] + " " + test[1] + " " + test[2] + " " + test[3] + "\n");
        }
    }
}
