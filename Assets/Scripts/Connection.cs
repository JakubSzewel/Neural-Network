using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public double weight;
    //public Neuron inputNeuron;
    public Neuron outputNeuron;

    public Connection(Neuron _outputNeuron){
        outputNeuron = _outputNeuron;
        weight = Random.Range(-1f, 1f);
    }
}
