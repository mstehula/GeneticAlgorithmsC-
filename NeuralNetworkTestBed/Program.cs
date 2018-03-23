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

            Console.WriteLine( "Running Test Network Test: " );
            //RunTestNeuralNetwork( );
            Console.WriteLine( );

            Console.WriteLine( "Running Adding Network Test: " );
            RunXORNeuralNetwork( );
            Console.WriteLine( );

            while ( true ) ;
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
        
        public void RunXORNeuralNetwork()
        {
            var activationFunction = new TanHActivationFunction( );
            var learningRate = .05;

            XORNetwork = new NeuralNetwork.NeuralNetwork( new List<int> { 2, 3, 1 }, activationFunction, learningRate );

            var random = new Random( ( int )DateTime.Now.Ticks );

            var pass = 0;
            var fail = 0;
            
            for ( double i = 1; i < 1000000; i++ )
            {
                var j = ( int )( random.NextDouble( ) * 2 );
                var k = ( int )( random.NextDouble( ) * 2 );

                var inputs = new double[] { j, k };
                var expectedOutputs = new double[] { j ^ k };

                var actualOutputs = XORNetwork.RunNetwork( inputs );
                XORNetwork.Train( inputs, expectedOutputs );

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
                    Console.WriteLine( $"Pass: {pass}, Fail: {fail}, {( double )pass*100 / ( pass + fail )}% ; Total Error: {XORNetwork.TotalError}" );

                    if ( fail == 0 )
                    {
                        Console.WriteLine( $"Learning complete, learned in {i}" );
                        return;
                    }

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
