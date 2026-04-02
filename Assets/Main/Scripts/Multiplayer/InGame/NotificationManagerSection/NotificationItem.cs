using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection
{
    public class NotificationItem : NetworkBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite firstIcon;
        [SerializeField] private Sprite secondIcon;
        [SerializeField] private Sprite thirdIcon;
        [SerializeField] private Sprite otherIcon;


        public Image IconImage
        {
            get => iconImage;
            set => iconImage = value;
        }

        public Sprite FirstIcon
        {
            get => firstIcon;
            set => firstIcon = value;
        }

        public Sprite SecondIcon
        {
            get => secondIcon;
            set => secondIcon = value;
        }

        public Sprite ThirdIcon
        {
            get => thirdIcon;
            set => thirdIcon = value;
        }

        public Sprite OtherIcon
        {
            get => otherIcon;
            set => otherIcon = value;
        }
    }
}