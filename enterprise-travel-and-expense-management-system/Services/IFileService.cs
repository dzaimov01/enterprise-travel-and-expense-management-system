namespace enterprise_travel_and_expense_management_system.Services;

/// <summary>
/// Service for handling file operations (receipt uploads, storage).
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Saves a receipt file from Base64 string.
    /// </summary>
    /// <param name="base64Content">Base64-encoded file content.</param>
    /// <param name="fileName">Name of the file to save (e.g., "receipt.pdf").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The relative file path where the receipt was saved (e.g., "receipts/12345_receipt.pdf").</returns>
    Task<string> SaveReceiptFromBase64Async(string base64Content, string fileName, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a receipt file by its path.
    /// </summary>
    /// <param name="filePath">Relative file path (e.g., "receipts/12345_receipt.pdf").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if deletion succeeded; false otherwise.</returns>
    Task<bool> DeleteReceiptAsync(string filePath, CancellationToken cancellationToken);
}
