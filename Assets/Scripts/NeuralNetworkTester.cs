using System.Text;
using UnityEngine;

public class NeuralNetworkTester : MonoBehaviour
{
    [SerializeField]
    private int epoches = 1; // Number of times it goes through the whole data set in learning
    [SerializeField]
    private int[] layers = {11, 50, 20, 6}; // Number of layers (First must be 11, and last 6 for this dataset)

    public void ButtonCreateNetwork()
    {
        NeuralNetwork existingNetwork = FindObjectOfType<NeuralNetwork>();
        // Remove existing network
        if (existingNetwork != null)
        {
            Destroy(existingNetwork.gameObject);
        }

        // Create new network
        GameObject nnObject = new GameObject("NeuralNetwork");
        NeuralNetwork neuralNetwork = nnObject.AddComponent<NeuralNetwork>();

        neuralNetwork.Initialize(layers);
    }

    public void ButtonTestNetwork()
    {
        NeuralNetwork neuralNetwork = FindObjectOfType<NeuralNetwork>();
        if(neuralNetwork != null){
            neuralNetwork.ReadFromCSV("C:/Users/kubas/Downloads/winequality-red.csv");
            neuralNetwork.TrainNetwork(epoches);
        }
    }
}
