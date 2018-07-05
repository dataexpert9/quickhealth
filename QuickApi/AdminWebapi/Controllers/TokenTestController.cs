using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdminWebapi.Controllers
{
    public class TokenTestController : ApiController
    {
        [Authorize("Admin")]
        public IHttpActionResult AuthorizeAdmin()
        {
            return Ok("Authorize");
        }

        [Authorize("User")]
        public IHttpActionResult AuthorizeUser()
        {
            return Ok("Authorize1");
        }
    }
}
