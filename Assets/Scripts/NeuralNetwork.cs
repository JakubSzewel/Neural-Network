using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    public Layer[] layers;
    public NeuralNetwork(params int[] layerSizes){
        if (layerSizes.Length < 2){
            // Error
        }
        // Create layers
        layers = new Layer[layerSizes.Length];
        for (int i = 0; i < layers.Length; i++){
            layers[i] = new Layer(layerSizes[i]);
        }
        // Estabilish connections
        for (int i = 1; i < layers.Length; i++){
            layers[i].SetInputConnections(layers[i-1]);
        }
    }

    public double[] CalculateOutputs(double[] inputValues)
    {
        // Set input values to the input layer
        for (int i = 0; i < inputValues.Length; i++)
        {
            layers[0].neurons[i].value = inputValues[i];
        }

        double[] output = new double[layers[layers.Length - 1].numNeurons];
        for (int i = 0; i < layers[layers.Length - 1].numNeurons; i++){
            output[i] = layers[layers.Length - 1].neurons[i].getValue();
        }
        return output;
    }
}
