using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class NeuralNetwork
    {
        private class Neuron
        {
            public double BiasWeight { get; set; }
            public List<double> Weights { get; set; }
            public double Net { get; set; }
            public double Output { get; set; }
            public double Delta { get; set; }

            public Neuron()
            {
                Weights = new List<double>( );
            }
        }

        /// <summary>
        /// 2D list of nuerons, seperated by layer
        /// </summary>
        private List<List<Neuron>> Neurons;

        /// <summary>
        /// activator function for a given neural network
        /// </summary>
        private IActivationFunction<double, double> ActivatorFunction;

        /// <summary>
        /// Learning rate for the back propogation
        /// </summary>
        private double LearningRate { get; set; }

        public double TotalError { get; set; }

        /// <summary>
        /// Random for reading random things
        /// </summary>
        private static readonly Random Random = new Random( );

        /// <summary>
        /// Creates neural network from given inputs
        /// </summary>
        public NeuralNetwork( List<int> layers, IActivationFunction<double, double> function, double learningRate )
        {
            ActivatorFunction = function;
            LearningRate = learningRate;
            
            if ( layers.Count <= 1 ) throw new InvalidNumberOfLayersException( "Number of layers cannot 1 or below" );
            for ( int i = 0; i < layers.Count; i++ )
            {
                if ( layers[ i ] <= 0 )
                {
                    throw new InvalidNumberOfNeuronsException( "Number of neurons in a layer cannot be zero or below " );
                }
            }
            
            Neurons = new List<List<Neuron>>( layers.Count );
            
            for ( int l = 0; l < layers.Count; l++ )
            {
                List<Neuron> layer = new List<Neuron>(layers[l]);
                Neurons.Add( layer );
                
                for ( int n = 0; n < layers[ l ]; n++ )
                {
                    Neuron neuron = new Neuron( );
                    layer.Add( neuron );
                    
                    if ( l != 0 )
                    {
                        var layerPrev = Neurons[ l - 1 ];

                        neuron.Weights = new List<double>(layerPrev.Count);

                        for ( int w = 0; w < layers[ l - 1 ]; w++ )
                        {
                            neuron.Weights.Add( Random.NextDouble( ) );
                        }

                        neuron.BiasWeight = Random.NextDouble( );
                    }
                }
            }
        }

        /// <summary>
        /// Gets the weight of the given neuron connection
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="neuron"></param>
        /// <param name="inputNeuron"></param>
        /// <returns></returns>
        public double GetWeight( int layer, int neuron, int inputNeuron )
        {
            if ( layer == 0 )
                throw new InvalidNumberOfLayersException( "There are not weights for the first layer" );
            return Neurons[ layer ][ neuron ].Weights[ inputNeuron ];
        }

        /// <summary>
        /// Returns the output of the neuron
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="neuron"></param>
        /// <returns></returns>
        public double GetOutput( int layer, int neuron )
        {
            return Neurons[ layer ][ neuron ].Output;
        }

        /// <summary>
        /// Gets the delta of the previous learning
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="neuron"></param>
        /// <returns></returns>
        public double GetDelta( int layer, int neuron )
        {
            return Neurons[ layer ][ neuron ].Delta;
        }

        /// <summary>
        /// Creates a neural network from a serialized saveFile
        /// </summary>
        /// <param name="saveFile"></param>
        public NeuralNetwork( NeuralNetworkSaveFile saveFile )
        {

        }

        /// <summary>
        /// Runs the network and outputs the values of all the output neurons
        /// </summary>
        /// <returns></returns>
        public double[ ] RunNetwork( double[ ] inputs )
        {
            
            var inputsLength = Neurons.First( ).Count;
            if ( inputs.Length != inputsLength )
                throw new InvalidNumberOfNeuronsException( "Number of inputs and input neurons does not match" );
            
            for ( int n = 0; n < Neurons[ 0 ].Count; n++ )
            {
                var neuron = Neurons[ 0 ][ n ];
                neuron.Output = inputs[ n ];
            }

            for ( int l = 1; l < Neurons.Count; l++ )
            {
                var layer = Neurons[ l ];

                for ( int n = 0; n < layer.Count; n++ )
                {
                    var neuron = Neurons[ l ][ n ];

                    neuron.Net = 0.0;

                    for ( int w = 0; w < neuron.Weights.Count; w++ )
                    {
                        var layerPrevious = Neurons[ l - 1 ];
                        neuron.Net += neuron.Weights[ w ] * layerPrevious[ w ].Output;
                    }

                    neuron.Net += neuron.BiasWeight;

                    neuron.Output = ActivatorFunction.Execute( neuron.Net );
                }
            }

            var outputNeurons = Neurons[ Neurons.Count - 1 ];
            var outputNeuronsLength = outputNeurons.Count;
            var output = new double[ outputNeuronsLength ];
            for ( int n = 0; n < outputNeuronsLength; n++ )
            {
                Neuron neuron = outputNeurons[ n ];
                output[ n ] = neuron.Output;
            }

            return output;
        }
    
        /// <summary>
        /// Trains the Neural Network by feeding forward, then back propgating
        /// </summary>
        public void Train( double[] inputs, double[] targetOutputs )
        {
            double[ ] actualOutputs = RunNetwork( inputs );
            
            var outputNeuronLayer = Neurons.Count - 1;
            var outputNeurons = Neurons[ outputNeuronLayer ];
            var errorTotal = 0.0;
            for (int n = 0; n < outputNeurons.Count; n++ )
            {
                errorTotal += .5 * ( targetOutputs[ n ] - actualOutputs[ n ] ) * ( targetOutputs[ n ] - actualOutputs[ n ] );
            }

            TotalError = errorTotal;

            for ( int l = Neurons.Count - 1; l > 0;  l-- )
            {
                var layerPrev = Neurons[ l - 1 ];
                var layer = Neurons[ l ];
                
                for ( int n = 0; n < layer.Count; n++ )
                {
                    var neuron = layer[ n ];

                    if ( l == outputNeuronLayer )
                    {
                        neuron.Delta = -1 * ( targetOutputs[ n ] - neuron.Output ) * ActivatorFunction.ExecuteDerivative( neuron.Net );
                    }
                    else
                    {
                        var layerNext = Neurons[ l + 1 ];
                        var wDeltaSum = 0.0;
                        for ( int w = 0; w < layerNext.Count; w++ )
                        {
                            wDeltaSum = layerNext[ w ].Delta;
                        }

                        var derivedPart = ActivatorFunction.ExecuteDerivative( neuron.Net );

                        neuron.Delta = -1 * derivedPart * wDeltaSum;
                    }

                    for ( int w = 0; w < neuron.Weights.Count; w++ )
                    {
                        var wPrime = neuron.Weights[ w ] - LearningRate * neuron.Delta * layerPrev[ w ].Output;
                        neuron.Weights[ w ] = wPrime;
                    }

                    neuron.BiasWeight = neuron.BiasWeight - LearningRate * neuron.Delta;
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
    public class NeuralNetworkSaveFile
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
