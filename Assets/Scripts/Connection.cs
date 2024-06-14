using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static System.Math;

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

        System.Random random = new System.Random();

        // https://www.wolframalpha.com/input?i=plot+sqrt%28-2.0*log%28a%29%29+*+cos%282.0*pi*b%29+for+a+from+0.0001+to+1%2C+b+from+0+to+1
        double a, b;
        a = random.NextDouble();
        b = random.NextDouble();
        weight = Sqrt(-2.0*Log(a)) * Cos(2.0*PI*b);
        //weight = 0.5;
    }

    public void addDelta(double delta){
        deltaSum += delta * previousNeuron.getValue();
    }
    
    public void learn(double step){
        weight -= step * deltaSum; // ??? +
        deltaSum = 0;
    }
}
