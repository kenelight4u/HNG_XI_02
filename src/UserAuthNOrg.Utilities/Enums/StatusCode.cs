using System.ComponentModel;

namespace UserAuthNOrg.Utilities.Enums
{
    public enum StatusCode
    {
        [Description("success")] OK = 200,

        [Description("Created")] Created = 201,

        [Description("Bad request")] BadRequest = 400,
        
        [Description("UnAuthorize")] UnAuthorize = 401,
        
        [Description("Not Found")] NotFound = 404,

        [Description("UnProcessable entity")] UnProcessableEntity = 422,

        [Description("Internal Server Error")] ERROR = 500,
        
       
        
        
        
        

        


    }
}
