using Windows.Storage.Pickers;
using X_Open_Launcher_Publisher;

public static class FileDialog
{
    public static string Path { get; set; }
    public static async Task<bool> SaveAndOpenPublishFile()
    {
        var folderPicker = new FolderPicker();
        var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

        // Associate the HWND with the file picker
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        var result = await folderPicker.PickSingleFolderAsync();
        if (result != null)
        {
            Path = result.Path;
            return true;
        }
        return false;
    }
    public static async Task<FileResult> OpenImageFile()
    {
        return await FilePicker.PickAsync();
    }
}

