using System.Text;
using UnityEngine;

public class NeuralNetworkTester : MonoBehaviour
{
    [SerializeField]
    private int epoches = 100;
    [SerializeField]
    private int[] layers = {11, 50, 20, 6};

    public void ButtonCreateNetwork()
    {
        NeuralNetwork existingNetwork = FindObjectOfType<NeuralNetwork>();
        if (existingNetwork != null)
        {
            Destroy(existingNetwork.gameObject);
        }

        // Create a new GameObject for the neural network
        GameObject nnObject = new GameObject("NeuralNetwork");
        NeuralNetwork neuralNetwork = nnObject.AddComponent<NeuralNetwork>();

        neuralNetwork.Initialize(layers);
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

    public void ButtonTestNetwork()
    {
        /*
        // Provide input values
        double[] inputValues = { 0.5, 0.3 }; // Example input values

        // Pass input through the network
        double[] output = neuralNetwork.CalculateOutputs(inputValues);

        // Display the output
        Display(output);
        */
        NeuralNetwork neuralNetwork = FindObjectOfType<NeuralNetwork>();
        if(neuralNetwork != null){
            neuralNetwork.ReadFromCSV("C:/Users/kubas/Downloads/winequality-red.csv");
            neuralNetwork.TrainNetwork(epoches);
        }
    }
}
