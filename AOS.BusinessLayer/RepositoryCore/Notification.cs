using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<Notification> GetAllNotifications();
        Notification GetNotificationById(int Id);
        Notification GetNotificationByCode(string Code);
        void AddNotification(params Notification[] Notification);
        void UpdateNotification(params Notification[] Notification);
        void RemoveNotification(params Notification[] Notification);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Notification> GetAllNotifications()
        {
            return _NotificationRepository.GetAll();
        }

        public Notification GetNotificationById(int Id)
        {
            return _NotificationRepository.GetSingle(d => d.NotificationID.Equals(Id));
        }

        public Notification GetNotificationByCode(string Code)
        {
            return _NotificationRepository.GetSingle(d => d.Code.ToLower().Equals(Code.ToLower()));
        }

        public void AddNotification(params Notification[] Notification)
        {
            /* Validation and error handling omitted */
            _NotificationRepository.Add(Notification);
        }

        public void UpdateNotification(params Notification[] Notification)
        {
            /* Validation and error handling omitted */
            _NotificationRepository.Update(Notification);
        }

        public void RemoveNotification(params Notification[] Notification)
        {
            /* Validation and error handling omitted */
            _NotificationRepository.Remove(Notification);
        }
    }
}
