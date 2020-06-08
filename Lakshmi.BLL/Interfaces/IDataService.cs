using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.BLL.DTO;

namespace Lakshmi.BLL.Interfaces
{
    public interface IDataService
    {
        

        void MakePhoto(PhotoDTO photoDto);
        void DeletePhoto(int id);
        void ChangePhoto(PhotoDTO photoDto);
        PhotoDTO GetPhoto(int id);
        PhotoDTO GetPhotoForLooking(int id, string userSelfId);
        IEnumerable<PhotoDTO> GetPhotosAll(string userSelfId);//все фотки из бд(для ленты)        
        IEnumerable<PhotoDTO> GetPhotos(string userId, string userSelfId);//фотки конкретного юзера(для странички)
        IEnumerable<PhotoDTO> FindPhotos(string caption, string userSelfId);//все фотки из бд(для поиска)
        bool ChangeLikes(int photoId, string userId);
        int CountLikes(int id);


        void MakeComment(CommentDTO commentDto);
        void DeleteComment(int id); 
        void ChangeComment(CommentDTO commentDTO);
        CommentDTO GetComment(int id);
        IEnumerable<CommentDTO> GetComments(int photoId);//все комменты под фотографией
        IEnumerable<CommentDTO> FindComments();//все фотки из бд(для поиска)

        void Dispose();
    }
}