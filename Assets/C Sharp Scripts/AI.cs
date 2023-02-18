using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <note>
/// ================== Multilayer perceptron ==========================
///
/// Layers
///
/// ========================== Neuron =================================
///
///      - thing that hold a number between 0 and 1
///      - A neuron is a function - takes in the outputs of all the previous layer and spits out a number between 0 and 1.
///
/// ===================== First Layer/Input Layer =====================
///
///      - Chess input neurons are 64 (8x8 grid)
///      - an (input neuron)(activation)(should be 0-1) Chess hold the value of a piece. (white queen = 9, black rook = -5, empty square = 0)
///
/// ================ Last Layer/Output Layer/Move to make =============
///
///      - 64 squares and 32 pieces equals 2048 unique moves that could be made.
///
///      - Think of all possible unique moves. Castling. Each bishops can only ever move on one colour.
///
/// ======================== Hidden Layer =============================
///
///      - All the connects in the hidden layer could be imagined as an image. Its image is made up of all the previous weights going into it arranged in a matrix. Colour the weights 
///
/// =========================== Connection ============================
///
///      - Weights are attached to the connection of neurons between layers
///      - These weights are just numbers.
///      - The weight represent the strength of that connection
///
/// ========================== Sigmoid & Bias =========================
/// 
///      - Squashes numbers in a range of between 0-1
///      - Uses bias to control how active the neuron becomes before sigmoid squash of range.
///
///      sigmoid([weight matrix] * [activation vector] + [bias vector])
///
/// ============================ Learning =============================
/// 
///      - Will it always choose a legal move? (no) Does it know what a legal move is? (no)
///      - Gradient descent.
///      - Find the minima of the function.
///
///      - Learning is changing the setting for all these numbers (weights)(biases) to achieve the correct result
///      - Cost function is used. Cost of a single training exercise is adding up the squares of the differences between the out layer and expected results.
///              The cost will be small when the network classifies the example correctly. But large when it is unsure. Evaluate how well the network is working
///              You don't allow one training example to effect the network. It must be the average of all costs.
///              - Cost function take in all the weights and biases and output a single score representing the success of network.
///              - FUNCTION_COST Use calculas to find the minimum cost of the function
///
///      - Should be self taught. Learning from randomness. should be better. But take longer to reach the minimum cost.
///
///      or
///
///      - training should involve one move. not an entire games. board set > move checked against expected move done by a pro.
/// 
/// ===================== What the AI "Sees" / Input ==================
/// 
/// [-5][-3][-3][-9][-∞][-3][-3][-5]
/// [-1][-1][-1][-1][-1][-1][-1][-1]
/// [  ][  ][  ][  ][  ][  ][  ][  ]
/// [  ][  ][  ][  ][  ][  ][  ][  ]
/// [  ][  ][  ][  ][  ][  ][  ][  ]
/// [  ][  ][  ][  ][  ][  ][  ][  ]
/// [ 1][ 1][ 1][ 1][ 1][ 1][ 1][ 1]
/// [ 5][ 3][ 3][ 9][ ∞][ 3][ 3][ 5]
/// 
/// </note>
public class AI : MonoBehaviour // IComparable<AI>
{
    // AI states

    public bool hasTurn = false;
    public bool isWhite = false;

    // The board the AI "sees".

    public float[] input = new float[NUMBER_OF_SQUARES];

    // Constants

    public static int NUMBER_OF_HIDDEN_LAYERS = 1;
    public static int NUMBER_OF_NEURONS_IN_LAYER = 10000;
    public static int NUMBER_OF_SQUARES = 64;
    public static int NUMBER_OF_PIECES = 32;

    NeuralNetwork net;

    //==========================================
    // Example
    //int[] layers = new int[3] { 5, 4, 3 };
    //int[] layers = new int[3] { "input neurons", "Hidden layer neurons", "output neurons" }
    //
    // input  hidden  output
    //  []  
    //  []      []      []
    //  []      []      []
    //  []      []      []
    //  []      []
    //===========================================

    int[] layers = new int[3] { 3, 5, 1 };
    string[] activation = new string[2] { "leakyrelu", "leakyrelu" };

    void Start()
    {
        this.net = new NeuralNetwork(layers, activation);
        for (int i = 0; i < 100000; i++)
        {
            net.BackPropagate(new float[] { 0, 0, 0 }, new float[] { 0 });
            net.BackPropagate(new float[] { 1, 0, 0 }, new float[] { 1 });
            net.BackPropagate(new float[] { 0, 1, 0 }, new float[] { 1 });
            net.BackPropagate(new float[] { 0, 0, 1 }, new float[] { 1 });
            net.BackPropagate(new float[] { 1, 1, 0 }, new float[] { 1 });
            net.BackPropagate(new float[] { 0, 1, 1 }, new float[] { 1 });
            net.BackPropagate(new float[] { 1, 0, 1 }, new float[] { 1 });
            net.BackPropagate(new float[] { 1, 1, 1 }, new float[] { 1 });
        }
        print("cost: " + net.cost);

        UnityEngine.Debug.Log(" 0, 0, 0 " + net.FeedForward(new float[] { 0, 0, 0 })[0]);
        UnityEngine.Debug.Log(" 1, 0, 0 " + net.FeedForward(new float[] { 1, 0, 0 })[0]);
        UnityEngine.Debug.Log(" 0, 1, 0 " + net.FeedForward(new float[] { 0, 1, 0 })[0]);
        UnityEngine.Debug.Log(" 0, 0, 1 " + net.FeedForward(new float[] { 0, 0, 1 })[0]);
        UnityEngine.Debug.Log(" 1, 1, 0 " + net.FeedForward(new float[] { 1, 1, 0 })[0]);
        UnityEngine.Debug.Log(" 0, 1, 1 " + net.FeedForward(new float[] { 0, 1, 1 })[0]);
        UnityEngine.Debug.Log(" 1, 0, 1 " + net.FeedForward(new float[] { 1, 0, 1 })[0]);
        UnityEngine.Debug.Log(" 1, 1, 1 " + net.FeedForward(new float[] { 1, 1, 1 })[0]);
        //We want the gate to simulate 3 input or gate (A or B or C)
        // 0 0 0    => 0
        // 1 0 0    => 1
        // 0 1 0    => 1
        // 0 0 1    => 1
        // 1 1 0    => 1
        // 0 1 1    => 1
        // 1 0 1    => 1
        // 1 1 1    => 1
    }

    public void LookAtBoard()
    { 
        // Go through each square and get the value of the piece
        // if no piece exists the value of the square is equals 0.
    }

    public void MakeMove()
    { 
    
    }

    public void Forfeit()
    { 
    
    }
}
