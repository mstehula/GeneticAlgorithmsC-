using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeuralNetwork;

namespace NeuralNetwork
{
    interface INeuron
    {
        IActivationFunction<double, double> Function { get; set; }
        double Output { get; set; }
    }

    interface INeuronWithInputs
    {
        INeuron[ ] InputNeurons { get; set; }
        double[ ] Weights { get; set; }
    }

    class InputNeuron : INeuron
    {
        public double Input { get; set; }

        public IActivationFunction<double, double> Function { get; set; }
        public double Output
        {
            get
            {
                return Input;
            }
            set { }
        }

        public InputNeuron( IActivationFunction< double, double > function )
        {
        }
    }

    class HiddenNeuron : INeuron, INeuronWithInputs
    {
        public IActivationFunction<double, double> Function { get; set; }
        public double Output { get; set; }

        public INeuron[ ] InputNeurons { get; set; }
        public double[ ] Weights { get; set; }

        public HiddenNeuron( IActivationFunction< double, double > function )
        {
        }
    }

    class OutputNeuron : INeuron, INeuronWithInputs
    {
        public IActivationFunction<double, double> Function { get; set; }
        public double Output { get; set; }

        public INeuron[ ] InputNeurons { get; set; }
        public double[ ] Weights { get; set; }

        public OutputNeuron( IActivationFunction< double, double > function )
        {
        }
    }

    class NeuralNetwork
    {
        /// <summary>
        /// All storage location of neurons
        /// </summary>
        private InputNeuron[ ] _inputNeurons;
        private HiddenNeuron[ ][ ] _hiddenNeurons;
        private OutputNeuron[ ] _outputNeurons;

        /// <summary>
        /// activator function for a given neural network
        /// </summary>
        private IActivationFunction< double, double > _activatorFunction;

        /// <summary>
        /// Creates neural network from given inputs
        /// </summary>
        public NeuralNetwork( int numInputNeurons, int numHiddenLayers, int[ ] numHiddenNeurons, int numOutputNeurons, IActivationFunction<double, double> function )
        {
            // Set the activation function
            _activatorFunction = function;

            // Check to make sure the number of input, output neurons for this layer and hidden layers is non-zero and positive.
            if ( numInputNeurons <= 0 ) throw new InvalidNumberOfNeuronsException( "Number of input neurons cannot be zero or below" );
            if ( numHiddenLayers <= 0 ) throw new InvalidNumberOfLayersException( "Number of hidden layers cannot be zero or below" );
            if ( numOutputNeurons <= 0 ) throw new InvalidNumberOfNeuronsException( "Number of outputN neurons cannot be zero or below" );

            // Create the array that holds the array(s) of hidden neurons
            _hiddenNeurons = new HiddenNeuron[ numHiddenLayers ][ ];

            // Fill the array(s) of hidden neurons with standard neurons
            for ( int i = 0; i <= numHiddenLayers; i++ )
            {
                // Check to make sure the number of hidden neurons for layer i is non-zero and positive.
                if ( numHiddenNeurons[ i ] <= 0 ) throw new InvalidNumberOfNeuronsException( "Number of neurons in a hidden layer cannot be zero or below" );

                // Generate the array of hidden neurons for layer i
                _hiddenNeurons[ i ] = new HiddenNeuron[ numHiddenNeurons[ i ] ];
            }

            Random random = new Random( (int) DateTime.Now.Ticks );
            
            // Loop through each input neuron we need to create
            for ( int i = 0; i < numInputNeurons; i++ )
            {
                // Creates imput neruons
                _inputNeurons[ i ] = new InputNeuron( function );

                // No input neruons are needed to save

                // No need to create weights
            }

            // Loop through each layer of hidden neruon(s)
            for ( int i = 0; i < numHiddenLayers; i++ )
            {
                // Loop through each hidden neuron we need to create in the layer
                for ( int j = 0; j < numHiddenNeurons[i]; j++ )
                {
                    // Create the hidden neuron
                    _hiddenNeurons[ i ][ j ] = new HiddenNeuron( function );

                    // If we are on the first layer, use the input neuron(s) as the hidden layer input
                    if ( i == 0 )
                    {
                        _hiddenNeurons[ i ][ j ].InputNeurons = _inputNeurons;
                    }
                    // Else use the last hidden layer as the input neuron(s)
                    else
                    {
                        _hiddenNeurons[ i ][ j ].InputNeurons = _hiddenNeurons[ i - 1 ];
                    }

                    // Create the array of randomly assigned weights for each hidden neuron
                    for ( int k = 0; k < _hiddenNeurons [ i - j ].Length; k++ )
                    {
                        _hiddenNeurons[ i ][ j ].Weights[ k ] = random.NextDouble( );
                    }

                    // Set activator function
                    _hiddenNeurons[ i ][ j ].Function = _activatorFunction;
                }
            }

            // Loop through each output nuron we need to create
            for ( int i = 0; i < numOutputNeurons; i++ )
            {
                // Create output neuron
                _outputNeurons[ i ] = new OutputNeuron( function );

                // Save last line out hidden neuron(s) as the input neurons of the output neruon(s)
                _outputNeurons[ i ].InputNeurons = _hiddenNeurons[ numOutputNeurons - 1];

                // Create the array of randomly assigned weights for each hidden neuron
                for ( int j = 0; j < _outputNeurons.Length; j++ )
                {
                    _outputNeurons[ i ].Weights[ j ] = random.NextDouble( );
                }

                // Set activator function
                _outputNeurons[ i ][ j ].Function = _activatorFunction;
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
        /// Loads inputs into the network
        /// </summary>
        /// <param name="inputs"></param>
        public void LoadNetwork(double[] inputs)
        {
            if ( inputs.Length != _inputNeurons.Length ) { throw new InvalidInputLengthException( "Input length mismatch. Make sure you are inputting to the network correctly."); }

            // Load the input neurons from the input array
            for ( int i = 0; i < inputs.Length; i++ )
            {
                _inputNeurons[ i ].Input = inputs[ i ];
            }
        }

        /// <summary>
        /// Combination load/run network
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] RunNetwork(double[] inputs)
        {
            LoadNetwork( inputs );
            return RunNetwork();
        }

        /// <summary>
        /// Runs the network and outputs the values of all the output neurons
        /// </summary>
        /// <returns></returns>
        public double[] RunNetwork()
        {
            double[ ] output = new double[ _outputNeurons.Length ];

            // Run the network recursively, 
            for ( int i = 0; i < output.Length; i++ )
            {
                // Call the recursive function on the neuron above it
                RunNetworkRecursiveCall( _outputNeurons[i] );

                // Save the output of the neuron for function return
                output[ i ] = _outputNeurons[ i ].Output;
            }

            return output;
        }

        /// <summary>
        /// Runs the network cursively starting at the neuron given
        /// </summary>
        /// <param name="startingNeuron"></param>
        private void RunNetworkRecursiveCall(INeuronWithInputs startingNeuron)
        { 
            // Calculate all inputs first
            for ( int i = 0; i < startingNeuron.InputNeurons.Length; i++ )
            {
                // If the input neuron has input neurons of it's own, calculate those first.
                // TODO: Optimization: remove redundent calculations
                // Pseudo code : run through all hidden/output, set to "magic number" (-inf?)
                // If the output is this number, run, otherwise skip
                if ( startingNeuron.InputNeurons[ i ] is INeuronWithInputs )
                {
                    RunNetworkRecursiveCall( (INeuronWithInputs) startingNeuron.InputNeurons[ i ] );
                }
            }

            // Sum all products of inputs and weights
            double output = 0;
            for ( int i = 0; i < startingNeuron.InputNeurons.Length; i++ )
            {
                output += startingNeuron.InputNeurons[ i ].Output * startingNeuron.Weights[ i ];
            }

            // run through activation function
            ( (INeuron) startingNeuron).Output = ( ( INeuron ) startingNeuron ).Function.Execute( output );

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
