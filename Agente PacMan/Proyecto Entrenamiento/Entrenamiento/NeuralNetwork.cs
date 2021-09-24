using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class NeuralNetwork
{
    private readonly System.Random _random = new System.Random();
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights;
    private int[] activations;

    public float learningRate = 0.01f;//learning rate
    public float cost = 0;

    private float[][] deltaBiases;//biasses
    private float[][][] deltaWeights;//weights
    private int deltaCount;

    public float fitness = 0;

    public NeuralNetwork()
    {
    }

    //En unity no se usa new, hay que hacer GameObject.AddComponent(), esto no permite pasar
    //Param en el constructor, por eso se necesita este metodo
    public void Constructor(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int index = 0; index < layers.Length; ++index)
        {
            this.layers[index] = layers[index];
        }
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    private void InitNeurons()
    {
        List<float[]> neuronList = new List<float[]>();
        for (int index = 0; index < layers.Length; ++index)
        {
            neuronList.Add(new float[layers[index]]);
        }
        neurons = neuronList.ToArray();
    }

    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for (int index = 0; index < layers.Length; ++index)
        {
            float[] bias = new float[layers[index]];
            for (int index2 = 0; index2 < layers[index]; ++index2)
            {
                //bias[index2] = Convert.ToSingle(_random.NextDouble() - 0.5); //Analizar cambio sesgos iniciales
                bias[index2] = 0; //Analizar cambio sesgos iniciales
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int index = 1; index < layers.Length; ++index)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[index - 1];
            foreach (var item in neurons[index])
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int index2 = 0; index2 < neuronsInPreviousLayer; index2++)
                {
                    neuronWeights[index2] = Convert.ToSingle(_random.NextDouble() - 0.5); //Analizar cambio de pesos iniciales
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = activate(value + biases[i][j]);
            }
        }
        return neurons[neurons.Length - 1];
    }

    private float activate(float x)
    {
        return (float)Math.Tanh(x);
    }

    public float activateDer(float x)
    {
        return 1 - (x * x);
    }

    public void BackPropagate(float[] inputs, float[] expected)
    {
        float[] output = FeedForward(inputs);

        cost = 0;
        for (int i = 0; i < output.Length; i++) cost += (float)Math.Pow(output[i] - expected[i], 2);
        cost = cost / 2;

        float[][] gamma;


        List<float[]> gammaList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            gammaList.Add(new float[layers[i]]);
        }
        gamma = gammaList.ToArray();

        int layer = layers.Length - 2;
        for (int i = 0; i < output.Length; i++) gamma[layers.Length - 1][i] = (output[i] - expected[i]) * activateDer(output[i]);
        for (int i = 0; i < layers[layers.Length - 1]; i++)
        {
            biases[layers.Length - 2][i] -= gamma[layers.Length - 1][i] * learningRate;
            for (int j = 0; j < layers[layers.Length - 2]; j++)
            {

                weights[layers.Length - 2][i][j] -= gamma[layers.Length - 1][i] * neurons[layers.Length - 2][j] * learningRate;
            }
        }

        for (int i = layers.Length - 2; i > 0; i--)
        {
            layer = i - 1;
            for (int j = 0; j < layers[i]; j++)
            {
                gamma[i][j] = 0;
                for (int k = 0; k < gamma[i + 1].Length; k++)
                {
                    gamma[i][j] += gamma[i + 1][k] * weights[i][k][j];
                }
                gamma[i][j] *= activateDer(neurons[i][j]);
            }
            for (int j = 0; j < layers[i]; j++)
            {
                biases[i - 1][j] -= gamma[i][j] * learningRate;
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    weights[i - 1][j][k] -= gamma[i][j] * neurons[i - 1][k] * learningRate;
                }
            }
        }
    }

    public void Load(string path)
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }

    public void Save(string path)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }
}