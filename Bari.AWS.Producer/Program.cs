using System;
using System.Threading.Tasks;

namespace Bari.AWS.Producer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            MessageProducer.Sender();                    
        }
    }
}
