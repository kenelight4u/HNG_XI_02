using System.ComponentModel.DataAnnotations;

namespace UserAuthNOrg.Core.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        public string Level { get; set; }

        public DateTime? TimeStamp { get; set; }

        [StringLength(20)]
        public string Exception { get; set; }

        public string Properties { get; set; }

        public string LogEvent { get; set; }

        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }

        public string RequestPath { get; set; }

        [StringLength(50)]
        public string MachineName { get; set; }

        [StringLength(50)]
        public string ClientIp { get; set; }

        [StringLength(30)]
        public string RequestMethod { get; set; }

        [StringLength(10)]
        public string ResponseCode { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

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
