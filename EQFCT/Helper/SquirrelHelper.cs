using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NuGet;
using Squirrel;

namespace EQFCT.Helper
{
    public static class SquirrelHelper
    {
        #region Squirrel

        private static string _newVersion;

        public static string NewVersion
        {
            get => _newVersion;
            private set
            {
                if (_newVersion == value)
                    return;
                _newVersion = value;
            }
        }

        private static SemanticVersion _currentVersion;

        public static SemanticVersion CurrentVersion
        {
            get
            {
                if (_currentVersion == null)
                {
                    _currentVersion = new SemanticVersion(0, 0, 0, 0);

                    try
                    {
                        using (var mgr = new UpdateManager("C:"))
                        {
                            if (mgr.CurrentlyInstalledVersion() != null)
                                _currentVersion = mgr.CurrentlyInstalledVersion();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Unable to load Squirrel Version.");
                    }
                }

                return _currentVersion;
            }
        }

        private static readonly string S3UpdatePath = "https://opendkp-publisher.s3.us-east-2.amazonaws.com/eqfct";

        private static IUpdateManager GetUpdateManager()
        {
            return new UpdateManager(S3UpdatePath);
        }

        public static async Task<bool> IsUpdateAvailable(Action<int> progress = null)
        {
            if (CurrentVersion == new SemanticVersion(0, 0, 0, 0))
                return false;

            try
            {
                Debug.WriteLine("SquirrelHelper: Checking for Update");
                using (var mgr = GetUpdateManager())
                {
                    var update = await mgr.CheckForUpdate(progress: progress);
                    if (update.ReleasesToApply.Any())
                    {
                        Debug.WriteLine("SquirrelHelper: Update available");
                        NewVersion = update.FutureReleaseEntry.Version.ToString();

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.Message.StartsWith("Update.exe not found"))
                    Debug.WriteLine("SquirrelHelper: Error with Squirrel.");
            }

            return false;
        }

        public static async void UpdateSquirrel(Action<int> progress = null,
            Action<bool, SemanticVersion> finished = null)
        {
            try
            {
                Debug.WriteLine("SquirrelHelper: Doing Update");

                using (var mgr = GetUpdateManager())
                {
                    var release = await mgr.UpdateApp(progress);
                    Debug.WriteLine("Update finished");

                    finished?.Invoke(true, release.Version);
                }
            }
            catch (Exception e)
            {
                if (!e.Message.StartsWith("Update.exe not found"))
                    Debug.WriteLine("SquirrelHelper: Error with Squirrel.");
                finished?.Invoke(false, null);
            }
        }

        #endregion Squirrel
    }
}