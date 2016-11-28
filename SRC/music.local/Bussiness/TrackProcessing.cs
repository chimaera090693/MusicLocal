using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using music.local.Models;
using NAudio;
using NAudio.Wave;
using WebGrease.Css.Ast.Selectors;

namespace music.local.Bussiness
{
    public class TrackProcessing
    {
        /// <summary>
        /// build tree object
        /// </summary>
        /// <returns></returns>
        public static List<SoundTrackModel> GetTree()
        {
            List<SoundTrackModel> list = new List<SoundTrackModel>();

            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var stParent = new SoundTrackModel();
            stParent.FilePath = "";
            list = ReclusiveTree(physPath, (int)TrackType.Singer,ref stParent);
            return list;
        }

        /// <summary>
        /// đệ quy cây thư mục
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="lever"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static List<SoundTrackModel> ReclusiveTree(string parentPath, int lever, ref SoundTrackModel parent)
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
                        if (!(dirInf.Name == "image" && string.IsNullOrEmpty(parent.FilePath)))
                        {
                            SoundTrackModel st = new SoundTrackModel();
                            st.ItemType = lever;
                            st.Name = dirInf.Name;
                            st.Gid = Guid.NewGuid().ToString().Replace("-", "");
                            st.ParentGid = parent.Gid;
                            st.FilePath = parent.FilePath + "\\" + dirInf.Name;
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
                        if(extension != null && (extension.ToLower() != ".mp3"))
                        {
                            parent.CoverPath = parent.FilePath + "\\" + file.Name;
                        }
                        else
                        {
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

        
    }
}