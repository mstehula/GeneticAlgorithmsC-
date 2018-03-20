
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeuralNetwork;

namespace NeuralNetwork
{
    public interface IActivationFunction<O, I>
    {
        O Execute( I input );
        O ExecuteDerivative( I input );
    }

    public abstract class ActivationFunction: IActivationFunction<double, double>
    {
        public double H = .000001;

        public abstract double MinValue { get; set; }
        public abstract double MaxValue { get; set; }

        public abstract double Execute( double input );
        public double ExecuteDerivative( double input )
        {
            return ( Execute( input + H ) - Execute( input - H ) ) / 2 * H;
        }
    }

    public class SigmoidActivationFunction : ActivationFunction, IActivationFunction< double, double >
    {
        public override double MinValue { get { return 0.0; } set { return; } }
        public override double MaxValue { get { return 1.0; } set { return; } }

        public override double Execute( double input )
        {
            return 1 / ( 1 + Math.Exp( -input ) );
        }
    }

    public class MaxActivationFunction : ActivationFunction, IActivationFunction< double, double >
    {
        public override double MinValue { get { return 0.0; } set { return; } }
        public override double MaxValue { get { return Double.PositiveInfinity; } set { return; } }

        public override double Execute( double input )
        {
            return Math.Max( input, 0 );
        }
    }

    public class StandardActivationFunction : ActivationFunction, IActivationFunction< double, double >
    {
        public override double MinValue { get { return Double.NegativeInfinity; } set { return; } }
        public override double MaxValue { get { return Double.PositiveInfinity; } set { return; } }

        public override double Execute( double input )
        {
            return input;
        }
    }

    public class TanHActivationFunction : ActivationFunction, IActivationFunction< double, double >
    {
        public override double MinValue { get { return -1.0; } set { return; } }
        public override double MaxValue { get { return 1.0; } set { return; } }

        public override double Execute( double input )
        {
            return Math.Tanh( input );
        }
    }
}
