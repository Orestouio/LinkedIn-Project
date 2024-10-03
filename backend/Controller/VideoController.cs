

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using ImageManipulation.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApp.Controller;


[Route("api/[Controller]")]
[ApiController]
public class VideoController
(IFileService fileService)
: ControllerBase
{
    private readonly string videoDirectory = "./Files/Video";
    private readonly IFileService fileService = fileService;

    private readonly ulong fileMaxSizeInBytes = 4 * 1024 * 1024;
    private readonly ImmutableArray<string> allowedExtensions
        = [".mp4"];

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo([FromForm] IFormFile video)
    {
        FileFilterResult result = this.fileService
            .FilterFile(
                video,
                (video) => 
                    video.Length > (long)fileMaxSizeInBytes
                    ? FileFilterResult.TooBig : FileFilterResult.Ok,
                (video) => 
                    this.allowedExtensions.Contains(Path.GetExtension(video.FileName)) 
                    ? FileFilterResult.ExtensionNotAllowed : FileFilterResult.Ok
            );

        if(result is not FileFilterResult.Ok)
            return this.GetBadRequestResponse(result, video);
        
        string createdFileName = await this.fileService.SaveFileAsync(video, this.videoDirectory);
        return this.Ok(createdFileName);
    }

    [HttpGet]
    [Authorize]
    [Route("download")]
    public async Task<IActionResult> ReturnVideo([Required] string videoName)
    {
        var file = await this.fileService.GetFileContents(videoName, this.videoDirectory);
        var fileType = Path.GetExtension(videoName);
        return File(file, $"video/{Path.GetExtension(videoName)}");
    }

    private BadRequestObjectResult GetBadRequestResponse(FileFilterResult filterResult, IFormFile image){
        return filterResult switch
        {
            FileFilterResult.ExtensionNotAllowed 
                => this.BadRequest($"Invalid file extension. Only {string.Join(",",this.allowedExtensions)} files are allowed."),
            FileFilterResult.TooBig
                => this.BadRequest($"File is too big ({image.Length} bytes). Maximum accepted file size is {this.fileMaxSizeInBytes} bytes."),
            _ 
                => this.BadRequest($"Something must have gone really wrong for yout to see this...") 
        };
    }


}