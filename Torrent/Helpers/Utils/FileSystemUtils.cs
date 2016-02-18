using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Torrent.Infrastructure;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils
{
    public static class FileSystemUtils
    {
        static readonly Logger logger = LogManager.GetLogger("Simple.FSU.Mover");

        static LogEventInfoCloneable getLogEventSubject(string subject, Dictionary<string, object> newEventDict = null)
        {
            var logEventClassBase = new LogEventInfoCloneable(LogLevel.Info, logger.Name, "File System Mover");
            return logEventClassBase.Clone(subject: subject, newEventDict: newEventDict);
        }

        public enum MoveFileResultStatus
        {
            InProgress,
            Error,
            DupeError,
            Dupe,
            Success
        }

        public struct MoveFileResult
        {
            public MoveFileResultStatus Status;
            public string NewFileName;
            public bool NewlyAdded;

            public MoveFileResult(string newFileName, bool newlyAdded = false,
                                  MoveFileResultStatus status = MoveFileResultStatus.InProgress)
            {
                Status = status;
                NewFileName = newFileName;
                NewlyAdded = newlyAdded;
            }
        }

        public static async Task<MoveFileResult> MoveAddedFile(FileInfo fi, string addedDir, TorrentLabel label = null,
                                                               bool doMove = true, bool doLogInfo = false,
                                                               string addedFileName = null, string addedLabel = null)
        {
            var filename = fi.FullName;
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var newFilenameSuffix = "";
            var newDirectory = fi.DirectoryName;
            newFilenameSuffix = "";

#if DEBUG || TRACE_EXT
			if (addedFileName != null && addedFileName != filename) {
				logger.WARN("{0} to move '{1}' does not match added {0} '{2}'", "File Name", filename, addedFileName);
			}
			if (addedLabel != null && addedLabel != label.Computed) {
				logger.WARN("{0} to move '{1}' does not match added {0} '{2}'", "Label", label.Computed, addedLabel);
			}
			#endif

            if (doMove) {
                newDirectory = (label == null ? addedDir : Path.Combine(addedDir, label.Base));
            } else {
                newFilenameSuffix = ".loaded";
            }
            var newFilename = Path.Combine(newDirectory, fi.Name + newFilenameSuffix);
            newFilename = newFilename.Replace(nameWithoutExtension + "\\" + nameWithoutExtension, nameWithoutExtension);
            return await MoveFile(fi, newFilename, doLogInfo, newFilenameSuffix, true);
        }


        public static async Task<MoveFileResult> MoveFile(FileInfo fi, string newFileName, bool doLogInfo = true,
                                                          string newFilenameSuffix = "", bool newlyAdded = false)
        {
            var filename = fi.FullName;
            var extension = fi.Extension.Substring(1);
            var extensionTitle = extension.Capitalize();
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);
            var newDirectory = Path.GetDirectoryName(newFileName);
            var result = new MoveFileResult(filename, newlyAdded);

            Directory.CreateDirectory(newDirectory);

            FileComparer comparer = null;

            var i = 0;

            var eventDict = new Dictionary<string, object>
                            {
                                {"Original Path", filename},
                                {"New Path", newFileName}
                            };
            var subjectEvent = getLogEventSubject("FSU.Mover", eventDict);

            while (File.Exists(newFileName)) {
                if (comparer == null) {
                    comparer = new FileComparer(fi);
                }
                if (comparer.FileEquals(newFileName)) {
                    logger.INFO("Deleting Dupe " + extensionTitle + ": " + filename + " == " + newFileName);
                    result.NewFileName = filename;

                    try {
                        File.Delete(filename);
                        result.Status = MoveFileResultStatus.Dupe;
                        return result;
                    } catch (FileNotFoundException ex) {
                        logger.Error(ex, "Error during deletion: Couldn't open the file");
                    } catch (UnauthorizedAccessException ex) {
                        logger.Error(ex, "Error during deletion: Couldn't access the file");
                    } catch (Exception ex) {
                        logger.Error(ex, "Error during deletion: Unknown Exception");
                    }
                    result.Status = MoveFileResultStatus.DupeError;
                    return result;
                }
                i++;
                newFileName = filenameWithoutExtension + "." + i + "." + extension + newFilenameSuffix;
            }
            bool retry;
            do {
                retry = false;
                try {
                    File.Move(filename, newFileName);
                    if (doLogInfo) {
                        logger.LOG(subjectEvent.Clone("Successfully moved " + extensionTitle));
                    }
                    result.NewFileName = newFileName;
                    result.Status = MoveFileResultStatus.Success;
                    return result;
                } catch (FileNotFoundException ex) {
                    logger.Error(subjectEvent.Clone(ex, "File Not Found while trying to move the " + extensionTitle));
                } catch (UnauthorizedAccessException ex) {
                    logger.Error(subjectEvent.Clone(ex,
                                                    "File Access Error occurred while trying to move the "
                                                    + extensionTitle));
                } catch (IOException ex) {
                    if (ex.Message == "The process cannot access the file because it is being used by another process.") {
                        retry = true;
                    } else {
                        var errorEvent =
                            subjectEvent.Clone(ex, "IO Exception occurred while trying to move the " + extensionTitle)
                                        .SetError();
                        logger.Error(errorEvent);
                    }
                } catch (Exception ex) {
                    logger.Error(subjectEvent.Clone(ex,
                                                    "Unknown Exception occurred while trying to move the "
                                                    + extensionTitle));
                }
                if (retry) {
                    await Task.Delay(1000);
                }
            } while (retry);
            result.Status = MoveFileResultStatus.Error;
            return result;
        }
    }
}
