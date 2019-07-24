using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeycomb.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Handlers;

namespace Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IHoneycombEventManager EventManager { get; }
        private readonly IBusControl _busController;

        public ValuesController(IHoneycombEventManager eventManager, IBusControl busController)
        {
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _busController = busController ?? throw new ArgumentNullException(nameof(busController));
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var values = new string[] { "value1", "value2" };
            EventManager.AddData("values_count", values.Length);
            return values;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] string value)
        {
            await _busController.Publish<IAddValues>(new { Value = value });

            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
