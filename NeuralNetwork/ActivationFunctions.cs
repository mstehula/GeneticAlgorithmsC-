
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
    }

    abstract class ActivationFunction: IActivationFunction<double, double>
    {
        public abstract double MinValue { get; set; }
        public abstract double MaxValue { get; set; }

        public abstract double Execute( double input );
    }

    class SigmoidActivationFunction: IActivationFunction< double, double >
    {
        public double MinValue = 0.0;
        public double MaxValue = 1.0;

        public double Execute( double input )
        {
            return 1 / ( 1 + Math.Exp( -input ) );
        }
    }

    class MaxActivationFunction: IActivationFunction< double, double >
    {
        public double MinValue = 0.0;
        public double MaxValue = double.PositiveInfinity;

        public double Execute( double input )
        {
            return Math.Max( input, 0 );
        }
    }

    class StandardActivationFunction: IActivationFunction< double, double >
    {
        public double MinValue = double.NegativeInfinity;
        public double MaxValue = double.PositiveInfinity;

        public double Execute( double input )
        {
            return input;
        }
    }

    class TanHActivationFunction: IActivationFunction< double, double >
    {
        public double MinValue = -1.0;
        public double MaxValue = 1.0;

        public double Execute( double input )
        {
            return Math.Tanh( input );
        }
    }
}
