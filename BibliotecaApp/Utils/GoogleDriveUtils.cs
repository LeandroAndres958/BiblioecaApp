namespace BibliotecaApp.Utils
{
    public static class GoogleDriveUtils
    {
        public static string ConvertirLinkGoogleDrive(string linkOriginal)
        {
            try
            {
                string id = "";

                if (linkOriginal.Contains("/d/"))
                {
                    int start = linkOriginal.IndexOf("/d/") + 3;
                    int end = linkOriginal.IndexOf('/', start);
                    if (end == -1)
                        end = linkOriginal.Length;
                    id = linkOriginal.Substring(start, end - start);
                }
                else if (linkOriginal.Contains("id="))
                {
                    int start = linkOriginal.IndexOf("id=") + 3;
                    int end = linkOriginal.IndexOf('&', start);
                    if (end == -1)
                        end = linkOriginal.Length;
                    id = linkOriginal.Substring(start, end - start);
                }
                else
                {
                    return linkOriginal;
                }

                return $"https://drive.google.com/uc?export=download&id={id}";
            }
            catch
            {
                return linkOriginal;
            }
        }
    }
}
