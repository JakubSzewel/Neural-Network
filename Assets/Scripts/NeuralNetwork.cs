using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    private List<Data> data = new List<Data>();
    private List<double> costs = new List<double>();
    public Layer[] layers;

    [SerializeField]
    private int batchSize = 10; // Number of iterations before the network learns
    [SerializeField]
    private double step = 0.5; // Defines how fast the network should learn

    private int trainingIteration = 0;
    public void Initialize(params int[] layerSizes){
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
        // Set the values in network to default
        foreach (var layer in layers)
        {
            if (layer.neurons[0].connectionsIn.Count > 0){
                foreach (var neuron in layer.neurons)
                {
                    neuron.value=default(double);
                }
            }
        }
        // Set input values on to the input layer
        for (int i = 0; i < inputValues.Count; i++)
        {
            layers[0].neurons[i].value = inputValues[i];
        }

        // Calculates the output using feeding forward
        double[] output = new double[layers[layers.Length - 1].numNeurons];
        for (int i = 0; i < layers[layers.Length - 1].numNeurons; i++){
            output[i] = layers[layers.Length - 1].neurons[i].getValue(true);
        }
        return output;
    }

    public double GetCost(double[] output, List<double> expected){
        if (output.Length == expected.Count)
        {
           double sum = 0;
            for (int i = 0; i < output.Length; i++)
            {
                sum += Math.Pow((expected[i] - output[i]), 2);
            }
            return sum; 
        }
        else
        {
            Debug.LogError("Length of output vector is not equal to the length of expected vector");
            return 0;
        }
    }

    public void Train(List<double> inputValues, List<double> expected){
        trainingIteration++;

        double[] output;
        double cost;

        // Calculate values on the output layer and applies SoftMax for the result
        output = SoftMax(CalculateOutputs(inputValues));

        // Calculate delta for the output layer
        for (int i = 0; i < expected.Count; i++){
            layers[layers.Length-1].neurons[i].setOutputDelta(expected[i]);
        }

        // Calculate cost - difference between the correct answer
        cost = GetCost(DenormalizeOutput(output), DenormalizeOutput(expected));

        

        // Vack propagate the deltas
        for (int i = 0; i < layers[0].numNeurons; i++){
            layers[0].neurons[i].getDelta();
        }

        // After the amount of iterations set in batchSize the network learns
        if (trainingIteration % batchSize == 0){
            for (int i = 0; i < layers.Length; i++){
                layers[i].learn((double)step/batchSize);
            }
        }

        // Added to the average costs
        costs.Add(cost);
    }

    private double[] SoftMax(double[] array){ // Makes the answer percent based
        double max = array.Max();
        double sum = array.Sum(val => Math.Exp(val - max));
        return array.Select(val => Math.Exp(val - max) / sum).ToArray();
    }

    public void TrainNetwork(int epoches){
        float startTime = Time.realtimeSinceStartup;

        for (int i = 0; i < epoches; i++){
            for (int j = 0; j < data.Count; j++)
            {
                List<double> input = data[j].getInput();
                List<double> expected = data[j].getExpected();

                // Train on the single data
                Train(input, expected);
            }
        }
        
        // Time measurement
        float endTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Train time: " + (int)(endTime/60) + "m " + endTime +"s");

        // Test result for the network
        TestNetwork();
    }

      public void TestNetwork(){
        int goodGuesses = 0;

        for(int i =0; i<data.Count;i++){
                double[] output;
                List<double> input = data[i].getInput();
                List<double> expected = data[i].getExpected();
                output = SoftMax(CalculateOutputs(input));
                double max = -1000.0;
                int eMax = -1;
                int oMax = -1;

                // Checks if the guess made by the network is correct
                for (int j = 0; j < 6; j++) {
                    if (output[j] > max){
                        max = output[j];
                        oMax = j;
                    }
                    if (expected[j] == 1.0){
                        eMax = j;
                    }
                }
                if(oMax == eMax)
                    goodGuesses++;
        }

        Debug.Log("Number of correct guesses: " + goodGuesses);
        Debug.Log("Percentage of correct guesses: " + Math.Round((double)goodGuesses/(double)data.Count,3)*100 + "%");
    }

    public void ReadFromCSV(string path){ // Reads the file and adds the data to the list
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
                dataTemp.AddExpected((int)list[values.Length-1]);
                data.Add(dataTemp);
            }

            // Normalizes the data so the values fit between 0 and 1
            NormalizeData();
        }
        else {
            //Error
        }
    }

    public double getAverageCost(){
        double sum =0;
        for (int i = 0; i < costs.Count; i++) {
            sum += costs[i];
        }
        double result = sum / costs.Count;
        costs.Clear();
        return result;
    }

    // ######################################
    // Data normalization and denormalization
    // ######################################

    double minExpected, maxExpected;

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
        minExpected = double.MaxValue;
        maxExpected = double.MinValue;
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

    public double[] DenormalizeOutput(double[] normalizedOutput)
    {
        double[] denormalizedOutput = new double[normalizedOutput.Length];
        int i = 0;
        foreach (double normalizedVal in normalizedOutput)
        {
            double denormalizedVal = normalizedVal * (maxExpected - minExpected) + minExpected;
            denormalizedOutput[i] = denormalizedVal;
            i++;
        }
        return denormalizedOutput;
    }

    public List<double> DenormalizeOutput(List<double> normalizedOutput)
    {
        List<double> denormalizedOutput = new List<double>();
        foreach (double normalizedVal in normalizedOutput)
        {
            double denormalizedVal = normalizedVal * (maxExpected - minExpected) + minExpected;
            denormalizedOutput.Add(denormalizedVal);
        }
        return denormalizedOutput;
    }
    
}