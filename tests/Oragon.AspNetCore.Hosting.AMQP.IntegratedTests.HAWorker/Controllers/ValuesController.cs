using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Oragon.AspNetCore.Hosting.AMQP.IntegratedTests.HAWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "worker", "worker" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "worker/get";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            System.Diagnostics.Debug.Write($"ValuesController.Post(string value = \"{value}\")");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            System.Diagnostics.Debug.Write($"ValuesController.Put(int id = {id}, string value = \"{value}\")");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            System.Diagnostics.Debug.Write($"ValuesController.Delete(int id = {id})");
        }
    }
}
