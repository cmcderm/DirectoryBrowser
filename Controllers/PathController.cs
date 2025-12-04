using Microsoft.AspNetCore.Mvc;
using TestProject.Managers;

namespace TestProject.Controllers {
    [ApiController]
    [Route("api/[controller]")]
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
            } catch (Exception ex) {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("download/{*path}")]
        public IActionResult DownloadFile(string path = "") {
            try {
                FileInfo? fileInfo = _storageManager.GetFileInfo(path);

                if (fileInfo == null) {
                    return NotFound(new { Error = "File does not exist or is a directory" });
                }

                FileStream? fileStream = fileInfo?.OpenRead();

                if (fileStream != null) {
                    return File(fileStream, "application/octet-stream", fileInfo?.Name);
                } else {
                    throw new Exception("Failed to open file: " + fileInfo?.Name);
                }
            } catch (Exception ex) {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("upload/{*path}")]
        public async Task<IActionResult> UploadFile(string path, IFormFile file) {
            if (file == null || file.Length == 0) {
                return BadRequest("No File Uploaded");
            }

            try {
                var result = await _storageManager.UploadFile(path, file);
                if (!result) {
                    return BadRequest(new { Error = $"File already exists at path {path}" });
                }
                return Ok(new { Message = "File successfully uploaded." });
            } catch (Exception ex) {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
