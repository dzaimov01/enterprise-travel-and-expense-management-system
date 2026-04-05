namespace enterprise_travel_and_expense_management_system.Services;

/// <summary>
/// Service for handling file operations (receipt uploads, storage).
/// Files are saved to wwwroot/receipts directory.
/// </summary>
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string ReceiptDirectory = "receipts";

    /// <summary>
    /// Initializes a new instance of the FileService class.
    /// </summary>
    /// <param name="webHostEnvironment">The web host environment for accessing wwwroot.</param>
    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Saves a receipt file from Base64 string to wwwroot/receipts.
    /// </summary>
    /// <param name="base64Content">Base64-encoded file content.</param>
    /// <param name="fileName">Name of the file to save (e.g., "receipt.pdf").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The relative file path where the receipt was saved (e.g., "receipts/12345_receipt.pdf").</returns>
    public async Task<string> SaveReceiptFromBase64Async(string base64Content, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            // Ensure wwwroot/receipts directory exists
            var wwwrootPath = _webHostEnvironment.WebRootPath
                ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            var receiptDirectoryPath = Path.Combine(wwwrootPath, ReceiptDirectory);

            if (!Directory.Exists(receiptDirectoryPath))
            {
                Directory.CreateDirectory(receiptDirectoryPath);
            }

            // Generate unique filename to avoid collisions
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var uniqueFileName = $"{timestamp}_{Path.GetFileName(fileName)}";
            var filePath = Path.Combine(receiptDirectoryPath, uniqueFileName);

            // Decode Base64 and write to file
            var fileBytes = Convert.FromBase64String(base64Content);
            await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);

            // Return relative path for database storage
            return $"{ReceiptDirectory}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save receipt file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes a receipt file from wwwroot/receipts.
    /// </summary>
    /// <param name="filePath">Relative file path (e.g., "receipts/12345_receipt.pdf").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if deletion succeeded; false otherwise.</returns>
    public async Task<bool> DeleteReceiptAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            var wwwrootPath = _webHostEnvironment.WebRootPath
                ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            var fullPath = Path.Combine(wwwrootPath, filePath);

            // Security check: ensure file is within receipts directory
            var receiptDirectoryPath = Path.Combine(wwwrootPath, ReceiptDirectory);
            var fullResolvedPath = Path.GetFullPath(fullPath);
            var receiptResolvedPath = Path.GetFullPath(receiptDirectoryPath);

            if (!fullResolvedPath.StartsWith(receiptResolvedPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Attempted to delete file outside receipts directory.");
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                await Task.CompletedTask; // Simulate async operation
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting receipt file: {ex.Message}");
            return false;
        }
    }
}
