using System;
using System.Collections.Generic;
using UnityEngine;

public class MLModel : MonoBehaviour
{
    private int inputSize;     // Size of the board state feature vector
    private int hiddenSize;    // Number of neurons in the hidden layer
    private int outputSize;    // Equal to the number of players

    private float[,] weights1; // Weights between input and hidden layer
    private float[] biases1;

    private float[,] weights2; // Weights between hidden and output layer
    private float[] biases2;

    private float learningRate = 0.01f;

    public MLModel(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        InitializeWeights();
    }

    private void InitializeWeights()
    {
        System.Random random = new System.Random();

        weights1 = new float[inputSize, hiddenSize];
        biases1 = new float[hiddenSize];
        weights2 = new float[hiddenSize, outputSize];
        biases2 = new float[outputSize];

        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                weights1[i, j] = (float)(random.NextDouble() * 2 - 1); // Random values between -1 and 1
            }
        }

        for (int i = 0; i < hiddenSize; i++)
        {
            biases1[i] = (float)(random.NextDouble() * 2 - 1);
            for (int j = 0; j < outputSize; j++)
            {
                weights2[i, j] = (float)(random.NextDouble() * 2 - 1);
            }
        }

        for (int i = 0; i < outputSize; i++)
        {
            biases2[i] = (float)(random.NextDouble() * 2 - 1);
        }
    }

    /// <summary>
    /// Predicts the payoff vector for all players given the game state.
    /// </summary>
    public float[] Predict(float[] inputFeatures)
    {
        float[] hiddenLayer = new float[hiddenSize];
        float[] outputLayer = new float[outputSize];

        // Forward pass: Input to hidden layer
        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenLayer[i] = biases1[i];
            for (int j = 0; j < inputSize; j++)
            {
                hiddenLayer[i] += inputFeatures[j] * weights1[j, i];
            }
            hiddenLayer[i] = ReLU(hiddenLayer[i]); // Activation function
        }

        // Forward pass: Hidden to output layer
        for (int i = 0; i < outputSize; i++)
        {
            outputLayer[i] = biases2[i];
            for (int j = 0; j < hiddenSize; j++)
            {
                outputLayer[i] += hiddenLayer[j] * weights2[j, i];
            }
        }

        return outputLayer; // Raw scores for each player
    }

    /// <summary>
    /// Trains the model using a single board state and its target payoffs.
    /// </summary>
    public void Train(float[] inputFeatures, float[] targetPayoffs)
    {
        float[] hiddenLayer = new float[hiddenSize];
        float[] outputLayer = new float[outputSize];

        // Forward pass
        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenLayer[i] = biases1[i];
            for (int j = 0; j < inputSize; j++)
            {
                hiddenLayer[i] += inputFeatures[j] * weights1[j, i];
            }
            hiddenLayer[i] = ReLU(hiddenLayer[i]);
        }

        for (int i = 0; i < outputSize; i++)
        {
            outputLayer[i] = biases2[i];
            for (int j = 0; j < hiddenSize; j++)
            {
                outputLayer[i] += hiddenLayer[j] * weights2[j, i];
            }
        }

        // Compute output layer error
        float[] outputError = new float[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            outputError[i] = outputLayer[i] - targetPayoffs[i]; // Error: prediction - target
        }

        // Backpropagation: Hidden to output
        float[,] gradWeights2 = new float[hiddenSize, outputSize];
        float[] gradBiases2 = new float[outputSize];
        float[] hiddenError = new float[hiddenSize];

        for (int i = 0; i < outputSize; i++)
        {
            gradBiases2[i] = outputError[i];
            for (int j = 0; j < hiddenSize; j++)
            {
                gradWeights2[j, i] = outputError[i] * hiddenLayer[j];
                hiddenError[j] += outputError[i] * weights2[j, i];
            }
        }

        // Backpropagation: Input to hidden
        float[,] gradWeights1 = new float[inputSize, hiddenSize];
        float[] gradBiases1 = new float[hiddenSize];

        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenError[i] *= ReLUDerivative(hiddenLayer[i]);
            gradBiases1[i] = hiddenError[i];
            for (int j = 0; j < inputSize; j++)
            {
                gradWeights1[j, i] = hiddenError[i] * inputFeatures[j];
            }
        }

        // Update weights and biases
        for (int i = 0; i < hiddenSize; i++)
        {
            biases1[i] -= learningRate * gradBiases1[i];
            for (int j = 0; j < inputSize; j++)
            {
                weights1[j, i] -= learningRate * gradWeights1[j, i];
            }
        }

        for (int i = 0; i < outputSize; i++)
        {
            biases2[i] -= learningRate * gradBiases2[i];
            for (int j = 0; j < hiddenSize; j++)
            {
                weights2[j, i] -= learningRate * gradWeights2[j, i];
            }
        }
    }

    // ReLU activation function
    private float ReLU(float x) => Mathf.Max(0, x);

    // ReLU derivative
    private float ReLUDerivative(float x) => x > 0 ? 1 : 0;
}
