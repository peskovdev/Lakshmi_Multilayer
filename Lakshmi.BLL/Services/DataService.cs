using System;
using Lakshmi.BLL.DTO;
using Lakshmi.DAL.Entities;
using Lakshmi.DAL.Interfaces;
using Lakshmi.BLL.Infrastructure;
using Lakshmi.BLL.Interfaces;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNet.Identity;

namespace Lakshmi.BLL.Services
{
    public class DataService : IDataService
    {
        IUnitOfWork Database { get; set; }

        public DataService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public void MakePhoto(PhotoDTO photoDto)
        {            
            Photo photo = new Photo
            {
                Caption = photoDto.Caption,
                Image = photoDto.Image,
                ApplicationUserId = photoDto.UserDtoId
            };
            Database.Photos.Create(photo);
            Database.Save();
        }
        public void DeletePhoto(int id)
        {
            Database.Photos.Delete(id);
            Database.Save();
        }

        public bool ChangeLikes(int photoId, string userId)
        {
            if (!Database.Likes.Check(photoId, userId)) //если лайка не существует, то мы его создаем
            {
                Like like = new Like
                {
                    ApplicationUserId = userId,
                    PhotoId = photoId
                };
                Database.Likes.Create(like);
                Database.Save();
                return true;
            }
            else //если лайк есть, то мы его удаляем
            {
                Database.Likes.Delete(photoId, userId);
                Database.Save();
                return false;
            }    
        }
        public int CountLikes(int id)
        {
            return Database.Photos.Get(id).Likes.Count;
        }

        public void ChangePhoto(PhotoDTO photoDto)
        {
            Photo photo = Database.Photos.Get(photoDto.Id);
            photo.Caption = photoDto.Caption;
            Database.Photos.Update(photo);
            Database.Save();
        }
        public PhotoDTO GetPhoto(int id) //вывод одной фотографии
        {
            var photo = Database.Photos.Get(id);
            if (photo == null)
                throw new ValidationException("Фотография не найдена", "");

            ApplicationUser user = Database.UserManager.FindById(photo.ApplicationUserId);
            PhotoDTO photoDto = new PhotoDTO
            {
                Id = photo.Id,
                Caption = photo.Caption,
                Image = photo.Image,
                ImageMini = photo.ImageMini,
                UserDtoId = photo.ApplicationUserId,
                Likes = CountLikes(photo.Id),                
                FullName = user.ClientProfile.SecondName + " " + user.ClientProfile.FirstName,
                NickName = user.ClientProfile.NickName,
                userpicMini = user.ClientProfile.UserpicMini
            };
            return photoDto;
        }
        public PhotoDTO GetPhotoForLooking(int id, string userSelfId) //вывод одной фотографии
        {
            var photo = Database.Photos.Get(id);
            if (photo == null)
                throw new ValidationException("Фотография не найдена", "");

            ApplicationUser user = Database.UserManager.FindById(photo.ApplicationUserId);
            PhotoDTO photoDto = new PhotoDTO
            {
                Id = photo.Id,
                Caption = photo.Caption,
                Image = photo.Image,
                ImageMini = photo.ImageMini,
                UserDtoId = photo.ApplicationUserId,
                Likes = CountLikes(photo.Id),
                Liked = Database.Likes.Check(photo.Id, userSelfId),
                FullName = user.ClientProfile.SecondName + " " + user.ClientProfile.FirstName,
                NickName = user.ClientProfile.NickName,
                userpicMini = user.ClientProfile.UserpicMini
            };
            return photoDto;
        }

        public IEnumerable<PhotoDTO> GetPhotosAll(string userSelfId) //Получение списка всех фотографий
        {
            var photos = Database.Photos.GetAll();
            List<PhotoDTO> PhotosDTO = new List<PhotoDTO>();
            foreach (var photo in photos)
            {
                ApplicationUser user = Database.UserManager.FindById(photo.ApplicationUserId);
                PhotosDTO.Add(new PhotoDTO
                {
                    Id = photo.Id,
                    Caption = photo.Caption,
                    Image = photo.Image,
                    ImageMini = photo.ImageMini,
                    Likes = CountLikes(photo.Id),
                    Liked = Database.Likes.Check(photo.Id, userSelfId),
                    UserDtoId = photo.ApplicationUserId,
                    NickName = user.ClientProfile.NickName,
                    userpicMini = user.ClientProfile.UserpicMini
                });
            }
            return PhotosDTO;
        }

        public IEnumerable<PhotoDTO> GetPhotos(string userId, string userSelfId) //Получение списка фоток конкретного юзера
        {
            var photos = Database.Photos.Find(c => c.ApplicationUserId == userId);
            List<PhotoDTO> PhotosDTO = new List<PhotoDTO>();
            foreach (var photo in photos)
            {
                ApplicationUser user = Database.UserManager.FindById(photo.ApplicationUserId);
                PhotosDTO.Add(new PhotoDTO
                {
                    Id = photo.Id,
                    Caption = photo.Caption,
                    Likes = CountLikes(photo.Id),
                    Liked = Database.Likes.Check(photo.Id, userSelfId),
                    Image = photo.Image,
                    UserDtoId = photo.ApplicationUserId
                });
            }
            return PhotosDTO;
        }

        public IEnumerable<PhotoDTO> FindPhotos(string caption, string userSelfId) //Получение списка фоток конкретного юзера
        {            
            if (!String.IsNullOrEmpty(caption))
            {
                var photos = Database.Photos.Find(s => s.Caption.Contains(caption));
                List<PhotoDTO> PhotosDTO = new List<PhotoDTO>();
                foreach (var photo in photos)
                {
                    ApplicationUser user = Database.UserManager.FindById(photo.ApplicationUserId);
                    PhotosDTO.Add(new PhotoDTO
                    {
                        Id = photo.Id,
                        Caption = photo.Caption,
                        Image = photo.Image,
                        ImageMini = photo.ImageMini,
                        Likes = CountLikes(photo.Id),
                        Liked = Database.Likes.Check(photo.Id, userSelfId),
                        UserDtoId = photo.ApplicationUserId,
                        NickName = user.ClientProfile.NickName,
                        userpicMini = user.ClientProfile.UserpicMini
                    });
                }
                return PhotosDTO;
            }
            return GetPhotosAll(userSelfId);
        }

        public void MakeComment(CommentDTO commentDto)
        {
            Photo photo = Database.Photos.Get(commentDto.PhotoDtoId);
            // валидация
            if (photo == null)
                throw new ValidationException("Такой фотографии не существует", "");
            Comment comment = new Comment
            {
                Signature = commentDto.Signature,
                PhotoId = commentDto.PhotoDtoId,
                ApplicationUserId = commentDto.UserDtoId
            };
            Database.Comments.Create(comment);
            Database.Save();
        }

        public void DeleteComment(int id)
        {
            Database.Comments.Delete(id);
            Database.Save();
        }

        public void ChangeComment(CommentDTO commentDto)
        {
            Comment comment = Database.Comments.Get(commentDto.Id);
            comment.Signature = commentDto.Signature;
            Database.Comments.Update(comment);
            Database.Save();
        }

        public CommentDTO GetComment(int id)
        {
            var comment = Database.Comments.Get(id);
            if (comment == null)
                throw new ValidationException("Комментарий не найден", "");
            ApplicationUser user = Database.UserManager.FindById(comment.ApplicationUserId);
            CommentDTO сommentDTO = new CommentDTO
            {
                Id = comment.Id,
                Signature = comment.Signature,
                PhotoDtoId = comment.PhotoId,
                UserDtoId = comment.ApplicationUserId,
                NickName = user.UserName
            };
            return сommentDTO;
        }

        public IEnumerable<CommentDTO> GetComments(int photoId) //Получение списка коментариев по фотографии
        {
            var comments = Database.Comments.Find(c => c.PhotoId == photoId);

            List<CommentDTO> CommentsDTO = new List<CommentDTO>();
            foreach (var comment in comments) { 
                ApplicationUser user = Database.UserManager.FindById(comment.ApplicationUserId);
                CommentsDTO.Add(new CommentDTO 
                {
                    Id = comment.Id,
                    Signature = comment.Signature,
                    PhotoDtoId = comment.PhotoId,
                    UserDtoId = comment.ApplicationUserId,
                    NickName = user.UserName
                });                
            }
            return CommentsDTO;
        }

        public IEnumerable<CommentDTO> FindComments() //Получение списка коментариев по фотографии
        {
            var comments = Database.Comments.GetAll();

            List<CommentDTO> CommentsDTO = new List<CommentDTO>();
            foreach (var comment in comments)
            {
                ApplicationUser user = Database.UserManager.FindById(comment.ApplicationUserId);
                CommentsDTO.Add(new CommentDTO
                {
                    Id = comment.Id,
                    Signature = comment.Signature,
                    PhotoDtoId = comment.PhotoId,
                    UserDtoId = comment.ApplicationUserId,
                    NickName = user.UserName
                });
            }
            return CommentsDTO;
        }

        public void Dispose()
        {
            Database.Dispose();
        }

    }
}
