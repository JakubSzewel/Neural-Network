using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class NeuralNetwork
{
    private List<Data> data = new List<Data>();
    private List<double> costs = new List<double>();
    public Layer[] layers;
    private int batchSize = 10; // Serializable
    private int step = 3; // Serializable
    private int trainingIteration = 0;
    public NeuralNetwork(params int[] layerSizes){
        if (layerSizes.Length < 2){
            Debug.LogError("Too few layers created");
            return;
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

    public double[] CalculateOutputs(List<double> inputValues)
    {
        // Set input values on to the input layer
        for (int i = 0; i < inputValues.Count; i++)
        {
            layers[0].neurons[i].value = inputValues[i];
        }

        double[] output = new double[layers[layers.Length - 1].numNeurons];
        for (int i = 0; i < layers[layers.Length - 1].numNeurons; i++){
            output[i] = layers[layers.Length - 1].neurons[i].getValue();
        }
        return output;
    }

    public double GetCost(double[] output, List<double> expected){
        if (output.Length == expected.Count)
        {
           double sum = 0;
            for (int i = 0; i < output.Length; i++)
            {
                sum += Math.Pow((expected[i] - output[i]), 2) / (2 * output.Length);
            }
            return sum; 
        }
        else
        {
            Debug.LogError("Length of output vector is not equal to the length of expected vector");
            return 0;
        }
    }

    public void Train(List<double> inputValues, List<double> expected){ // Add input?
        trainingIteration++;

        double[] output;
        double cost;

        // Calculate values
        output = CalculateOutputs(inputValues);

        // Calculates delta for the output layer
        for (int i = 0; i < expected.Count; i++){
            layers[layers.Length-1].neurons[i].setOutputDelta(expected[i]);
        }

        // Cost
        cost = GetCost(output, expected);

        // back propagate the deltas
        for (int i = 0; i < layers[0].numNeurons; i++){
            layers[0].neurons[i].getDelta(); // Add it to the layers class?
        }

        // ???
        if (trainingIteration % batchSize == 0){
            for (int i = 0; i < layers.Length; i++){
                layers[i].learn((double)step/batchSize);
            }
        }

        costs.Add(cost);
    }

    public void TrainNetwork(int epoches){
        //double startTime = Time.time * 1000;
        for (int i = 0; i < epoches; i++){
            for (int j = 0; j < data.Count; j++)
            {
                List<double> input = data[j].getInput();
                List<double> expected = data[j].getExpected();
                Train(input, expected);
                if (trainingIteration%100==0){
                    Debug.Log("Cos: " + j + " Cost: " + getAverageCost());
                }
            }
            
            //Debug.Log("Train time: " + (double)(Time.time*1000 - startTime));
        }
    }

    public void ReadFromCSV(string path){
        StreamReader reader = null;
        if(File.Exists(path)){
            reader = new StreamReader(File.OpenRead(path));
            string skip = reader.ReadLine();
            while (!reader.EndOfStream){
                List<double> list = new List<double>();
                var line = reader.ReadLine();
                var values = line.Split(',');
                foreach(var item in values){
                    try{
                        list.Add(double.Parse(item, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    catch (Exception e){
                        Debug.Log(e);
                    }
                }

                Data dataTemp = new Data();

                for (int i = 0; i < values.Length - 1; i++){
                    dataTemp.AddInput(list[i]);
                }
                dataTemp.AddExpected(list[values.Length-1]);
                data.Add(dataTemp);
            }

            // ????????????????
            NormalizeData();
        }
        else {
            //Error
        }
    }

   public void NormalizeData()
    {
        // Find min and max values for each feature (input and expected)
        List<double> mins = new List<double>();
        List<double> maxs = new List<double>();
        for (int i = 0; i < data[0].getInput().Count; i++)
        {
            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            foreach (Data entry in data)
            {
                minVal = Math.Min(minVal, entry.getInput()[i]);
                maxVal = Math.Max(maxVal, entry.getInput()[i]);
            }
            mins.Add(minVal);
            maxs.Add(maxVal);
        }

        // Normalize each input feature
        foreach (Data entry in data)
        {
            List<double> normalizedInputs = new List<double>();
            for (int i = 0; i < entry.getInput().Count; i++)
            {
                double normalizedValue = (entry.getInput()[i] - mins[i]) / (maxs[i] - mins[i]);
                normalizedInputs.Add(normalizedValue);
            }
            entry.setInput(normalizedInputs);
        }

        // Normalize the expected values
        double minExpected = double.MaxValue;
        double maxExpected = double.MinValue;
        foreach (Data entry in data)
        {
            foreach (double val in entry.getExpected())
            {
                minExpected = Math.Min(minExpected, val);
                maxExpected = Math.Max(maxExpected, val);
            }
        }
        foreach (Data entry in data)
        {
            List<double> normalizedExpected = new List<double>();
            foreach (double val in entry.getExpected())
            {
                double normalizedVal = (val - minExpected) / (maxExpected - minExpected);
                normalizedExpected.Add(normalizedVal);
            }
            entry.setExpected(normalizedExpected);
        }
    }



    public double getAverageCost(){
        double sum =0;
        for (int i = 0; i < costs.Count; i++) {
            sum += costs[i];
        }
        return sum / costs.Count;
    }
    
}