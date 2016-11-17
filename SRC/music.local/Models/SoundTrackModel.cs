using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace music.local.Models
{
    public enum TrackType
    {
        Singer =0, //ca sĩ
        Group=1,   //các nhóm sản phẩm
        Album=2,   //album
        Track =3   //tracks
    }
    public class SoundTrackModel
    {
        public string Name { get; set; }
        public string Gid { get; set; }
        public string ParentGid { get; set; }
        public string FilePath { get; set; }
        public string CoverPath { get; set; }
        public int ItemType { get; set; }
        public List<SoundTrackModel> ListTrack { get; set; }
        public int order { get; set; }
        public SoundTrackModel()
        {
            Name = "";
            Gid = "";
            ParentGid = "";
            FilePath = "";
            ItemType = 0;
            order = 0;
            ListTrack = new List<SoundTrackModel>();
        }
    }
}