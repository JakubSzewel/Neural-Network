using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int numNeurons;
    public Neuron[] neurons;
    public Layer(int numNeurons){
        this.numNeurons = numNeurons;
        neurons = new Neuron[numNeurons];
        for(int i = 0; i < numNeurons; i++){
            neurons[i] = new Neuron();
        }
    }

    public void SetInputConnections(Layer previousLayer){
        foreach(Neuron neuron in neurons){
            for(int i = 0; i < previousLayer.numNeurons; i++) {
                neuron.connectionsIn.Add(new Connection(previousLayer.neurons[i], neuron));
            }
        }
    }
    public void learn(double step){
        for (int i = 0; i < numNeurons; i++){
            neurons[i].learn(step);
        }
    }
}
