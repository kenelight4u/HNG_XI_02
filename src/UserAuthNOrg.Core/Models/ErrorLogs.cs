using System.ComponentModel.DataAnnotations;

namespace UserAuthNOrg.Core.Models
{
    //having issue creating this with serilog
    public class ErrorLogs
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        [MaxLength(128)]
        public string Level { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; } 

        public string Exception { get; set; }

        public string Properties { get; set; }

        [MaxLength(20)]
        public string ClientId { get; set; }

        [MaxLength(50)]
        public string CorrelationId { get; set; }

        [MaxLength(50)]
        public string RequestId { get; set; }

        [MaxLength(50)]
        public string ProductId { get; set; }
    }
}
