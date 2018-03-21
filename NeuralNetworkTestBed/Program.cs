using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeuralNetwork;

namespace NeuralNetworkTestBed
{
    class Program
    {
        private NeuralNetwork.NeuralNetwork AddingNetwork { get; set; }

        public Program()
        {
            Console.WriteLine( "Input Encoder Testing:" );
            for ( int i = 0; i < 10; i++ )
            {
                PrintArray( AdderInputEncode( new double[ ] { i, i } ) );
            }
            Console.WriteLine( );

            Console.WriteLine( "Output Encoder Testing:" );
            for ( int i = 0; i < 20; i++ )
            {
                PrintArray( AdderOutputEncode( new double[ ] { i-.2 } ) );
            }
            Console.WriteLine( );

            Console.WriteLine( "Output Decoder Testing:" );
            double[ ] adderEncodedOutput = new double[ 12 ];
            for ( int i = 0; i < 20; i++ )
            {
                adderEncodedOutput[ 0 ] = ( i % 10 == 0 ) ? .8 : .2;
                adderEncodedOutput[ 1 ] = ( i % 10 == 1 ) ? .8 : .2;
                adderEncodedOutput[ 2 ] = ( i % 10 == 2 ) ? .8 : .2;
                adderEncodedOutput[ 3 ] = ( i % 10 == 3 ) ? .8 : .2;
                adderEncodedOutput[ 4 ] = ( i % 10 == 4 ) ? .8 : .2;
                adderEncodedOutput[ 5 ] = ( i % 10 == 5 ) ? .8 : .2;
                adderEncodedOutput[ 6 ] = ( i % 10 == 6 ) ? .8 : .2;
                adderEncodedOutput[ 7 ] = ( i % 10 == 7 ) ? .8 : .2;
                adderEncodedOutput[ 8 ] = ( i % 10 == 8 ) ? .8 : .2;
                adderEncodedOutput[ 9 ] = ( i % 10 == 9 ) ? .8 : .2;

                adderEncodedOutput[ 10 ] = ( i / 10 == 0 ) ? .8 : .2;
                adderEncodedOutput[ 11 ] = ( i / 10 == 1 ) ? .8 : .2;

                PrintArray( AdderOutputDecode( adderEncodedOutput ) );
            }
            Console.WriteLine( );

            Console.WriteLine( "Activation Function Testing" );
            var sigmoidActivationFunction = new SigmoidActivationFunction( );
            Console.WriteLine($"{sigmoidActivationFunction.Execute( 1.0 )}");
            Console.WriteLine( $"{sigmoidActivationFunction.ExecuteDerivative( 1.0 )}");
            Console.WriteLine( );

            Console.WriteLine( "NeuralNetwork Testing: " );
            RunAddingNeuralNetwork( );
            Console.WriteLine( );

            while ( true ) ;
        }

        public double[ ] AdderInputEncode( double in1, double in2 )
        {
            return AdderInputEncode( new double[ ] { in1, in2 } );
        }

        public double[ ] AdderInputEncode(double[] input)
        {
            double[ ] output = new double[ 20 ];

            output[ ( int )input[ 0 ] ] = 1;
            output[ 10 + ( int )input[ 1 ] ] = 1;

            return output;
        }

        public double[ ] AdderOutputEncode( double in1)
        {
            return AdderOutputEncode( new double[ ] { in1 } );
        }

        public double[ ] AdderOutputEncode( double[ ] input )
        {
            double[ ] output = new double[ 12 ];

            output[ ( ( int )input[ 0 ] ) % 10 ] = 1;
            output[ ( ( int )input[ 0 ] < 10 ) ? 10 : 11 ] = 1;

            return output;

        }

        public double[] AdderOutputDecode(double[] input)
        {
            double[ ] output = new double[ 1 ];

            for ( int i = 0; i < 10; i++ )
            {
                if ( input[i] > .5 )
                {
                    output[ 0 ] += i;
                }
            }

            for ( int i = 10; i < input.Length; i++ )
            {
                if ( input[i] > .5 )
                {
                    output[ 0 ] += 10 * (i-10);
                }
            }
            return output;
        }

        public void PrintArray(double[] input)
        {
            Console.Write( "{ " );
            for( int i = 0; i < input.Length; i++ )
            {
                Console.Write( $"{Math.Truncate(input[ i ] * 100)/100}" );
                if ( i != input.Length - 1 )
                    Console.Write( ", " );
            }
            Console.WriteLine( " }" );
        }

        public void RunAddingNeuralNetwork()
        {
            var numInputs = 20;
            var numHiddenLayers = 1;
            var numHiddenNeurons = new int[ ] { 5 };
            var numOutputs = 12;
            var activationFunctionSigmoid = new SigmoidActivationFunction( );
            var learningRate = .0005;

            AddingNetwork = new NeuralNetwork.NeuralNetwork( numInputs, numHiddenLayers, numHiddenNeurons, numOutputs, activationFunctionSigmoid, learningRate );

            var random = new Random( ( int )DateTime.Now.Ticks );

            var pass = 0;
            var fail = 0;

            // This is the generation
            for ( double i = 0; i < 1000; i++ )
            {
                var j = ( int )( random.NextDouble( ) * 2 );
                var k = ( int )( random.NextDouble( ) * 2 );

                var addingInput = new double[ 1 ];
                var addingOutput = new double[ 1 ];
                var actualOutput = new double[ 1 ];

                addingInput = AdderInputEncode( j, k );
                //PrintArray( addingInput );
                addingOutput = AdderOutputEncode( j + k );
                //PrintArray( addingOutput );
                actualOutput = AddingNetwork.RunNetwork( addingInput, addingOutput );
                //PrintArray( actualOutput );
                AddingNetwork.BackPropogate( );
                //Console.WriteLine( AdderOutputDecode( actualOutput )[0] );
                //Console.WriteLine( $"{j + k}, {AdderOutputDecode(actualOutput)[ 0 ]}" );

                if ( j + k == actualOutput[ 0 ] )
                {
                    pass++;
                }
                else
                {
                    fail++;
                };

                if ( i % 1000 == 0 )
                {
                    Console.WriteLine( $"Pass: {pass}, Fail: {fail}, {( double )pass / ( pass + fail )}% ;" );
                    pass = 0;
                    fail = 0;
                }
            }

        }

        public static void Main(string[] args)
        {
            new Program();
        }
    }
}
