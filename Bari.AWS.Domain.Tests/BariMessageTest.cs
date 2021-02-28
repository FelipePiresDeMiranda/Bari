using Bari.AWS.Infrastructure;
using System;
using Xunit;


namespace Bari.AWS.Domain.Tests
{
    public class BariMessageTest
    {
        [Fact(DisplayName = "Criação da Mensagem")]
        public void CriarMensagem()
        {
            string timestamp = Stamp.GetTimestamp(DateTime.Now);
            string body = "Hello World";

            var message = new BariMessage();

            Assert.Equal(timestamp, message.Timestamp);
            Assert.Equal(body, message.Body);
        }
    }
}
