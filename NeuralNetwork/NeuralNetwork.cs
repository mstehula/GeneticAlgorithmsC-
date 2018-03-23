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
            public List<Synapse> InputSynapses { get; set; }
            public List<Synapse> OutputSynapses { get; set; }

            public double Bias { get; set; }
            public double Output { get; set; }
            public double Delta { get; set; }

            public double Error { get; set; }

            public IActivationFunction<double, double> ActivationFunction { get; set; }

            public Neuron( IActivationFunction<double, double> function, bool inputNeuron = false, bool outputNeuron = false)
            {
                ActivationFunction = function;
            }

            public double CalculateOutput()
            {
                return Output = ActivationFunction.Execute( InputSynapses.Sum( s => s.CalculateOutput( ) ) + Bias );
            }
            
            public double CalculateError( double expectedOutput )
            {
                return Error = .5 * ( expectedOutput - Output ) * ( expectedOutput - Output );
            }

            public double CalculateDelta( double? expectedOutput = null)
            {
                if( expectedOutput == null )
                    return Delta = OutputSynapses.Sum( s => s.OutputNeuron.Delta * s.Weight ) * ActivationFunction.ExecuteDerivative( Output );

                return Delta = CalculateError( expectedOutput.Value ) * ActivationFunction.ExecuteDerivative( Output );
            }

            public void UpdateWeights(double learningRate)
            {
                Bias += learningRate * Delta;
                InputSynapses.ForEach( s => s.Weight += learningRate * Delta * s.InputNeuron.Output );
            }
        }

        private class Synapse
        {
            public Neuron InputNeuron { get; set; }
            public Neuron OutputNeuron { get; set; }

            public double Output { get; set; }
            public double Weight { get; set; }

            public double CalculateOutput()
            {
                return Output = Weight * InputNeuron.Output;
            }
        }

        /// <summary>
        /// 2D list of nuerons, seperated by layer
        /// </summary>
        private List<List<Neuron>> Neurons;

        /// <summary>
        /// activator function for a given neural network
        /// </summary>
        private IActivationFunction<double, double> ActivationFunction;

        /// <summary>
        /// Learning rate for the back propogation
        /// </summary>
        private double LearningRate { get; set; }
        
        /// <summary>
        /// Random for reading random things
        /// </summary>
        private static readonly Random Random = new Random( );

        /// <summary>
        /// Creates neural network from given inputs
        /// </summary>
        public NeuralNetwork( List<int> layers, IActivationFunction<double, double> function, double learningRate )
        {
            ActivationFunction = function;
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
                List<Neuron> layer = new List<Neuron>( layers[ l ] );
                Neurons.Add( layer );

                for ( int n = 0; n < layers[ l ]; n++ )
                {
                    Neuron neuron = new Neuron( ActivationFunction );
                    layer.Add( neuron );

                    neuron.InputSynapses = new List<Synapse>( );
                    neuron.OutputSynapses = new List<Synapse>( );

                    if ( l != 0 )
                    {
                        var layerPrev = Neurons[ l - 1 ];
                        for ( int w = 0; w < layers[ l - 1 ]; w++ )
                        {
                            Neuron inputNeuron = Neurons[ l - 1 ][ w ];
                            Synapse synapse = new Synapse( );

                            neuron.InputSynapses.Add( synapse );
                            inputNeuron.OutputSynapses.Add( synapse );

                            synapse.InputNeuron = inputNeuron;
                            synapse.OutputNeuron = neuron;

                            synapse.Weight = Random.NextDouble( );
                        }

                        neuron.Bias = Random.NextDouble( );
                    }
                }
            }
        }

        public double GetDelta( int layer, int neuron)
        {
            return Neurons[ layer ][ neuron ].Delta;
        }

        public double GetWeight(int layer, int neuron, int synapse )
        {
            return Neurons[ layer ][ neuron ].InputSynapses[ synapse ].Weight;
        }

        public double GetBias(int layer, int neuron)
        {
            return Neurons[ layer ][ neuron ].Bias;
        }

        public double GetOutput( int layer, int neuron )
        {
            return Neurons[ layer ][ neuron ].Output;
        }
        
        public List<double> GetOutputs()
        {
            return Neurons.Last( ).Select( n => n.Output ).ToList( );
        }

        public double GetError()
        {
            return Neurons.Last( ).Sum( n => n.Error );
        }

        public void ForwardPropogate( List<double> inputs )
        {
            var i = 0;
            Neurons.First().ForEach( n => n.Output = inputs[ i++ ] );
            Neurons.FindAll( l => l != Neurons.First( )).ForEach( l => l.ForEach( n => n.CalculateOutput( ) ) );
        }

        public void BackPropogate( List<double> expectedOutputs )
        {
            var i = 0;
            Neurons.Reverse( );
            Neurons.First( ).ForEach( n => n.CalculateDelta( expectedOutputs[ i++ ] ) );
            Neurons.FindAll(l => l != Neurons.First()).ForEach( l => l.ForEach( n => n.CalculateDelta( ) ) );
            Neurons.ForEach( l => l.ForEach( n => n.UpdateWeights( LearningRate ) ) );
            Neurons.Reverse( );
        }

        public List<double> Compute(List<double> inputs)
        {
            ForwardPropogate( inputs );
            return Neurons.Last( ).Select( n => n.Output ).ToList( );
        }

        public void Train(List<double> inputs, List<double> outputs)
        {
            ForwardPropogate( inputs );
            BackPropogate( outputs );
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
