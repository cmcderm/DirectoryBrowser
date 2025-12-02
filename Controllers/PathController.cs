using Microsoft.AspNetCore.Mvc;
using TestProject.Managers;

namespace TestProject.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class PathController : ControllerBase {

        private readonly ILogger<PathController> _logger;
        private readonly IStorageManager _storageManager;

        public PathController(ILogger<PathController> logger, IStorageManager storageManager) {
            _logger = logger;
            _storageManager = storageManager;
        }

        [HttpGet("{*path}")]
        public IActionResult Get(string path = "") {
            try {
                return Ok(_storageManager.GetPathInfo(path));
            } catch (DirectoryNotFoundException ex) {
                return NotFound(new { Error = ex.Message });
            }
        }
    }
}
