using System.IO;

namespace ArchiWindRevitAddIn.Tasks
{
    public sealed class TempDir : IDisposable
    {
        private bool _isDisposed;

        private TempDir(string fullName)
        {
            FullName = fullName;
        }

        public string FullName { get; }

        public static TempDir Create(string? prefix = null)
        {
#if NET7_0_OR_GREATER
            var tempDir = Directory.CreateTempSubdirectory(prefix);
            return new TempDir(tempDir.FullName);
#else
            var fullName = Path.Combine(Path.GetTempPath(), $"{prefix}{Guid.NewGuid()}");
            Directory.CreateDirectory(fullName);
            return new TempDir(fullName);
#endif
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Directory.Delete(FullName, recursive: true);
            }
        }

        public override string ToString() => $"{nameof(TempDir)}({FullName})";
    }
}
