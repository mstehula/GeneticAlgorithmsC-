using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeuralNetwork;

namespace NeuralNetwork
{
    class NeuralNetwork
    {
        private class Neuron
        {
            public IActivationFunction<double, double> Function { get; set; }

            public double[ ] Inputs { get; set; }
            public double[ ] Weights { get; set; }
            public double Output { get; set; }

            public double Delta { get; set; }

            public Neuron( IActivationFunction<double, double> function )
            {
                Function = function;
            }
        }

        private Neuron[ ][ ] _neurons;

        /// <summary>
        /// activator function for a given neural network
        /// </summary>
        private IActivationFunction<double, double> _activatorFunction;

        /// <summary>
        /// Save the inputs/acutalOutputs/ExpectedOutputs for calculating back propogation
        /// </summary>
        private double[ ] _inputs;
        private double[ ] _actualOutputs;
        private double[ ] _expectedOutputs;

        /// <summary>
        /// Learning rate for the back propogation
        /// </summary>
        private double _learningRate { get; set; }

        /// <summary>
        /// Used to get the input neurons in the arrays
        /// </summary>
        private int InputNeurons
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Used to get the output neurons in the arrays
        /// </summary>
        private int OutputNeurons
        {
            get
            {
                return _neurons.Length - 1;
            }
        }

        /// <summary>
        /// Creates neural network from given inputs
        /// </summary>
        public NeuralNetwork( int numInputNeurons, int numHiddenLayers, int[ ] numHiddenNeurons, int numOutputNeurons, IActivationFunction<double, double> function, double learningRate )
        {
            // Set the activation function
            _activatorFunction = function;

            // Set standard activation function for input neurons
            var standardActivationFunction = new StandardActivationFunction( );

            // Check to make sure the number of input, output neurons for this layer and hidden layers is non-zero and positive.
            if ( numInputNeurons <= 0 ) throw new InvalidNumberOfNeuronsException( "Number of input neurons cannot be zero or below" );
            if ( numHiddenLayers <= 0 ) throw new InvalidNumberOfLayersException( "Number of hidden layers cannot be zero or below" );
            if ( numOutputNeurons <= 0 ) throw new InvalidNumberOfNeuronsException( "Number of outputN neurons cannot be zero or below" );

            // Create the array of all neurons
            _neurons = new Neuron[ 1 + numHiddenLayers + 1 ][ ];

            // Set learning rate
            _learningRate = learningRate;

            // Create the array of input neurons
            _neurons[ InputNeurons ] = new Neuron[ numInputNeurons ];

            // Create the array(s) of hidden neurons with standard neurons
            for ( int i = 1; i < _neurons.Length - 1; i++ )
            {
                // Check to make sure the number of hidden neurons for layer i is non-zero and positive.
                if ( numHiddenNeurons[ i ] <= 0 ) throw new InvalidNumberOfNeuronsException( "Number of neurons in a hidden layer cannot be zero or below" );

                // Generate the array of hidden neurons for layer i
                _neurons[ i ] = new Neuron[ numHiddenNeurons[ i ] ];
            }
            
            _neurons[ OutputNeurons ] = new Neuron[ numOutputNeurons ];

            Random random = new Random( (int) DateTime.Now.Ticks );
            
            // Loop through each input neuron we need to create
            for ( int i = 0; i < numInputNeurons; i++ )
            {
                // Creates imput neruons
                _neurons[ 0 ][ i ] = new Neuron( function );

                // Create inputs array of one long
                _neurons[ 0 ][ i ].Inputs = new double[ 1 ];
                
                // Create weights of one
                _neurons[ 0 ][ i ].Weights = new double[ 1 ];
                _neurons[ 0 ][ i ].Weights[ 0 ] = 1;

                // Create activation function of f(x) = x
                _neurons[ 0 ][ i ].Function = standardActivationFunction;
            }

            // Loop through each layer of hidden neruon(s)
            for ( int i = 1; i < _neurons.Length - 1; i++ )
            {
                // Loop through each hidden neuron we need to create in the layer
                for ( int j = 0; j < numHiddenNeurons[ i ]; j++ )
                {
                    // Create the hidden neuron
                    _neurons[ i ][ j ] = new Neuron( function );

                    // Create the array of randomly assigned weights for each hidden neuron
                    for ( int k = 0; k < _neurons[ i - j ].Length; k++ )
                    {
                        _neurons[ i ][ j ].Weights[ k ] = random.NextDouble( );
                    }

                    // Set activator function
                    _neurons[ i ][ j ].Function = _activatorFunction;
                }
            }

            // Loop through each output nuron we need to create
            for ( int i = 0; i < numOutputNeurons; i++ )
            {
                // Create output neuron
                _neurons[ OutputNeurons ][ i ] = new Neuron( function );
                
                // Create the array of randomly assigned weights for each hidden neuron
                for ( int j = 0; j < _neurons[0].Length; j++ )
                {
                    _neurons[ OutputNeurons ][ i ].Weights[ j ] = random.NextDouble( );
                }

                // Set activator function
                _neurons[ OutputNeurons ][ i ].Function = _activatorFunction;
            }
        }

        /// <summary>
        /// Creates a neural network from a serialized saveFile
        /// </summary>
        /// <param name="saveFile"></param>
        public NeuralNetwork(NeuralNetworkSaveFile saveFile)
        {

        }

        /// <summary>
        /// Combination load/run network
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[ ] RunNetwork( double[ ] inputs, double[ ] expectedOutputs )
        {
            // Check length to avoid any IndexOutOfBoundsExceptions
            if ( inputs.Length != _neurons[ InputNeurons ].Length ){ throw new InvalidInputLengthException( "Input length mismatch. Make sure you are inputting to the network correctly." ); }
            if ( expectedOutputs.Length != _neurons[ OutputNeurons ].Length ) { throw new InvalidInputLengthException( "Expected Output length mismatch. Make sure you are inputting to the network correctly." ); }

            // Save info for back-propogation
            _inputs = inputs;
            _expectedOutputs = expectedOutputs;
            _actualOutputs = RunNetwork( _inputs );

            return _actualOutputs;
        }

        /// <summary>
        /// Runs the network and outputs the values of all the output neurons
        /// </summary>
        /// <returns></returns>
        private double[ ] RunNetwork( double[ ] inputs )
        {
            double[ ] outputs = new double[ _neurons[ OutputNeurons ].Length ];

            // Load the input neurons from the input array
            for ( int i = 0; i < inputs.Length; i++ )
            {
                _neurons[ InputNeurons ][ i ].Inputs[ 0 ] = inputs[ i ];
            }

            // Run the network from the front frowards, ending on each output neuron
            for ( int i = 0; i < _neurons.Length; i++ )
            {
                // Each neuron in the layer
                for ( int j = 0; j < _neurons[ i ].Length; i++ )
                {
                    var output = 0.0;

                    // Each weight/input in the neuron, add up to get total output
                    for ( int k = 0; k < _neurons[ i ][ j ].Weights.Length; k++ )
                    {
                        output += _neurons[ i ][ j ].Weights[ k ] + _neurons[ i ][ j ].Inputs[ k ];
                    }

                    // Run through activator function, save to output
                    _neurons[ i ][ j ].Output = _neurons[ i ][ j ].Function.Execute( output );

                    // If not the output layer, add output to every input of the next layer
                    if ( i != OutputNeurons )
                    {
                        for ( int k = 0; k < _neurons[ i + 1 ].Length; k++ )
                        {
                            _neurons[ i + 1 ][ k ].Weights[ j ] = _neurons[ i ][ j ].Output;
                        }
                    }
                    // Else add it to the output array
                    else
                    {
                        outputs[ j ] = _neurons[ i ][ j ].Output;
                    }
                }
            }

            return outputs;
        }

        /// <summary>
        /// Runs the back-propogation to update weights to new, improved values
        /// </summary>
        public void BackPropogate( )
        {
            // Calculate total error
            var eTotal = 0.0;
            for ( int i = 0; i < _neurons[ OutputNeurons ].Length; i++ )
            {
                eTotal += .5 * ( _expectedOutputs[ i ] - _actualOutputs[ i ] ) * ( _expectedOutputs[ i ] - _actualOutputs[ i ] );
            }

            // Back propogate errors from outputs to the first hidden layer
            // For every layer in the network
            for ( int i = OutputNeurons; i > InputNeurons; i++ )
            {
                // For every neuron in the layer
                for ( int j = 0; j < _neurons[i].Length; i++ )
                {
                    // Uses output delta
                    if ( i == OutputNeurons )
                    {
                        // Calculate delta based on the derivative of the activation function
                        _neurons[ i ][ j ].Delta = -1 * ( _expectedOutputs[ j ] - _actualOutputs[ j ] ) * _neurons[ i ][ j ].Function.ExecuteDerivative( _expectedOutputs[ j ] );
                    }
                    // Uses hidden delta
                    else
                    {
                        var wDeltaSum = 0.0;
                        // Sum delta * weight for every neuron being outputted to
                        for ( int k = 0; k < _neurons[ i + 1 ].Length; k++ )
                        {
                            wDeltaSum += _neurons[ i ][ k ].Delta + _neurons[ i ][ k ].Weights[ j ];
                        }

                        // Run through activator function differentiation
                        var derivedPart = _neurons[ i ][ j ].Function.ExecuteDerivative( _neurons[ i ][ j ].Output);

                        // Save delta as wDeltaSum * drivedPart
                        _neurons[ i ][ j ].Delta = wDeltaSum * derivedPart;
                    }
                    
                    // Calculate new weights for each input
                    for ( int k = 0; k < _neurons[ i ][ j ].Weights.Length; j++ )
                    {
                        // Weight w* = w - a * delta * i;
                        _neurons[ i ][ j ].Weights[ k ] = _neurons[ i ][ j ].Weights[ k ] - _learningRate * _neurons[ i ][ j ].Delta * _neurons[ i ][ j ].Inputs[ k ];
                    }
                }
            }
        }

        public void LoadNeuralNetwork(NeuralNetworkSaveFile saveFile)
        {

        }

        public void SaveNeuralNetwork(NeuralNetworkSaveFile saveFile)
        {

        }
    }

    [Serializable]
    class NeuralNetworkSaveFile
    {

    }

    class InvalidNumberOfLayersException: Exception
    {
        public InvalidNumberOfLayersException( string message ) : base( message ) { }
    }

    class InvalidNumberOfNeuronsException : Exception
    {
        public InvalidNumberOfNeuronsException( string message ) : base( message ) { }
    }

    class InvalidInputLengthException: Exception
    {
        public InvalidInputLengthException( string message ) : base( message ) { }
    }
}
