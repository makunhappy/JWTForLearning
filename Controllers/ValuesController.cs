using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using test.Services;
using test.Entity;

using Microsoft.AspNetCore.Authorization;

namespace test.Controllers
{
    // [Route("api/[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        private readonly IUserService _userService;
        public ValuesController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        // [HttpGet("{id}")]
        // public string Get(int id)
        // {
        //     return "value";
        // }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult  Auth( User user)
        {
            var one = _userService.Auth(user);
            if(!one)
            {
                return BadRequest(new {message="bad"});
            }
            Response.Headers.Add("token",user.Token);
            return Ok(user);
        }
    }
}
