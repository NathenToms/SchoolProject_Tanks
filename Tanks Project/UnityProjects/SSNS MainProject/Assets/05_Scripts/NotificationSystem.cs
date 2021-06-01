using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private float timeToFade = 5.0f;
    [SerializeField] private int maxNotifications = 3;

    private Queue<Notification> notifications;

    private void Awake()
    {
        notifications = new Queue<Notification>();
    }

    public int AddNotification(string text, NotificationType type)
    {
        int id = 0;

        Notification notification;
        notification.id = id;
        notification.text = text;
        notification.type = type;

        return id;
    }

    private void Update()
    {
        
    }

    private struct Notification
    {
        public int id;
        public string text;
        public NotificationType type;

    }
}
