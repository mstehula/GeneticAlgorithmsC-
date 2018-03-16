using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetworkTest
{
    class Program
    {
        private MyThing MyThing1;
        private MyThing MyThing2;

        public Program( )
        {
            MyThing1 = new MyThing( );
            MyThing2 = MyThing1;
            MyThing1.Storage = 1;

            Console.WriteLine( $"{ MyThing1.Storage }" );
            Console.WriteLine( $"{ MyThing2.Storage }" );

            while ( true )
                ;
        }

        static void Main( string[ ] args )
        {
            Program program = new Program( );
        }
    }

    class MyThing
    {
        public int Storage { get; set; }

        public MyThing( )
        {
            Storage = 0;
        }
    }
}
