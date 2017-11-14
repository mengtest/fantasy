﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace SuperBoBo
{
    public class BuildTool
    {



        static CustomBuildLog lastBuildLog;
        static CustomBuildLog newBuildLog;

        /// <summary>
        /// fileName,MD5
        /// </summary>
        static Dictionary<string, string> lastestCSMD5 = new Dictionary<string, string>();

        /// <summary>
        /// fileName,MD5
        /// </summary>
        static Dictionary<string, string> newCSMD5 = new Dictionary<string, string>();


        public static string versionNameFile = "config/buildVersion.txt";
        public static string versionResFile = "config/versionRes.txt";
        public static string versionCodeFile = "config/versionCode.txt";
        public static string isDevFile = "config/dev.txt";
        public static string tempDataFile = "config/buildLast.json";
        public static string buildLogFile = "config/buildLog.json";
        public static string diffLogFile = "config/buildDiff.json";
        public static string startBuildSceneName = "config/startScene.txt";
        public static string sceneConfigPath = "config/scenes.txt";



        static string buildLogContent;

        /// <summary>
        /// 资源版本号
        /// </summary>
        public static Version versionRes
        {
            get
            {
                if (_versionRes == null)
                {
                    if (File.Exists(versionResFile))
                    {
                        string content = FileUtilTool.ReadFile(versionResFile);

                        content = content.Replace("\r\n", "\n").Replace("\n", "");

                        _versionRes = new Version(content);
                    }
                    else
                    {
                        _versionRes = new Version(0, 0, 0);
                    }
                }

                return _versionRes;
            }
        }

        private static Version _versionRes;

        /// <summary>
        /// APP版本号
        /// </summary>
        public static Version versionName
        {
            get
            {
                if (_versionName == null)
                {
                    if (File.Exists(versionNameFile))
                    {
                        string content = FileUtilTool.ReadFile(versionNameFile);
                        _versionName = new Version(content);
                    }
                    else
                    {
                        _versionName = new Version(0, 0, 0);
                    }

                    UnityEditor.PlayerSettings.bundleVersion = _versionName.ToString();
                }
                return _versionName;
            }
        }

        public static string startScene
        {
            get
            {
                if (File.Exists(startBuildSceneName))
                {
                    string content = FileUtilTool.ReadFile(startBuildSceneName);

                    content = content.Replace("\r\n", "\n").Replace("\n", "");

                    return content;
                }
                else
                {
                    Debug.LogError("no find first scene name");
                    throw new System.Exception("no find first scene name,make sure config/startScene.txt is set");
                }
            }
        }


        private static Version _versionName;

        public static int versionCode
        {
            get
            {

                string cont = FileUtilTool.ReadFile(versionCodeFile);
                int codeVersion = 1;
                if (!string.IsNullOrEmpty(cont))
                {
                    string[] blines = cont.Replace("\r\n", "\n").Split('\n');
                    codeVersion = int.Parse(blines[0]);
                }
                PlayerSettings.Android.bundleVersionCode = codeVersion;
                return codeVersion;
            }
        }

        private static bool _isDev;

        public static bool isDev
        {
            get
            {

                string cont = FileUtilTool.ReadFile(isDevFile);
                bool _isDev = false;
                if (!string.IsNullOrEmpty(cont))
                {
                    string[] blines = cont.Replace("\r\n", "\n").Split('\n');
                    _isDev = bool.Parse(blines[0]);
                }

                return _isDev;
            }
        }


        [System.Serializable]
        public class FileMD5Data
        {
            public string filename;
            public string md5;

            public FileMD5Data()
            {

            }

            public FileMD5Data(string f, string m)
            {
                filename = f;
                md5 = m;
            }
        }

        [System.Serializable]
        public class CustomBuildLog
        {
            public string version;

            public List<FileMD5Data> csmd5;

            public CustomBuildLog()
            {
                csmd5 = new List<FileMD5Data>();
            }
            public void AddData(FileMD5Data data)
            {
                csmd5.Add(data);
            }
        }

        public static string[] stepNames =
        {
        "①删除记录文件",
        "②生成记录文件",
        "③打标签",
        "④生成Assetbundle资源",
        "⑤清除标签",
        "⑥清除清单文件",
        "⑦压缩资源包",
        "⑧拷贝流媒体到版本资源目录",
    };


        [System.Serializable]
        public class BuildLog
        {
            public static int allStep = 7;
            public int curentStep;
            public string errorLog;


            public void First()
            {
                curentStep = 0;
                EditorUtility.DisplayProgressBar("安卓打包", "打包资源 " + BuildTool.stepNames[BuildTool.buildLog.curentStep],
                        (float)BuildTool.buildLog.curentStep / (float)BuildTool.BuildLog.allStep);
            }

            public void Next()
            {
                BuildTool.buildLog.curentStep++;
                EditorUtility.DisplayProgressBar("安卓打包", "打包资源 " + BuildTool.stepNames[BuildTool.buildLog.curentStep],
                    (float)BuildTool.buildLog.curentStep / (float)BuildTool.BuildLog.allStep);
            }

            public void End()
            {
                EditorUtility.ClearProgressBar();
            }


        }

        public static BuildLog buildLog = new BuildLog();


        public static void SaveBuildLog(BuildLog buildLog)
        {
            string data = JsonMapper.ToJson(buildLog);
            FileUtilTool.WriteFile(buildLogFile, data);
        }

        public static void ReadLastBuildLog()
        {
            try
            {

                Debug.Log(versionName);
                Debug.Log(versionRes);
                Debug.Log(versionCode);


                if (File.Exists(tempDataFile))
                {
                    string content = FileUtilTool.ReadFile(tempDataFile);

                    lastBuildLog = JsonMapper.ToObject<CustomBuildLog>(content);

                    foreach (var k in lastBuildLog.csmd5)
                    {
                        lastestCSMD5.Add(k.filename, k.md5);
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        static void UpgradeAPKVer()
        {
            versionRes.UpgradeLittleVersion();

            UpgradeCodeVersion();
        }

        static void UpgradeResVer()
        {
            versionRes.UpgradeLittleVersion();

            UpgradeCodeVersion();
        }

        public static string svnpath = Application.streamingAssetsPath;

        public static void CallShellCommand()
        {
            string srcdir = svnpath;
            srcdir = srcdir.Replace("\\", "/");

            System.Diagnostics.Process p = new System.Diagnostics.Process();


            p.StartInfo.FileName = "svn";

            p.StartInfo.WorkingDirectory = srcdir;

            p.StartInfo.Arguments = "update";
            p.StartInfo.CreateNoWindow = true;
            Debug.Log("cmd-> " + p.StartInfo.FileName + p.StartInfo.Arguments);
            p.Start();
            p.WaitForExit();
        }

        public static void CallShellCommandForVersionCode()
        {

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "svn";
            p.StartInfo.Arguments = " commit " + versionCodeFile + " -m [VersionCode]提交";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }

        [MenuItem("Package/工具/提交VersionRes")]
        public static void CallShellCommandForVersionRes()
        {

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "svn";
            p.StartInfo.Arguments = " commit " + versionRes + " -m [VersionRes]提交";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }


        [MenuItem("Package/工具/提交VersionCode")]
        public static void UpgradeCodeVersion()
        {
            PlayerSettings.Android.bundleVersionCode = versionCode + 1;
            FileUtilTool.WriteFile(versionCodeFile, PlayerSettings.Android.bundleVersionCode.ToString());
            FileUtilTool.WriteFile(Application.streamingAssetsPath + "/" + versionCodeFile, PlayerSettings.Android.bundleVersionCode.ToString());
            CallShellCommandForVersionCode();
            CallShellCommandForVersionRes();
        }

        [MenuItem("Package/工具/更新到最新")]
        public static void UpdateStreaming()
        {
            //确保svn最新
            FileUtilTool.DeleteFolder(Application.streamingAssetsPath);
            FileUtilTool.CreateFolder(Application.streamingAssetsPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            CallShellCommand();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }



        public enum BuildType
        {
            Project,
            APK
        }

        public static BuildType type = BuildType.APK;

        [MenuItem("Package/静默打包")]
        public static void StartBuildQuiet()
        {
            Caching.CleanCache();
            lastestCSMD5.Clear();
            newCSMD5.Clear();
            ReadLastBuildLog();
            newBuildLog = new CustomBuildLog();
            type = BuildType.APK;
            Debug.Log("开始静默打包");
            CheckScriptsQuiet();
        }

        //[MenuItem("Package/工程")]
        public static void StartBuildQuietProject()
        {
            Caching.CleanCache();
            lastestCSMD5.Clear();
            newCSMD5.Clear();
            ReadLastBuildLog();
            newBuildLog = new CustomBuildLog();
            type = BuildType.Project;
            Debug.Log("开始静默打工程");
            CheckScriptsQuiet();
        }

        [MenuItem("Package/QA专用")]
        public static void StartBuild()
        {

            Caching.CleanCache();
            lastestCSMD5.Clear();
            newCSMD5.Clear();
            if (EditorUtility.DisplayDialog("打包", "请确认Resources目录和StreamingAssets目录为SVN最新资源", "是", "否"))
            {
                ReadLastBuildLog();
                newBuildLog = new CustomBuildLog();
                CheckScripts();
            }
        }

        static void BuildQuietScriptDiff()
        {

            UpgradeAPKVer();

            Version.SaveVersion2StreamingAsset(versionRes);

            BuildAPK();

            SaveLastBuildLog();

            MarkTool.SaveLog();




        }

        static void BuildQuietResources()
        {

            UpgradeResVer();

            Version.SaveVersion2StreamingAsset(versionRes);

            BuildAPK();

            SaveLastBuildLog();

            MarkTool.SaveLog();
        }

        static void ShowScriptDiff(List<string> datas)
        {
            string content = "程序差异清单:\r\n";
            foreach (string s in datas)
            {
                content += s + "\r\n";
            }

            content += "资源清单:\r\n";
            string rescontent = GetDiffRes();
            content += rescontent;

            CustomDialog.Show("打包", content, "继续", "取消", () =>
            {

                UpgradeAPKVer();

                Version.SaveVersion2StreamingAsset(versionRes);

                BuildAPK();

                SaveLastBuildLog();

                MarkTool.SaveLog();

            }, () =>
            {



            });


        }


        static void BuildAPK()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    StandaloneTool.BuildStandaloneFull();
                    break;
                case BuildTarget.iOS:
                    IOSTool.BuildIOSFull();
                    break;
             
                case BuildTarget.Android:
                    AndroidTool.BuildApkGenRes();
                    break;
                default:
                    break;
            }
        }

        static void SaveLastBuildLog()
        {
            newBuildLog.version = versionRes.ToString();
            string jSon = JsonMapper.ToJson(newBuildLog);
            FileUtilTool.WriteFile(tempDataFile, jSon);

            FileUtilTool.WriteFile(versionResFile, versionRes.ToString());

        }


        public static void CheckScriptsQuiet()
        {
            List<string> difList = new List<string>();

            List<string> checkFiles = new List<string>();
            string[] csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            string ManifestFile = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
            string[] jarFiles = Directory.GetFiles(Application.dataPath, "*.jar", SearchOption.AllDirectories);

            checkFiles.AddRange(csFiles);
            checkFiles.AddRange(jarFiles);
            checkFiles.Add(ManifestFile);

            int i = 0;
            CustomBuildLog fl = new CustomBuildLog();
            foreach (string f in checkFiles)
            {
                string path = f.Replace('\\', '/');
                string md5 = ResCommon.GetFileMD5(path);

                if (lastestCSMD5.ContainsKey(path))
                {
                    if (lastestCSMD5[path] != md5)
                    {
                        difList.Add(path);
                    }
                }
                else
                {
                    difList.Add(path);
                }

                i++;
                newBuildLog.AddData(new FileMD5Data(path, md5));

                EditorUtility.DisplayProgressBar("安卓打包", "检查脚本 " + f, (float)i / (float)csFiles.Length);
            }

            EditorUtility.ClearProgressBar();

            if (difList.Count > 0)
            {
                BuildQuietScriptDiff();
            }
            else
            {
                BuildQuietResources();
            }

            string json = JsonMapper.ToJson(difList);
            FileUtilTool.DelFile(diffLogFile);
            FileUtilTool.WriteFile(diffLogFile, json);
        }


        public static void CheckScripts()
        {
            List<string> difList = new List<string>();

            List<string> checkFiles = new List<string>();
            string[] csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            string ManifestFile = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
            string[] jarFiles = Directory.GetFiles(Application.dataPath, "*.jar", SearchOption.AllDirectories);

            checkFiles.AddRange(csFiles);
            checkFiles.AddRange(jarFiles);
            checkFiles.Add(ManifestFile);

            int i = 0;
            CustomBuildLog fl = new CustomBuildLog();
            foreach (string f in checkFiles)
            {
                string path = f.Replace('\\', '/');
                string md5 = ResCommon.GetFileMD5(path);

                if (lastestCSMD5.ContainsKey(path))
                {
                    if (lastestCSMD5[path] != md5)
                    {
                        difList.Add(path);
                    }
                }
                else
                {
                    difList.Add(path);
                }

                //Debug.Log(f + " md5:" + md5);
                i++;
                newBuildLog.AddData(new FileMD5Data(path, md5));

                EditorUtility.DisplayProgressBar("安卓打包", "检查脚本 " + f, (float)i / (float)csFiles.Length);
            }

            EditorUtility.ClearProgressBar();

            if (difList.Count > 0)
            {

                if (EditorUtility.DisplayDialog("安卓打包", string.Format("本次打包含程序和资源更新\r\n 原资源版本号:{0}", versionRes), "继续", "取消"))
                {
                    ShowScriptDiff(difList);

                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("安卓打包", string.Format("本次打包只包含资源更新\r\n 原资源版本号:{0}", versionRes), "继续", "取消"))
                {
                    CheckResources();
                }
            }

            //Debug.Log("diff list :");
            //foreach (string str in difList)
            //{
            //    Debug.Log(str);
            //}

            string json = JsonMapper.ToJson(difList);
            FileUtilTool.DelFile(diffLogFile);
            FileUtilTool.WriteFile(diffLogFile, json);
        }

        /// <summary>
        /// 获得不同的资源，包括新增和修改
        /// </summary>
        /// <returns></returns>
        public static string GetDiffRes()
        {
            string rescontent = "";

            //遍历所有的Resource目录
            Analysis.Analysing();


            List<string> files = new List<string>();

            MarkTool.FitterRecord(files);


            //差异化打包
            List<MarkTool.RecordInfo> localRecords = MarkTool.LoadRecordFile(MarkTool.recordPath);
            List<MarkTool.RecordInfo> newRecords = MarkTool.GenRecords(files.ToArray());
            List<MarkTool.RecordInfo> rets = MarkTool.CompareRecord(newRecords, localRecords);

            List<string> resfiles = new List<string>();

            foreach (var k in rets)
            {
                resfiles.Add(k.fileName);
            }

            rescontent = string.Join("\n", resfiles.ToArray());

            return rescontent;
        }

        public static void CheckResources()
        {
            string rescontent = GetDiffRes();

            if (rescontent != "")
            {

                CustomDialog.Show("资源更新", rescontent, "继续", "取消", () =>
                {
                    UpgradeResVer();

                    Version.SaveVersion2StreamingAsset(versionRes);

                    BuildAPK();

                    SaveLastBuildLog();

                    MarkTool.SaveLog();
                },

                () =>
                {



                });
            }
            else
            {

                if (EditorUtility.DisplayDialog("打包", "本次无更新,仍要出资源？", "是", "否"))
                {
                    UpgradeResVer();

                    Version.SaveVersion2StreamingAsset(versionRes);

                    BuildAPK();

                    SaveLastBuildLog();

                    MarkTool.SaveLog();
                }
            }


        }

        public static void SwitchActiveBuildTarget(BuildTarget target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        }

        public static void ShowResourcesFolder(bool b)
        {
            if (b)
            {
                if (Directory.Exists("Assets/Resources"))
                {
                    Debug.LogError("存在目录Assets/Resources");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    throw new System.Exception("已经存在目录Assets/Resources，build失败");
                }
                Debug.Log(AssetDatabase.MoveAsset("Assets/Res", "Assets/Resources"));
            }
            else
            {
                if (Directory.Exists("Assets/Res"))
                {
                    Debug.LogError("存在目录Assets/Res");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    throw new System.Exception("已经存在目录Assets/Res,build失败");


                }
                Debug.Log( AssetDatabase.MoveAsset("Assets/Resources", "Assets/Res"));
            }
        }


        [MenuItem("Package/工具/导出场景信息")]
        public static void ExportScene()
        {
            string[] scenes = MarkTool.GetSceneArray();
            string content = string.Join("\n", scenes);
            FileUtilTool.WriteFile(sceneConfigPath, content);
        }

        /// <summary>
        /// 外部调用
        /// </summary>
        public static void RefreshAndWaitForCompile()
        {
            AssetDatabase.ImportAsset("Assets/Plugins/Android", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset("Assets/Scripts", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset("ProjectSettings", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();
            EditorApplication.update += WaitCompile;
        }

        public static void WaitCompile()
        {
            if (!EditorApplication.isCompiling)
            {
                Debug.Log("编译结束");
                EditorApplication.update -= WaitCompile;

            }
        }
    }
}
