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

    public void SetInputConnections(Layer previousLayer){ // Creating the connections between neurons
        foreach(Neuron neuron in neurons){
            for(int i = 0; i < previousLayer.numNeurons; i++) {
                Connection newConnection = new Connection(previousLayer.neurons[i], neuron);
                neuron.connectionsIn.Add(newConnection);
                previousLayer.neurons[i].connectionsOut.Add(newConnection);
            }
        }
    }
    public void learn(double step){ // Making the neurons learn
        for (int i = 0; i < numNeurons; i++){
            neurons[i].learn(step);
        }
    }
}
