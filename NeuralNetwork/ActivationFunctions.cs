
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeuralNetwork;

namespace NeuralNetwork
{
    interface IActivationFunction<O, I>
    {
        O Execute( I input );
        O ExecuteDerivative( I input );
    }

    abstract class ActivationFunction: IActivationFunction<double, double>
    {
        public abstract double MinValue { get; set; }
        public abstract double MaxValue { get; set; }

        public abstract double Execute( double input );
<<<<<<< Updated upstream
=======
        public double ExecuteDerivative( double input )
        {
            return ( Execute( input + H ) - Execute( input - H ) ) / 2 * H;
        }
>>>>>>> Stashed changes
    }

    class SigmoidActivationFunction: IActivationFunction< double, double >
    {
        public double H = .000001;

        public double MinValue = 0.0;
        public double MaxValue = 1.0;

        public double Execute( double input )
        {
            return 1 / ( 1 + Math.Exp( -input ) );
        }

        public double ExecuteDerivative( double input )
        {
            return ( Execute( input + H ) - Execute( input - H ) ) / 2 * H;
        }
    }

    class MaxActivationFunction: IActivationFunction< double, double >
    {
        public double H = .000001;

        public double MinValue = 0.0;
        public double MaxValue = double.PositiveInfinity;

        public double Execute( double input )
        {
            return Math.Max( input, 0 );
        }

        public double ExecuteDerivative( double input )
        {
            return ( Execute( input + H ) - Execute( input - H ) ) / 2 * H;
        }
    }

    class StandardActivationFunction: IActivationFunction< double, double >
    {
        public double H = .000001;

        public double MinValue = double.NegativeInfinity;
        public double MaxValue = double.PositiveInfinity;

        public double Execute( double input )
        {
            return input;
        }

        public double ExecuteDerivative( double input )
        {
            return ( Execute( input + H ) - Execute( input - H ) ) / 2 * H;
        }
    }

    class TanHActivationFunction: IActivationFunction< double, double >
    {
        public double H = .000001;

        public double MinValue = -1.0;
        public double MaxValue = 1.0;

        public double Execute( double input )
        {
            return Math.Tanh( input );
        }

        public double ExecuteDerivative( double input )
        {
            return ( Execute( input + H ) - Execute( input - H ) ) / 2 * H;
        }
    }
}
