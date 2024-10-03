
using System.ComponentModel.DataAnnotations.Schema;
using BackendApp.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Model;

public class PostFile
(string path, PostFileType type)
{
    private PostFile(string path) : this(path, PostFileType.Image)
    {}

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get; set;}
    public string Path { get; set; } = path;
    public string FileType { get; set; } = type.ToString();
}