using System.Text;
using UnityEngine;

public class NeuralNetworkTester : MonoBehaviour
{
    NeuralNetwork neuralNetwork;

    public void Click()
    {
        // Define your neural network architecture
        neuralNetwork = new NeuralNetwork(2, 1000, 1000, 5);

        // Test the network
        TestNetwork();
    }

    void Display(double[] doubles){
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < doubles.Length; i++)
        {
            sb.Append(doubles[i]);
            sb.Append(" ");
        }
        Debug.Log(sb.ToString());
    }

    void TestNetwork()
    {
        // Provide input values
        double[] inputValues = { 0.5, 0.3 }; // Example input values

        // Pass input through the network
        double[] output = neuralNetwork.CalculateOutputs(inputValues);

        // Display the output
        Display(output);
    }
}
