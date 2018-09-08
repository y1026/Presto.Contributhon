﻿using Presto.Plugin.YouTube.Dialogs;
using Presto.Plugin.YouTube.Models;
using Presto.Plugin.YouTube.Supports;
using Presto.SDK;
using System.Collections.Generic;
using System.Windows.Input;

namespace Presto.Plugin.YouTube.ViewModels
{
    public class AddViewModel : BaseViewModel
    {
        #region 변수
        private IEnumerable<Music> _musics;
        #endregion

        #region 속성
        public ICommand Add { get; }

        public ICommand Cancel { get; }

        public IEnumerable<Music> Musics
        {
            get => _musics;
            set
            {
                _musics = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 생성자
        public AddViewModel()
        {
            Add = new DelegateCommand(Add_Execute);
            Cancel = new DelegateCommand(Cancel_Execute);
        }
        #endregion

        #region 커멘드 함수
        private void Add_Execute(object obj)
        {
            foreach (var music in Musics)
            {
                // 태그 정보 작성
                using (var aacFile = TagLib.File.Create(music.Path))
                {
                    aacFile.Tag.Title = music.Title;
                    aacFile.Tag.Album = music.Album;
                    aacFile.Tag.AlbumArtists = new[] { music.Artist };
                    aacFile.Tag.Performers = new[] { music.Artist };
                    aacFile.Tag.Pictures = new[] { new TagLib.Picture(music.Picture) };
                    aacFile.Tag.Genres = new[] { music.Genre };
                    aacFile.Save();
                }

                // 라이브러리 추가
                var prestoMusic = PrestoSDK.PrestoService?.Library.LoadMusic(music.Path);
                if (prestoMusic != null)
                {
                    PrestoSDK.PrestoService.Library.AddMusic(prestoMusic);
                }
            }

            RaiseCloseRequested();
        }

        private void Cancel_Execute(object obj)
        {
            new SearchDialog().Show();
            RaiseCloseRequested();
        }
        #endregion
    }
}
