using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseRepaymentTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/Test
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 1)
            {
                throw new BadRequestException("Id Cannot be 1");
            }

            if (id == 2)
            {
                throw new NotFoundException("Record with ID: 2 cannot be found");
            }

            if (id == 3)
            {
                throw new Exception("An error has occured");
            }

            return await Task.FromResult(Ok("Hello World"));

        }

        // POST: api/Test
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Test/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}