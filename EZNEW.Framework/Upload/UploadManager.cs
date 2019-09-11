using EZNEW.Framework.Extension;
using EZNEW.Framework.IoC;
using EZNEW.Framework.Net.Http;
using EZNEW.Framework.Serialize;
using EZNEW.Framework.Upload.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// upload manager
    /// </summary>
    public static class UploadManager
    {
        static UploadManager()
        {
            Default = new UploadOption()
            {
                Remote = false
            };
        }

        #region properties

        /// <summary>
        /// default upload option
        /// </summary>
        static UploadOption Default = null;

        /// <summary>
        /// upload object collection
        /// </summary>
        public static Dictionary<string, UploadObject> UploadObjectCollection = new Dictionary<string, UploadObject>();

        #endregion

        #region methods

        #region config upload

        /// <summary>
        /// config upload
        /// </summary>
        /// <param name="uploadConfig">upload config</param>
        public static void ConfigUpload(UploadConfig uploadConfig)
        {
            if (uploadConfig == null)
            {
                return;
            }
            if (uploadConfig.Default != null)
            {
                ConfigDefaultUpload(uploadConfig.Default);
            }
            if (!uploadConfig.UploadObjects.IsNullOrEmpty())
            {
                ConfigUploadObject(uploadConfig.UploadObjects.ToArray());
            }
        }

        #endregion

        #region config default upload

        /// <summary>
        /// config default upload
        /// </summary>
        /// <param name="uploadOption">upload option</param>
        public static void ConfigDefaultUpload(UploadOption uploadOption)
        {
            Default = uploadOption;
        }

        #endregion

        #region config upload object

        /// <summary>
        /// config upload object
        /// </summary>
        /// <param name="uploadObjects">upload objects</param>
        public static void ConfigUploadObject(params UploadObject[] uploadObjects)
        {
            if (uploadObjects.IsNullOrEmpty())
            {
                return;
            }
            foreach (var uploadObject in uploadObjects)
            {
                UploadObjectCollection[uploadObject.Name] = uploadObject;
            }
        }

        #endregion

        #region get upload option

        /// <summary>
        /// get upload option
        /// </summary>
        /// <param name="uploadObjectName">upload object name</param>
        /// <returns></returns>
        static UploadOption GetUploadOption(string uploadObjectName)
        {
            var currentOption = Default;
            if (UploadObjectCollection.TryGetValue(uploadObjectName ?? string.Empty, out var uploadObject) || uploadObject?.UploadOption != null)
            {
                currentOption = uploadObject.UploadOption;
            }
            return currentOption;
        }

        #endregion

        #region upload by config

        /// <summary>
        /// upload by config
        /// </summary>
        /// <param name="fileOptions">file options</param>
        /// <param name="files">files</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        public static async Task<UploadResult> UploadByConfigAsync(IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            if (fileOptions.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(fileOptions));
            }
            if (files.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (files == null || files.Count <= 0)
            {
                throw new ArgumentNullException(nameof(files));
            }
            List<string> uploadObjectGroups = fileOptions.Select(c => c.UploadObjectName).Distinct().ToList();
            UploadResult uploadResult = null;
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFileOptions = fileOptions.Where(c => c.UploadObjectName == uploadObjectName).ToList();
                var groupFiles = files.Where(c => groupFileOptions.Exists(fc => fc.FileName == c.Key)).ToDictionary(c => c.Key, c => c.Value);
                var uploadOption = GetUploadOption(uploadObjectName);
                var groupResult = await UploadByOptionAsync(uploadOption, groupFileOptions, groupFiles, parameters).ConfigureAwait(false);
                if (groupResult == null)
                {
                    continue;
                }
                if (uploadResult == null)
                {
                    uploadResult = groupResult;
                }
                else
                {
                    uploadResult.Combine(groupResult);
                }
            }
            return uploadResult;
        }

        #endregion

        #region upload by upload option

        /// <summary>
        /// upload by upload option
        /// </summary>
        /// <param name="uploadOption">upload option</param>
        /// <param name="fileOptions">file options</param>
        /// <param name="files">files</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        public static async Task<UploadResult> UploadByOptionAsync(UploadOption uploadOption, IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            if (files == null || files.Count <= 0)
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (uploadOption == null)
            {
                throw new ArgumentNullException(nameof(uploadOption));
            }
            UploadResult uploadResult = null;
            if (uploadOption.Remote)
            {
                uploadResult = await RemoteUploadAsync(uploadOption.GetRemoteOption(), fileOptions.ToList(), files, parameters).ConfigureAwait(false);
            }
            else
            {
                uploadResult = await LocalUploadAsync(uploadOption, fileOptions.ToList(), files).ConfigureAwait(false);
            }
            return uploadResult;
        }

        #endregion

        #region remote upload

        /// <summary>
        /// remote upload file
        /// </summary>
        /// <param name="remoteOption">remote option</param>
        /// <param name="fileOptions">file options</param>
        /// <param name="files">upload files</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteOption remoteOption, List<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            if (remoteOption == null || string.IsNullOrWhiteSpace(remoteOption.Server))
            {
                throw new ArgumentNullException(nameof(remoteOption));
            }
            if (files == null || files.Count <= 0)
            {
                throw new ArgumentNullException(nameof(files));
            }
            RemoteUploadParameter uploadParameter = new RemoteUploadParameter()
            {
                Files = fileOptions
            };
            parameters = parameters ?? new Dictionary<string, string>();
            parameters.Add(RemoteUploadParameter.REQUEST_KEY, JsonSerialize.ObjectToJson(uploadParameter));
            string url = remoteOption.GetUploadUrl();
            return await HttpUtil.UploadAsync(url, files, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// remote upload file
        /// </summary>
        /// <param name="remoteOption">remote options</param>
        /// <param name="fileOption">file option</param>
        /// <param name="file">upload file</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteOption remoteOption, UploadFile fileOption, byte[] file, object parameters = null)
        {
            if (fileOption == null)
            {
                throw new ArgumentNullException(nameof(fileOption));
            }
            Dictionary<string, string> parameterDic = null;
            if (parameters != null)
            {
                parameterDic = parameters.ObjectToStringDcitionary();
            }
            return await RemoteUploadAsync(remoteOption, new List<UploadFile>() { fileOption }, new Dictionary<string, byte[]>() { { "file1", file } }, parameterDic).ConfigureAwait(false);
        }

        #endregion

        #region local upload

        /// <summary>
        /// local upload file
        /// </summary>
        /// <param name="uploadOption">upload option</param>
        /// <param name="fileOptions">file options</param>
        /// <param name="files">files</param>
        /// <returns></returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadOption uploadOption, List<UploadFile> fileOptions, Dictionary<string, byte[]> files)
        {
            if (fileOptions.IsNullOrEmpty() || files == null || files.Count <= 0)
            {
                return new UploadResult()
                {
                    Code = "400",
                    ErrorMessage = "没有指定任何上传文件配置或文件信息",
                    Success = false
                };
            }
            UploadResult result = new UploadResult()
            {
                Success = true,
                Files = new List<UploadFileResult>(0)
            };
            foreach (var fileItem in files)
            {
                string fileName = fileItem.Key;
                var fileConfig = fileOptions.FirstOrDefault(c => c.FileName == fileName);
                if (fileConfig == null)
                {
                    fileConfig = new UploadFile()
                    {
                        FileName = fileName,
                        Rename = false
                    };
                }
                var fileResult = await LocalUploadFileAsync(uploadOption, fileConfig, fileItem.Value);
                result.Files.Add(fileResult);
            }
            return result;
        }

        /// <summary>
        ///  local upload file
        /// </summary>
        /// <param name="uploadOption">upload option</param>
        /// <param name="fileOption">file option</param>
        /// <param name="file">file</param>
        /// <returns></returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadOption uploadOption, UploadFile fileOption, byte[] file)
        {
            var fileResult = await LocalUploadFileAsync(uploadOption, fileOption, file).ConfigureAwait(false);
            return new UploadResult()
            {
                Code = "200",
                Success = true,
                Files = new List<UploadFileResult>()
                {
                    fileResult
                }
            };
        }

        /// <summary>
        /// local upload file
        /// </summary>
        /// <param name="uploadOption">upload option</param>
        /// <param name="fileOption">file option</param>
        /// <param name="file">file</param>
        /// <returns></returns>
        static async Task<UploadFileResult> LocalUploadFileAsync(UploadOption uploadOption, UploadFile fileOption, byte[] file)
        {
            #region verify parameters

            if (uploadOption == null)
            {
                throw new ArgumentNullException(nameof(uploadOption));
            }
            if (fileOption == null)
            {
                throw new ArgumentNullException(nameof(fileOption));
            }

            #endregion

            #region set save path

            string savePath = uploadOption.SavePath;
            string realSavePath = savePath;

            if (realSavePath.IsNullOrEmpty())
            {
                realSavePath = Directory.GetCurrentDirectory();
            }
            realSavePath = Path.Combine(realSavePath, fileOption.Folder ?? string.Empty);
            if (!Path.IsPathRooted(realSavePath))
            {
                if (uploadOption.SaveToContentRoot)
                {
                    savePath = Path.Combine(string.IsNullOrWhiteSpace(uploadOption.ContentRootPath) ? "content" : uploadOption.ContentRootPath, realSavePath);
                }
                realSavePath = Path.Combine(Directory.GetCurrentDirectory(), savePath);
            }
            if (!Directory.Exists(realSavePath))
            {
                Directory.CreateDirectory(realSavePath);
            }

            #endregion

            #region file suffix

            string suffix = Path.GetExtension(fileOption.FileName).Trim('.');
            if (!string.IsNullOrWhiteSpace(fileOption.Suffix))
            {
                suffix = fileOption.Suffix.Trim('.');
            }

            #endregion

            #region file name

            string fileName = Path.GetFileNameWithoutExtension(fileOption.FileName);
            if (fileOption.Rename)
            {
                fileName = Guid.NewGuid().ToInt64().ToString();
            }
            fileName = string.Format("{0}.{1}", fileName, suffix);

            #endregion

            #region save file

            string fileFullPath = Path.Combine(realSavePath, fileName);
            File.WriteAllBytes(fileFullPath, file);
            string relativePath = Path.Combine(savePath, fileName);

            #endregion

            var result = new UploadFileResult()
            {
                FileName = fileName,
                FullPath = fileFullPath,
                Suffix = Path.GetExtension(fileName).Trim('.'),
                RelativePath = relativePath,
                UploadDate = DateTime.Now,
                OriginalFileName = fileOption.FileName,
                Target = UploadTarget.Local
            };
            return await Task.FromResult(result).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
