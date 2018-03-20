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
                PrintArray( AdderOutputEncode( new double[ ] { i } ) );
            }
            Console.WriteLine( );

            Console.WriteLine( "Output Decoder Testing:" );
            double[ ] adderEncodedOutput = new double[ 12 ];
            for ( int i = 0; i < 20; i ++ )
            {
                adderEncodedOutput[ 0 ] = ( i % 10 == 0 ) ? 1 : 0;
                adderEncodedOutput[ 1 ] = ( i % 10 == 1 ) ? 1 : 0;
                adderEncodedOutput[ 2 ] = ( i % 10 == 2 ) ? 1 : 0;
                adderEncodedOutput[ 3 ] = ( i % 10 == 3 ) ? 1 : 0;
                adderEncodedOutput[ 4 ] = ( i % 10 == 4 ) ? 1 : 0;
                adderEncodedOutput[ 5 ] = ( i % 10 == 5 ) ? 1 : 0;
                adderEncodedOutput[ 6 ] = ( i % 10 == 6 ) ? 1 : 0;
                adderEncodedOutput[ 7 ] = ( i % 10 == 7 ) ? 1 : 0;
                adderEncodedOutput[ 8 ] = ( i % 10 == 8 ) ? 1 : 0;
                adderEncodedOutput[ 9 ] = ( i % 10 == 9 ) ? 1 : 0;

                adderEncodedOutput[ 10 ] = ( i / 10 == 0 ) ? 1 : 0;
                adderEncodedOutput[ 11 ] = ( i / 10 == 1 ) ? 1 : 0;

                PrintArray( AdderOutputDecode( adderEncodedOutput ) );
            }
            Console.WriteLine( );

            Console.WriteLine( " NeuralNetwork Testing: " );
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
                if ( input[i] == 1)
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
                Console.Write( $"{input[ i ]}" );
                if ( i != input.Length - 1 )
                    Console.Write( ", " );
            }
            Console.WriteLine( " }" );
        }

        public void RunAddingNeuralNetwork()
        {
            var numInputs = 20;
            var numHiddenLayers = 1;
            var numHiddenNeurons = new int[ ] { 20 };
            var numOutputs = 12;
            var activationFunctionSigmoid = new SigmoidActivationFunction( );
            var learningRate = 0.05;

            AddingNetwork = new NeuralNetwork.NeuralNetwork( numInputs, numHiddenLayers, numHiddenNeurons, numOutputs, activationFunctionSigmoid, learningRate );

            // This is the generation
            for ( double i = 0; i < 1000; i++ )
            {
                var pass = 0;
                var fail = 0;

                // These two are for genomes
                for ( double j = 0; j < 10; j++ )
                {
                    for ( double k = 0; k < 10; k++ )
                    {
                        double[ ] addingInput = AdderInputEncode( j, k );
                        double[ ] addingOutput = AdderOutputEncode( j + k );
                        double[ ] actualOutput =  AddingNetwork.RunNetwork( addingInput, addingOutput );
                        AddingNetwork.BackPropogate( );
                        //PrintArray( actualOutput );
                        //Console.WriteLine( AdderOutputDecode( actualOutput )[0] );
                        //Console.WriteLine( $"{j + k}, {actualOutput[ 0 ]}" );
                        if ( j + k == actualOutput[ 0 ] )
                        {
                            pass++;
                        }
                        else
                        {
                            fail++;
                        };
                    }
                }

                Console.WriteLine( $"{i} = Pass: {pass}, Fail: {fail}, {(double)pass/(pass+fail)}%" );
                if (fail == 0)
                {
                    Console.WriteLine($"We have completely learned the network at {i}");
                }
            }
        }

        public static void Main(string[] args)
        {
            new Program();
        }
    }
}
