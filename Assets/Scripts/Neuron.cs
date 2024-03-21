using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class Neuron
{
    public double bias = 0;
    public List<Connection> connectionsIn;
    public double value;

    public Neuron(){
        connectionsIn = new List<Connection>();
    }


    public void CalculateValue(){
        for (int i = 0; i < connectionsIn.Count; i++){
            value += connectionsIn[i].outputNeuron.getValue() * connectionsIn[i].weight;
        }
        value = ActivationFunction(value + bias);
    }

    public double getValue(){
        if (value == default(double)) {
            CalculateValue();
        }
        return value;
    }

    double ActivationFunction(double weightedInput){
        return 1 / (1 + Exp(-weightedInput)); // Sigmoid 
    }
}
