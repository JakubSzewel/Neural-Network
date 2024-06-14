using System;
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
    public List<Connection> connectionsOut;
    public double value;
    private double delta;
    private double deltaSum = 0;
    private double z;

    public Neuron(){
        connectionsIn = new List<Connection>();
        connectionsOut = new List<Connection>();
        System.Random random = new System.Random();

        // https://www.wolframalpha.com/input?i=plot+sqrt%28-2.0*log%28a%29%29+*+cos%282.0*pi*b%29+for+a+from+0.0001+to+1%2C+b+from+0+to+1
        double a, b;
        a = random.NextDouble();
        b = random.NextDouble();
        bias = Sqrt(-2.0*Log(a)) * Cos(2.0*PI*b);
    }


    public void CalculateValue(){
        z = 0;
        for (int i = 0; i < connectionsIn.Count; i++){
            // Calculating the value recursively
            z += connectionsIn[i].previousNeuron.getValue(false) * connectionsIn[i].weight;
        }
        z += bias;
    }

    public double getValue(bool isOutputLayer = false){
        // If the value is not yet calculated (the values on the input layer are known already)
        if (connectionsIn.Count > 0 && value == Double.MinValue){
            CalculateValue();
            if (!isOutputLayer)
                value = ActivationFunction(z);
            else
                value = z;
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
        if (delta == Double.MinValue){
            double sum = 0;
            foreach(Connection c in connectionsOut){
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
        if (delta != Double.MinValue){
            if (connectionsIn.Count > 0) {
                bias -= step * deltaSum;
                for (int i = 0; i < connectionsIn.Count; i++){
                    // Adjust weights
                    connectionsIn[i].learn(step);
                }
            }
            deltaSum = 0;
            delta = Double.MinValue;
        }
    }

    double ActivationFunction(double x){
        return SigmoidFunction(x);
    }

    double SigmoidFunction(double x){
        return 1 / (1 + Exp(-x)); // Sigmoid
    }

    double ReLUFunction(double x){
        return Max(0.0,x); // ReLU
    }

    double ActivationDerivitive(double x){
        double a = ActivationFunction(x);
        return a * (1-a);
    }
}
