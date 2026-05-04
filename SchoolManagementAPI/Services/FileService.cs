using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class FileService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    private readonly long MaxFileSize = 2 * 1024 * 1024; // 2MB

    private readonly string[] AllowedTypes =
    {
        "image/jpeg",
        "image/png",
        "application/pdf"
    };

    public FileService(IConfiguration config, IWebHostEnvironment env)
    {
        _basePath = Path.Combine(env.ContentRootPath, "Uploads");
        _baseUrl = "api/files";
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is empty");

        if (file.Length > MaxFileSize)
            throw new Exception("File exceeds 2MB limit");

        if (!AllowedTypes.Contains(file.ContentType))
            throw new Exception("Invalid file type");
    }

    private string EnsureFolder(string schoolId, string type, string subId = null)
    {
        var path = subId == null
            ? Path.Combine(_basePath, schoolId, type)
            : Path.Combine(_basePath, schoolId, type, subId);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public async Task<(string url, string fileName)> SaveStudentFile(
        IFormFile file, string schoolId, string admissionId)
    {
        ValidateFile(file);

        var folder = EnsureFolder(schoolId, "Students", admissionId);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var fullPath = Path.Combine(folder, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{_baseUrl}/student/{schoolId}/{admissionId}/{fileName}";

        return (url, fileName);
    }

    public async Task<(string url, string fileName)> SaveSchoolLogo(
        IFormFile file, string schoolId)
    {
        ValidateFile(file);

        var folder = EnsureFolder(schoolId, "LOGO");

        // delete old logos
        foreach (var existing in Directory.GetFiles(folder))
            File.Delete(existing);

        var fileName = $"logo_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(folder, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{_baseUrl}/logo/{schoolId}/{fileName}";

        return (url, fileName);
    }

    public string GetFilePath(string schoolId, string type, string fileName, string subId = null)
    {
        return subId == null
            ? Path.Combine(_basePath, schoolId, type, fileName)
            : Path.Combine(_basePath, schoolId, type, subId, fileName);
    }
}