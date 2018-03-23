using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeuralNetwork;

namespace NeuralNetworkTestBed
{
    class Program
    {
        private NeuralNetwork.NeuralNetwork XORNetwork { get; set; }

        public Program()
        {
            Console.WriteLine( "Activation Function Testing" );
            var sigmoidActivationFunction = new SigmoidActivationFunction( );
            Console.WriteLine($"{sigmoidActivationFunction.Execute( 1.0 )}");
            Console.WriteLine( $"{sigmoidActivationFunction.ExecuteDerivative( 1.0 )}");
            Console.WriteLine( );

            Console.WriteLine( "SumTest" );
            SumTest( );
            Console.WriteLine( );

            Console.WriteLine( "Running Test Network Test: " );
            //RunTestNeuralNetwork( );
            Console.WriteLine( );

            Console.WriteLine( "Running Adding Network Test: " );
            RunXORNeuralNetwork( );
            Console.WriteLine( );

            while ( true ) ;
        }

        public class SumTestClass
        {
            public double Output;

            public double CalculateOutput(double input )
            {
                return Output = input * 5;
            }
        }

        public void SumTest( )
        {
            var input = 1.0;
            var addList = new List<SumTestClass> { new SumTestClass( ), new SumTestClass(), new SumTestClass() };

            var sum = addList.Sum( s => s.CalculateOutput( input ) );

            Console.WriteLine(sum);
        }
        
        public void RunXORNeuralNetwork()
        {
            var activationFunction = new TanHActivationFunction( );
            var learningRate = .05;

            XORNetwork = new NeuralNetwork.NeuralNetwork( new List<int> { 2, 3, 1 }, activationFunction, learningRate );

            var random = new Random( ( int )DateTime.Now.Ticks );

            var totalError = 0.0;

            var pass = 0;
            var fail = 0;
            
            for ( double i = 1; i < 10000; i++ )
            {
                var j = ( int )( random.NextDouble( ) * 2 );
                var k = ( int )( random.NextDouble( ) * 2 );

                var inputs = new List<double> { j, k };
                var expectedOutputs = new List<double> { j ^ k };

                XORNetwork.Train( inputs, expectedOutputs );
                var actualOutputs = XORNetwork.GetOutputs();

                /*
                Console.Write( XORNetwork.GetOutput( 0, 0 ) + " " );
                Console.Write( XORNetwork.GetDelta( 0, 0 ) + " " );
                Console.Write( XORNetwork.GetBias( 0, 0 ) + " " );
                Console.WriteLine( );

                Console.Write( XORNetwork.GetOutput( 1, 0 ) + " " );
                Console.Write( XORNetwork.GetDelta( 1, 0 ) + " " );
                Console.Write( XORNetwork.GetWeight( 1, 0, 0 ) + " " );
                Console.Write( XORNetwork.GetBias( 1, 0 ) + " " );
                Console.WriteLine( );

                Console.Write( XORNetwork.GetOutput( 2, 0 ) + " " );
                Console.Write( XORNetwork.GetDelta( 2, 0 ) + " " );
                Console.Write( XORNetwork.GetWeight( 2, 0, 0 ) + " " );
                Console.Write( XORNetwork.GetBias( 2, 0 ) + " " );
                Console.WriteLine( actualOutputs[ 0 ] );
                */

                totalError += XORNetwork.GetError( );

                if ( ( ( j ^ k ) == 1 && actualOutputs[ 0 ] > .9 ) || ( ( j ^ k ) == 0 && actualOutputs[ 0 ] < .1 ) )
                {
                    pass++;
                }
                else
                {
                    fail++;
                };

                if ( i % 100 == 0 )
                {
                    Console.WriteLine( $"Pass: { pass }, Fail: { fail }, { ( double )pass*100 / ( pass + fail ) }% ; Total Error: { totalError/100 }" );

                    if ( fail == 0 )
                    {
                        Console.WriteLine( $"Learning complete, learned in {i}" );
                        return;
                    }

                    pass = 0;
                    fail = 0;
                    totalError = 0;
                }
            }
        }

        public static void Main(string[] args)
        {
            new Program();
        }
    }
}
