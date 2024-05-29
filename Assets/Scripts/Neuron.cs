using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using static System.Math;

public class Neuron
{
    public double bias = 0;
    public List<Connection> connectionsIn;
    public double value;
    private double delta;
    private double deltaSum = 0;
    private double z;

    public Neuron(){
        connectionsIn = new List<Connection>();

        System.Random random = new System.Random();

        double a, b;
        a = random.NextDouble();
        b = random.NextDouble();
        bias = Sqrt(-2.0*Log(a)) * Cos(2.0*PI*b);
    }


    public void CalculateValue(){
        z = 0;
        for (int i = 0; i < connectionsIn.Count; i++){
            z += connectionsIn[i].previousNeuron.getValue() * connectionsIn[i].weight;
        }
        if (connectionsIn.Count > 0)
            z /= (double)connectionsIn.Count;
        z += bias;
        value = ActivationFunction(z);
    }

    public double getValue(){
        if (value == default(double)){
            CalculateValue();
        }
        return value;
    }

    public void setOutputDelta(double expected){
        delta = (value - expected) * ActivationDerivitive(z);
        for (int i = 0; i < connectionsIn.Count; i++){
            connectionsIn[i].addDelta(delta);
        }
        deltaSum += delta;
    }

    public double getDelta(){
        if (delta == default(double)){
            double sum = 0;
            foreach(Connection c in connectionsIn){
                sum += c.nextNeuron.getDelta() * c.weight;
            }
            delta = sum + ActivationDerivitive(z);
            for (int i = 0; i < connectionsIn.Count; i++){
                connectionsIn[i].addDelta(delta);
            }
            deltaSum += delta;
        }
        return delta;
    }

    public void learn(double step){
        if (delta != default(double)){
            bias -= step * deltaSum;
            for (int i = 0; i < connectionsIn.Count; i++){
                // Adjust weights
                connectionsIn[i].learn(step);
            }
            deltaSum = 0;
            delta = default(double);
        }
    }

    double ActivationFunction(double x){
        return 1 / (1 + Exp(-x)); // Sigmoid 
    }

    double ActivationDerivitive(double x){
        double a = ActivationFunction(x);
        return a * (1-a);
    }
}
