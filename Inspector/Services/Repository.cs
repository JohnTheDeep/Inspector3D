using System;
using System.IO;

namespace WpfApp4
{
    internal class Repository : IDisposable
    {
        private bool disposed = false;
        private static string modelsDirectory = Path.Combine($@"{Environment.CurrentDirectory}", "3dmodels");
        private static string modelFolderPath, modelName, semiModelName;
        public Repository(string modelPath)
        {
            try
            {
                if (File.Exists(modelPath))
                {
                    modelName = Path.GetFileNameWithoutExtension(modelPath);
                    modelFolderPath = Path.Combine(modelsDirectory, modelName);
                    semiModelName = Path.GetFileName(modelPath);

                    if (!Directory.Exists(modelsDirectory))
                    { Directory.CreateDirectory(modelsDirectory); }

                    if (!Directory.Exists(modelFolderPath))
                    { Directory.CreateDirectory(modelFolderPath); }

                    if (!File.Exists(Path.Combine(modelFolderPath, semiModelName)))
                        File.Copy(modelPath, Path.Combine(modelFolderPath, semiModelName), false);
                }
                else { throw new Exception(nameof(modelPath), new FileNotFoundException()); }
            }
            catch (FileNotFoundException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                disposed = true;
            }
        }
    }
}
