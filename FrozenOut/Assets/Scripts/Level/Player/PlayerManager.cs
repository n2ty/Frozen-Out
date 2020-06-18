using System;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Settings;

using Scripts.Level.Sound;
using Scripts.Level.Item;
using UnityEngine.ProBuilder;

namespace Scripts.Level.Player
{
    public enum PlayerForm {Normal, Melted}

    public class PlayerManager : BaseManager
    {
        public LevelManager LevelManager;

        public NormalController NormalController;
        public MeltedController MeltedController;
        public GameObject Player;
        public Animator Animator;
        public PlayerForm PlayerForm = PlayerForm.Normal;
        public List<ItemEquipper> EquippableObjects;

        public bool IsGrounded = true;

        
        private MusicManager SoundManager => LevelManager.GetSoundManager();
        private SettingsManager SettingsManager => LevelManager.GetSettingsManager();

        private string PickAnimationName = "isPicking";
        [SerializeField] private string InteractAnimation = "";

        public KeyCode GetJumpKey()
        {
            return SettingsManager.JumpKey;
        }

        public KeyCode GetCrouchKey()
        {
            return SettingsManager.CrouchKey;
        }

        public KeyCode [] GetMovementKeys()
        {
            return SettingsManager.MovementKeys;
        }

        public KeyCode GetForwardKey()
        {
            return SettingsManager.ForwardKey;
        }

        public KeyCode GetBackKey()
        {
            return SettingsManager.BackwardKey;
        }

        public KeyCode GetRightKey()
        {
            return SettingsManager.RightKey;
        }

        public KeyCode GetLeftKey()
        {
            return SettingsManager.LeftKey;
        }

        public void ChangeToNormal() 
        {
            Animator.SetTrigger("isChanging");
            NormalController.enabled = true;
            MeltedController.enabled = false;
            PlayerForm = PlayerForm.Normal;
        }

        public void ChangeToMelted()
        {
            Animator.SetTrigger("isChanging");
            NormalController.enabled = false;
            MeltedController.enabled = true;
            PlayerForm = PlayerForm.Melted;
        }

        public void DisableController()
        {
            NormalController.enabled = false;
            MeltedController.enabled = false;
        }

        public void EnableController()
        {
            NormalController.enabled = true;
        }

        public void ToCheckPoint(Transform transform)
        {
            Instantiate(Player, transform.position, transform.rotation);
        }

        public void SetInteractiveItem(Transform itemPos, Transform itemLook)
        {
            if (NormalController.isActiveAndEnabled == true) 
            {
                NormalController.InteractPos = itemPos;
                NormalController.InteractLook = itemLook;
            }
            else if (MeltedController.isActiveAndEnabled == true)
            {
                MeltedController.InteractPos = itemPos;
                MeltedController.InteractLook = itemLook;
            }
        }

        public bool GetIsInteracting()
        {
            if (NormalController.isActiveAndEnabled == true)
            {
                return NormalController.IsInteracting;
            }
            else if (MeltedController.isActiveAndEnabled == true)
            {
                return MeltedController.IsInteracting;
            }

            return false;
        }

        public void SetIsInteracting(bool state)
        {
            if (NormalController.isActiveAndEnabled == true)
            {
                NormalController.IsInteracting = state;
            }
            else if (MeltedController.isActiveAndEnabled == true)
            {
                MeltedController.IsInteracting = state;
            }
        }

        public void EquipItem(ItemInfo itemEquipped)
        {
            foreach (ItemEquipper equipper in EquippableObjects)
            {
                if (itemEquipped.Equals(equipper.Item))
                {
                    PickAnimation();
                    equipper.OnEquip();
                }
                else
                {
                    equipper.OnUnequip();
                }
            }
        }

        public void UnequipItem()
        {
            PickAnimation();
            foreach (ItemEquipper equipper in EquippableObjects)
            {
                equipper.OnUnequip();
            }
        }

        public void PickAnimation()
        {
            Animator.SetTrigger(PickAnimationName);
        }

        public void PlayInteractAnimation()
        {
            Animator.SetTrigger(InteractAnimation);
        }

        public void SetInteractAnimation(String animation)
        {
            InteractAnimation = animation;
        }

        #region Events
        public event EventHandler<PlayerControllerEventArgs> Moving;
        public event EventHandler Idle;

        public void OnMoving()
        {
            PlayerControllerEventArgs controllerEvent = new PlayerControllerEventArgs();
            Moving?.Invoke(this, controllerEvent);
        }

        public void OnIdle()
        {
            Idle?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }

    [Serializable]
    public class PlayerControllerEventArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }
    }
}
