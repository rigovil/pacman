using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Train
{
    public static void Main()
    {
        NeuralNetwork neuralNetwork = new NeuralNetwork();
        neuralNetwork.Constructor(new int[] { 8, 5, 4 });
        string pathWeights = "C:/Users/rodri/Desktop/UCR/I-2021/CI-0129/Entrenamiento/Training/Entrenamiento/ConsoleApp1/Weights.txt";
        string[] text = System.IO.File.ReadAllLines("C:/Users/rodri/Desktop/UCR/I-2021/CI-0129/Entrenamiento/Training/Entrenamiento/ConsoleApp1/TrainingSet.txt");
        float[] input = new float[8];
        float[] output = new float[4];
        int epoch = 0;
        int batch;
        do
        {
            batch = 0;
            foreach (string line in text)
            {
                string[] values = line.Split(' ');
                for (int index = 0; index < values.Length; ++index)
                {
                    if (index < 8)
                        input[index] = float.Parse(values[index]);
                    else
                        output[index % 4] = float.Parse(values[index]);
                }
                //System.Console.WriteLine("Epoch: " + epoch + ", Batch: " + batch + "/n");
                neuralNetwork.BackPropagate(input, output);
                ++batch;
            }
            ++epoch;
        } while (epoch < 100);
        neuralNetwork.Save(pathWeights);
        //neuralNetwork.Load(pathWeights);
        //float[] test = neuralNetwork.FeedForward(new float[] { 19, 15, 19, 16, 1f, 0f, 1f, 0f, 0.0f, 0.0f });
        //System.Console.WriteLine("Prueba: " + test[0] + " " + test[1] + " " + test[2] + " " + test[3] + "/n");
        //test = neuralNetwork.FeedForward(new float[] { 19, 14, 19, 13, 1f, 0f, 1f, 1f, 0.0f, 0.0f });
        //System.Console.WriteLine("Prueba: " + test[0] + " " + test[1] + " " + test[2] + " " + test[3] + "/n");
        //System.Console.ReadLine();
    }
}
