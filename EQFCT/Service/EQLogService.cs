using EQFCT.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace EQFCT.Service
{
    /// <summary>
    /// Log Monitoring service, uses FileSystemWatcher to trigger change event on file, we keep track of the last known location of file
    /// and process only new lines in the log file.
    /// </summary>
    public class EqLogService
    {
        private bool FirstTime = true;
        private FileSystemWatcher fFileWatcher = new FileSystemWatcher();
        private long Position = 0;

        public void MonitorLog(string pLogFilePath)
        {
            FirstTime = true;
            fFileWatcher.EnableRaisingEvents = false;
            fFileWatcher.Changed -= new FileSystemEventHandler(onChanged);
            fFileWatcher.Dispose();
            fFileWatcher = new FileSystemWatcher();
            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = "Validating Log File" });
            if (File.Exists(pLogFilePath))
            {
                Properties.Settings.Default.LogFile = pLogFilePath;
                Properties.Settings.Default.Save();
                var DirectoryPath = Path.GetDirectoryName(pLogFilePath);
                var FileName = Path.GetFileName(pLogFilePath);

                Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = "Starting Initial Log Parse" });
                onChanged(this, new FileSystemEventArgs(WatcherChangeTypes.All, DirectoryPath, FileName));
                Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = "Finished. Monitoring: "+ FileName });

                var vTest = new FileSystemEventHandler(onChanged);
                fFileWatcher.Path = DirectoryPath;
                fFileWatcher.Filter = FileName;
                fFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fFileWatcher.Changed += new FileSystemEventHandler(onChanged);
                fFileWatcher.EnableRaisingEvents = true;
            }
            else
            {
                Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = "I don't think you entered a valid Log File, try again?" });
            }
        }

        private void onChanged(object pSender, FileSystemEventArgs pFileSystemEventArgs)
        {
            string Line = string.Empty;
            try
            {
                var fs = new FileStream(pFileSystemEventArgs.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (FirstTime) Position = fs.Length;
                fs.Position = Position;
                using (StreamReader sr = new StreamReader(fs))
                {
                    while ((Line = sr.ReadLine()) != null)
                    {
                        Line = Line.ToLower();

                        //You take damage
                        MatchCollection mc = Regex.Matches(Line, "(.+)(\\w+) you for (\\d+) points? of damage");
                        if (mc.Count > 0)
                        {
                            var vDamage = Convert.ToInt32(mc[0].Groups[3].Value);
                            var vDmgModel = new DmgModel()
                            {
                                Text = String.Format("-{0:n0}", vDamage),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            vDmgModel.IsCritical = Line.Contains("(critical)");
                            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }
                        //Mob Misses You
                        mc = Regex.Matches(Line, "(.+) tries to (.+) you, but misses!");
                        if (mc.Count > 0)
                        {
                            var vDmgModel = new DmgModel()
                            {
                                Text = String.Format("miss"),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            vDmgModel.IsCritical = Line.Contains("(critical)");
                            vDmgModel.IsMiss = true;
                            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }
                        //You resist a spell
                        mc = Regex.Matches(Line, "you (resist) (.+)");
                        if (mc.Count > 0)
                        {
                            var vDamage = mc[0].Groups[2].Value;
                            var vDmgModel = new DmgModel()
                            {
                                Text = string.Format("resist {0}", vDamage),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }


                        //You melee
                        mc = Regex.Matches(Line, "you (crush|backstab|hit|punch|kick|slash|bite|bash|pierce|strike) (.+) for (\\d+) points of");
                        if (mc.Count > 0)
                        {
                            var vDamage = Convert.ToInt32(mc[0].Groups[3].Value);
                            var vDmgModel = new DmgModel()
                            {
                                Text = String.Format("{0:n0}", vDamage),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            vDmgModel.IsCritical = Line.Contains("(critical)");
                            Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }

                        //You Miss
                        mc = Regex.Matches(Line, "you try to (crush|backstab|hit|punch|kick|slash|bite|bash|pierce|strike) (.+), but miss!");
                        if (mc.Count > 0)
                        {
                            var vDmgModel = new DmgModel()
                            {
                                Text = String.Format("miss"),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            vDmgModel.IsCritical = Line.Contains("(critical)");
                            vDmgModel.IsMiss = true;
                            Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }

                        //You get healed
                        mc = Regex.Matches(Line, "(.+) (\\w+) healed you for (\\d+)(.+)");
                        if (mc.Count > 0)
                        {
                            var vDamage = Convert.ToInt32(mc[0].Groups[3].Value);
                            var vDmgModel = new DmgModel()
                            {
                                Text = String.Format("+{0:n0}", vDamage),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            vDmgModel.IsHeal = true;
                            Messenger.Default.Send<DmgTakenMessage>(new DmgTakenMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }

                        //You Heal
                        //You healed (.+) for (\d+)(.+)
                        mc = Regex.Matches(Line, "you healed (.+) for (\\d+)(.+)");
                        if (mc.Count > 0)
                        {
                            var vDamage = Convert.ToInt32(mc[0].Groups[2].Value);
                            var vDmgModel = new DmgModel()
                            {
                                Text = String.Format("+{0:n0}", vDamage),
                                Left = 0,
                                Top = 0,
                                FontSize = 24
                            };
                            vDmgModel.IsHeal = true;
                            Messenger.Default.Send<DmgDoneMessage>(new DmgDoneMessage() { Damage = vDmgModel });
                            Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = Line });
                        }
                    }
                    Position = fs.Position;
                    FirstTime = false;
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = "Error on Line: " + Environment.NewLine + Line });
                Messenger.Default.Send<GenericMessage>(new GenericMessage() { Message = "Critical Error detected: " + Environment.NewLine + ex.Message });

                //Set First Time to skip to end of log and bypass messages causing issues
                FirstTime = true;
            }
        }
    }
}
