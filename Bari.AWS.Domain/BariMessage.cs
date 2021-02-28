using System;
using System.Reflection;
using System.Text;
using Bari.AWS.Infrastructure;

namespace Bari.AWS.Domain
{
    public class BariMessage
    {
        private string _Timestamp;

        public string MessageId { get; set; }
        public Guid MicroServiceId { get; set; }
        public string RequisitionId { get; set; }
        public string ReceiptHandle { get; set; }
        public string MD5OfBody { get; set; }
        public string Body { get; set; }
        public string Timestamp {
            get
            {
                return _Timestamp;
            }
            protected set
            {
                _Timestamp = value;
            }
        }

        public BariMessage() {
            Timestamp = Stamp.GetTimestamp(DateTime.Now);
            Body = "Hello World";            
        }                

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {            
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString() + " \n");
            }
            return sb.ToString();
        }
    }
        
}
