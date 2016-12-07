using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using music.local.Models;

namespace music.local.Bussiness
{
    public class TrackProcessing
    {
        private static string[] arrImageExt = new[] { ".png", ".bmp", ".jpg", ".jpeg" };

        #region Audio
        /// <summary>
        /// build tree object for audio
        /// </summary>
        /// <returns></returns>
        public static List<SoundTrackModel> GetTree()
        {
            List<SoundTrackModel> list;

            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var stParent = new SoundTrackModel();
            stParent.FilePath = "";
            list = ReclusiveTree(physPath, (int)TrackType.Singer, ref stParent);
            return list;
        }

        /// <summary>
        /// đệ quy cây thư mục
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="lever"></param>
        /// <param name="parent"></param>
        /// <param name="extType">type file(".mp3" or ".mp4")</param>
        /// <returns></returns>
        private static List<SoundTrackModel> ReclusiveTree(string parentPath, int lever, ref SoundTrackModel parent, string extType=".mp3")
        {
            try
            {
                var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
                var list = new List<SoundTrackModel>();
                //find all dir
                var listDir = Directory.EnumerateDirectories(parentPath, "*", SearchOption.TopDirectoryOnly).ToList();
                if (listDir.Any())
                {
                    //foreach folder
                    foreach (var item in listDir)
                    {
                        var dirInf = new DirectoryInfo(item);
                        if (!((dirInf.Name == "image" || dirInf.Name == "video") && string.IsNullOrEmpty(parent.FilePath)))
                        {
                            SoundTrackModel st = new SoundTrackModel();
                            st.ItemType = lever;
                            st.Name = dirInf.Name;
                            st.Gid = Guid.NewGuid().ToString().Replace("-", "");
                            st.ParentGid = parent.Gid;
                            st.FilePath = parent.FilePath + "\\" + dirInf.Name;
                            if (extType.Equals(".mp3"))
                               st.ListTrack = ReclusiveTree(physPath + "\\" + st.FilePath, lever + 1, ref st);
                            list.Add(st);
                        }
                    }
                }

                var listFile = Directory.EnumerateFiles(parentPath, "*", SearchOption.TopDirectoryOnly).ToList();
                if (listFile.Any())
                {
                    var count = 0;
                    //foreach file
                    foreach (var item in listFile)
                    {
                        var file = new FileInfo(item);
                        var extension = Path.GetExtension(item);
                        if (extension != null && (extension.ToLower() != extType) && arrImageExt.Contains(extension.ToLower()))
                        {
                            extension = extension.ToLower();
                            // image file
                                parent.CoverPath = parent.FilePath + "\\" + file.Name;
                        }
                        else
                        {
                            //mp3 file
                            SoundTrackModel st = new SoundTrackModel();
                            st.ItemType = (int)TrackType.Track;
                            st.order = count;
                            st.Name = file.Name;
                            st.ParentGid = parent.Gid;
                            st.CoverPath = parent.Name;
                            st.FilePath = parent.FilePath + "\\" + file.Name;
                            //st.ListTrack = ReclusiveTree(st.FilePath, lever + 1, ref st);
                            count++;
                            list.Add(st);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name, ex.Message + ex.StackTrace);
                return null;
            }
        }

        #endregion 

        public static List<SoundTrackModel> GetVideoList()
        {
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];

            var stParent = new SoundTrackModel();
            stParent.FilePath = "";
            var list = ReclusiveTree(physPath+"\\Video", (int)TrackType.Singer, ref stParent, ".mp4");
            return list;
        }

    }
}