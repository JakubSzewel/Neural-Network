using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public double weight;
    //public Neuron inputNeuron;
    public Neuron previousNeuron;
    public Neuron nextNeuron;
    double deltaSum = 0;
    public Connection(Neuron _previousNeuron, Neuron _nextNeuron){
        previousNeuron = _previousNeuron;
        nextNeuron = _nextNeuron;
        weight = Random.Range(-1f, 1f);
    }

    public void addDelta(double delta){
        deltaSum += delta * previousNeuron.getValue();
    }

    public void learn(double step){
        weight += step * deltaSum; // +?
        deltaSum = 0;
    }
}
