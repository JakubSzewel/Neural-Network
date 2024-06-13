using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public List<double> input = new List<double>();
    public List<double> expected = new List<double>();

    public void AddInput(double inputValue){
        input.Add(inputValue);
    }

    public void AddExpected(int expectedValue){
        double[] oneHot = new double[6];
        oneHot[expectedValue - 3] = 1;
        expected.AddRange(oneHot);
    }

    public List<double> getInput(){
        return input;
    }

    public List<double> getExpected(){
        return expected;
    }

    public void setInput(List<double> list){
        input = list;
    }

    public void setExpected(List<double> list){
        expected = list;
    }
}
